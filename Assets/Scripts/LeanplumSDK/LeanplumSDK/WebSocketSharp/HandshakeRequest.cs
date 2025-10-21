using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using LeanplumSDK.WebSocketSharp.Net;

namespace LeanplumSDK.WebSocketSharp
{
	internal class HandshakeRequest : HandshakeBase
	{
		private NameValueCollection _queryString;

		public CookieCollection Cookies
		{
			get
			{
				return base.Headers.GetCookies(false);
			}
		}

		public string HttpMethod { get; private set; }

		public bool IsWebSocketRequest
		{
			get
			{
				return HttpMethod == "GET" && base.ProtocolVersion >= HttpVersion.Version11 && base.Headers.Contains("Upgrade", "websocket") && base.Headers.Contains("Connection", "Upgrade");
			}
		}

		public NameValueCollection QueryString
		{
			get
			{
				if (_queryString == null)
				{
					_queryString = new NameValueCollection();
					int num = RawUrl.IndexOf('?');
					if (num > 0)
					{
						string text = RawUrl.Substring(num + 1);
						string[] array = text.Split('&');
						string[] array2 = array;
						foreach (string nameAndValue in array2)
						{
							KeyValuePair<string, string> nameAndValue2 = nameAndValue.GetNameAndValue("=");
							if (nameAndValue2.Key != null)
							{
								string name = nameAndValue2.Key.UrlDecode();
								string val = nameAndValue2.Value.UrlDecode();
								_queryString.Add(name, val);
							}
						}
					}
				}
				return _queryString;
			}
		}

		public string RawUrl
		{
			get
			{
				return (!RequestUri.IsAbsoluteUri) ? RequestUri.OriginalString : RequestUri.PathAndQuery;
			}
		}

		public Uri RequestUri { get; private set; }

		private HandshakeRequest()
		{
		}

		public HandshakeRequest(string uriString)
		{
			HttpMethod = "GET";
			RequestUri = uriString.ToUri();
			AddHeader("User-Agent", "websocket-sharp/1.0");
			AddHeader("Upgrade", "websocket");
			AddHeader("Connection", "Upgrade");
		}

		public static HandshakeRequest Parse(string[] request)
		{
			string[] array = request[0].Split(' ');
			if (array.Length != 3)
			{
				string message = "Invalid HTTP Request-Line: " + request[0];
				throw new ArgumentException(message, "request");
			}
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			for (int i = 1; i < request.Length; i++)
			{
				webHeaderCollection.SetInternal(request[i], false);
			}
			HandshakeRequest handshakeRequest = new HandshakeRequest();
			handshakeRequest.Headers = webHeaderCollection;
			handshakeRequest.HttpMethod = array[0];
			handshakeRequest.RequestUri = array[1].ToUri();
			handshakeRequest.ProtocolVersion = new Version(array[2].Substring(5));
			return handshakeRequest;
		}

		public void SetCookies(CookieCollection cookies)
		{
			if (cookies == null || cookies.Count == 0)
			{
				return;
			}
			Cookie[] array = cookies.Sorted.ToArray();
			StringBuilder stringBuilder = new StringBuilder(array[0].ToString(), 64);
			for (int i = 1; i < array.Length; i++)
			{
				if (!array[i].Expired)
				{
					stringBuilder.AppendFormat("; {0}", array[i].ToString());
				}
			}
			AddHeader("Cookie", stringBuilder.ToString());
		}

		public void SetAuthorization(AuthenticationResponse response)
		{
			string value = response.ToString();
			AddHeader("Authorization", value);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}{3}", HttpMethod, RawUrl, base.ProtocolVersion, "\r\n");
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
