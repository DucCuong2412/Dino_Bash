using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using LeanplumSDK.MiniJSON;
using LeanplumSDK.SocketIOClient;

namespace LeanplumSDK
{
	internal class LeanplumSocket
	{
		private readonly System.Timers.Timer reconnectTimer;

		private readonly Client socketIOClient;

		private bool authSent;

		private bool connected;

		private bool connecting;

		private Action onUpdateVars;

		public LeanplumSocket(Action onUpdate)
		{
			onUpdateVars = onUpdate;
			socketIOClient = new Client("http://" + Constants.SOCKET_HOST + ":" + Constants.SOCKET_PORT);
			socketIOClient.Opened += OnSocketOpened;
			socketIOClient.Message += OnSocketMessage;
			socketIOClient.SocketConnectionClosed += OnSocketConnectionClosed;
			socketIOClient.Error += OnSocketError;
			reconnectTimer = new System.Timers.Timer(Constants.NETWORK_SOCKET_TIMEOUT_SECONDS * 1000);
			reconnectTimer.AutoReset = true;
			reconnectTimer.Elapsed += delegate
			{
				if (!connected && !connecting)
				{
					Connect();
				}
			};
			Connect();
			reconnectTimer.Start();
		}

		private void Connect()
		{
			connecting = true;
			Thread thread = new Thread(socketIOClient.Connect);
			thread.Start();
		}

		public void Close()
		{
			socketIOClient.Close();
		}

		private void OnSocketOpened(object obj, EventArgs e)
		{
			if (connecting)
			{
				LeanplumNative.CompatibilityLayer.Log("Connected to development server.");
				connected = true;
				connecting = false;
				if (!authSent && connected)
				{
					IDictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary["appId"] = LeanplumRequest.AppId;
					dictionary["deviceId"] = LeanplumRequest.DeviceId;
					socketIOClient.Emit("auth", dictionary);
					authSent = true;
				}
			}
		}

		private void OnSocketMessage(object obj, MessageEventArgs e)
		{
			if (e.Message.MessageType != SocketIOMessageTypes.Event || string.IsNullOrEmpty(e.Message.MessageText))
			{
				return;
			}
			IDictionary<string, object> dictionary = Json.Deserialize(e.Message.MessageText) as IDictionary<string, object>;
			string text = ((!dictionary.ContainsKey("name")) ? string.Empty : (dictionary["name"] as string));
			if (text == "updateVars")
			{
				onUpdateVars();
			}
			else if (text == "registerDevice")
			{
				IDictionary<string, object> dictionary2 = (IDictionary<string, object>)((IList<object>)dictionary["args"])[0];
				string email = (string)dictionary2["email"];
				LeanplumUnityHelper.QueueOnMainThread(delegate
				{
					LeanplumNative.OnHasStartedAndRegisteredAsDeveloper();
					LeanplumNative.CompatibilityLayer.DisplayModal("Leanplum", "Your device is registered to " + email + ".");
				});
			}
		}

		private void OnSocketConnectionClosed(object obj, EventArgs e)
		{
			if (connected)
			{
				LeanplumNative.CompatibilityLayer.Log("Disconnected from development server.");
				connected = false;
				connecting = false;
				authSent = false;
			}
		}

		private void OnSocketError(object obj, ErrorEventArgs e)
		{
			LeanplumNative.CompatibilityLayer.LogError("Closing development socket with error: " + e.Message + ". If this problem persists, please confirm that your Internet firewall allows WebSockets, or try a different Internet connection.");
			socketIOClient.Close();
		}
	}
}
