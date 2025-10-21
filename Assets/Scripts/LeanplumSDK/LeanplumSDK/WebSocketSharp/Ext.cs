using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using LeanplumSDK.WebSocketSharp.Net;
using LeanplumSDK.WebSocketSharp.Net.WebSockets;

namespace LeanplumSDK.WebSocketSharp
{
	internal static class Ext
	{
		private const string _tspecials = "()<>@,;:\\\"/[]?={} \t";

		private static byte[] compress(this byte[] value)
		{
			//Discarded unreachable code: IL_0020
			if (value.LongLength == 0L)
			{
				return value;
			}
			using (MemoryStream stream = new MemoryStream(value))
			{
				return stream.compressToArray();
			}
		}

		private static MemoryStream compress(this Stream stream)
		{
			//Discarded unreachable code: IL_0040
			MemoryStream memoryStream = new MemoryStream();
			if (stream.Length == 0L)
			{
				return memoryStream;
			}
			stream.Position = 0L;
			using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
			{
				stream.CopyTo(deflateStream);
				deflateStream.Close();
				memoryStream.Position = 0L;
				return memoryStream;
			}
		}

		private static byte[] compressToArray(this Stream stream)
		{
			//Discarded unreachable code: IL_0019
			using (MemoryStream memoryStream = stream.compress())
			{
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		private static byte[] decompress(this byte[] value)
		{
			//Discarded unreachable code: IL_0020
			if (value.LongLength == 0L)
			{
				return value;
			}
			using (MemoryStream stream = new MemoryStream(value))
			{
				return stream.decompressToArray();
			}
		}

		private static MemoryStream decompress(this Stream stream)
		{
			//Discarded unreachable code: IL_0033
			MemoryStream memoryStream = new MemoryStream();
			if (stream.Length == 0L)
			{
				return memoryStream;
			}
			stream.Position = 0L;
			using (DeflateStream src = new DeflateStream(stream, CompressionMode.Decompress, true))
			{
				src.CopyTo(memoryStream, true);
				return memoryStream;
			}
		}

		private static byte[] decompressToArray(this Stream stream)
		{
			//Discarded unreachable code: IL_0019
			using (MemoryStream memoryStream = stream.decompress())
			{
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		private static byte[] readBytes(this Stream stream, byte[] buffer, int offset, int length)
		{
			int i = stream.Read(buffer, offset, length);
			if (i < 1)
			{
				return buffer.SubArray(0, offset);
			}
			int num = 0;
			for (; i < length; i += num)
			{
				num = stream.Read(buffer, offset + i, length - i);
				if (num < 1)
				{
					break;
				}
			}
			return (i >= length) ? buffer : buffer.SubArray(0, offset + i);
		}

		private static bool readBytes(this Stream stream, byte[] buffer, int offset, int length, Stream dest)
		{
			byte[] array = stream.readBytes(buffer, offset, length);
			int num = array.Length;
			dest.Write(array, 0, num);
			return num == offset + length;
		}

		private static void times(this ulong n, Action act)
		{
			for (ulong num = 0uL; num < n; num++)
			{
				act();
			}
		}

		internal static byte[] Append(this ushort code, string reason)
		{
			//Discarded unreachable code: IL_0052
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] buffer = code.ToByteArrayInternally(ByteOrder.BIG);
				memoryStream.Write(buffer, 0, 2);
				if (reason != null && reason.Length > 0)
				{
					buffer = Encoding.UTF8.GetBytes(reason);
					memoryStream.Write(buffer, 0, buffer.Length);
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static string CheckIfCanRead(this Stream stream)
		{
			return (stream == null) ? "'stream' must not be null." : (stream.CanRead ? null : "'stream' cannot be read.");
		}

		internal static string CheckIfOpen(this WebSocketState state)
		{
			return (state == WebSocketState.OPEN) ? null : "A WebSocket connection isn't established or has been closed.";
		}

		internal static string CheckIfValidCloseData(this byte[] data)
		{
			return (data.Length <= 125) ? null : "'reason' length must be less.";
		}

		internal static string CheckIfValidCloseStatusCode(this ushort code)
		{
			return code.IsCloseStatusCode() ? null : "Invalid close status code.";
		}

		internal static string CheckIfValidPingData(this byte[] data)
		{
			return (data.Length <= 125) ? null : "'message' length must be less.";
		}

		internal static string CheckIfValidSendData(this byte[] data)
		{
			return (data != null) ? null : "'data' must not be null.";
		}

		internal static string CheckIfValidSendData(this string data)
		{
			return (data != null) ? null : "'data' must not be null.";
		}

		internal static string CheckIfValidServicePath(this string servicePath)
		{
			return (servicePath == null || servicePath.Length == 0) ? "'servicePath' must not be null or empty." : ((servicePath[0] != '/') ? "'servicePath' not absolute path." : ((servicePath.IndexOfAny(new char[2] { '?', '#' }) == -1) ? null : "'servicePath' must not contain either or both query and fragment components."));
		}

		internal static string CheckIfValidSessionID(this string id)
		{
			return (id != null && id.Length != 0) ? null : "'id' must not be null or empty.";
		}

		internal static byte[] Compress(this byte[] value, CompressionMethod method)
		{
			return (method != CompressionMethod.DEFLATE) ? value : value.compress();
		}

		internal static Stream Compress(this Stream stream, CompressionMethod method)
		{
			return (method != CompressionMethod.DEFLATE) ? stream : stream.compress();
		}

		internal static byte[] CompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method != CompressionMethod.DEFLATE) ? stream.ToByteArray() : stream.compressToArray();
		}

		internal static T[] Copy<T>(this T[] src, long length)
		{
			T[] array = new T[length];
			Array.Copy(src, 0L, array, 0L, length);
			return array;
		}

		internal static void CopyTo(this Stream src, Stream dest)
		{
			src.CopyTo(dest, false);
		}

		internal static void CopyTo(this Stream src, Stream dest, bool setDefaultPosition)
		{
			int num = 0;
			int num2 = 256;
			byte[] buffer = new byte[num2];
			while ((num = src.Read(buffer, 0, num2)) > 0)
			{
				dest.Write(buffer, 0, num);
			}
			if (setDefaultPosition)
			{
				dest.Position = 0L;
			}
		}

		internal static byte[] Decompress(this byte[] value, CompressionMethod method)
		{
			return (method != CompressionMethod.DEFLATE) ? value : value.decompress();
		}

		internal static Stream Decompress(this Stream stream, CompressionMethod method)
		{
			return (method != CompressionMethod.DEFLATE) ? stream : stream.decompress();
		}

		internal static byte[] DecompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method != CompressionMethod.DEFLATE) ? stream.ToByteArray() : stream.decompressToArray();
		}

		internal static bool Equals(this string value, CompressionMethod method)
		{
			return value == method.ToCompressionExtension();
		}

		internal static bool EqualsWith(this int value, char c, Action<int> action)
		{
			if (value < 0 || value > 255)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			action(value);
			return value == c;
		}

		internal static string GetAbsolutePath(this Uri uri)
		{
			if (uri.IsAbsoluteUri)
			{
				return uri.AbsolutePath;
			}
			string originalString = uri.OriginalString;
			if (originalString[0] != '/')
			{
				return null;
			}
			int num = originalString.IndexOfAny(new char[2] { '?', '#' });
			return (num <= 0) ? originalString : originalString.Substring(0, num);
		}

		internal static string GetMessage(this CloseStatusCode code)
		{
			object result;
			switch (code)
			{
			case CloseStatusCode.PROTOCOL_ERROR:
				result = "A WebSocket protocol error has occurred.";
				break;
			case CloseStatusCode.INCORRECT_DATA:
				result = "An incorrect data has been received.";
				break;
			case CloseStatusCode.ABNORMAL:
				result = "An exception has occurred.";
				break;
			case CloseStatusCode.INCONSISTENT_DATA:
				result = "An inconsistent data has been received.";
				break;
			case CloseStatusCode.POLICY_VIOLATION:
				result = "A policy violation data has been received.";
				break;
			case CloseStatusCode.TOO_BIG:
				result = "A too big data has been received.";
				break;
			case CloseStatusCode.IGNORE_EXTENSION:
				result = "WebSocket client did not receive expected extension(s).";
				break;
			case CloseStatusCode.SERVER_ERROR:
				result = "WebSocket server got an internal error.";
				break;
			case CloseStatusCode.TLS_HANDSHAKE_FAILURE:
				result = "An error has occurred while handshaking.";
				break;
			default:
				result = string.Empty;
				break;
			}
			return (string)result;
		}

		internal static string GetNameInternal(this string nameAndValue, string separator)
		{
			int num = nameAndValue.IndexOf(separator);
			return (num <= 0) ? null : nameAndValue.Substring(0, num).Trim();
		}

		internal static string GetValueInternal(this string nameAndValue, string separator)
		{
			int num = nameAndValue.IndexOf(separator);
			return (num < 0 || num >= nameAndValue.Length - 1) ? null : nameAndValue.Substring(num + 1).Trim();
		}

		internal static TcpListenerWebSocketContext GetWebSocketContext(this ITcpClient client, bool secure, X509Certificate cert)
		{
			return new TcpListenerWebSocketContext(client, secure, cert);
		}

		internal static bool IsCompressionExtension(this string value)
		{
			return value.StartsWith("permessage-");
		}

		internal static bool IsPortNumber(this int value)
		{
			return value > 0 && value < 65536;
		}

		internal static bool IsReserved(this ushort code)
		{
			return code == 1004 || code == 1005 || code == 1006 || code == 1015;
		}

		internal static bool IsReserved(this CloseStatusCode code)
		{
			return code == CloseStatusCode.UNDEFINED || code == CloseStatusCode.NO_STATUS_CODE || code == CloseStatusCode.ABNORMAL || code == CloseStatusCode.TLS_HANDSHAKE_FAILURE;
		}

		internal static bool IsText(this string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				if (c < ' ' && !"\r\n\t".Contains(c))
				{
					return false;
				}
				switch (c)
				{
				case '\u007f':
					return false;
				case '\n':
					if (++i < length)
					{
						c = value[i];
						if (!" \t".Contains(c))
						{
							return false;
						}
					}
					break;
				}
			}
			return true;
		}

		internal static bool IsToken(this string value)
		{
			foreach (char c in value)
			{
				if (c < ' ' || c >= '\u007f' || "()<>@,;:\\\"/[]?={} \t".Contains(c))
				{
					return false;
				}
			}
			return true;
		}

		internal static string Quote(this string value)
		{
			return (!value.IsToken()) ? string.Format("\"{0}\"", value.Replace("\"", "\\\"")) : value;
		}

		internal static byte[] ReadBytes(this Stream stream, int length)
		{
			return stream.readBytes(new byte[length], 0, length);
		}

		internal static byte[] ReadBytes(this Stream stream, long length, int bufferLength)
		{
			//Discarded unreachable code: IL_007b
			using (MemoryStream memoryStream = new MemoryStream())
			{
				long num = length / bufferLength;
				int num2 = (int)(length % bufferLength);
				byte[] buffer = new byte[bufferLength];
				bool flag = false;
				for (long num3 = 0L; num3 < num; num3++)
				{
					if (!stream.readBytes(buffer, 0, bufferLength, memoryStream))
					{
						flag = true;
						break;
					}
				}
				if (!flag && num2 > 0)
				{
					stream.readBytes(new byte[num2], 0, num2, memoryStream);
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static void ReadBytesAsync(this Stream stream, int length, Action<byte[]> completed, Action<Exception> error)
		{
			byte[] buffer = new byte[length];
			AsyncCallback callback = delegate(IAsyncResult ar)
			{
				try
				{
					int num = stream.EndRead(ar);
					byte[] obj = ((num < 1) ? new byte[0] : ((num >= length) ? buffer : stream.readBytes(buffer, num, length - num)));
					if (completed != null)
					{
						completed(obj);
					}
				}
				catch (Exception obj2)
				{
					if (error != null)
					{
						error(obj2);
					}
				}
			};
			stream.BeginRead(buffer, 0, length, callback, null);
		}

		internal static string RemovePrefix(this string value, params string[] prefixes)
		{
			int num = 0;
			foreach (string text in prefixes)
			{
				if (value.StartsWith(text))
				{
					num = text.Length;
					break;
				}
			}
			return (num <= 0) ? value : value.Substring(num);
		}

		internal static IEnumerable<string> SplitHeaderValue(this string value, params char[] separator)
		{
			string separators = new string(separator);
			StringBuilder buffer = new StringBuilder(64);
			int len = value.Length;
			bool quoted = false;
			bool escaped = false;
			for (int i = 0; i < len; i++)
			{
				char c = value[i];
				switch (c)
				{
				case '"':
					if (escaped)
					{
						escaped = !escaped;
					}
					else
					{
						quoted = !quoted;
					}
					break;
				case '\\':
					if (i < len - 1 && value[i + 1] == '"')
					{
						escaped = true;
					}
					break;
				default:
					if (separators.Contains(c) && !quoted)
					{
						yield return buffer.ToString();
						buffer.Length = 0;
						continue;
					}
					break;
				}
				buffer.Append(c);
			}
			if (buffer.Length > 0)
			{
				yield return buffer.ToString();
			}
		}

		internal static byte[] ToByteArray(this Stream stream)
		{
			//Discarded unreachable code: IL_0027
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.Position = 0L;
				stream.CopyTo(memoryStream);
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static byte[] ToByteArrayInternally(this ushort value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return (!order.IsHostOrder()) ? bytes.Reverse().ToArray() : bytes;
		}

		internal static byte[] ToByteArrayInternally(this ulong value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return (!order.IsHostOrder()) ? bytes.Reverse().ToArray() : bytes;
		}

		internal static string ToCompressionExtension(this CompressionMethod method)
		{
			return (method == CompressionMethod.NONE) ? string.Empty : string.Format("permessage-{0}", method.ToString().ToLower());
		}

		internal static CompressionMethod ToCompressionMethod(this string value)
		{
			foreach (byte value2 in Enum.GetValues(typeof(CompressionMethod)))
			{
				if (value.Equals((CompressionMethod)value2))
				{
					return (CompressionMethod)value2;
				}
			}
			return CompressionMethod.NONE;
		}

		internal static IPAddress ToIPAddress(this string hostNameOrAddress)
		{
			//Discarded unreachable code: IL_0010, IL_001d
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(hostNameOrAddress);
				return hostAddresses[0];
			}
			catch
			{
				return null;
			}
		}

		internal static ushort ToUInt16(this byte[] src, ByteOrder srcOrder)
		{
			return BitConverter.ToUInt16(src.ToHostOrder(srcOrder), 0);
		}

		internal static ulong ToUInt64(this byte[] src, ByteOrder srcOrder)
		{
			return BitConverter.ToUInt64(src.ToHostOrder(srcOrder), 0);
		}

		internal static string TrimEndSlash(this string value)
		{
			value = value.TrimEnd('/');
			return (value.Length <= 0) ? "/" : value;
		}

		internal static bool TryCreateWebSocketUri(this string uriString, out Uri result, out string message)
		{
			result = null;
			if (uriString.Length == 0)
			{
				message = "Must not be empty.";
				return false;
			}
			Uri uri = uriString.ToUri();
			if (!uri.IsAbsoluteUri)
			{
				message = "Must be the absolute URI: " + uriString;
				return false;
			}
			string scheme = uri.Scheme;
			if (scheme != "ws" && scheme != "wss")
			{
				message = "The scheme part must be 'ws' or 'wss': " + scheme;
				return false;
			}
			string fragment = uri.Fragment;
			if (fragment.Length != 0)
			{
				message = "Must not contain the fragment component: " + uriString;
				return false;
			}
			int port = uri.Port;
			if (port > 0)
			{
				if (port > 65535)
				{
					message = "The port part must be between 1 and 65535: " + port;
					return false;
				}
				if ((scheme == "ws" && port == 443) || (scheme == "wss" && port == 80))
				{
					message = string.Format("Invalid pair of scheme and port: {0}, {1}", scheme, port);
					return false;
				}
			}
			else
			{
				port = ((!(scheme == "ws")) ? 443 : 80);
				string uriString2 = string.Format("{0}://{1}:{2}{3}", scheme, uri.Host, port, uri.PathAndQuery);
				uri = uriString2.ToUri();
			}
			result = uri;
			message = string.Empty;
			return true;
		}

		internal static string Unquote(this string value)
		{
			int num = value.IndexOf('"');
			int num2 = value.LastIndexOf('"');
			if (num < num2)
			{
				value = value.Substring(num + 1, num2 - num - 1).Replace("\\\"", "\"");
			}
			return value.Trim();
		}

		internal static void WriteBytes(this Stream stream, byte[] value)
		{
			using (MemoryStream src = new MemoryStream(value))
			{
				src.CopyTo(stream);
			}
		}

		public static bool Contains(this string value, params char[] chars)
		{
			return chars == null || chars.Length == 0 || (value != null && value.Length != 0 && value.IndexOfAny(chars) != -1);
		}

		public static bool Contains(this NameValueCollection collection, string name)
		{
			return collection != null && collection[name] != null;
		}

		public static bool Contains(this NameValueCollection collection, string name, string value)
		{
			if (collection == null)
			{
				return false;
			}
			string text = collection[name];
			if (text == null)
			{
				return false;
			}
			string[] array = text.Split(',');
			foreach (string text2 in array)
			{
				if (text2.Trim().Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public static void Emit(this EventHandler eventHandler, object sender, EventArgs e)
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		public static void Emit<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e) where TEventArgs : EventArgs
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		public static LeanplumSDK.WebSocketSharp.Net.CookieCollection GetCookies(this NameValueCollection headers, bool response)
		{
			string name = ((!response) ? "Cookie" : "Set-Cookie");
			return (headers != null && headers.Contains(name)) ? LeanplumSDK.WebSocketSharp.Net.CookieCollection.Parse(headers[name], response) : new LeanplumSDK.WebSocketSharp.Net.CookieCollection();
		}

		public static string GetDescription(this LeanplumSDK.WebSocketSharp.Net.HttpStatusCode code)
		{
			return ((int)code).GetStatusDescription();
		}

		public static string GetName(this string nameAndValue, string separator)
		{
			return (nameAndValue == null || nameAndValue.Length == 0 || separator == null || separator.Length == 0) ? null : nameAndValue.GetNameInternal(separator);
		}

		public static KeyValuePair<string, string> GetNameAndValue(this string nameAndValue, string separator)
		{
			string name = nameAndValue.GetName(separator);
			string value = nameAndValue.GetValue(separator);
			return (name == null) ? new KeyValuePair<string, string>(null, null) : new KeyValuePair<string, string>(name, value);
		}

		public static string GetStatusDescription(this int code)
		{
			switch (code)
			{
			case 100:
				return "Continue";
			case 101:
				return "Switching Protocols";
			case 102:
				return "Processing";
			case 200:
				return "OK";
			case 201:
				return "Created";
			case 202:
				return "Accepted";
			case 203:
				return "Non-Authoritative Information";
			case 204:
				return "No Content";
			case 205:
				return "Reset Content";
			case 206:
				return "Partial Content";
			case 207:
				return "Multi-Status";
			case 300:
				return "Multiple Choices";
			case 301:
				return "Moved Permanently";
			case 302:
				return "Found";
			case 303:
				return "See Other";
			case 304:
				return "Not Modified";
			case 305:
				return "Use Proxy";
			case 307:
				return "Temporary Redirect";
			case 400:
				return "Bad Request";
			case 401:
				return "Unauthorized";
			case 402:
				return "Payment Required";
			case 403:
				return "Forbidden";
			case 404:
				return "Not Found";
			case 405:
				return "Method Not Allowed";
			case 406:
				return "Not Acceptable";
			case 407:
				return "Proxy Authentication Required";
			case 408:
				return "Request Timeout";
			case 409:
				return "Conflict";
			case 410:
				return "Gone";
			case 411:
				return "Length Required";
			case 412:
				return "Precondition Failed";
			case 413:
				return "Request Entity Too Large";
			case 414:
				return "Request-Uri Too Long";
			case 415:
				return "Unsupported Media Type";
			case 416:
				return "Requested Range Not Satisfiable";
			case 417:
				return "Expectation Failed";
			case 422:
				return "Unprocessable Entity";
			case 423:
				return "Locked";
			case 424:
				return "Failed Dependency";
			case 500:
				return "Internal Server Error";
			case 501:
				return "Not Implemented";
			case 502:
				return "Bad Gateway";
			case 503:
				return "Service Unavailable";
			case 504:
				return "Gateway Timeout";
			case 505:
				return "Http Version Not Supported";
			case 507:
				return "Insufficient Storage";
			default:
				return string.Empty;
			}
		}

		public static string GetValue(this string nameAndValue, string separator)
		{
			return (nameAndValue == null || nameAndValue.Length == 0 || separator == null || separator.Length == 0) ? null : nameAndValue.GetValueInternal(separator);
		}

		public static bool IsCloseStatusCode(this ushort value)
		{
			return value > 999 && value < 5000;
		}

		public static bool IsEnclosedIn(this string value, char c)
		{
			return value != null && value.Length > 1 && value[0] == c && value[value.Length - 1] == c;
		}

		public static bool IsHostOrder(this ByteOrder order)
		{
			return !(BitConverter.IsLittleEndian ^ (order == ByteOrder.LITTLE));
		}

		public static bool IsLocal(this IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Equals(IPAddress.Any) || IPAddress.IsLoopback(address))
			{
				return true;
			}
			string hostName = Dns.GetHostName();
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
			IPAddress[] array = hostAddresses;
			foreach (IPAddress other in array)
			{
				if (address.Equals(other))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNullOrEmpty(this string value)
		{
			return value == null || value.Length == 0;
		}

		public static bool IsPredefinedScheme(this string scheme)
		{
			if (scheme == null && scheme.Length < 2)
			{
				return false;
			}
			char c = scheme[0];
			if (c == 'h')
			{
				return scheme == "http" || scheme == "https";
			}
			if (c == 'f')
			{
				return scheme == "file" || scheme == "ftp";
			}
			if (c == 'w')
			{
				return scheme == "ws" || scheme == "wss";
			}
			if (c == 'n')
			{
				c = scheme[1];
				return (c != 'e') ? (scheme == "nntp") : (scheme == "news" || scheme == "net.pipe" || scheme == "net.tcp");
			}
			return (c == 'g' && scheme == "gopher") || (c == 'm' && scheme == "mailto");
		}

		public static bool IsUpgradeTo(this LeanplumSDK.WebSocketSharp.Net.HttpListenerRequest request, string protocol)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (protocol == null)
			{
				throw new ArgumentNullException("protocol");
			}
			if (protocol.Length == 0)
			{
				throw new ArgumentException("Must not be empty.", "protocol");
			}
			return request.Headers.Contains("Upgrade", protocol) && request.Headers.Contains("Connection", "Upgrade");
		}

		public static bool MaybeUri(this string uriString)
		{
			if (uriString == null || uriString.Length == 0)
			{
				return false;
			}
			int num = uriString.IndexOf(':');
			if (num == -1)
			{
				return false;
			}
			if (num >= 10)
			{
				return false;
			}
			return uriString.Substring(0, num).IsPredefinedScheme();
		}

		public static T[] SubArray<T>(this T[] array, int startIndex, int length)
		{
			if (array == null || array.Length == 0)
			{
				return new T[0];
			}
			if (startIndex < 0 || length <= 0)
			{
				return new T[0];
			}
			if (startIndex + length > array.Length)
			{
				return new T[0];
			}
			if (startIndex == 0 && array.Length == length)
			{
				return array;
			}
			T[] array2 = new T[length];
			Array.Copy(array, startIndex, array2, 0, length);
			return array2;
		}

		public static void Times(this int n, Action act)
		{
			if (n > 0 && act != null)
			{
				((ulong)n).times(act);
			}
		}

		public static void Times(this long n, Action act)
		{
			if (n > 0 && act != null)
			{
				((ulong)n).times(act);
			}
		}

		public static void Times(this uint n, Action act)
		{
			if (n != 0 && act != null)
			{
				times(n, act);
			}
		}

		public static void Times(this ulong n, Action act)
		{
			if (n != 0 && act != null)
			{
				n.times(act);
			}
		}

		public static void Times(this int n, Action<int> act)
		{
			if (n > 0 && act != null)
			{
				for (int i = 0; i < n; i++)
				{
					act(i);
				}
			}
		}

		public static void Times(this long n, Action<long> act)
		{
			if (n > 0 && act != null)
			{
				for (long num = 0L; num < n; num++)
				{
					act(num);
				}
			}
		}

		public static void Times(this uint n, Action<uint> act)
		{
			if (n != 0 && act != null)
			{
				for (uint num = 0u; num < n; num++)
				{
					act(num);
				}
			}
		}

		public static void Times(this ulong n, Action<ulong> act)
		{
			if (n != 0 && act != null)
			{
				for (ulong num = 0uL; num < n; num++)
				{
					act(num);
				}
			}
		}

		public static T To<T>(this byte[] src, ByteOrder srcOrder) where T : struct
		{
			if (src == null)
			{
				throw new ArgumentNullException("src");
			}
			if (src.Length == 0)
			{
				return default(T);
			}
			Type typeFromHandle = typeof(T);
			byte[] value = src.ToHostOrder(srcOrder);
			return (typeFromHandle == typeof(bool)) ? ((T)(object)BitConverter.ToBoolean(value, 0)) : ((typeFromHandle == typeof(char)) ? ((T)(object)BitConverter.ToChar(value, 0)) : ((typeFromHandle == typeof(double)) ? ((T)(object)BitConverter.ToDouble(value, 0)) : ((typeFromHandle == typeof(short)) ? ((T)(object)BitConverter.ToInt16(value, 0)) : ((typeFromHandle == typeof(int)) ? ((T)(object)BitConverter.ToInt32(value, 0)) : ((typeFromHandle == typeof(long)) ? ((T)(object)BitConverter.ToInt64(value, 0)) : ((typeFromHandle == typeof(float)) ? ((T)(object)BitConverter.ToSingle(value, 0)) : ((typeFromHandle == typeof(ushort)) ? ((T)(object)BitConverter.ToUInt16(value, 0)) : ((typeFromHandle == typeof(uint)) ? ((T)(object)BitConverter.ToUInt32(value, 0)) : ((typeFromHandle != typeof(ulong)) ? default(T) : ((T)(object)BitConverter.ToUInt64(value, 0)))))))))));
		}

		public static byte[] ToByteArray<T>(this T value, ByteOrder order) where T : struct
		{
			Type typeFromHandle = typeof(T);
			byte[] array = ((typeFromHandle == typeof(bool)) ? BitConverter.GetBytes((bool)(object)value) : ((typeFromHandle == typeof(byte)) ? new byte[1] { (byte)(object)value } : ((typeFromHandle == typeof(char)) ? BitConverter.GetBytes((char)(object)value) : ((typeFromHandle == typeof(double)) ? BitConverter.GetBytes((double)(object)value) : ((typeFromHandle == typeof(short)) ? BitConverter.GetBytes((short)(object)value) : ((typeFromHandle == typeof(int)) ? BitConverter.GetBytes((int)(object)value) : ((typeFromHandle == typeof(long)) ? BitConverter.GetBytes((long)(object)value) : ((typeFromHandle == typeof(float)) ? BitConverter.GetBytes((float)(object)value) : ((typeFromHandle == typeof(ushort)) ? BitConverter.GetBytes((ushort)(object)value) : ((typeFromHandle == typeof(uint)) ? BitConverter.GetBytes((uint)(object)value) : ((typeFromHandle != typeof(ulong)) ? new byte[0] : BitConverter.GetBytes((ulong)(object)value))))))))))));
			return (array.Length > 1 && !order.IsHostOrder()) ? array.Reverse().ToArray() : array;
		}

		public static byte[] ToHostOrder(this byte[] src, ByteOrder srcOrder)
		{
			if (src == null)
			{
				throw new ArgumentNullException("src");
			}
			return (src.Length > 1 && !srcOrder.IsHostOrder()) ? src.Reverse().ToArray() : src;
		}

		public static string ToString<T>(this T[] array, string separator)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = array.Length;
			if (num == 0)
			{
				return string.Empty;
			}
			if (separator == null)
			{
				separator = string.Empty;
			}
			StringBuilder buffer = new StringBuilder(64);
			(num - 1).Times(delegate(int i)
			{
				buffer.AppendFormat("{0}{1}", array[i].ToString(), separator);
			});
			buffer.Append(array[num - 1].ToString());
			return buffer.ToString();
		}

		public static Uri ToUri(this string uriString)
		{
			return (uriString == null || uriString.Length == 0) ? null : ((!uriString.MaybeUri()) ? new Uri(uriString, UriKind.Relative) : new Uri(uriString));
		}

		public static string UrlDecode(this string s)
		{
			return (s != null && s.Length != 0) ? HttpUtility.UrlDecode(s) : s;
		}

		public static string UrlEncode(this string s)
		{
			return (s != null && s.Length != 0) ? HttpUtility.UrlEncode(s) : s;
		}

		public static void WriteContent(this LeanplumSDK.WebSocketSharp.Net.HttpListenerResponse response, byte[] content)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			if (content != null && content.Length != 0)
			{
				Stream outputStream = response.OutputStream;
				response.ContentLength64 = content.Length;
				outputStream.Write(content, 0, content.Length);
				outputStream.Close();
			}
		}
	}
}
