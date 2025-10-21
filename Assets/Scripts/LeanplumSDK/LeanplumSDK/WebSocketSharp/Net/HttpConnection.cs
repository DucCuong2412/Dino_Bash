using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using LeanplumSDK.WebSocketSharp.Net.Security;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal sealed class HttpConnection
	{
		private enum InputState
		{
			RequestLine = 0,
			Headers = 1
		}

		private enum LineState
		{
			None = 0,
			CR = 1,
			LF = 2
		}

		private const int BufferSize = 8192;

		private byte[] _buffer;

		private bool _chunked;

		private HttpListenerContext _context;

		private bool _contextWasBound;

		private StringBuilder _currentLine;

		private EndPointListener _epListener;

		private InputState _inputState;

		private RequestStream _inputStream;

		private HttpListener _lastListener;

		private LineState _lineState;

		private ResponseStream _outputStream;

		private int _position;

		private ListenerPrefix _prefix;

		private MemoryStream _requestBuffer;

		private int _reuses;

		private bool _secure;

		private ISocket _socket;

		private Stream _stream;

		private int _timeout;

		private Timer _timer;

		public bool IsClosed
		{
			get
			{
				return _socket == null;
			}
		}

		public bool IsSecure
		{
			get
			{
				return _secure;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return (IPEndPoint)_socket.LocalEndPoint;
			}
		}

		public ListenerPrefix Prefix
		{
			get
			{
				return _prefix;
			}
			set
			{
				_prefix = value;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return (IPEndPoint)_socket.RemoteEndPoint;
			}
		}

		public int Reuses
		{
			get
			{
				return _reuses;
			}
		}

		public Stream Stream
		{
			get
			{
				return _stream;
			}
		}

		public HttpConnection(ISocket socket, EndPointListener listener, bool secure, X509Certificate2 cert)
		{
			_socket = socket;
			_epListener = listener;
			_secure = secure;
			Stream stream = SocketUtilsFactory.Utils.CreateNetworkStream(socket, false);
			if (!secure)
			{
				_stream = stream;
			}
			else
			{
				SslStream sslStream = new SslStream(stream, false);
				sslStream.AuthenticateAsServer(cert);
				_stream = sslStream;
			}
			_timer = new Timer(OnTimeout, null, -1, -1);
			Init();
		}

		private void CloseSocket()
		{
			if (_socket == null)
			{
				return;
			}
			try
			{
				_socket.Close();
			}
			catch
			{
			}
			finally
			{
				_socket = null;
			}
			RemoveConnection();
		}

		private void Init()
		{
			_chunked = false;
			_context = new HttpListenerContext(this);
			_contextWasBound = false;
			_inputState = InputState.RequestLine;
			_inputStream = null;
			_lineState = LineState.None;
			_outputStream = null;
			_position = 0;
			_prefix = null;
			_requestBuffer = new MemoryStream();
			_timeout = 90000;
		}

		private static void OnRead(IAsyncResult asyncResult)
		{
			HttpConnection httpConnection = (HttpConnection)asyncResult.AsyncState;
			httpConnection.OnReadInternal(asyncResult);
		}

		private void OnReadInternal(IAsyncResult asyncResult)
		{
			//Discarded unreachable code: IL_009d
			_timer.Change(-1, -1);
			int num = -1;
			try
			{
				num = _stream.EndRead(asyncResult);
				_requestBuffer.Write(_buffer, 0, num);
				if (_requestBuffer.Length > 32768)
				{
					SendError();
					Close(true);
					return;
				}
			}
			catch
			{
				if (_requestBuffer != null && _requestBuffer.Length > 0)
				{
					SendError();
				}
				if (_socket != null)
				{
					CloseSocket();
					Unbind();
				}
				return;
			}
			if (num == 0)
			{
				CloseSocket();
				Unbind();
			}
			else if (ProcessInput(_requestBuffer.GetBuffer()))
			{
				if (!_context.HaveError)
				{
					_context.Request.FinishInitialization();
				}
				if (_context.HaveError)
				{
					SendError();
					Close(true);
					return;
				}
				if (!_epListener.BindContext(_context))
				{
					SendError("Invalid host", 400);
					Close(true);
					return;
				}
				HttpListener listener = _context.Listener;
				if (_lastListener != listener)
				{
					RemoveConnection();
					listener.AddConnection(this);
					_lastListener = listener;
				}
				listener.RegisterContext(_context);
				_contextWasBound = true;
			}
			else
			{
				_stream.BeginRead(_buffer, 0, 8192, OnRead, this);
			}
		}

		private void OnTimeout(object unused)
		{
			CloseSocket();
			Unbind();
		}

		private bool ProcessInput(byte[] data)
		{
			//Discarded unreachable code: IL_00d1
			int num = data.Length;
			int used = 0;
			try
			{
				string text;
				while ((text = ReadLine(data, _position, num - _position, ref used)) != null)
				{
					_position += used;
					if (text.Length == 0)
					{
						if (_inputState == InputState.RequestLine)
						{
							continue;
						}
						_currentLine = null;
						return true;
					}
					if (_inputState == InputState.RequestLine)
					{
						_context.Request.SetRequestLine(text);
						_inputState = InputState.Headers;
					}
					else
					{
						_context.Request.AddHeader(text);
					}
					if (!_context.HaveError)
					{
						continue;
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				_context.ErrorMessage = ex.Message;
				return true;
			}
			_position += used;
			if (used == num)
			{
				_requestBuffer.SetLength(0L);
				_position = 0;
			}
			return false;
		}

		private string ReadLine(byte[] buffer, int offset, int length, ref int used)
		{
			if (_currentLine == null)
			{
				_currentLine = new StringBuilder();
			}
			int num = offset + length;
			used = 0;
			for (int i = offset; i < num; i++)
			{
				if (_lineState == LineState.LF)
				{
					break;
				}
				used++;
				byte b = buffer[i];
				switch (b)
				{
				case 13:
					_lineState = LineState.CR;
					break;
				case 10:
					_lineState = LineState.LF;
					break;
				default:
					_currentLine.Append((char)b);
					break;
				}
			}
			string result = null;
			if (_lineState == LineState.LF)
			{
				_lineState = LineState.None;
				result = _currentLine.ToString();
				_currentLine.Length = 0;
			}
			return result;
		}

		private void RemoveConnection()
		{
			if (_lastListener == null)
			{
				_epListener.RemoveConnection(this);
			}
			else
			{
				_lastListener.RemoveConnection(this);
			}
		}

		private void Unbind()
		{
			if (_contextWasBound)
			{
				_epListener.UnbindContext(_context);
				_contextWasBound = false;
			}
		}

		internal void Close(bool force)
		{
			if (_socket == null)
			{
				return;
			}
			if (_outputStream != null)
			{
				_outputStream.Close();
				_outputStream = null;
			}
			force |= !_context.Request.KeepAlive;
			if (!force)
			{
				force = _context.Response.Headers["Connection"] == "close";
			}
			if (!force && _context.Request.FlushInput() && _chunked && !_context.Response.ForceCloseChunked)
			{
				_reuses++;
				Unbind();
				Init();
				BeginReadRequest();
				return;
			}
			ISocket socket = _socket;
			_socket = null;
			try
			{
				if (socket != null)
				{
					socket.Shutdown();
				}
			}
			catch
			{
			}
			finally
			{
				if (socket != null)
				{
					socket.Close();
				}
			}
			Unbind();
			RemoveConnection();
		}

		public void BeginReadRequest()
		{
			if (_buffer == null)
			{
				_buffer = new byte[8192];
			}
			try
			{
				if (_reuses == 1)
				{
					_timeout = 15000;
				}
				_timer.Change(_timeout, -1);
				_stream.BeginRead(_buffer, 0, 8192, OnRead, this);
			}
			catch
			{
				_timer.Change(-1, -1);
				CloseSocket();
				Unbind();
			}
		}

		public void Close()
		{
			Close(false);
		}

		public RequestStream GetRequestStream(bool chunked, long contentlength)
		{
			if (_inputStream == null)
			{
				byte[] buffer = _requestBuffer.GetBuffer();
				int num = buffer.Length;
				_requestBuffer = null;
				if (chunked)
				{
					_chunked = true;
					_context.Response.SendChunked = true;
					_inputStream = new ChunkedInputStream(_context, _stream, buffer, _position, num - _position);
				}
				else
				{
					_inputStream = new RequestStream(_stream, buffer, _position, num - _position, contentlength);
				}
			}
			return _inputStream;
		}

		public ResponseStream GetResponseStream()
		{
			if (_outputStream == null)
			{
				HttpListener listener = _context.Listener;
				bool ignore_errors = listener == null || listener.IgnoreWriteExceptions;
				_outputStream = new ResponseStream(_stream, _context.Response, ignore_errors);
			}
			return _outputStream;
		}

		public void SendError()
		{
			SendError(_context.ErrorMessage, _context.ErrorStatus);
		}

		public void SendError(string message, int status)
		{
			try
			{
				HttpListenerResponse response = _context.Response;
				response.StatusCode = status;
				response.ContentType = "text/html";
				string statusDescription = status.GetStatusDescription();
				string s = (message.IsNullOrEmpty() ? string.Format("<h1>{0}</h1>", statusDescription) : string.Format("<h1>{0} ({1})</h1>", statusDescription, message));
				byte[] bytes = _context.Response.ContentEncoding.GetBytes(s);
				response.Close(bytes, false);
			}
			catch
			{
			}
		}
	}
}
