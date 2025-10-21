using System;
using System.Collections.Generic;

namespace LeanplumSDK
{
	public static class Util
	{
		public static TValue GetValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = null) where TValue : class
		{
			TValue value;
			if (dictionary != null && dictionary.TryGetValue(key, out value))
			{
				return value;
			}
			return defaultValue;
		}

		public static string Capitalize(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			char c = s[0];
			if (char.IsUpper(c))
			{
				return s;
			}
			return char.ToUpper(c) + s.Substring(1);
		}

		public static WebRequest CreateWebRequest(string hostName, string path, IDictionary<string, string> parameters, string httpMethod, bool ssl, int timeout)
		{
			WebRequest webRequest = CreateWebRequest(hostName, path, ssl, timeout);
			if (httpMethod.Equals("GET"))
			{
				webRequest.AttachGetParameters(parameters);
			}
			else
			{
				webRequest.AttachPostParameters(parameters);
			}
			return webRequest;
		}

		public static WebRequest CreateWebRequest(string hostName, string path, bool ssl, int timeout)
		{
			string url = ((!path.StartsWith("http")) ? (((!ssl) ? "http://" : "https://") + hostName + "/" + path) : path);
			return LeanplumNative.CompatibilityLayer.CreateWebRequest(url, timeout);
		}

		internal static int NumResponses(object response)
		{
			//Discarded unreachable code: IL_0020, IL_003d, IL_005a
			try
			{
				return ((response as IDictionary<string, object>)["response"] as IList<object>).Count;
			}
			catch (KeyNotFoundException error)
			{
				LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", error);
				return 0;
			}
			catch (NullReferenceException error2)
			{
				LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", error2);
				return 0;
			}
		}

		internal static object GetResponseAt(object response, int index)
		{
			//Discarded unreachable code: IL_0021, IL_003e, IL_005b
			try
			{
				return ((response as IDictionary<string, object>)["response"] as IList<object>)[index];
			}
			catch (KeyNotFoundException error)
			{
				LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", error);
				return null;
			}
			catch (NullReferenceException error2)
			{
				LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", error2);
				return null;
			}
		}

		internal static object GetLastResponse(object response)
		{
			int num = NumResponses(response);
			return (num <= 0) ? null : GetResponseAt(response, num - 1);
		}

		internal static int GetUnixTimestamp()
		{
			return Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
		}

		public static void MaybeThrow(LeanplumException exception)
		{
			if (Constants.isDevelopmentModeEnabled)
			{
				throw exception;
			}
			LeanplumNative.CompatibilityLayer.LogError(exception.ToString());
		}

		internal static bool IsNumber(object value)
		{
			return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal;
		}
	}
}
