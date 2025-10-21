using System;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
	internal class LeanplumRequest
	{
		public delegate void NoPendingDownloadsHandler();

		private static object padLock = new object();

		private readonly string apiMethod;

		private readonly string httpMethod;

		private readonly string resourceUrl;

		private readonly IDictionary<string, string> parameters;

		public static string AppId { get; private set; }

		public static string DeviceId { get; internal set; }

		public static string AccesssKey { get; private set; }

		public static string Token { get; set; }

		public static string UserId { get; set; }

		public static int PendingDownloads { get; private set; }

		private static event NoPendingDownloadsHandler noPendingDownloads;

		public static event NoPendingDownloadsHandler NoPendingDownloads
		{
			add
			{
				LeanplumRequest.noPendingDownloads = (NoPendingDownloadsHandler)Delegate.Combine(LeanplumRequest.noPendingDownloads, value);
				if (PendingDownloads == 0)
				{
					value();
				}
			}
			remove
			{
				LeanplumRequest.noPendingDownloads = (NoPendingDownloadsHandler)Delegate.Remove(LeanplumRequest.noPendingDownloads, value);
			}
		}

		public event Action<object> Response;

		public event Action<Exception> Error;

		public LeanplumRequest(string httpMethod, string resourceUrl)
		{
			this.httpMethod = httpMethod;
			this.resourceUrl = resourceUrl;
		}

		public LeanplumRequest(string httpMethod, string apiMethod, IDictionary<string, string> parameters)
		{
			this.httpMethod = httpMethod;
			this.apiMethod = apiMethod;
			this.parameters = parameters ?? new Dictionary<string, string>();
		}

		protected virtual void OnError(Exception obj)
		{
			Action<Exception> error = this.Error;
			if (error != null)
			{
				error(obj);
			}
		}

		protected virtual void OnResponse(object obj)
		{
			Action<object> response = this.Response;
			if (response != null)
			{
				response(obj);
			}
		}

		public static void ClearNoPendingDownloads()
		{
			LeanplumRequest.noPendingDownloads = null;
		}

		public static void SetAppId(string appId, string accessKey)
		{
			AppId = appId;
			AccesssKey = accessKey;
		}

		public static LeanplumRequest Get(string resourceUrl)
		{
			return new LeanplumRequest("GET", resourceUrl);
		}

		public static LeanplumRequest Get(string apiMethod, IDictionary<string, string> parameters)
		{
			return new LeanplumRequest("GET", apiMethod, parameters);
		}

		public static LeanplumRequest Post(string apiMethod, IDictionary<string, string> parameters)
		{
			return new LeanplumRequest("POST", apiMethod, parameters);
		}

		internal virtual IDictionary<string, string> CreateArgsDictionary()
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["deviceId"] = DeviceId;
			dictionary["action"] = apiMethod;
			dictionary["userId"] = UserId;
			dictionary["sdkVersion"] = "1.2.11";
			dictionary["devMode"] = Constants.isDevelopmentModeEnabled.ToString();
			dictionary["time"] = Util.GetUnixTimestamp().ToString();
			if (Token != null)
			{
				dictionary["token"] = Token;
			}
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				dictionary[parameter.Key] = parameter.Value;
			}
			return dictionary;
		}

		internal virtual void DownloadAssetNow()
		{
			PendingDownloads++;
			Util.CreateWebRequest(Constants.API_HOST_NAME, resourceUrl, null, httpMethod, Constants.API_SSL, Constants.NETWORK_TIMEOUT_SECONDS).GetAssetBundle(delegate(WebResponse response)
			{
				PendingDownloads--;
				if (response.GetError() != null)
				{
					OnError(new LeanplumException("Error sending request: " + response.GetError()));
				}
				else
				{
					OnResponse(response.GetResponseAsAsset());
				}
				if (PendingDownloads == 0 && LeanplumRequest.noPendingDownloads != null)
				{
					LeanplumRequest.noPendingDownloads();
				}
			});
		}

		internal static void SaveRequestForLater(IDictionary<string, string> args)
		{
			lock (padLock)
			{
				int num = LeanplumNative.CompatibilityLayer.GetSavedInt("__leanplum_unsynced_start");
				int savedInt = LeanplumNative.CompatibilityLayer.GetSavedInt("__leanplum_unsynced");
				savedInt++;
				if (savedInt > 10000)
				{
					savedInt = 10000;
					LeanplumNative.CompatibilityLayer.DeleteSavedSetting(string.Format("__leanplum_unsynced_{0}", num));
					num++;
					LeanplumNative.CompatibilityLayer.StoreSavedInt("__leanplum_unsynced_start", num);
				}
				string key = string.Format("__leanplum_unsynced_{0}", num + savedInt - 1);
				LeanplumNative.CompatibilityLayer.StoreSavedString(key, Json.Serialize(args));
				LeanplumNative.CompatibilityLayer.StoreSavedInt("__leanplum_unsynced", savedInt);
			}
		}

		internal virtual void Send()
		{
			if (Constants.isDevelopmentModeEnabled && LeanplumNative.CompatibilityLayer.IsConnected())
			{
				SendNow();
			}
			else
			{
				SendEventually();
			}
		}

		internal virtual void SendIfConnected()
		{
			if (LeanplumNative.CompatibilityLayer.IsConnected())
			{
				SendNow();
				return;
			}
			SendEventually();
			OnError(new Exception("Device is offline"));
		}

		internal virtual bool AttachApiKeys(IDictionary<string, string> dict)
		{
			if (string.IsNullOrEmpty(AppId) || string.IsNullOrEmpty(AccesssKey))
			{
				LeanplumNative.CompatibilityLayer.LogError("API keys are not set. Please use Leanplum.SetAppIdForDevelopmentMode or Leanplum.SetAppIdForProductionMode.");
				return false;
			}
			dict["appId"] = AppId;
			dict["clientKey"] = AccesssKey;
			dict["client"] = "unity" + '-' + LeanplumNative.CompatibilityLayer.GetPlatformName().ToLower();
			return true;
		}

		internal virtual void SendNow()
		{
			if (Constants.isNoop)
			{
				return;
			}
			if (AppId == null)
			{
				LeanplumNative.CompatibilityLayer.LogError("Cannot send request. appId is not set.");
				return;
			}
			if (AccesssKey == null)
			{
				LeanplumNative.CompatibilityLayer.LogError("Cannot send request. accessKey is not set.");
				return;
			}
			SendEventually();
			IList<IDictionary<string, string>> requestsToSend = PopUnsentRequests();
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["data"] = JsonEncodeUnsentRequests(requestsToSend);
			dictionary["sdkVersion"] = "1.2.11";
			dictionary["action"] = "multi";
			dictionary["time"] = Util.GetUnixTimestamp().ToString();
			if (!AttachApiKeys(dictionary))
			{
				return;
			}
			LeanplumNative.CompatibilityLayer.LogDebug("sending: " + Json.Serialize(dictionary));
			Util.CreateWebRequest(Constants.API_HOST_NAME, Constants.API_SERVLET, dictionary, httpMethod, Constants.API_SSL, Constants.NETWORK_TIMEOUT_SECONDS).GetResponseAsync(delegate(WebResponse response)
			{
				if (!string.IsNullOrEmpty(response.GetError()))
				{
					string text = null;
					string text2 = null;
					text2 = ((response.GetError()[0] != '4' && response.GetError()[0] != '5') ? response.GetError() : response.GetError().Substring(0, 3));
					bool flag = response.GetError() != null && response.GetError().Contains("Could not resolve host");
					switch (text2)
					{
					case "408":
					case "502":
					case "503":
					case "504":
						text = "Server is busy; will retry later";
						LeanplumNative.CompatibilityLayer.LogWarning(text);
						PushUnsentRequests(requestsToSend);
						break;
					default:
						if (flag)
						{
							LeanplumNative.CompatibilityLayer.LogWarning("Could not connect to Leanplum. Will retry later.");
							PushUnsentRequests(requestsToSend);
						}
						else
						{
							text = text2;
							object responseBodyAsJson = response.GetResponseBodyAsJson();
							if (responseBodyAsJson != null && responseBodyAsJson.GetType() == typeof(IDictionary<string, object>))
							{
								IDictionary<string, object> dictionary2 = Util.GetLastResponse(response.GetResponseBodyAsJson()) as IDictionary<string, object>;
								if (dictionary2 != null)
								{
									string text3 = GetResponseError(dictionary2);
									if (text3 != null)
									{
										if (text3.StartsWith("App not found"))
										{
											text3 = "No app matching the provided app ID was found.";
											Constants.isInPermanentFailureState = true;
											Constants.isNoop = true;
										}
										else if (text3.StartsWith("Invalid access key"))
										{
											text3 = "The access key you provided is not valid for this app.";
											Constants.isInPermanentFailureState = true;
											Constants.isNoop = true;
										}
										else if (text3.StartsWith("Development mode requested but not permitted"))
										{
											text3 = "A call to Leanplum.setAppIdForDevelopmentMode with your production key was made, which is not permitted.";
											Constants.isInPermanentFailureState = true;
											Constants.isNoop = true;
										}
										text = text + ", message: " + text3;
									}
								}
							}
							if (text != "Request timed out.")
							{
								LeanplumNative.CompatibilityLayer.LogError(text);
							}
						}
						break;
					}
					OnError(new LeanplumException("Error sending request: " + text));
				}
				else
				{
					IDictionary<string, object> response2 = Util.GetLastResponse(response.GetResponseBodyAsJson()) as IDictionary<string, object>;
					LeanplumNative.CompatibilityLayer.LogDebug("received: " + response.GetResponseBody());
					if (IsResponseSuccess(response2))
					{
						OnResponse(response.GetResponseBodyAsJson());
					}
					else
					{
						string responseError = GetResponseError(response2);
						LeanplumNative.CompatibilityLayer.LogError(responseError);
						OnError(new LeanplumException(responseError));
					}
				}
			});
		}

		internal virtual void SendEventually()
		{
			if (!Constants.isNoop)
			{
				IDictionary<string, string> args = CreateArgsDictionary();
				SaveRequestForLater(args);
			}
		}

		internal static IList<IDictionary<string, string>> PopUnsentRequests()
		{
			IList<IDictionary<string, string>> list = new List<IDictionary<string, string>>();
			lock (padLock)
			{
				int savedInt = LeanplumNative.CompatibilityLayer.GetSavedInt("__leanplum_unsynced_start");
				int savedInt2 = LeanplumNative.CompatibilityLayer.GetSavedInt("__leanplum_unsynced");
				if (savedInt2 == 0)
				{
					return list;
				}
				LeanplumNative.CompatibilityLayer.DeleteSavedSetting("__leanplum_unsynced_start");
				LeanplumNative.CompatibilityLayer.DeleteSavedSetting("__leanplum_unsynced");
				for (int i = savedInt; i < savedInt + savedInt2; i++)
				{
					string key = string.Format("__leanplum_unsynced_{0}", i);
					string savedString = LeanplumNative.CompatibilityLayer.GetSavedString(key);
					if (savedString != null)
					{
						IDictionary<string, object> dictionary = Json.Deserialize(savedString) as IDictionary<string, object>;
						if (dictionary != null)
						{
							IDictionary<string, string> dictionary2 = new Dictionary<string, string>();
							foreach (KeyValuePair<string, object> item in dictionary)
							{
								if (item.Value != null)
								{
									dictionary2[item.Key] = item.Value.ToString();
								}
							}
							list.Add(dictionary2);
						}
					}
					LeanplumNative.CompatibilityLayer.DeleteSavedSetting(key);
				}
				LeanplumNative.CompatibilityLayer.FlushSavedSettings();
				return list;
			}
		}

		internal static string JsonEncodeUnsentRequests(IList<IDictionary<string, string>> requestData)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["data"] = requestData;
			return Json.Serialize(dictionary);
		}

		internal static void PushUnsentRequests(IList<IDictionary<string, string>> requestData)
		{
			foreach (IDictionary<string, string> requestDatum in requestData)
			{
				SaveRequestForLater(requestDatum);
			}
		}

		internal static bool IsResponseSuccess(IDictionary<string, object> response)
		{
			object value;
			if (response.TryGetValue("success", out value))
			{
				return (bool)value;
			}
			LeanplumNative.CompatibilityLayer.LogError("Invalid response (missing field: success)");
			return false;
		}

		internal static string GetResponseError(IDictionary<string, object> response)
		{
			if (response.ContainsKey("error"))
			{
				IDictionary<string, object> dictionary = response["error"] as IDictionary<string, object>;
				if (dictionary != null && dictionary.ContainsKey("message"))
				{
					return dictionary["message"] as string;
				}
			}
			LeanplumNative.CompatibilityLayer.LogError("Invalid response (missing field: error)");
			return null;
		}
	}
}
