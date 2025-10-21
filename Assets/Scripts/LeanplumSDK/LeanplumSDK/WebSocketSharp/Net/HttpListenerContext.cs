using System;
using System.Net;
using System.Security.Principal;
using System.Text;
using LeanplumSDK.WebSocketSharp.Net.WebSockets;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal sealed class HttpListenerContext
	{
		private HttpConnection _connection;

		private string _error;

		private int _errorStatus;

		private HttpListenerRequest _request;

		private HttpListenerResponse _response;

		private IPrincipal _user;

		internal HttpListener Listener;

		internal HttpConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				return _error;
			}
			set
			{
				_error = value;
			}
		}

		internal int ErrorStatus
		{
			get
			{
				return _errorStatus;
			}
			set
			{
				_errorStatus = value;
			}
		}

		internal bool HaveError
		{
			get
			{
				return _error != null;
			}
		}

		public HttpListenerRequest Request
		{
			get
			{
				return _request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return _response;
			}
		}

		public IPrincipal User
		{
			get
			{
				return _user;
			}
		}

		internal HttpListenerContext(HttpConnection connection)
		{
			_connection = connection;
			_errorStatus = 400;
			_request = new HttpListenerRequest(this);
			_response = new HttpListenerResponse(this);
		}

		internal void ParseAuthentication(AuthenticationSchemes expectedSchemes)
		{
			if (expectedSchemes == AuthenticationSchemes.Anonymous)
			{
				return;
			}
			string text = _request.Headers["Authorization"];
			if (text != null && text.Length >= 2)
			{
				string[] array = text.Split(new char[1] { ' ' }, 2);
				if (array[0].ToLower() == "basic")
				{
					_user = ParseBasicAuthentication(array[1]);
				}
			}
		}

		internal IPrincipal ParseBasicAuthentication(string authData)
		{
			//Discarded unreachable code: IL_0064, IL_0072
			try
			{
				string @string = Encoding.Default.GetString(Convert.FromBase64String(authData));
				int num = @string.IndexOf(':');
				string text = @string.Substring(0, num);
				string password = @string.Substring(num + 1);
				num = text.IndexOf('\\');
				if (num > 0)
				{
					text = text.Substring(num + 1);
				}
				HttpListenerBasicIdentity identity = new HttpListenerBasicIdentity(text, password);
				return new GenericPrincipal(identity, new string[0]);
			}
			catch
			{
				return null;
			}
		}

		public HttpListenerWebSocketContext AcceptWebSocket()
		{
			return new HttpListenerWebSocketContext(this);
		}
	}
}
