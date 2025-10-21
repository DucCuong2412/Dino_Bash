using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal sealed class HttpListenerRequest
	{
		private static byte[] _100continue = Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");

		private string[] _acceptTypes;

		private bool _chunked;

		private Encoding _contentEncoding;

		private long _contentLength;

		private bool _contentLengthWasSet;

		private HttpListenerContext _context;

		private CookieCollection _cookies;

		private WebHeaderCollection _headers;

		private Guid _identifier;

		private Stream _inputStream;

		private bool _keepAlive;

		private bool _keepAliveWasSet;

		private string _method;

		private NameValueCollection _queryString;

		private string _rawUrl;

		private Uri _referer;

		private Uri _url;

		private string[] _userLanguages;

		private Version _version;

		public string[] AcceptTypes
		{
			get
			{
				return _acceptTypes;
			}
		}

		public int ClientCertificateError
		{
			get
			{
				return 0;
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				if (_contentEncoding == null)
				{
					_contentEncoding = Encoding.Default;
				}
				return _contentEncoding;
			}
		}

		public long ContentLength64
		{
			get
			{
				return _contentLength;
			}
		}

		public string ContentType
		{
			get
			{
				return _headers["Content-Type"];
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				if (_cookies == null)
				{
					_cookies = _headers.GetCookies(false);
				}
				return _cookies;
			}
		}

		public bool HasEntityBody
		{
			get
			{
				return _contentLength > 0 || _chunked;
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return _headers;
			}
		}

		public string HttpMethod
		{
			get
			{
				return _method;
			}
		}

		public Stream InputStream
		{
			get
			{
				if (_inputStream == null)
				{
					_inputStream = ((!HasEntityBody) ? Stream.Null : _context.Connection.GetRequestStream(_chunked, _contentLength));
				}
				return _inputStream;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return false;
			}
		}

		public bool IsLocal
		{
			get
			{
				return RemoteEndPoint.Address.IsLocal();
			}
		}

		public bool IsSecureConnection
		{
			get
			{
				return _context.Connection.IsSecure;
			}
		}

		public bool IsWebSocketRequest
		{
			get
			{
				return _method == "GET" && _version >= HttpVersion.Version11 && _headers.Contains("Upgrade", "websocket") && _headers.Contains("Connection", "Upgrade");
			}
		}

		public bool KeepAlive
		{
			get
			{
				if (!_keepAliveWasSet)
				{
					_keepAlive = _headers.Contains("Connection", "keep-alive") || _version == HttpVersion.Version11 || (_headers.Contains("Keep-Alive") && !_headers.Contains("Keep-Alive", "closed"));
					_keepAliveWasSet = true;
				}
				return _keepAlive;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return _context.Connection.LocalEndPoint;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return _version;
			}
		}

		public NameValueCollection QueryString
		{
			get
			{
				return _queryString;
			}
		}

		public string RawUrl
		{
			get
			{
				return _rawUrl;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return _context.Connection.RemoteEndPoint;
			}
		}

		public Guid RequestTraceIdentifier
		{
			get
			{
				return _identifier;
			}
		}

		public Uri Url
		{
			get
			{
				return _url;
			}
		}

		public Uri UrlReferrer
		{
			get
			{
				return _referer;
			}
		}

		public string UserAgent
		{
			get
			{
				return _headers["User-Agent"];
			}
		}

		public string UserHostAddress
		{
			get
			{
				return LocalEndPoint.ToString();
			}
		}

		public string UserHostName
		{
			get
			{
				return _headers["Host"];
			}
		}

		public string[] UserLanguages
		{
			get
			{
				return _userLanguages;
			}
		}

		internal HttpListenerRequest(HttpListenerContext context)
		{
			_context = context;
			_contentLength = -1L;
			_headers = new WebHeaderCollection();
			_identifier = Guid.NewGuid();
			_version = HttpVersion.Version10;
		}

		private void CreateQueryString(string query)
		{
			if (query == null || query.Length == 0)
			{
				_queryString = new NameValueCollection(1);
				return;
			}
			_queryString = new NameValueCollection();
			if (query[0] == '?')
			{
				query = query.Substring(1);
			}
			string[] array = query.Split('&');
			string[] array2 = array;
			foreach (string text in array2)
			{
				int num = text.IndexOf('=');
				if (num == -1)
				{
					_queryString.Add(null, HttpUtility.UrlDecode(text));
					continue;
				}
				string name = HttpUtility.UrlDecode(text.Substring(0, num));
				string val = HttpUtility.UrlDecode(text.Substring(num + 1));
				_queryString.Add(name, val);
			}
		}

		internal void AddHeader(string header)
		{
			int num = header.IndexOf(':');
			if (num <= 0)
			{
				_context.ErrorMessage = "Invalid header";
				return;
			}
			string text = header.Substring(0, num).Trim();
			string text2 = header.Substring(num + 1).Trim();
			string text3 = text.ToLower(CultureInfo.InvariantCulture);
			_headers.SetInternal(text, text2, false);
			switch (text3)
			{
			case "accept":
				_acceptTypes = text2.SplitHeaderValue(',').ToArray();
				break;
			case "accept-language":
				_userLanguages = text2.Split(',');
				break;
			case "content-length":
			{
				long result;
				if (long.TryParse(text2, out result) && result >= 0)
				{
					_contentLength = result;
					_contentLengthWasSet = true;
				}
				else
				{
					_context.ErrorMessage = "Invalid Content-Length header";
				}
				break;
			}
			case "content-type":
			{
				string[] array = text2.Split(';');
				string[] array2 = array;
				foreach (string text4 in array2)
				{
					string text5 = text4.Trim();
					if (text5.StartsWith("charset"))
					{
						string value = text5.GetValue("=");
						if (!value.IsNullOrEmpty())
						{
							try
							{
								_contentEncoding = Encoding.GetEncoding(value);
								break;
							}
							catch
							{
								_context.ErrorMessage = "Invalid Content-Type header";
								break;
							}
						}
						break;
					}
				}
				break;
			}
			case "referer":
				_referer = text2.ToUri();
				break;
			}
		}

		internal void FinishInitialization()
		{
			string text = UserHostName;
			if (_version > HttpVersion.Version10 && text.IsNullOrEmpty())
			{
				_context.ErrorMessage = "Invalid Host header";
				return;
			}
			Uri result = null;
			string text2 = ((!_rawUrl.MaybeUri() || !Uri.TryCreate(_rawUrl, UriKind.Absolute, out result)) ? HttpUtility.UrlDecode(_rawUrl) : result.PathAndQuery);
			if (text.IsNullOrEmpty())
			{
				text = UserHostAddress;
			}
			if (result != null)
			{
				text = result.Host;
			}
			int num = text.IndexOf(':');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			string text3 = string.Format("{0}://{1}:{2}", (!IsSecureConnection) ? "http" : "https", text, LocalEndPoint.Port);
			if (!Uri.TryCreate(text3 + text2, UriKind.Absolute, out _url))
			{
				_context.ErrorMessage = "Invalid request url: " + text3 + text2;
				return;
			}
			CreateQueryString(_url.Query);
			string text4 = Headers["Transfer-Encoding"];
			if (_version >= HttpVersion.Version11 && !text4.IsNullOrEmpty())
			{
				_chunked = text4.ToLower() == "chunked";
				if (!_chunked)
				{
					_context.ErrorMessage = string.Empty;
					_context.ErrorStatus = 501;
					return;
				}
			}
			if (!_chunked && !_contentLengthWasSet)
			{
				string text5 = _method.ToLower();
				if (text5 == "post" || text5 == "put")
				{
					_context.ErrorMessage = string.Empty;
					_context.ErrorStatus = 411;
					return;
				}
			}
			string text6 = Headers["Expect"];
			if (!text6.IsNullOrEmpty() && text6.ToLower() == "100-continue")
			{
				ResponseStream responseStream = _context.Connection.GetResponseStream();
				responseStream.InternalWrite(_100continue, 0, _100continue.Length);
			}
		}

		internal bool FlushInput()
		{
			//Discarded unreachable code: IL_0091
			if (!HasEntityBody)
			{
				return true;
			}
			int num = 2048;
			if (_contentLength > 0)
			{
				num = (int)Math.Min(_contentLength, num);
			}
			byte[] buffer = new byte[num];
			while (true)
			{
				try
				{
					IAsyncResult asyncResult = InputStream.BeginRead(buffer, 0, num, null, null);
					if (!asyncResult.IsCompleted && !asyncResult.AsyncWaitHandle.WaitOne(100))
					{
						return false;
					}
					if (InputStream.EndRead(asyncResult) <= 0)
					{
						return true;
					}
				}
				catch
				{
					return false;
				}
			}
		}

		internal void SetRequestLine(string requestLine)
		{
			string[] array = requestLine.Split(new char[1] { ' ' }, 3);
			if (array.Length != 3)
			{
				_context.ErrorMessage = "Invalid request line (parts)";
				return;
			}
			_method = array[0];
			if (!_method.IsToken())
			{
				_context.ErrorMessage = "Invalid request line (method)";
				return;
			}
			_rawUrl = array[1];
			if (array[2].Length != 8 || !array[2].StartsWith("HTTP/"))
			{
				_context.ErrorMessage = "Invalid request line (version)";
				return;
			}
			try
			{
				_version = new Version(array[2].Substring(5));
				if (_version.Major < 1)
				{
					throw new Exception();
				}
			}
			catch
			{
				_context.ErrorMessage = "Invalid request line (version)";
			}
		}

		public IAsyncResult BeginGetClientCertificate(AsyncCallback requestCallback, object state)
		{
			throw new NotImplementedException();
		}

		public X509Certificate2 EndGetClientCertificate(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public X509Certificate2 GetClientCertificate()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}\r\n", _method, _rawUrl, _version);
			string[] allKeys = _headers.AllKeys;
			foreach (string text in allKeys)
			{
				stringBuilder.AppendFormat("{0}: {1}\r\n", text, _headers[text]);
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}
	}
}
