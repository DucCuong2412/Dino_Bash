using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LeanplumSDK.WebSocketSharp.Net
{
	[Serializable]
	internal class CookieCollection : ICollection, IEnumerable
	{
		private sealed class CookieCollectionComparer : IComparer<Cookie>
		{
			public int Compare(Cookie x, Cookie y)
			{
				if (x == null || y == null)
				{
					return 0;
				}
				int num = x.Name.Length + x.Value.Length;
				int num2 = y.Name.Length + y.Value.Length;
				return num - num2;
			}
		}

		private static CookieCollectionComparer Comparer = new CookieCollectionComparer();

		private List<Cookie> list;

		private object sync;

		internal IList<Cookie> List
		{
			get
			{
				return list;
			}
		}

		internal IEnumerable<Cookie> Sorted
		{
			get
			{
				return from cookie in list
					orderby cookie.Version, cookie.Name
					select cookie;
			}
		}

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public Cookie this[int index]
		{
			get
			{
				if (index < 0 || index >= list.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return list[index];
			}
		}

		public Cookie this[string name]
		{
			get
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				foreach (Cookie item in Sorted)
				{
					if (item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					{
						return item;
					}
				}
				return null;
			}
		}

		public object SyncRoot
		{
			get
			{
				if (sync == null)
				{
					sync = new object();
				}
				return sync;
			}
		}

		public CookieCollection()
		{
			list = new List<Cookie>();
		}

		private static CookieCollection ParseRequest(string value)
		{
			CookieCollection cookieCollection = new CookieCollection();
			Cookie cookie = null;
			int num = 0;
			string[] array = Split(value).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.Length == 0)
				{
					continue;
				}
				if (text.StartsWith("$version", StringComparison.InvariantCultureIgnoreCase))
				{
					num = int.Parse(text.GetValueInternal("=").Trim('"'));
					continue;
				}
				if (text.StartsWith("$path", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Path = text.GetValueInternal("=");
					}
					continue;
				}
				if (text.StartsWith("$domain", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Domain = text.GetValueInternal("=");
					}
					continue;
				}
				if (text.StartsWith("$port", StringComparison.InvariantCultureIgnoreCase))
				{
					string port = ((!text.Equals("$port", StringComparison.InvariantCultureIgnoreCase)) ? text.GetValueInternal("=") : "\"\"");
					if (cookie != null)
					{
						cookie.Port = port;
					}
					continue;
				}
				if (cookie != null)
				{
					cookieCollection.Add(cookie);
				}
				string value2 = string.Empty;
				int num2 = text.IndexOf('=');
				string name;
				if (num2 == -1)
				{
					name = text;
				}
				else if (num2 == text.Length - 1)
				{
					name = text.Substring(0, num2).TrimEnd(' ');
				}
				else
				{
					name = text.Substring(0, num2).TrimEnd(' ');
					value2 = text.Substring(num2 + 1).TrimStart(' ');
				}
				cookie = new Cookie(name, value2);
				if (num != 0)
				{
					cookie.Version = num;
				}
			}
			if (cookie != null)
			{
				cookieCollection.Add(cookie);
			}
			return cookieCollection;
		}

		private static CookieCollection ParseResponse(string value)
		{
			CookieCollection cookieCollection = new CookieCollection();
			Cookie cookie = null;
			string[] array = Split(value).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.Length == 0)
				{
					continue;
				}
				if (text.StartsWith("version", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Version = int.Parse(text.GetValueInternal("=").Trim('"'));
					}
					continue;
				}
				if (text.StartsWith("expires", StringComparison.InvariantCultureIgnoreCase))
				{
					StringBuilder stringBuilder = new StringBuilder(text.GetValueInternal("="), 32);
					if (i < array.Length - 1)
					{
						stringBuilder.AppendFormat(", {0}", array[++i].Trim());
					}
					DateTime result;
					if (!DateTime.TryParseExact(stringBuilder.ToString(), new string[2] { "ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", "r" }, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
					{
						result = DateTime.Now;
					}
					if (cookie != null && cookie.Expires == DateTime.MinValue)
					{
						cookie.Expires = result.ToLocalTime();
					}
					continue;
				}
				if (text.StartsWith("max-age", StringComparison.InvariantCultureIgnoreCase))
				{
					int num = int.Parse(text.GetValueInternal("=").Trim('"'));
					DateTime expires = DateTime.Now.AddSeconds(num);
					if (cookie != null)
					{
						cookie.Expires = expires;
					}
					continue;
				}
				if (text.StartsWith("path", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Path = text.GetValueInternal("=");
					}
					continue;
				}
				if (text.StartsWith("domain", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Domain = text.GetValueInternal("=");
					}
					continue;
				}
				if (text.StartsWith("port", StringComparison.InvariantCultureIgnoreCase))
				{
					string port = ((!text.Equals("port", StringComparison.InvariantCultureIgnoreCase)) ? text.GetValueInternal("=") : "\"\"");
					if (cookie != null)
					{
						cookie.Port = port;
					}
					continue;
				}
				if (text.StartsWith("comment", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Comment = text.GetValueInternal("=").UrlDecode();
					}
					continue;
				}
				if (text.StartsWith("commenturl", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.CommentUri = text.GetValueInternal("=").Trim('"').ToUri();
					}
					continue;
				}
				if (text.StartsWith("discard", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Discard = true;
					}
					continue;
				}
				if (text.StartsWith("secure", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Secure = true;
					}
					continue;
				}
				if (text.StartsWith("httponly", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.HttpOnly = true;
					}
					continue;
				}
				if (cookie != null)
				{
					cookieCollection.Add(cookie);
				}
				string value2 = string.Empty;
				int num2 = text.IndexOf('=');
				string name;
				if (num2 == -1)
				{
					name = text;
				}
				else if (num2 == text.Length - 1)
				{
					name = text.Substring(0, num2).TrimEnd(' ');
				}
				else
				{
					name = text.Substring(0, num2).TrimEnd(' ');
					value2 = text.Substring(num2 + 1).TrimStart(' ');
				}
				cookie = new Cookie(name, value2);
			}
			if (cookie != null)
			{
				cookieCollection.Add(cookie);
			}
			return cookieCollection;
		}

		private int SearchCookie(Cookie cookie)
		{
			string name = cookie.Name;
			string path = cookie.Path;
			string domain = cookie.Domain;
			int version = cookie.Version;
			for (int num = list.Count - 1; num >= 0; num--)
			{
				Cookie cookie2 = list[num];
				if (cookie2.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && cookie2.Path.Equals(path, StringComparison.InvariantCulture) && cookie2.Domain.Equals(domain, StringComparison.InvariantCultureIgnoreCase) && cookie2.Version == version)
				{
					return num;
				}
			}
			return -1;
		}

		private static IEnumerable<string> Split(string value)
		{
			return value.SplitHeaderValue(',', ';');
		}

		internal static CookieCollection Parse(string value, bool response)
		{
			return (!response) ? ParseRequest(value) : ParseResponse(value);
		}

		internal void SetOrRemove(Cookie cookie)
		{
			int num = SearchCookie(cookie);
			if (num == -1)
			{
				if (!cookie.Expired)
				{
					list.Add(cookie);
				}
			}
			else if (!cookie.Expired)
			{
				list[num] = cookie;
			}
			else
			{
				list.RemoveAt(num);
			}
		}

		internal void SetOrRemove(CookieCollection cookies)
		{
			foreach (Cookie cookie in cookies)
			{
				SetOrRemove(cookie);
			}
		}

		internal void Sort()
		{
			if (list.Count > 0)
			{
				list.Sort(Comparer);
			}
		}

		public void Add(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			int num = SearchCookie(cookie);
			if (num == -1)
			{
				list.Add(cookie);
			}
			else
			{
				list[num] = cookie;
			}
		}

		public void Add(CookieCollection cookies)
		{
			if (cookies == null)
			{
				throw new ArgumentNullException("cookies");
			}
			foreach (Cookie cookie in cookies)
			{
				Add(cookie);
			}
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Must not be less than zero.");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("Must not be multidimensional.", "array");
			}
			if (array.Length - index < list.Count)
			{
				throw new ArgumentException("The number of elements in this collection is greater than the available space of the destination array.");
			}
			if (!array.GetType().GetElementType().IsAssignableFrom(typeof(Cookie)))
			{
				throw new InvalidCastException("The elements in this collection cannot be cast automatically to the type of the destination array.");
			}
			((ICollection)list).CopyTo(array, index);
		}

		public void CopyTo(Cookie[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Must not be less than zero.");
			}
			if (array.Length - index < list.Count)
			{
				throw new ArgumentException("The number of elements in this collection is greater than the available space of the destination array.");
			}
			list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}
