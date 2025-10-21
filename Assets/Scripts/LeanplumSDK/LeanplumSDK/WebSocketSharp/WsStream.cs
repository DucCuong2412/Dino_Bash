using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using LeanplumSDK.WebSocketSharp.Net;
using LeanplumSDK.WebSocketSharp.Net.Security;

namespace LeanplumSDK.WebSocketSharp
{
	internal class WsStream : IDisposable
	{
		private const int _handshakeLimitLen = 8192;

		private const int _handshakeTimeout = 90000;

		private object _forWrite;

		private Stream _innerStream;

		private bool _secure;

		public bool DataAvailable
		{
			get
			{
				return (!_secure) ? SocketUtilsFactory.Utils.IsDataAvailable(_innerStream) : ((LeanplumSDK.WebSocketSharp.Net.Security.SslStream)_innerStream).DataAvailable;
			}
		}

		public bool IsSecure
		{
			get
			{
				return _secure;
			}
		}

		private WsStream(Stream innerStream, bool secure)
		{
			_innerStream = innerStream;
			_secure = secure;
			_forWrite = new object();
		}

		internal WsStream(Stream innerStream)
			: this(innerStream, false)
		{
		}

		internal WsStream(LeanplumSDK.WebSocketSharp.Net.Security.SslStream innerStream)
			: this(innerStream, true)
		{
		}

		internal static WsStream CreateClientStream(ITcpClient client, bool secure, string host, RemoteCertificateValidationCallback validationCallback)
		{
			Stream stream = client.GetStream();
			if (secure)
			{
				if (validationCallback == null)
				{
					validationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
				}
				LeanplumSDK.WebSocketSharp.Net.Security.SslStream sslStream = new LeanplumSDK.WebSocketSharp.Net.Security.SslStream(stream, false, validationCallback);
				sslStream.AuthenticateAsClient(host);
				return new WsStream(sslStream);
			}
			return new WsStream(stream);
		}

		internal static WsStream CreateServerStream(ITcpClient client, bool secure, X509Certificate cert)
		{
			Stream stream = client.GetStream();
			if (secure)
			{
				LeanplumSDK.WebSocketSharp.Net.Security.SslStream sslStream = new LeanplumSDK.WebSocketSharp.Net.Security.SslStream(stream, false);
				sslStream.AuthenticateAsServer(cert);
				return new WsStream(sslStream);
			}
			return new WsStream(stream);
		}

		internal static WsStream CreateServerStream(HttpListenerContext context)
		{
			HttpConnection connection = context.Connection;
			return new WsStream(connection.Stream, connection.IsSecure);
		}

		internal bool Write(byte[] data)
		{
			//Discarded unreachable code: IL_0024, IL_0031, IL_0036
			lock (_forWrite)
			{
				try
				{
					_innerStream.Write(data, 0, data.Length);
					return true;
				}
				catch
				{
					return false;
				}
			}
		}

		public void Close()
		{
			_innerStream.Close();
		}

		public void Dispose()
		{
			_innerStream.Dispose();
		}

		public WsFrame ReadFrame()
		{
			return WsFrame.Parse(_innerStream, true);
		}

		public void ReadFrameAsync(Action<WsFrame> completed, Action<Exception> error)
		{
			WsFrame.ParseAsync(_innerStream, true, completed, error);
		}

		public string[] ReadHandshake()
		{
			bool flag = false;
			bool flag2 = false;
			List<byte> buffer = new List<byte>();
			Action<int> action = delegate(int i)
			{
				buffer.Add((byte)i);
			};
			bool timeout = false;
			Timer timer = new Timer(delegate
			{
				timeout = true;
				_innerStream.Close();
			}, null, 90000, -1);
			try
			{
				while (buffer.Count < 8192)
				{
					if (_innerStream.ReadByte().EqualsWith('\r', action) && _innerStream.ReadByte().EqualsWith('\n', action) && _innerStream.ReadByte().EqualsWith('\r', action) && _innerStream.ReadByte().EqualsWith('\n', action))
					{
						flag = true;
						break;
					}
				}
			}
			catch
			{
				flag2 = true;
			}
			finally
			{
				timer.Change(-1, -1);
				timer.Dispose();
			}
			string text = (timeout ? "A timeout has occurred while receiving a handshake." : (flag2 ? "An exception has occurred while receiving a handshake." : (flag ? null : "A handshake length is greater than the limit length.")));
			if (text != null)
			{
				throw new WebSocketException(text);
			}
			return Encoding.UTF8.GetString(buffer.ToArray()).Replace("\r\n", "\n").Replace("\n ", " ")
				.Replace("\n\t", " ")
				.TrimEnd('\n')
				.Split('\n');
		}

		public bool WriteFrame(WsFrame frame)
		{
			return Write(frame.ToByteArray());
		}

		public bool WriteHandshake(HandshakeBase handshake)
		{
			return Write(handshake.ToByteArray());
		}
	}
}
