using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using LeanplumSDK.WebSocketSharp.Net;
using LeanplumSDK.WebSocketSharp.Net.WebSockets;

namespace LeanplumSDK.WebSocketSharp
{
	internal class WebSocket : IDisposable
	{
		internal delegate bool SendBytesFunc(Opcode opcode, byte[] data);

		private const string _guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		private const string _version = "13";

		internal const int FragmentLength = 1016;

		private string _base64key;

		private RemoteCertificateValidationCallback _certValidationCallback;

		private bool _client;

		private Action _closeContext;

		private CompressionMethod _compression;

		private WebSocketContext _context;

		private CookieCollection _cookies;

		private Func<CookieCollection, CookieCollection, bool> _cookiesValidation;

		private WsCredential _credentials;

		private string _extensions;

		private AutoResetEvent _exitReceiving;

		private object _forClose;

		private object _forSend;

		private volatile Logger _logger;

		private string _origin;

		private bool _preAuth;

		private string _protocol;

		private string _protocols;

		private volatile WebSocketState _readyState;

		private AutoResetEvent _receivePong;

		private bool _secure;

		private WsStream _stream;

		private ITcpClient _tcpClient;

		private Uri _uri;

		internal Func<CookieCollection, CookieCollection, bool> CookiesValidation
		{
			get
			{
				return _cookiesValidation;
			}
			set
			{
				_cookiesValidation = value;
			}
		}

		internal bool IsOpened
		{
			get
			{
				return _readyState == WebSocketState.OPEN || _readyState == WebSocketState.CLOSING;
			}
		}

		public CompressionMethod Compression
		{
			get
			{
				return _compression;
			}
			set
			{
				if (IsOpened)
				{
					string message = "A WebSocket connection has already been established.";
					_logger.Error(message);
					error(message);
				}
				else
				{
					_compression = value;
				}
			}
		}

		public IEnumerable<Cookie> Cookies
		{
			get
			{
				//Discarded unreachable code: IL_0045
				lock (_cookies.SyncRoot)
				{
					return from Cookie cookie in _cookies
						select (cookie);
				}
			}
		}

		public WsCredential Credentials
		{
			get
			{
				return _credentials;
			}
		}

		public string Extensions
		{
			get
			{
				return _extensions;
			}
		}

		public bool IsAlive
		{
			get
			{
				return Ping();
			}
		}

		public bool IsSecure
		{
			get
			{
				return _secure;
			}
		}

		public Logger Log
		{
			get
			{
				return _logger;
			}
			internal set
			{
				if (value != null)
				{
					_logger = value;
				}
			}
		}

		public string Origin
		{
			get
			{
				return _origin;
			}
			set
			{
				string text = null;
				if (IsOpened)
				{
					text = "A WebSocket connection has already been established.";
				}
				else
				{
					if (value.IsNullOrEmpty())
					{
						_origin = string.Empty;
						return;
					}
					Uri uri = new Uri(value);
					if (!uri.IsAbsoluteUri || uri.Segments.Length > 1)
					{
						text = "The syntax of value of Origin must be '<scheme>://<host>[:<port>]'.";
					}
				}
				if (text != null)
				{
					_logger.Error(text);
					error(text);
				}
				else
				{
					_origin = value.TrimEnd('/');
				}
			}
		}

		public string Protocol
		{
			get
			{
				return _protocol;
			}
		}

		public WebSocketState ReadyState
		{
			get
			{
				return _readyState;
			}
		}

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				return _certValidationCallback;
			}
			set
			{
				_certValidationCallback = value;
			}
		}

		public Uri Url
		{
			get
			{
				return _uri;
			}
			internal set
			{
				if (_readyState == WebSocketState.CONNECTING && !_client)
				{
					_uri = value;
				}
			}
		}

		public event EventHandler<CloseEventArgs> OnClose;

		public event EventHandler<ErrorEventArgs> OnError;

		public event EventHandler<MessageEventArgs> OnMessage;

		public event EventHandler OnOpen;

		private WebSocket()
		{
			_compression = CompressionMethod.NONE;
			_cookies = new CookieCollection();
			_extensions = string.Empty;
			_forClose = new object();
			_forSend = new object();
			_origin = string.Empty;
			_preAuth = false;
			_protocol = string.Empty;
			_readyState = WebSocketState.CONNECTING;
		}

		internal WebSocket(HttpListenerWebSocketContext context)
			: this()
		{
			_stream = context.Stream;
			_closeContext = delegate
			{
				context.Close();
			};
			init(context);
		}

		internal WebSocket(TcpListenerWebSocketContext context)
			: this()
		{
			_stream = context.Stream;
			_closeContext = delegate
			{
				context.Close();
			};
			init(context);
		}

		public WebSocket(string url, params string[] protocols)
			: this()
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			string message;
			if (!url.TryCreateWebSocketUri(out _uri, out message))
			{
				throw new ArgumentException(message, "url");
			}
			_protocols = protocols.ToString(", ");
			_secure = ((_uri.Scheme == "wss") ? true : false);
			_client = true;
			_base64key = createBase64Key();
			_logger = new Logger();
		}

		public WebSocket(string url, EventHandler onOpen, EventHandler<MessageEventArgs> onMessage, EventHandler<ErrorEventArgs> onError, EventHandler<CloseEventArgs> onClose, params string[] protocols)
			: this(url, protocols)
		{
			this.OnOpen = onOpen;
			this.OnMessage = onMessage;
			this.OnError = onError;
			this.OnClose = onClose;
			Connect();
		}

		private bool acceptHandshake()
		{
			_logger.Debug(string.Format("A WebSocket connection request from {0}:\n{1}", _context.UserEndPoint, _context));
			if (!validateConnectionRequest(_context))
			{
				_logger.Error("An invalid WebSocket connection request.");
				error("An error has occurred while handshaking.");
				Close(HttpStatusCode.BadRequest);
				return false;
			}
			_base64key = _context.SecWebSocketKey;
			if (_protocol.Length > 0 && !_context.Headers.Contains("Sec-WebSocket-Protocol", _protocol))
			{
				_protocol = string.Empty;
			}
			string text = _context.Headers["Sec-WebSocket-Extensions"];
			if (text != null && text.Length > 0)
			{
				processRequestedExtensions(text);
			}
			return send(createHandshakeResponse());
		}

		private void close(CloseStatusCode code, string reason, bool wait)
		{
			close(new PayloadData(((ushort)code).Append(reason)), !code.IsReserved(), wait);
		}

		private void close(PayloadData payload, bool send, bool wait)
		{
			lock (_forClose)
			{
				if (_readyState != WebSocketState.CLOSING && _readyState != WebSocketState.CLOSED)
				{
					_readyState = WebSocketState.CLOSING;
				}
			}
		}

		private bool close(byte[] frameAsBytes, int timeOut, Func<bool> release)
		{
			bool flag = frameAsBytes != null && _stream.Write(frameAsBytes);
			bool flag2 = timeOut == 0 || (flag && _exitReceiving.WaitOne(timeOut));
			bool flag3 = release();
			bool flag4 = flag && flag2 && flag3;
			_logger.Debug(string.Format("Was clean?: {0}\nsent: {1} received: {2} released: {3}", flag4, flag, flag2, flag3));
			return flag4;
		}

		private bool closeClientResources()
		{
			//Discarded unreachable code: IL_0041, IL_006c
			try
			{
				if (_stream != null)
				{
					_stream.Dispose();
					_stream = null;
				}
				if (_tcpClient != null)
				{
					_tcpClient.Close();
					_tcpClient = null;
				}
				return true;
			}
			catch (Exception ex)
			{
				_logger.Fatal(ex.ToString());
				error("An exception has occurred.");
				return false;
			}
		}

		private bool closeServerResources()
		{
			//Discarded unreachable code: IL_002b, IL_0056
			try
			{
				if (_closeContext != null)
				{
					_closeContext();
				}
				_stream = null;
				_context = null;
				return true;
			}
			catch (Exception ex)
			{
				_logger.Fatal(ex.ToString());
				error("An exception has occurred.");
				return false;
			}
		}

		private bool concatenateFragmentsInto(Stream dest)
		{
			//Discarded unreachable code: IL_00d5
			WsFrame wsFrame;
			while (true)
			{
				wsFrame = _stream.ReadFrame();
				if (!wsFrame.IsFinal && wsFrame.IsContinuation)
				{
					dest.WriteBytes(wsFrame.PayloadData.ApplicationData);
					continue;
				}
				if (wsFrame.IsFinal && wsFrame.IsContinuation)
				{
					dest.WriteBytes(wsFrame.PayloadData.ApplicationData);
					return true;
				}
				if (wsFrame.IsFinal && wsFrame.IsPing)
				{
					processPingFrame(wsFrame);
					continue;
				}
				if (!wsFrame.IsFinal || !wsFrame.IsPong)
				{
					break;
				}
				processPongFrame();
			}
			if (wsFrame.IsFinal && wsFrame.IsClose)
			{
				return processCloseFrame(wsFrame);
			}
			return processUnsupportedFrame(wsFrame, CloseStatusCode.INCORRECT_DATA, null);
		}

		private bool connect()
		{
			return (!_client) ? acceptHandshake() : doHandshake();
		}

		private static string createBase64Key()
		{
			byte[] array = new byte[16];
			Random random = new Random();
			random.NextBytes(array);
			return Convert.ToBase64String(array);
		}

		private HandshakeRequest createHandshakeRequest()
		{
			string pathAndQuery = _uri.PathAndQuery;
			string value = ((_uri.Port != 80) ? _uri.Authority : _uri.DnsSafeHost);
			HandshakeRequest handshakeRequest = new HandshakeRequest(pathAndQuery);
			handshakeRequest.AddHeader("Host", value);
			if (_origin.Length > 0)
			{
				handshakeRequest.AddHeader("Origin", _origin);
			}
			handshakeRequest.AddHeader("Sec-WebSocket-Key", _base64key);
			if (!_protocols.IsNullOrEmpty())
			{
				handshakeRequest.AddHeader("Sec-WebSocket-Protocol", _protocols);
			}
			string text = createRequestExtensions();
			if (text.Length > 0)
			{
				handshakeRequest.AddHeader("Sec-WebSocket-Extensions", text);
			}
			handshakeRequest.AddHeader("Sec-WebSocket-Version", "13");
			if (_preAuth && _credentials != null)
			{
				handshakeRequest.SetAuthorization(new AuthenticationResponse(_credentials));
			}
			if (_cookies.Count > 0)
			{
				handshakeRequest.SetCookies(_cookies);
			}
			return handshakeRequest;
		}

		private HandshakeResponse createHandshakeResponse()
		{
			HandshakeResponse handshakeResponse = new HandshakeResponse();
			handshakeResponse.AddHeader("Sec-WebSocket-Accept", createResponseKey());
			if (_protocol.Length > 0)
			{
				handshakeResponse.AddHeader("Sec-WebSocket-Protocol", _protocol);
			}
			if (_extensions.Length > 0)
			{
				handshakeResponse.AddHeader("Sec-WebSocket-Extensions", _extensions);
			}
			if (_cookies.Count > 0)
			{
				handshakeResponse.SetCookies(_cookies);
			}
			return handshakeResponse;
		}

		private HandshakeResponse createHandshakeResponse(HttpStatusCode code)
		{
			HandshakeResponse handshakeResponse = HandshakeResponse.CreateCloseResponse(code);
			handshakeResponse.AddHeader("Sec-WebSocket-Version", "13");
			return handshakeResponse;
		}

		private string createRequestExtensions()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			if (_compression != 0)
			{
				stringBuilder.Append(_compression.ToCompressionExtension());
			}
			return (stringBuilder.Length <= 0) ? string.Empty : stringBuilder.ToString();
		}

		private string createResponseKey()
		{
			StringBuilder stringBuilder = new StringBuilder(_base64key, 64);
			stringBuilder.Append("258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
			SHA1 sHA = new SHA1CryptoServiceProvider();
			byte[] inArray = sHA.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
			return Convert.ToBase64String(inArray);
		}

		private bool doHandshake()
		{
			setClientStream();
			HandshakeResponse handshakeResponse = sendHandshakeRequest();
			string text = (handshakeResponse.IsUnauthorized ? string.Format("An HTTP {0} authorization is required.", handshakeResponse.AuthChallenge.Scheme) : (validateConnectionResponse(handshakeResponse) ? null : "An invalid response to this WebSocket connection request."));
			if (text != null)
			{
				_logger.Error(text);
				string text2 = "An error has occurred while handshaking.";
				error(text2);
				close(CloseStatusCode.ABNORMAL, text2, false);
				return false;
			}
			string text3 = handshakeResponse.Headers["Sec-WebSocket-Protocol"];
			if (!text3.IsNullOrEmpty())
			{
				_protocol = text3;
			}
			processRespondedExtensions(handshakeResponse.Headers["Sec-WebSocket-Extensions"]);
			CookieCollection cookies = handshakeResponse.Cookies;
			if (cookies.Count > 0)
			{
				_cookies.SetOrRemove(cookies);
			}
			return true;
		}

		private void error(string message)
		{
			this.OnError.Emit(this, new ErrorEventArgs(message));
		}

		private void init(WebSocketContext context)
		{
			_context = context;
			_uri = context.Path.ToUri();
			_secure = context.IsSecureConnection;
			_client = false;
		}

		private void open()
		{
			_readyState = WebSocketState.OPEN;
			this.OnOpen.Emit(this, EventArgs.Empty);
			startReceiving();
		}

		private bool processCloseFrame(WsFrame frame)
		{
			PayloadData payloadData = frame.PayloadData;
			close(payloadData, !payloadData.ContainsReservedCloseStatusCode, false);
			return false;
		}

		private bool processDataFrame(WsFrame frame)
		{
			MessageEventArgs e = ((!frame.IsCompressed) ? new MessageEventArgs(frame.Opcode, frame.PayloadData) : new MessageEventArgs(frame.Opcode, frame.PayloadData.ApplicationData.Decompress(_compression)));
			this.OnMessage.Emit(this, e);
			return true;
		}

		private void processException(Exception exception, string reason)
		{
			CloseStatusCode closeStatusCode = CloseStatusCode.ABNORMAL;
			string text = reason;
			if (exception.GetType() == typeof(WebSocketException))
			{
				WebSocketException ex = (WebSocketException)exception;
				closeStatusCode = ex.Code;
				reason = ex.Message;
			}
			if (closeStatusCode == CloseStatusCode.ABNORMAL || closeStatusCode == CloseStatusCode.TLS_HANDSHAKE_FAILURE)
			{
				_logger.Fatal(exception.ToString());
				reason = text;
			}
			else
			{
				_logger.Error(reason);
				text = null;
			}
			error(text ?? closeStatusCode.GetMessage());
			if (_readyState == WebSocketState.CONNECTING && !_client)
			{
				Close(HttpStatusCode.BadRequest);
			}
			else
			{
				close(closeStatusCode, reason ?? closeStatusCode.GetMessage(), false);
			}
		}

		private bool processFragmentedFrame(WsFrame frame)
		{
			return frame.IsContinuation || processFragments(frame);
		}

		private bool processFragments(WsFrame first)
		{
			//Discarded unreachable code: IL_0073
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.WriteBytes(first.PayloadData.ApplicationData);
				if (!concatenateFragmentsInto(memoryStream))
				{
					return false;
				}
				byte[] data;
				if (_compression != 0)
				{
					data = memoryStream.DecompressToArray(_compression);
				}
				else
				{
					memoryStream.Close();
					data = memoryStream.ToArray();
				}
				this.OnMessage.Emit(this, new MessageEventArgs(first.Opcode, data));
				return true;
			}
		}

		private bool processFrame(WsFrame frame)
		{
			return (frame.IsCompressed && _compression == CompressionMethod.NONE) ? processUnsupportedFrame(frame, CloseStatusCode.INCORRECT_DATA, "A compressed data has been received without available decompression method.") : (frame.IsFragmented ? processFragmentedFrame(frame) : (frame.IsData ? processDataFrame(frame) : (frame.IsPing ? processPingFrame(frame) : (frame.IsPong ? processPongFrame() : ((!frame.IsClose) ? processUnsupportedFrame(frame, CloseStatusCode.POLICY_VIOLATION, null) : processCloseFrame(frame))))));
		}

		private bool processPingFrame(WsFrame frame)
		{
			if (send(WsFrame.CreatePongFrame(_client ? Mask.MASK : Mask.UNMASK, frame.PayloadData)))
			{
				_logger.Trace("Returned Pong.");
			}
			return true;
		}

		private bool processPongFrame()
		{
			_receivePong.Set();
			_logger.Trace("Received Pong.");
			return true;
		}

		private void processRequestedExtensions(string extensions)
		{
			bool flag = false;
			List<string> list = new List<string>();
			foreach (string item in extensions.SplitHeaderValue(','))
			{
				string text = item.Trim();
				string value = text.RemovePrefix("x-webkit-");
				if (!flag && value.IsCompressionExtension())
				{
					CompressionMethod compressionMethod = value.ToCompressionMethod();
					if (compressionMethod != 0)
					{
						_compression = compressionMethod;
						flag = true;
						list.Add(text);
					}
				}
			}
			if (list.Count > 0)
			{
				_extensions = list.ToArray().ToString(", ");
			}
		}

		private void processRespondedExtensions(string extensions)
		{
			bool flag = ((_compression != 0) ? true : false);
			bool flag2 = false;
			if (extensions != null && extensions.Length > 0)
			{
				foreach (string item in extensions.SplitHeaderValue(','))
				{
					string text = item.Trim();
					if (flag && !flag2 && text.Equals(_compression))
					{
						flag2 = true;
					}
				}
				_extensions = extensions;
			}
			if (flag && !flag2)
			{
				_compression = CompressionMethod.NONE;
			}
		}

		private bool processUnsupportedFrame(WsFrame frame, CloseStatusCode code, string reason)
		{
			_logger.Debug("Unsupported frame:\n" + frame.PrintToString(false));
			processException(new WebSocketException(code, reason), null);
			return false;
		}

		private HandshakeResponse receiveHandshakeResponse()
		{
			HandshakeResponse handshakeResponse = HandshakeResponse.Parse(_stream.ReadHandshake());
			_logger.Debug("A response to this WebSocket connection request:\n" + handshakeResponse.ToString());
			return handshakeResponse;
		}

		private bool send(byte[] frameAsBytes)
		{
			if (_readyState != WebSocketState.OPEN)
			{
				string message = "A WebSocket connection isn't established or has been closed.";
				_logger.Error(message);
				error(message);
				return false;
			}
			return _stream.Write(frameAsBytes);
		}

		private void send(HandshakeRequest request)
		{
			_logger.Debug(string.Format("A WebSocket connection request to {0}:\n{1}", _uri, request));
			_stream.WriteHandshake(request);
		}

		private bool send(HandshakeResponse response)
		{
			_logger.Debug("A response to a WebSocket connection request:\n" + response.ToString());
			return _stream.WriteHandshake(response);
		}

		private bool send(WsFrame frame)
		{
			if (_readyState != WebSocketState.OPEN)
			{
				string message = "A WebSocket connection isn't established or has been closed.";
				_logger.Error(message);
				error(message);
				return false;
			}
			return _stream.Write(frame.ToByteArray());
		}

		private bool send(Opcode opcode, byte[] data)
		{
			//Discarded unreachable code: IL_007f
			lock (_forSend)
			{
				bool result = false;
				try
				{
					bool compressed = false;
					if (_compression != 0)
					{
						data = data.Compress(_compression);
						compressed = true;
					}
					result = send(WsFrame.CreateFrame(Fin.FINAL, opcode, _client ? Mask.MASK : Mask.UNMASK, data, compressed));
				}
				catch (Exception ex)
				{
					_logger.Fatal(ex.ToString());
					error("An exception has occurred.");
				}
				return result;
			}
		}

		private bool send(Opcode opcode, Stream stream)
		{
			//Discarded unreachable code: IL_0090
			lock (_forSend)
			{
				bool result = false;
				Stream stream2 = stream;
				bool flag = false;
				try
				{
					if (_compression != 0)
					{
						stream = stream.Compress(_compression);
						flag = true;
					}
					result = sendFragmented(opcode, stream, _client ? Mask.MASK : Mask.UNMASK, flag);
				}
				catch (Exception ex)
				{
					_logger.Fatal(ex.ToString());
					error("An exception has occurred.");
				}
				finally
				{
					if (flag)
					{
						stream.Dispose();
					}
					stream2.Dispose();
				}
				return result;
			}
		}

		private void send(Opcode opcode, byte[] data, Action<bool> completed)
		{
			SendBytesFunc sender = send;
			AsyncCallback callback = delegate(IAsyncResult ar)
			{
				try
				{
					bool obj = sender.EndInvoke(ar);
					if (completed != null)
					{
						completed(obj);
					}
				}
				catch (Exception ex)
				{
					_logger.Fatal(ex.ToString());
					error("An exception has occurred.");
				}
			};
			sender.BeginInvoke(opcode, data, callback, null);
		}

		private void send(Opcode opcode, Stream stream, Action<bool> completed)
		{
			Func<Opcode, Stream, bool> sender = send;
			AsyncCallback callback = delegate(IAsyncResult ar)
			{
				try
				{
					bool obj = sender.EndInvoke(ar);
					if (completed != null)
					{
						completed(obj);
					}
				}
				catch (Exception ex)
				{
					_logger.Fatal(ex.ToString());
					error("An exception has occurred.");
				}
			};
			sender.BeginInvoke(opcode, stream, callback, null);
		}

		private bool sendFragmented(Opcode opcode, Stream stream, Mask mask, bool compressed)
		{
			long length = stream.Length;
			if (sendFragmented(opcode, stream, length, mask, compressed) == length)
			{
				return true;
			}
			string text = "Sending fragmented data is interrupted.";
			_logger.Error(text);
			error(text);
			close(CloseStatusCode.ABNORMAL, text, false);
			return false;
		}

		private long sendFragmented(Opcode opcode, Stream stream, long length, Mask mask, bool compressed)
		{
			long num = length / 1016;
			int num2 = (int)(length % 1016);
			long num3 = ((num2 != 0) ? (num - 1) : (num - 2));
			long result = 0L;
			int num4 = 0;
			byte[] array = null;
			if (num == 0L)
			{
				array = new byte[num2];
				num4 = stream.Read(array, 0, num2);
				if (num4 == num2 && send(WsFrame.CreateFrame(Fin.FINAL, opcode, mask, array, compressed)))
				{
					result = num4;
				}
				return result;
			}
			array = new byte[1016];
			num4 = stream.Read(array, 0, 1016);
			if (num4 == 1016 && send(WsFrame.CreateFrame(Fin.MORE, opcode, mask, array, compressed)))
			{
				result = num4;
				for (long num5 = 0L; num5 < num3; num5++)
				{
					num4 = stream.Read(array, 0, 1016);
					if (num4 == 1016 && send(WsFrame.CreateFrame(Fin.MORE, Opcode.CONT, mask, array, compressed)))
					{
						result += num4;
						continue;
					}
					return result;
				}
				int num6 = 1016;
				if (num2 != 0)
				{
					array = new byte[num6 = num2];
				}
				num4 = stream.Read(array, 0, num6);
				if (num4 == num6 && send(WsFrame.CreateFrame(Fin.FINAL, Opcode.CONT, mask, array, compressed)))
				{
					result += num4;
				}
				return result;
			}
			return result;
		}

		private HandshakeResponse sendHandshakeRequest()
		{
			HandshakeRequest handshakeRequest = createHandshakeRequest();
			HandshakeResponse handshakeResponse = sendHandshakeRequest(handshakeRequest);
			if (!_preAuth && handshakeResponse.IsUnauthorized && _credentials != null)
			{
				AuthenticationChallenge authChallenge = handshakeResponse.AuthChallenge;
				handshakeRequest.SetAuthorization(new AuthenticationResponse(_credentials, authChallenge));
				handshakeResponse = sendHandshakeRequest(handshakeRequest);
			}
			return handshakeResponse;
		}

		private HandshakeResponse sendHandshakeRequest(HandshakeRequest request)
		{
			send(request);
			return receiveHandshakeResponse();
		}

		private void setClientStream()
		{
			string dnsSafeHost = _uri.DnsSafeHost;
			int port = _uri.Port;
			_tcpClient = SocketUtilsFactory.Utils.CreateTcpClient(dnsSafeHost, port);
			_stream = WsStream.CreateClientStream(_tcpClient, _secure, dnsSafeHost, _certValidationCallback);
		}

		private void startReceiving()
		{
			_exitReceiving = new AutoResetEvent(false);
			_receivePong = new AutoResetEvent(false);
			Action receive = null;
			receive = delegate
			{
				_stream.ReadFrameAsync(delegate(WsFrame frame)
				{
					if (processFrame(frame))
					{
						receive();
					}
					else
					{
						_exitReceiving.Set();
					}
				}, delegate(Exception ex)
				{
					processException(ex, "An exception has occurred while receiving a message.");
				});
			};
			receive();
		}

		private bool validateConnectionRequest(WebSocketContext context)
		{
			string secWebSocketVersion;
			return context.IsWebSocketRequest && validateHostHeader(context.Host) && !context.SecWebSocketKey.IsNullOrEmpty() && (secWebSocketVersion = context.SecWebSocketVersion) != null && secWebSocketVersion == "13" && validateCookies(context.CookieCollection, _cookies);
		}

		private bool validateConnectionResponse(HandshakeResponse response)
		{
			string text;
			string text2;
			return response.IsWebSocketResponse && (text = response.Headers["Sec-WebSocket-Accept"]) != null && text == createResponseKey() && ((text2 = response.Headers["Sec-WebSocket-Version"]) == null || text2 == "13");
		}

		private bool validateCookies(CookieCollection request, CookieCollection response)
		{
			return _cookiesValidation == null || _cookiesValidation(request, response);
		}

		private bool validateHostHeader(string value)
		{
			if (value == null || value.Length == 0)
			{
				return false;
			}
			if (!_uri.IsAbsoluteUri)
			{
				return true;
			}
			int num = value.IndexOf(':');
			string text = ((num <= 0) ? value : value.Substring(0, num));
			UriHostNameType uriHostNameType = Uri.CheckHostName(text);
			return uriHostNameType != UriHostNameType.Dns || Uri.CheckHostName(_uri.DnsSafeHost) != UriHostNameType.Dns || text == _uri.DnsSafeHost;
		}

		internal void Close(HttpStatusCode code)
		{
			_readyState = WebSocketState.CLOSING;
			send(createHandshakeResponse(code));
			closeServerResources();
			_readyState = WebSocketState.CLOSED;
		}

		internal void Close(CloseEventArgs args, byte[] frameAsBytes, int waitTimeOut)
		{
			lock (_forClose)
			{
				if (_readyState == WebSocketState.CLOSING || _readyState == WebSocketState.CLOSED)
				{
					return;
				}
				_readyState = WebSocketState.CLOSING;
			}
			args.WasClean = close(frameAsBytes, waitTimeOut, closeServerResources);
			_readyState = WebSocketState.CLOSED;
			this.OnClose.Emit(this, args);
		}

		internal bool Ping(byte[] frameAsBytes, int timeOut)
		{
			return send(frameAsBytes) && _receivePong.WaitOne(timeOut);
		}

		internal void Send(Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
		{
			lock (_forSend)
			{
				try
				{
					byte[] value;
					if (!cache.TryGetValue(_compression, out value))
					{
						value = WsFrame.CreateFrame(Fin.FINAL, opcode, Mask.UNMASK, data.Compress(_compression), _compression != CompressionMethod.NONE).ToByteArray();
						cache.Add(_compression, value);
					}
					send(value);
				}
				catch (Exception ex)
				{
					_logger.Fatal(ex.ToString());
					error("An exception has occurred.");
				}
			}
		}

		internal void Send(Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache)
		{
			lock (_forSend)
			{
				try
				{
					Stream value;
					if (!cache.TryGetValue(_compression, out value))
					{
						value = stream.Compress(_compression);
						cache.Add(_compression, value);
					}
					else
					{
						value.Position = 0L;
					}
					sendFragmented(opcode, value, Mask.UNMASK, _compression != CompressionMethod.NONE);
				}
				catch (Exception ex)
				{
					_logger.Fatal(ex.ToString());
					error("An exception has occurred.");
				}
			}
		}

		public void Close()
		{
			close(new PayloadData(), _readyState == WebSocketState.OPEN, true);
		}

		public void Close(ushort code)
		{
			string text = code.CheckIfValidCloseStatusCode();
			if (text != null)
			{
				_logger.Error(string.Format("{0}\ncode: {1}", text, code));
				error(text);
			}
			else
			{
				bool flag = _readyState == WebSocketState.OPEN && !code.IsReserved();
				close(new PayloadData(code.ToByteArrayInternally(ByteOrder.BIG)), flag, true);
			}
		}

		public void Close(CloseStatusCode code)
		{
			bool flag = _readyState == WebSocketState.OPEN && !code.IsReserved();
			close(new PayloadData(((ushort)code).ToByteArrayInternally(ByteOrder.BIG)), flag, true);
		}

		public void Close(ushort code, string reason)
		{
			byte[] appData = null;
			string text = code.CheckIfValidCloseStatusCode() ?? (appData = code.Append(reason)).CheckIfValidCloseData();
			if (text != null)
			{
				_logger.Error(string.Format("{0}\ncode: {1}\nreason: {2}", text, code, reason));
				error(text);
			}
			else
			{
				bool flag = _readyState == WebSocketState.OPEN && !code.IsReserved();
				close(new PayloadData(appData), flag, true);
			}
		}

		public void Close(CloseStatusCode code, string reason)
		{
			byte[] array = ((ushort)code).Append(reason);
			string text = array.CheckIfValidCloseData();
			if (text != null)
			{
				_logger.Error(string.Format("{0}\nreason: {1}", text, reason));
				error(text);
			}
			else
			{
				bool flag = _readyState == WebSocketState.OPEN && !code.IsReserved();
				close(new PayloadData(array), flag, true);
			}
		}

		public void Connect()
		{
			if (IsOpened)
			{
				string message = "A WebSocket connection has already been established.";
				_logger.Error(message);
				error(message);
				return;
			}
			try
			{
				if (connect())
				{
					open();
				}
			}
			catch (Exception exception)
			{
				processException(exception, "An exception has occurred while connecting or opening.");
			}
		}

		public void Dispose()
		{
			Close(CloseStatusCode.AWAY);
		}

		public bool Ping()
		{
			return (!_client) ? Ping(WsFrame.EmptyUnmaskPingData, 1000) : Ping(WsFrame.CreatePingFrame(Mask.MASK).ToByteArray(), 5000);
		}

		public bool Ping(string message)
		{
			if (message == null || message.Length == 0)
			{
				return Ping();
			}
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			string text = bytes.CheckIfValidPingData();
			if (text != null)
			{
				_logger.Error(text);
				error(text);
				return false;
			}
			return (!_client) ? Ping(WsFrame.CreatePingFrame(Mask.UNMASK, bytes).ToByteArray(), 1000) : Ping(WsFrame.CreatePingFrame(Mask.MASK, bytes).ToByteArray(), 5000);
		}

		public void Send(byte[] data)
		{
			Send(data, null);
		}

		public void Send(FileInfo file)
		{
			Send(file, null);
		}

		public void Send(string data)
		{
			Send(data, null);
		}

		public void Send(byte[] data, Action<bool> completed)
		{
			string text = _readyState.CheckIfOpen() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				error(text);
				return;
			}
			long longLength = data.LongLength;
			if (longLength <= 1016)
			{
				send(Opcode.BINARY, (longLength <= 0 || !_client || _compression != 0) ? data : data.Copy(longLength), completed);
			}
			else
			{
				send(Opcode.BINARY, new MemoryStream(data), completed);
			}
		}

		public void Send(FileInfo file, Action<bool> completed)
		{
			string text = _readyState.CheckIfOpen() ?? ((file != null) ? null : "'file' must not be null.");
			if (text != null)
			{
				_logger.Error(text);
				error(text);
			}
			else
			{
				send(Opcode.BINARY, file.OpenRead(), completed);
			}
		}

		public void Send(string data, Action<bool> completed)
		{
			string text = _readyState.CheckIfOpen() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				error(text);
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			if (bytes.LongLength <= 1016)
			{
				send(Opcode.TEXT, bytes, completed);
			}
			else
			{
				send(Opcode.TEXT, new MemoryStream(bytes), completed);
			}
		}

		public void Send(Stream stream, int length)
		{
			Send(stream, length, null);
		}

		public void Send(Stream stream, int length, Action<bool> completed)
		{
			string text = _readyState.CheckIfOpen() ?? stream.CheckIfCanRead() ?? ((length >= 1) ? null : "'length' must be greater than 0.");
			if (text != null)
			{
				_logger.Error(text);
				error(text);
				return;
			}
			stream.ReadBytesAsync(length, delegate(byte[] data)
			{
				int num = data.Length;
				if (num == 0)
				{
					string message = "A data cannot be read from 'stream'.";
					_logger.Error(message);
					error(message);
				}
				else
				{
					if (num < length)
					{
						_logger.Warn(string.Format("A data with 'length' cannot be read from 'stream'.\nexpected: {0} actual: {1}", length, num));
					}
					bool obj = ((num > 1016) ? send(Opcode.BINARY, new MemoryStream(data)) : send(Opcode.BINARY, data));
					if (completed != null)
					{
						completed(obj);
					}
				}
			}, delegate(Exception ex)
			{
				_logger.Fatal(ex.ToString());
				error("An exception has occurred.");
			});
		}

		public void SetCookie(Cookie cookie)
		{
			string text = (IsOpened ? "A WebSocket connection has already been established." : ((cookie != null) ? null : "'cookie' must not be null."));
			if (text != null)
			{
				_logger.Error(text);
				error(text);
				return;
			}
			lock (_cookies.SyncRoot)
			{
				_cookies.SetOrRemove(cookie);
			}
		}

		public void SetCredentials(string userName, string password, bool preAuth)
		{
			string text = null;
			if (IsOpened)
			{
				text = "A WebSocket connection has already been established.";
			}
			else
			{
				if (userName == null)
				{
					_credentials = null;
					_preAuth = false;
					return;
				}
				text = ((userName.Length > 0 && (userName.Contains(':') || !userName.IsText())) ? "'userName' contains an invalid character." : ((password.IsNullOrEmpty() || password.IsText()) ? null : "'password' contains an invalid character."));
			}
			if (text != null)
			{
				_logger.Error(text);
				error(text);
			}
			else
			{
				_credentials = new WsCredential(userName, password, _uri.PathAndQuery);
				_preAuth = preAuth;
			}
		}
	}
}
