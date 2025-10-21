using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace LeanplumSDK.WebSocketSharp.Net.WebSockets
{
	internal class TcpListenerWebSocketContext : WebSocketContext
	{
		private CookieCollection _cookies;

		private ITcpClient _client;

		private HandshakeRequest _request;

		private bool _secure;

		private WsStream _stream;

		private WebSocket _websocket;

		internal WsStream Stream
		{
			get
			{
				return _stream;
			}
		}

		public override CookieCollection CookieCollection
		{
			get
			{
				if (_cookies == null)
				{
					_cookies = _request.Cookies;
				}
				return _cookies;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return _request.Headers;
			}
		}

		public override string Host
		{
			get
			{
				return _request.Headers["Host"];
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool IsLocal
		{
			get
			{
				return UserEndPoint.Address.IsLocal();
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return _secure;
			}
		}

		public override bool IsWebSocketRequest
		{
			get
			{
				return _request.IsWebSocketRequest;
			}
		}

		public override string Origin
		{
			get
			{
				return _request.Headers["Origin"];
			}
		}

		public override string Path
		{
			get
			{
				return _request.RequestUri.GetAbsolutePath();
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return _request.QueryString;
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return _request.RequestUri;
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return _request.Headers["Sec-WebSocket-Key"];
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				return _request.Headers.GetValues("Sec-WebSocket-Protocol");
			}
		}

		public override string SecWebSocketVersion
		{
			get
			{
				return _request.Headers["Sec-WebSocket-Version"];
			}
		}

		public override IPEndPoint ServerEndPoint
		{
			get
			{
				return (IPEndPoint)_client.GetLocalEndpoint();
			}
		}

		public override IPrincipal User
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override IPEndPoint UserEndPoint
		{
			get
			{
				return (IPEndPoint)_client.GetRemoteEndpoint();
			}
		}

		public override WebSocket WebSocket
		{
			get
			{
				return _websocket;
			}
		}

		internal TcpListenerWebSocketContext(ITcpClient client, bool secure, X509Certificate cert)
		{
			_client = client;
			_secure = secure;
			_stream = WsStream.CreateServerStream(client, secure, cert);
			_request = HandshakeRequest.Parse(_stream.ReadHandshake());
			_websocket = new WebSocket(this);
		}

		internal void Close()
		{
			_stream.Close();
			_client.Close();
		}

		public override string ToString()
		{
			return _request.ToString();
		}
	}
}
