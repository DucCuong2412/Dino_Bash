using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security.Principal;

namespace LeanplumSDK.WebSocketSharp.Net.WebSockets
{
	internal class HttpListenerWebSocketContext : WebSocketContext
	{
		private HttpListenerContext _context;

		private WebSocket _websocket;

		private WsStream _stream;

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
				return _context.Request.Cookies;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return _context.Request.Headers;
			}
		}

		public override string Host
		{
			get
			{
				return _context.Request.Headers["Host"];
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return _context.Request.IsAuthenticated;
			}
		}

		public override bool IsLocal
		{
			get
			{
				return _context.Request.IsLocal;
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return _context.Request.IsSecureConnection;
			}
		}

		public override bool IsWebSocketRequest
		{
			get
			{
				return _context.Request.IsWebSocketRequest;
			}
		}

		public override string Origin
		{
			get
			{
				return _context.Request.Headers["Origin"];
			}
		}

		public override string Path
		{
			get
			{
				return RequestUri.GetAbsolutePath();
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return _context.Request.QueryString;
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return _context.Request.RawUrl.ToUri();
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return _context.Request.Headers["Sec-WebSocket-Key"];
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				return _context.Request.Headers.GetValues("Sec-WebSocket-Protocol");
			}
		}

		public override string SecWebSocketVersion
		{
			get
			{
				return _context.Request.Headers["Sec-WebSocket-Version"];
			}
		}

		public override IPEndPoint ServerEndPoint
		{
			get
			{
				return _context.Connection.LocalEndPoint;
			}
		}

		public override IPrincipal User
		{
			get
			{
				return _context.User;
			}
		}

		public override IPEndPoint UserEndPoint
		{
			get
			{
				return _context.Connection.RemoteEndPoint;
			}
		}

		public override WebSocket WebSocket
		{
			get
			{
				return _websocket;
			}
		}

		internal HttpListenerWebSocketContext(HttpListenerContext context)
		{
			_context = context;
			_stream = WsStream.CreateServerStream(context);
			_websocket = new WebSocket(this);
		}

		internal void Close()
		{
			_context.Connection.Close(true);
		}

		public override string ToString()
		{
			return _context.Request.ToString();
		}
	}
}
