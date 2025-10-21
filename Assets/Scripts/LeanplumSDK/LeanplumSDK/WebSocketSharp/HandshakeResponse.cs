using System;
using System.Text;
using LeanplumSDK.WebSocketSharp.Net;

namespace LeanplumSDK.WebSocketSharp
{
	internal class HandshakeResponse : HandshakeBase
	{
		public AuthenticationChallenge AuthChallenge
		{
			get
			{
				string text = base.Headers["WWW-Authenticate"];
				return text.IsNullOrEmpty() ? null : AuthenticationChallenge.Parse(text);
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				return base.Headers.GetCookies(true);
			}
		}

		public bool IsUnauthorized
		{
			get
			{
				return StatusCode == "401";
			}
		}

		public bool IsWebSocketResponse
		{
			get
			{
				return base.ProtocolVersion >= HttpVersion.Version11 && StatusCode == "101" && base.Headers.Contains("Upgrade", "websocket") && base.Headers.Contains("Connection", "Upgrade");
			}
		}

		public string Reason { get; private set; }

		public string StatusCode { get; private set; }

		public HandshakeResponse()
			: this(HttpStatusCode.SwitchingProtocols)
		{
			AddHeader("Upgrade", "websocket");
			AddHeader("Connection", "Upgrade");
		}

		public HandshakeResponse(HttpStatusCode code)
		{
			int num = (int)code;
			StatusCode = num.ToString();
			Reason = code.GetDescription();
			AddHeader("Server", "websocket-sharp/1.0");
		}

		public static HandshakeResponse CreateCloseResponse(HttpStatusCode code)
		{
			HandshakeResponse handshakeResponse = new HandshakeResponse(code);
			handshakeResponse.AddHeader("Connection", "close");
			return handshakeResponse;
		}

		public static HandshakeResponse Parse(string[] response)
		{
			string[] array = response[0].Split(' ');
			if (array.Length < 3)
			{
				throw new ArgumentException("Invalid status line.");
			}
			StringBuilder stringBuilder = new StringBuilder(array[2], 64);
			for (int i = 3; i < array.Length; i++)
			{
				stringBuilder.AppendFormat(" {0}", array[i]);
			}
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			for (int j = 1; j < response.Length; j++)
			{
				webHeaderCollection.SetInternal(response[j], true);
			}
			HandshakeResponse handshakeResponse = new HandshakeResponse();
			handshakeResponse.Headers = webHeaderCollection;
			handshakeResponse.Reason = stringBuilder.ToString();
			handshakeResponse.StatusCode = array[1];
			handshakeResponse.ProtocolVersion = new Version(array[0].Substring(5));
			return handshakeResponse;
		}

		public void SetCookies(CookieCollection cookies)
		{
			if (cookies == null || cookies.Count == 0)
			{
				return;
			}
			foreach (Cookie item in cookies.Sorted)
			{
				AddHeader("Set-Cookie", item.ToResponseString());
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("HTTP/{0} {1} {2}{3}", base.ProtocolVersion, StatusCode, Reason, "\r\n");
			string[] allKeys = base.Headers.AllKeys;
			foreach (string text in allKeys)
			{
				stringBuilder.AppendFormat("{0}: {1}{2}", text, base.Headers[text], "\r\n");
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}
	}
}
