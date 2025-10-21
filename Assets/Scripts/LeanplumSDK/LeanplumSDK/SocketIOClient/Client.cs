using System;
using System.Collections.Concurrent;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using LeanplumSDK.SocketIOClient.Eventing;
using LeanplumSDK.SocketIOClient.Messages;
using LeanplumSDK.WebSocketSharp;

namespace LeanplumSDK.SocketIOClient
{
	internal class Client : IDisposable, IClient
	{
		private Timer socketHeartBeatTimer;

		private Thread dequeuOutBoundMsgTask;

		private ConcurrentQueue<string> outboundQueue;

		private int retryConnectionCount;

		private int retryConnectionAttempts = 3;

		private static readonly object padLock = new object();

		protected Uri uri;

		protected WebSocket wsClient;

		protected RegistrationManager registrationManager;

		public ManualResetEvent MessageQueueEmptyEvent = new ManualResetEvent(true);

		public ManualResetEvent ConnectionOpenEvent = new ManualResetEvent(false);

		public string LastErrorMessage = string.Empty;

		public int RetryConnectionAttempts
		{
			get
			{
				return retryConnectionAttempts;
			}
			set
			{
				retryConnectionAttempts = value;
			}
		}

		public SocketIOHandshake HandShake { get; private set; }

		public bool IsConnected
		{
			get
			{
				return ReadyState == WebSocketState.OPEN;
			}
		}

		public WebSocketState ReadyState
		{
			get
			{
				if (wsClient != null)
				{
					return wsClient.ReadyState;
				}
				return WebSocketState.CLOSED;
			}
		}

		public event EventHandler Opened;

		public event EventHandler<MessageEventArgs> Message;

		public event EventHandler ConnectionRetryAttempt;

		public event EventHandler HeartBeatTimerEvent;

		public event EventHandler SocketConnectionClosed;

		public event EventHandler<ErrorEventArgs> Error;

		public Client(string url)
		{
			uri = new Uri(url);
			registrationManager = new RegistrationManager();
			outboundQueue = new ConcurrentQueue<string>();
			dequeuOutBoundMsgTask = new Thread(dequeuOutboundMessages);
			dequeuOutBoundMsgTask.Start();
		}

		public void Connect()
		{
			lock (padLock)
			{
				if (ReadyState == WebSocketState.CONNECTING || ReadyState == WebSocketState.OPEN)
				{
					return;
				}
				try
				{
					ConnectionOpenEvent.Reset();
					HandShake = requestHandshake(uri);
					if (HandShake == null || string.IsNullOrEmpty(HandShake.SID) || HandShake.HadError)
					{
						LastErrorMessage = string.Format("Error initializing handshake with {0}", uri.ToString());
						OnErrorEvent(this, new ErrorEventArgs(LastErrorMessage, string.Empty));
						return;
					}
					string text = ((!(uri.Scheme == Uri.UriSchemeHttps)) ? "ws" : "wss");
					wsClient = new WebSocket(string.Format("{0}://{1}:{2}/socket.io/1/websocket/{3}", text, uri.Host, uri.Port, HandShake.SID), wsClient_OpenEvent, wsClient_MessageReceived, wsClient_Error, wsClient_Closed);
				}
				catch (Exception ex)
				{
					OnErrorEvent(this, new ErrorEventArgs("SocketIO.Client.Connect threw an exception", ex.ToString()));
				}
			}
		}

		public IEndPointClient Connect(string endPoint)
		{
			EndPointClient result = new EndPointClient(this, endPoint);
			Connect();
			Send(new ConnectMessage(endPoint));
			return result;
		}

		protected void ReConnect()
		{
			retryConnectionCount++;
			OnConnectionRetryAttemptEvent(this, EventArgs.Empty);
			closeHeartBeatTimer();
			closeWebSocketClient();
			Connect();
			if (ConnectionOpenEvent.WaitOne(4000))
			{
				retryConnectionCount = 0;
				return;
			}
			if (retryConnectionCount < RetryConnectionAttempts)
			{
				ReConnect();
				return;
			}
			Close();
			OnSocketConnectionClosedEvent(this, EventArgs.Empty);
		}

		public virtual void On(string eventName, Action<LeanplumSDK.SocketIOClient.Messages.IMessage> action)
		{
			registrationManager.AddOnEvent(eventName, action);
		}

		public virtual void On(string eventName, string endPoint, Action<LeanplumSDK.SocketIOClient.Messages.IMessage> action)
		{
			registrationManager.AddOnEvent(eventName, endPoint, action);
		}

		public void Emit(string eventName, object payload, string endPoint, Action<object> callback)
		{
			string text = eventName.ToLower();
			LeanplumSDK.SocketIOClient.Messages.IMessage message = null;
			switch (text)
			{
			case "message":
				if (payload is string)
				{
					TextMessage textMessage = new TextMessage();
					textMessage.MessageText = payload.ToString();
					message = textMessage;
				}
				else
				{
					message = new JSONMessage(payload);
				}
				Send(message);
				return;
			case "connect":
			case "disconnect":
			case "open":
			case "close":
			case "error":
			case "retry":
			case "reconnect":
				throw new ArgumentOutOfRangeException(eventName, "Event name is reserved by socket.io, and cannot be used by clients or servers with this message type");
			}
			if (!string.IsNullOrEmpty(endPoint) && !endPoint.StartsWith("/"))
			{
				endPoint = "/" + endPoint;
			}
			message = new EventMessage(eventName, payload, endPoint, callback);
			if (callback != null)
			{
				registrationManager.AddCallBack(message);
			}
			Send(message);
		}

		public void Emit(string eventName, object payload)
		{
			Emit(eventName, payload, string.Empty, null);
		}

		public void Send(LeanplumSDK.SocketIOClient.Messages.IMessage msg)
		{
			MessageQueueEmptyEvent.Reset();
			if (outboundQueue != null)
			{
				outboundQueue.Enqueue(msg.Encoded);
			}
		}

		public void Send(string msg)
		{
			TextMessage textMessage = new TextMessage();
			textMessage.MessageText = msg;
			LeanplumSDK.SocketIOClient.Messages.IMessage msg2 = textMessage;
			Send(msg2);
		}

		private void Send_backup(string rawEncodedMessageText)
		{
			MessageQueueEmptyEvent.Reset();
			if (outboundQueue != null)
			{
				outboundQueue.Enqueue(rawEncodedMessageText);
			}
		}

		protected void OnMessageEvent(LeanplumSDK.SocketIOClient.Messages.IMessage msg)
		{
			bool flag = false;
			if (!string.IsNullOrEmpty(msg.Event))
			{
				flag = registrationManager.InvokeOnEvent(msg);
			}
			EventHandler<MessageEventArgs> message = this.Message;
			if (message != null && !flag)
			{
				message(this, new MessageEventArgs(msg));
			}
		}

		public void Close()
		{
			retryConnectionCount = 0;
			closeHeartBeatTimer();
			closeOutboundQueue();
			if (registrationManager != null)
			{
				registrationManager.Dispose();
				registrationManager = null;
			}
			closeWebSocketClient();
		}

		protected void closeHeartBeatTimer()
		{
			if (socketHeartBeatTimer != null)
			{
				socketHeartBeatTimer.Change(-1, -1);
				socketHeartBeatTimer.Dispose();
				socketHeartBeatTimer = null;
			}
		}

		protected void closeOutboundQueue()
		{
			if (outboundQueue != null)
			{
				outboundQueue = null;
			}
		}

		protected void closeWebSocketClient()
		{
			if (wsClient == null)
			{
				return;
			}
			wsClient.Close();
			if (wsClient.ReadyState == WebSocketState.CONNECTING || wsClient.ReadyState == WebSocketState.OPEN)
			{
				try
				{
					wsClient.Close();
				}
				catch
				{
				}
			}
			wsClient = null;
		}

		private void wsClient_OpenEvent(object sender, EventArgs e)
		{
			socketHeartBeatTimer = new Timer(OnHeartBeatTimerCallback, new object(), HandShake.HeartbeatInterval, HandShake.HeartbeatInterval);
			ConnectionOpenEvent.Set();
			OnMessageEvent(new EventMessage
			{
				Event = "open"
			});
			if (this.Opened != null)
			{
				try
				{
					this.Opened(this, EventArgs.Empty);
				}
				catch (Exception)
				{
				}
			}
		}

		private void wsClient_MessageReceived(object sender, LeanplumSDK.WebSocketSharp.MessageEventArgs e)
		{
			LeanplumSDK.SocketIOClient.Messages.IMessage message = LeanplumSDK.SocketIOClient.Messages.Message.Factory(e.Data);
			if (message.Event == "responseMsg")
			{
			}
			switch (message.MessageType)
			{
			case SocketIOMessageTypes.Disconnect:
				OnMessageEvent(message);
				if (string.IsNullOrEmpty(message.Endpoint))
				{
					Close();
				}
				break;
			case SocketIOMessageTypes.Heartbeat:
				OnHeartBeatTimerCallback(null);
				break;
			case SocketIOMessageTypes.Connect:
			case SocketIOMessageTypes.Message:
			case SocketIOMessageTypes.JSONMessage:
			case SocketIOMessageTypes.Event:
			case SocketIOMessageTypes.Error:
				OnMessageEvent(message);
				break;
			case SocketIOMessageTypes.ACK:
				registrationManager.InvokeCallBack(message.AckId, message.Json);
				break;
			}
		}

		private void wsClient_Closed(object sender, EventArgs e)
		{
			if (retryConnectionCount < RetryConnectionAttempts)
			{
				ConnectionOpenEvent.Reset();
				ReConnect();
			}
			else
			{
				Close();
				OnSocketConnectionClosedEvent(this, EventArgs.Empty);
			}
		}

		private void wsClient_Error(object sender, LeanplumSDK.WebSocketSharp.ErrorEventArgs e)
		{
			OnErrorEvent(sender, new ErrorEventArgs("SocketClient error", e.Message));
		}

		protected void OnErrorEvent(object sender, ErrorEventArgs e)
		{
			LastErrorMessage = e.Message;
			if (this.Error != null)
			{
				try
				{
					this.Error(this, e);
				}
				catch
				{
				}
			}
		}

		protected void OnSocketConnectionClosedEvent(object sender, EventArgs e)
		{
			if (this.SocketConnectionClosed != null)
			{
				try
				{
					this.SocketConnectionClosed(sender, e);
				}
				catch
				{
				}
			}
		}

		protected void OnConnectionRetryAttemptEvent(object sender, EventArgs e)
		{
			if (this.ConnectionRetryAttempt != null)
			{
				try
				{
					this.ConnectionRetryAttempt(sender, e);
				}
				catch (Exception)
				{
				}
			}
		}

		protected void OnHeartBeatTimerCallback(object state)
		{
			if (ReadyState != WebSocketState.OPEN)
			{
				return;
			}
			LeanplumSDK.SocketIOClient.Messages.IMessage message = new Heartbeat();
			try
			{
				if (outboundQueue != null)
				{
					outboundQueue.Enqueue(message.Encoded);
					if (this.HeartBeatTimerEvent != null)
					{
						this.HeartBeatTimerEvent.BeginInvoke(this, EventArgs.Empty, EndAsyncEvent, null);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void EndAsyncEvent(IAsyncResult result)
		{
			AsyncResult asyncResult = (AsyncResult)result;
			EventHandler eventHandler = (EventHandler)asyncResult.AsyncDelegate;
			try
			{
				eventHandler.EndInvoke(result);
			}
			catch
			{
			}
		}

		protected void dequeuOutboundMessages()
		{
			while (outboundQueue != null)
			{
				if (ReadyState == WebSocketState.OPEN)
				{
					try
					{
						string item;
						if (outboundQueue.TryDequeue(out item))
						{
							wsClient.Send(item);
						}
						else
						{
							MessageQueueEmptyEvent.Set();
						}
					}
					catch (Exception)
					{
					}
				}
				else
				{
					ConnectionOpenEvent.WaitOne(2000);
				}
			}
		}

		protected SocketIOHandshake requestHandshake(Uri uri)
		{
			string value = string.Empty;
			string text = string.Empty;
			SocketIOHandshake socketIOHandshake = null;
			using (WebClient webClient = new WebClient())
			{
				try
				{
					value = webClient.DownloadString(string.Format("{0}://{1}:{2}/socket.io/1/{3}", uri.Scheme, uri.Host, uri.Port, uri.Query));
					if (string.IsNullOrEmpty(value))
					{
						text = "Did not receive handshake string from server";
					}
				}
				catch (Exception ex)
				{
					text = string.Format("Error getting handsake from Socket.IO host instance: {0}", ex.Message);
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				socketIOHandshake = SocketIOHandshake.LoadFromString(value);
			}
			else
			{
				socketIOHandshake = new SocketIOHandshake();
				socketIOHandshake.ErrorMessage = text;
			}
			return socketIOHandshake;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Close();
				MessageQueueEmptyEvent.Close();
				ConnectionOpenEvent.Close();
			}
		}
	}
}
