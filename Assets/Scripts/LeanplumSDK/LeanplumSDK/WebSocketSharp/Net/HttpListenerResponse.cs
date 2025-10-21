using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal sealed class HttpListenerResponse : IDisposable
	{
		private bool chunked;

		private bool cl_set;

		private Encoding content_encoding;

		private long content_length;

		private string content_type;

		private HttpListenerContext context;

		private CookieCollection cookies;

		private bool disposed;

		private bool force_close_chunked;

		private WebHeaderCollection headers;

		private bool keep_alive;

		private string location;

		private ResponseStream output_stream;

		private int status_code;

		private string status_description;

		private Version version;

		internal bool HeadersSent;

		internal bool ForceCloseChunked
		{
			get
			{
				return force_close_chunked;
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				if (content_encoding == null)
				{
					content_encoding = Encoding.Default;
				}
				return content_encoding;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				content_encoding = value;
			}
		}

		public long ContentLength64
		{
			get
			{
				return content_length;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Must be greater than or equal zero.", "value");
				}
				cl_set = true;
				content_length = value;
			}
		}

		public string ContentType
		{
			get
			{
				return content_type;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("Must not be empty.", "value");
				}
				content_type = value;
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				if (cookies == null)
				{
					cookies = new CookieCollection();
				}
				return cookies;
			}
			set
			{
				cookies = value;
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				return headers;
			}
			set
			{
				headers = value;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return keep_alive;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				keep_alive = value;
			}
		}

		public Stream OutputStream
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (output_stream == null)
				{
					output_stream = context.Connection.GetResponseStream();
				}
				return output_stream;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return version;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Major != 1 || (value.Minor != 0 && value.Minor != 1))
				{
					throw new ArgumentException("Must be 1.0 or 1.1", "value");
				}
				version = value;
			}
		}

		public string RedirectLocation
		{
			get
			{
				return location;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("Must not be empty.", "value");
				}
				location = value;
			}
		}

		public bool SendChunked
		{
			get
			{
				return chunked;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				chunked = value;
			}
		}

		public int StatusCode
		{
			get
			{
				return status_code;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				if (value < 100 || value > 999)
				{
					throw new ProtocolViolationException("StatusCode must be between 100 and 999.");
				}
				status_code = value;
				status_description = value.GetStatusDescription();
			}
		}

		public string StatusDescription
		{
			get
			{
				return status_description;
			}
			set
			{
				status_description = ((!value.IsNullOrEmpty()) ? value : status_code.GetStatusDescription());
			}
		}

		internal HttpListenerResponse(HttpListenerContext context)
		{
			this.context = context;
			Init();
		}

		void IDisposable.Dispose()
		{
			Close(true);
		}

		private bool CanAddOrUpdate(Cookie cookie)
		{
			if (Cookies.Count == 0)
			{
				return true;
			}
			IEnumerable<Cookie> enumerable = FindCookie(cookie);
			if (enumerable.Count() == 0)
			{
				return true;
			}
			foreach (Cookie item in enumerable)
			{
				if (item.Version == cookie.Version)
				{
					return true;
				}
			}
			return false;
		}

		private void Close(bool force)
		{
			disposed = true;
			context.Connection.Close(force);
		}

		private IEnumerable<Cookie> FindCookie(Cookie cookie)
		{
			string name = cookie.Name;
			string domain = cookie.Domain;
			string path = cookie.Path;
			return from Cookie c in Cookies
				where string.Compare(name, c.Name, true, CultureInfo.InvariantCulture) == 0 && string.Compare(domain, c.Domain, true, CultureInfo.InvariantCulture) == 0 && string.Compare(path, c.Path, false, CultureInfo.InvariantCulture) == 0
				select c;
		}

		private void Init()
		{
			headers = new WebHeaderCollection();
			keep_alive = true;
			status_code = 200;
			status_description = "OK";
			version = HttpVersion.Version11;
		}

		internal void SendHeaders(bool closing, MemoryStream ms)
		{
			Encoding @default = content_encoding;
			if (@default == null)
			{
				@default = Encoding.Default;
			}
			if (content_type != null)
			{
				if (content_encoding != null && content_type.IndexOf("charset=", StringComparison.Ordinal) == -1)
				{
					string webName = content_encoding.WebName;
					headers.SetInternal("Content-Type", content_type + "; charset=" + webName, true);
				}
				else
				{
					headers.SetInternal("Content-Type", content_type, true);
				}
			}
			if (headers["Server"] == null)
			{
				headers.SetInternal("Server", "WebSocketSharp-HTTPAPI/1.0", true);
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (headers["Date"] == null)
			{
				headers.SetInternal("Date", DateTime.UtcNow.ToString("r", invariantCulture), true);
			}
			if (!chunked)
			{
				if (!cl_set && closing)
				{
					cl_set = true;
					content_length = 0L;
				}
				if (cl_set)
				{
					headers.SetInternal("Content-Length", content_length.ToString(invariantCulture), true);
				}
			}
			Version protocolVersion = context.Request.ProtocolVersion;
			if (!cl_set && !chunked && protocolVersion >= HttpVersion.Version11)
			{
				chunked = true;
			}
			bool flag = status_code == 400 || status_code == 408 || status_code == 411 || status_code == 413 || status_code == 414 || status_code == 500 || status_code == 503;
			if (!flag)
			{
				flag = !context.Request.KeepAlive;
			}
			if (!keep_alive || flag)
			{
				headers.SetInternal("Connection", "close", true);
				flag = true;
			}
			if (chunked)
			{
				headers.SetInternal("Transfer-Encoding", "chunked", true);
			}
			int reuses = context.Connection.Reuses;
			if (reuses >= 100)
			{
				force_close_chunked = true;
				if (!flag)
				{
					headers.SetInternal("Connection", "close", true);
					flag = true;
				}
			}
			if (!flag)
			{
				headers.SetInternal("Keep-Alive", string.Format("timeout=15,max={0}", 100 - reuses), true);
				if (context.Request.ProtocolVersion <= HttpVersion.Version10)
				{
					headers.SetInternal("Connection", "keep-alive", true);
				}
			}
			if (location != null)
			{
				headers.SetInternal("Location", location, true);
			}
			if (cookies != null)
			{
				foreach (Cookie cookie in cookies)
				{
					headers.SetInternal("Set-Cookie", cookie.ToResponseString(), true);
				}
			}
			StreamWriter streamWriter = new StreamWriter(ms, @default, 256);
			streamWriter.Write("HTTP/{0} {1} {2}\r\n", version, status_code, status_description);
			string value = headers.ToStringMultiValue(true);
			streamWriter.Write(value);
			streamWriter.Flush();
			int num = ((@default.CodePage != 65001) ? @default.GetPreamble().Length : 3);
			if (output_stream == null)
			{
				output_stream = context.Connection.GetResponseStream();
			}
			ms.Position = num;
			HeadersSent = true;
		}

		public void Abort()
		{
			if (!disposed)
			{
				Close(true);
			}
		}

		public void AddHeader(string name, string value)
		{
			if (name.IsNullOrEmpty())
			{
				throw new ArgumentNullException("name");
			}
			if (value.Length > 65535)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			headers.Set(name, value);
		}

		public void AppendCookie(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			Cookies.Add(cookie);
		}

		public void AppendHeader(string name, string value)
		{
			if (name.IsNullOrEmpty())
			{
				throw new ArgumentException("'name' cannot be null or empty", "name");
			}
			if (value.Length > 65535)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			headers.Add(name, value);
		}

		public void Close()
		{
			if (!disposed)
			{
				Close(false);
			}
		}

		public void Close(byte[] responseEntity, bool willBlock)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (responseEntity == null)
			{
				throw new ArgumentNullException("responseEntity");
			}
			ContentLength64 = responseEntity.Length;
			OutputStream.Write(responseEntity, 0, (int)content_length);
			Close(false);
		}

		public void CopyFrom(HttpListenerResponse templateResponse)
		{
			headers.Clear();
			headers.Add(templateResponse.headers);
			content_length = templateResponse.content_length;
			status_code = templateResponse.status_code;
			status_description = templateResponse.status_description;
			keep_alive = templateResponse.keep_alive;
			version = templateResponse.version;
		}

		public void Redirect(string url)
		{
			StatusCode = 302;
			location = url;
		}

		public void SetCookie(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			if (!CanAddOrUpdate(cookie))
			{
				throw new ArgumentException("Cannot be replaced.", "cookie");
			}
			Cookies.Add(cookie);
		}
	}
}
