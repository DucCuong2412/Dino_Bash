using System;
using System.Globalization;
using System.Text;

namespace LeanplumSDK.WebSocketSharp.Net
{
	[Serializable]
	internal sealed class Cookie
	{
		private static char[] reservedCharsForName = new char[7] { ' ', '=', ';', ',', '\n', '\r', '\t' };

		private static char[] reservedCharsForValue = new char[2] { ';', ',' };

		private string comment;

		private Uri commentUri;

		private bool discard;

		private string domain;

		private DateTime expires;

		private bool httpOnly;

		private string name;

		private string path;

		private string port;

		private int[] ports;

		private bool secure;

		private DateTime timestamp;

		private string val;

		private int version;

		internal bool ExactDomain { get; set; }

		internal int MaxAge
		{
			get
			{
				if (expires == DateTime.MinValue || Expired)
				{
					return 0;
				}
				DateTime dateTime = ((expires.Kind != DateTimeKind.Local) ? expires.ToLocalTime() : expires);
				TimeSpan timeSpan = dateTime - DateTime.Now;
				return (!(timeSpan <= TimeSpan.Zero)) ? ((int)timeSpan.TotalSeconds) : 0;
			}
		}

		internal int[] Ports
		{
			get
			{
				return ports;
			}
		}

		public string Comment
		{
			get
			{
				return comment;
			}
			set
			{
				comment = value ?? string.Empty;
			}
		}

		public Uri CommentUri
		{
			get
			{
				return commentUri;
			}
			set
			{
				commentUri = value;
			}
		}

		public bool Discard
		{
			get
			{
				return discard;
			}
			set
			{
				discard = value;
			}
		}

		public string Domain
		{
			get
			{
				return domain;
			}
			set
			{
				if (value.IsNullOrEmpty())
				{
					domain = string.Empty;
					ExactDomain = true;
				}
				else
				{
					domain = value;
					ExactDomain = value[0] != '.';
				}
			}
		}

		public bool Expired
		{
			get
			{
				return expires <= DateTime.Now && expires != DateTime.MinValue;
			}
			set
			{
				expires = ((!value) ? DateTime.MinValue : DateTime.Now);
			}
		}

		public DateTime Expires
		{
			get
			{
				return expires;
			}
			set
			{
				expires = value;
			}
		}

		public bool HttpOnly
		{
			get
			{
				return httpOnly;
			}
			set
			{
				httpOnly = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				string message;
				if (!CanSetName(value, out message))
				{
					throw new CookieException(message);
				}
				name = value;
			}
		}

		public string Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value ?? string.Empty;
			}
		}

		public string Port
		{
			get
			{
				return port;
			}
			set
			{
				if (value.IsNullOrEmpty())
				{
					port = string.Empty;
					ports = new int[0];
					return;
				}
				if (!value.IsEnclosedIn('"'))
				{
					throw new CookieException(string.Format("The 'Port={0}' attribute of the cookie is invalid. The value must be enclosed in double quotes.", value));
				}
				string parseError;
				if (!TryCreatePorts(value, out ports, out parseError))
				{
					throw new CookieException(string.Format("The 'Port={0}' attribute of the cookie is invalid. Invalid value: {1}", value, parseError));
				}
				port = value;
			}
		}

		public bool Secure
		{
			get
			{
				return secure;
			}
			set
			{
				secure = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return timestamp;
			}
		}

		public string Value
		{
			get
			{
				return val;
			}
			set
			{
				string message;
				if (!CanSetValue(value, out message))
				{
					throw new CookieException(message);
				}
				val = ((value.Length != 0) ? value : "\"\"");
			}
		}

		public int Version
		{
			get
			{
				return version;
			}
			set
			{
				if (value < 0 || value > 1)
				{
					throw new ArgumentOutOfRangeException("value", "Must be 0 or 1.");
				}
				version = value;
			}
		}

		public Cookie()
		{
			comment = string.Empty;
			domain = string.Empty;
			expires = DateTime.MinValue;
			name = string.Empty;
			path = string.Empty;
			port = string.Empty;
			ports = new int[0];
			timestamp = DateTime.Now;
			val = string.Empty;
			version = 0;
		}

		public Cookie(string name, string value)
			: this()
		{
			Name = name;
			Value = value;
		}

		public Cookie(string name, string value, string path)
			: this(name, value)
		{
			Path = path;
		}

		public Cookie(string name, string value, string path, string domain)
			: this(name, value, path)
		{
			Domain = domain;
		}

		private static bool CanSetName(string name, out string message)
		{
			if (name.IsNullOrEmpty())
			{
				message = "Name must not be null or empty.";
				return false;
			}
			if (name[0] == '$' || name.Contains(reservedCharsForName))
			{
				message = "Name contains an invalid character.";
				return false;
			}
			message = string.Empty;
			return true;
		}

		private static bool CanSetValue(string value, out string message)
		{
			if (value == null)
			{
				message = "Value must not be null.";
				return false;
			}
			if (value.Contains(reservedCharsForValue) && !value.IsEnclosedIn('"'))
			{
				message = "Value contains an invalid character.";
				return false;
			}
			message = string.Empty;
			return true;
		}

		private static int Hash(int i, int j, int k, int l, int m)
		{
			return i ^ ((j << 13) | (j >> 19)) ^ ((k << 26) | (k >> 6)) ^ ((l << 7) | (l >> 25)) ^ ((m << 20) | (m >> 12));
		}

		private string ToResponseStringVersion0()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}", name, val);
			if (expires != DateTime.MinValue)
			{
				stringBuilder.AppendFormat("; Expires={0}", expires.ToUniversalTime().ToString("ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", CultureInfo.CreateSpecificCulture("en-US")));
			}
			if (!path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Path={0}", path);
			}
			if (!domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Domain={0}", domain);
			}
			if (secure)
			{
				stringBuilder.Append("; Secure");
			}
			if (httpOnly)
			{
				stringBuilder.Append("; HttpOnly");
			}
			return stringBuilder.ToString();
		}

		private string ToResponseStringVersion1()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}; Version={2}", name, val, version);
			if (expires != DateTime.MinValue)
			{
				stringBuilder.AppendFormat("; Max-Age={0}", MaxAge);
			}
			if (!path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Path={0}", path);
			}
			if (!domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Domain={0}", domain);
			}
			if (!port.IsNullOrEmpty())
			{
				if (port == "\"\"")
				{
					stringBuilder.Append("; Port");
				}
				else
				{
					stringBuilder.AppendFormat("; Port={0}", port);
				}
			}
			if (!comment.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Comment={0}", comment.UrlEncode());
			}
			if (commentUri != null)
			{
				stringBuilder.AppendFormat("; CommentURL={0}", commentUri.OriginalString.Quote());
			}
			if (discard)
			{
				stringBuilder.Append("; Discard");
			}
			if (secure)
			{
				stringBuilder.Append("; Secure");
			}
			return stringBuilder.ToString();
		}

		private static bool TryCreatePorts(string value, out int[] result, out string parseError)
		{
			string[] array = value.Trim('"').Split(',');
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = int.MinValue;
				string text = array[i].Trim();
				if (text.Length != 0 && !int.TryParse(text, out array2[i]))
				{
					result = new int[0];
					parseError = text;
					return false;
				}
			}
			result = array2;
			parseError = string.Empty;
			return true;
		}

		internal string ToRequestString(Uri uri)
		{
			if (name.Length == 0)
			{
				return string.Empty;
			}
			if (version == 0)
			{
				return string.Format("{0}={1}", name, val);
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("$Version={0}; {1}={2}", version, name, val);
			if (!path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; $Path={0}", path);
			}
			else if (uri != null)
			{
				stringBuilder.AppendFormat("; $Path={0}", uri.GetAbsolutePath());
			}
			else
			{
				stringBuilder.Append("; $Path=/");
			}
			if ((uri == null || uri.Host != domain) && !domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; $Domain={0}", domain);
			}
			if (!port.IsNullOrEmpty())
			{
				if (port == "\"\"")
				{
					stringBuilder.Append("; $Port");
				}
				else
				{
					stringBuilder.AppendFormat("; $Port={0}", port);
				}
			}
			return stringBuilder.ToString();
		}

		internal string ToResponseString()
		{
			return (name.Length == 0) ? string.Empty : ((version != 0) ? ToResponseStringVersion1() : ToResponseStringVersion0());
		}

		public override bool Equals(object comparand)
		{
			Cookie cookie = comparand as Cookie;
			return cookie != null && name.Equals(cookie.Name, StringComparison.InvariantCultureIgnoreCase) && val.Equals(cookie.Value, StringComparison.InvariantCulture) && path.Equals(cookie.Path, StringComparison.InvariantCulture) && domain.Equals(cookie.Domain, StringComparison.InvariantCultureIgnoreCase) && version == cookie.Version;
		}

		public override int GetHashCode()
		{
			return Hash(StringComparer.InvariantCultureIgnoreCase.GetHashCode(name), val.GetHashCode(), path.GetHashCode(), StringComparer.InvariantCultureIgnoreCase.GetHashCode(domain), version);
		}

		public override string ToString()
		{
			return ToRequestString(null);
		}
	}
}
