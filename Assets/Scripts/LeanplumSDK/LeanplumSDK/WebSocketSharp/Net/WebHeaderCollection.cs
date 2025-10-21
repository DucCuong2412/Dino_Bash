using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace LeanplumSDK.WebSocketSharp.Net
{
	[Serializable]
	[ComVisible(true)]
	internal class WebHeaderCollection : NameValueCollection, ISerializable
	{
		private static readonly Dictionary<string, HttpHeaderInfo> headers;

		private bool internallyCreated;

		private HttpHeaderType state;

		public override string[] AllKeys
		{
			get
			{
				return base.AllKeys;
			}
		}

		public override int Count
		{
			get
			{
				return base.Count;
			}
		}

		public string this[HttpRequestHeader header]
		{
			get
			{
				return Get(Convert(header));
			}
			set
			{
				Add(header, value);
			}
		}

		public string this[HttpResponseHeader header]
		{
			get
			{
				return Get(Convert(header));
			}
			set
			{
				Add(header, value);
			}
		}

		public override KeysCollection Keys
		{
			get
			{
				return base.Keys;
			}
		}

		internal WebHeaderCollection(bool internallyCreated)
		{
			this.internallyCreated = internallyCreated;
			state = HttpHeaderType.Unspecified;
		}

		protected WebHeaderCollection(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			//Discarded unreachable code: IL_0093
			if (serializationInfo == null)
			{
				throw new ArgumentNullException("serializationInfo");
			}
			try
			{
				internallyCreated = serializationInfo.GetBoolean("InternallyCreated");
				state = (HttpHeaderType)serializationInfo.GetInt32("State");
				int @int = serializationInfo.GetInt32("Count");
				for (int i = 0; i < @int; i++)
				{
					base.Add(serializationInfo.GetString(i.ToString()), serializationInfo.GetString((@int + i).ToString()));
				}
			}
			catch (SerializationException ex)
			{
				throw new ArgumentException(ex.Message, "serializationInfo", ex);
			}
		}

		public WebHeaderCollection()
		{
			internallyCreated = false;
			state = HttpHeaderType.Unspecified;
		}

		static WebHeaderCollection()
		{
			headers = new Dictionary<string, HttpHeaderInfo>(StringComparer.InvariantCultureIgnoreCase)
			{
				{
					"Accept",
					new HttpHeaderInfo
					{
						Name = "Accept",
						Type = (HttpHeaderType.Request | HttpHeaderType.Restricted | HttpHeaderType.MultiValue)
					}
				},
				{
					"AcceptCharset",
					new HttpHeaderInfo
					{
						Name = "Accept-Charset",
						Type = (HttpHeaderType.Request | HttpHeaderType.MultiValue)
					}
				},
				{
					"AcceptEncoding",
					new HttpHeaderInfo
					{
						Name = "Accept-Encoding",
						Type = (HttpHeaderType.Request | HttpHeaderType.MultiValue)
					}
				},
				{
					"AcceptLanguage",
					new HttpHeaderInfo
					{
						Name = "Accept-language",
						Type = (HttpHeaderType.Request | HttpHeaderType.MultiValue)
					}
				},
				{
					"AcceptRanges",
					new HttpHeaderInfo
					{
						Name = "Accept-Ranges",
						Type = (HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"Age",
					new HttpHeaderInfo
					{
						Name = "Age",
						Type = HttpHeaderType.Response
					}
				},
				{
					"Allow",
					new HttpHeaderInfo
					{
						Name = "Allow",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"Authorization",
					new HttpHeaderInfo
					{
						Name = "Authorization",
						Type = (HttpHeaderType.Request | HttpHeaderType.MultiValue)
					}
				},
				{
					"CacheControl",
					new HttpHeaderInfo
					{
						Name = "Cache-Control",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"Connection",
					new HttpHeaderInfo
					{
						Name = "Connection",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValue)
					}
				},
				{
					"ContentEncoding",
					new HttpHeaderInfo
					{
						Name = "Content-Encoding",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"ContentLanguage",
					new HttpHeaderInfo
					{
						Name = "Content-Language",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"ContentLength",
					new HttpHeaderInfo
					{
						Name = "Content-Length",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted)
					}
				},
				{
					"ContentLocation",
					new HttpHeaderInfo
					{
						Name = "Content-Location",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response)
					}
				},
				{
					"ContentMd5",
					new HttpHeaderInfo
					{
						Name = "Content-MD5",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response)
					}
				},
				{
					"ContentRange",
					new HttpHeaderInfo
					{
						Name = "Content-Range",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response)
					}
				},
				{
					"ContentType",
					new HttpHeaderInfo
					{
						Name = "Content-Type",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted)
					}
				},
				{
					"Cookie",
					new HttpHeaderInfo
					{
						Name = "Cookie",
						Type = HttpHeaderType.Request
					}
				},
				{
					"Cookie2",
					new HttpHeaderInfo
					{
						Name = "Cookie2",
						Type = HttpHeaderType.Request
					}
				},
				{
					"Date",
					new HttpHeaderInfo
					{
						Name = "Date",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted)
					}
				},
				{
					"Expect",
					new HttpHeaderInfo
					{
						Name = "Expect",
						Type = (HttpHeaderType.Request | HttpHeaderType.Restricted | HttpHeaderType.MultiValue)
					}
				},
				{
					"Expires",
					new HttpHeaderInfo
					{
						Name = "Expires",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response)
					}
				},
				{
					"ETag",
					new HttpHeaderInfo
					{
						Name = "ETag",
						Type = HttpHeaderType.Response
					}
				},
				{
					"From",
					new HttpHeaderInfo
					{
						Name = "From",
						Type = HttpHeaderType.Request
					}
				},
				{
					"Host",
					new HttpHeaderInfo
					{
						Name = "Host",
						Type = (HttpHeaderType.Request | HttpHeaderType.Restricted)
					}
				},
				{
					"IfMatch",
					new HttpHeaderInfo
					{
						Name = "If-Match",
						Type = (HttpHeaderType.Request | HttpHeaderType.MultiValue)
					}
				},
				{
					"IfModifiedSince",
					new HttpHeaderInfo
					{
						Name = "If-Modified-Since",
						Type = (HttpHeaderType.Request | HttpHeaderType.Restricted)
					}
				},
				{
					"IfNoneMatch",
					new HttpHeaderInfo
					{
						Name = "If-None-Match",
						Type = (HttpHeaderType.Request | HttpHeaderType.MultiValue)
					}
				},
				{
					"IfRange",
					new HttpHeaderInfo
					{
						Name = "If-Range",
						Type = HttpHeaderType.Request
					}
				},
				{
					"IfUnmodifiedSince",
					new HttpHeaderInfo
					{
						Name = "If-Unmodified-Since",
						Type = HttpHeaderType.Request
					}
				},
				{
					"KeepAlive",
					new HttpHeaderInfo
					{
						Name = "Keep-Alive",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"LastModified",
					new HttpHeaderInfo
					{
						Name = "Last-Modified",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response)
					}
				},
				{
					"Location",
					new HttpHeaderInfo
					{
						Name = "Location",
						Type = HttpHeaderType.Response
					}
				},
				{
					"MaxForwards",
					new HttpHeaderInfo
					{
						Name = "Max-Forwards",
						Type = HttpHeaderType.Request
					}
				},
				{
					"Pragma",
					new HttpHeaderInfo
					{
						Name = "Pragma",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response)
					}
				},
				{
					"ProxyConnection",
					new HttpHeaderInfo
					{
						Name = "Proxy-Connection",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted)
					}
				},
				{
					"ProxyAuthenticate",
					new HttpHeaderInfo
					{
						Name = "Proxy-Authenticate",
						Type = (HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"ProxyAuthorization",
					new HttpHeaderInfo
					{
						Name = "Proxy-Authorization",
						Type = HttpHeaderType.Request
					}
				},
				{
					"Public",
					new HttpHeaderInfo
					{
						Name = "Public",
						Type = (HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"Range",
					new HttpHeaderInfo
					{
						Name = "Range",
						Type = (HttpHeaderType.Request | HttpHeaderType.Restricted | HttpHeaderType.MultiValue)
					}
				},
				{
					"Referer",
					new HttpHeaderInfo
					{
						Name = "Referer",
						Type = (HttpHeaderType.Request | HttpHeaderType.Restricted)
					}
				},
				{
					"RetryAfter",
					new HttpHeaderInfo
					{
						Name = "Retry-After",
						Type = HttpHeaderType.Response
					}
				},
				{
					"SecWebSocketAccept",
					new HttpHeaderInfo
					{
						Name = "Sec-WebSocket-Accept",
						Type = (HttpHeaderType.Response | HttpHeaderType.Restricted)
					}
				},
				{
					"SecWebSocketExtensions",
					new HttpHeaderInfo
					{
						Name = "Sec-WebSocket-Extensions",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValueInRequest)
					}
				},
				{
					"SecWebSocketKey",
					new HttpHeaderInfo
					{
						Name = "Sec-WebSocket-Key",
						Type = (HttpHeaderType.Request | HttpHeaderType.Restricted)
					}
				},
				{
					"SecWebSocketProtocol",
					new HttpHeaderInfo
					{
						Name = "Sec-WebSocket-Protocol",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValueInRequest)
					}
				},
				{
					"SecWebSocketVersion",
					new HttpHeaderInfo
					{
						Name = "Sec-WebSocket-Version",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValueInResponse)
					}
				},
				{
					"Server",
					new HttpHeaderInfo
					{
						Name = "Server",
						Type = HttpHeaderType.Response
					}
				},
				{
					"SetCookie",
					new HttpHeaderInfo
					{
						Name = "Set-Cookie",
						Type = (HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"SetCookie2",
					new HttpHeaderInfo
					{
						Name = "Set-Cookie2",
						Type = (HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"Te",
					new HttpHeaderInfo
					{
						Name = "TE",
						Type = HttpHeaderType.Request
					}
				},
				{
					"Trailer",
					new HttpHeaderInfo
					{
						Name = "Trailer",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response)
					}
				},
				{
					"TransferEncoding",
					new HttpHeaderInfo
					{
						Name = "Transfer-Encoding",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValue)
					}
				},
				{
					"Translate",
					new HttpHeaderInfo
					{
						Name = "Translate",
						Type = HttpHeaderType.Request
					}
				},
				{
					"Upgrade",
					new HttpHeaderInfo
					{
						Name = "Upgrade",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"UserAgent",
					new HttpHeaderInfo
					{
						Name = "User-Agent",
						Type = (HttpHeaderType.Request | HttpHeaderType.Restricted)
					}
				},
				{
					"Vary",
					new HttpHeaderInfo
					{
						Name = "Vary",
						Type = (HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"Via",
					new HttpHeaderInfo
					{
						Name = "Via",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"Warning",
					new HttpHeaderInfo
					{
						Name = "Warning",
						Type = (HttpHeaderType.Request | HttpHeaderType.Response | HttpHeaderType.MultiValue)
					}
				},
				{
					"WwwAuthenticate",
					new HttpHeaderInfo
					{
						Name = "WWW-Authenticate",
						Type = (HttpHeaderType.Response | HttpHeaderType.Restricted | HttpHeaderType.MultiValue)
					}
				}
			};
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			GetObjectData(serializationInfo, streamingContext);
		}

		private void Add(string name, string value, bool ignoreRestricted)
		{
			Action<string, string> act = ((!ignoreRestricted) ? new Action<string, string>(AddWithoutCheckingName) : new Action<string, string>(AddWithoutCheckingNameAndRestricted));
			DoWithCheckingState(act, CheckName(name), value, true);
		}

		private void AddWithoutCheckingName(string name, string value)
		{
			DoWithoutCheckingName(base.Add, name, value);
		}

		private void AddWithoutCheckingNameAndRestricted(string name, string value)
		{
			base.Add(name, CheckValue(value));
		}

		private static int CheckColonSeparated(string header)
		{
			int num = header.IndexOf(':');
			if (num == -1)
			{
				throw new ArgumentException("No colon found.", "header");
			}
			return num;
		}

		private static HttpHeaderType CheckHeaderType(string name)
		{
			HttpHeaderInfo info;
			return TryGetHeaderInfo(name, out info) ? ((info.IsRequest && !info.IsResponse) ? HttpHeaderType.Request : ((!info.IsRequest && info.IsResponse) ? HttpHeaderType.Response : HttpHeaderType.Unspecified)) : HttpHeaderType.Unspecified;
		}

		private static string CheckName(string name)
		{
			if (name.IsNullOrEmpty())
			{
				throw new ArgumentNullException("name");
			}
			name = name.Trim();
			if (!IsHeaderName(name))
			{
				throw new ArgumentException("Contains invalid characters.", "name");
			}
			return name;
		}

		private void CheckRestricted(string name)
		{
			if (!internallyCreated && ContainsInRestricted(name, true))
			{
				throw new ArgumentException("This header must be modified with the appropiate property.");
			}
		}

		private void CheckState(bool response)
		{
			if (state != 0)
			{
				if (response && state == HttpHeaderType.Request)
				{
					throw new InvalidOperationException("This collection has already been used to store the request headers.");
				}
				if (!response && state == HttpHeaderType.Response)
				{
					throw new InvalidOperationException("This collection has already been used to store the response headers.");
				}
			}
		}

		private static string CheckValue(string value)
		{
			if (value.IsNullOrEmpty())
			{
				return string.Empty;
			}
			value = value.Trim();
			if (value.Length > 65535)
			{
				throw new ArgumentOutOfRangeException("value", "The length must not be greater than 65535.");
			}
			if (!IsHeaderValue(value))
			{
				throw new ArgumentException("Contains invalid characters.", "value");
			}
			return value;
		}

		private static string Convert(string key)
		{
			HttpHeaderInfo value;
			return (!headers.TryGetValue(key, out value)) ? string.Empty : value.Name;
		}

		private static bool ContainsInRestricted(string name, bool response)
		{
			HttpHeaderInfo info;
			return TryGetHeaderInfo(name, out info) && info.IsRestricted(response);
		}

		private void DoWithCheckingState(Action<string, string> act, string name, string value, bool setState)
		{
			switch (CheckHeaderType(name))
			{
			case HttpHeaderType.Request:
				DoWithCheckingState(act, name, value, false, setState);
				break;
			case HttpHeaderType.Response:
				DoWithCheckingState(act, name, value, true, setState);
				break;
			default:
				act(name, value);
				break;
			}
		}

		private void DoWithCheckingState(Action<string, string> act, string name, string value, bool response, bool setState)
		{
			CheckState(response);
			act(name, value);
			if (setState)
			{
				SetState(response);
			}
		}

		private void DoWithoutCheckingName(Action<string, string> act, string name, string value)
		{
			CheckRestricted(name);
			act(name, CheckValue(value));
		}

		private static HttpHeaderInfo GetHeaderInfo(string name)
		{
			return (from HttpHeaderInfo info in headers.Values
				where info.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
				select info).FirstOrDefault();
		}

		private void RemoveWithoutCheckingName(string name, string unuse)
		{
			CheckRestricted(name);
			base.Remove(name);
		}

		private void SetState(bool response)
		{
			if (state == HttpHeaderType.Unspecified)
			{
				state = ((!response) ? HttpHeaderType.Request : HttpHeaderType.Response);
			}
		}

		private void SetWithoutCheckingName(string name, string value)
		{
			DoWithoutCheckingName(base.Set, name, value);
		}

		private static bool TryGetHeaderInfo(string name, out HttpHeaderInfo info)
		{
			info = GetHeaderInfo(name);
			return info != null;
		}

		internal static string Convert(HttpRequestHeader header)
		{
			return Convert(header.ToString());
		}

		internal static string Convert(HttpResponseHeader header)
		{
			return Convert(header.ToString());
		}

		internal static bool IsHeaderName(string name)
		{
			return !name.IsNullOrEmpty() && name.IsToken();
		}

		internal static bool IsHeaderValue(string value)
		{
			return value.IsText();
		}

		internal static bool IsMultiValue(string headerName, bool response)
		{
			if (headerName.IsNullOrEmpty())
			{
				return false;
			}
			HttpHeaderInfo info;
			return TryGetHeaderInfo(headerName, out info) && info.IsMultiValue(response);
		}

		internal void RemoveInternal(string name)
		{
			base.Remove(name);
		}

		internal void SetInternal(string header, bool response)
		{
			int num = CheckColonSeparated(header);
			SetInternal(header.Substring(0, num), header.Substring(num + 1), response);
		}

		internal void SetInternal(string name, string value, bool response)
		{
			value = CheckValue(value);
			if (IsMultiValue(name, response))
			{
				base.Add(name, value);
			}
			else
			{
				base.Set(name, value);
			}
		}

		internal string ToStringMultiValue(bool response)
		{
			StringBuilder sb = new StringBuilder();
			Count.Times(delegate(int i)
			{
				string key = GetKey(i);
				if (IsMultiValue(key, response))
				{
					string[] values = GetValues(i);
					foreach (string arg in values)
					{
						sb.AppendFormat("{0}: {1}\r\n", key, arg);
					}
				}
				else
				{
					sb.AppendFormat("{0}: {1}\r\n", key, Get(i));
				}
			});
			return sb.Append("\r\n").ToString();
		}

		protected void AddWithoutValidate(string headerName, string headerValue)
		{
			Add(headerName, headerValue, true);
		}

		public void Add(string header)
		{
			if (header.IsNullOrEmpty())
			{
				throw new ArgumentNullException("header");
			}
			int num = CheckColonSeparated(header);
			Add(header.Substring(0, num), header.Substring(num + 1));
		}

		public void Add(HttpRequestHeader header, string value)
		{
			DoWithCheckingState(AddWithoutCheckingName, Convert(header), value, false, true);
		}

		public void Add(HttpResponseHeader header, string value)
		{
			DoWithCheckingState(AddWithoutCheckingName, Convert(header), value, true, true);
		}

		public override void Add(string name, string value)
		{
			Add(name, value, false);
		}

		public override void Clear()
		{
			base.Clear();
			state = HttpHeaderType.Unspecified;
		}

		public override string Get(int index)
		{
			return base.Get(index);
		}

		public override string Get(string name)
		{
			return base.Get(name);
		}

		public override IEnumerator GetEnumerator()
		{
			return base.GetEnumerator();
		}

		public override string GetKey(int index)
		{
			return base.GetKey(index);
		}

		public override string[] GetValues(string header)
		{
			string[] values = base.GetValues(header);
			return (values != null && values.Length != 0) ? values : null;
		}

		public override string[] GetValues(int index)
		{
			string[] values = base.GetValues(index);
			return (values != null && values.Length != 0) ? values : null;
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			if (serializationInfo == null)
			{
				throw new ArgumentNullException("serializationInfo");
			}
			serializationInfo.AddValue("InternallyCreated", internallyCreated);
			serializationInfo.AddValue("State", (int)state);
			int count = Count;
			serializationInfo.AddValue("Count", count);
			count.Times(delegate(int i)
			{
				serializationInfo.AddValue(i.ToString(), GetKey(i));
				serializationInfo.AddValue((count + i).ToString(), Get(i));
			});
		}

		public static bool IsRestricted(string headerName)
		{
			return IsRestricted(headerName, false);
		}

		public static bool IsRestricted(string headerName, bool response)
		{
			return ContainsInRestricted(CheckName(headerName), response);
		}

		public override void OnDeserialization(object sender)
		{
		}

		public void Remove(HttpRequestHeader header)
		{
			DoWithCheckingState(RemoveWithoutCheckingName, Convert(header), null, false, false);
		}

		public void Remove(HttpResponseHeader header)
		{
			DoWithCheckingState(RemoveWithoutCheckingName, Convert(header), null, true, false);
		}

		public override void Remove(string name)
		{
			DoWithCheckingState(RemoveWithoutCheckingName, CheckName(name), null, false);
		}

		public void Set(HttpRequestHeader header, string value)
		{
			DoWithCheckingState(SetWithoutCheckingName, Convert(header), value, false, true);
		}

		public void Set(HttpResponseHeader header, string value)
		{
			DoWithCheckingState(SetWithoutCheckingName, Convert(header), value, true, true);
		}

		public override void Set(string name, string value)
		{
			DoWithCheckingState(SetWithoutCheckingName, CheckName(name), value, true);
		}

		public byte[] ToByteArray()
		{
			return Encoding.UTF8.GetBytes(ToString());
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			Count.Times(delegate(int i)
			{
				sb.AppendFormat("{0}: {1}\r\n", GetKey(i), Get(i));
			});
			return sb.Append("\r\n").ToString();
		}
	}
}
