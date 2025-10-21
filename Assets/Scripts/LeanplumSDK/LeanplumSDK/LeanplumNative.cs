using System;
using System.Collections;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;
using UnityEngine;

namespace LeanplumSDK
{
	public class LeanplumNative : LeanplumSDKObject
	{
		internal static bool calledStart;

		private static bool startSuccessful;

		private static bool isPaused;

		private static string customDeviceId;

		internal static ICompatibilityLayer CompatibilityLayer = new UnityCompatibilityLayer();

		internal static LeanplumSocket leanplumSocket;

		internal static bool isStopped;

		private static bool _hasStarted;

		private static bool _HasStartedAndRegisteredAsDeveloper;

		private static event Leanplum.VariableChangedHandler variablesChanged;

		private static event Leanplum.VariablesChangedAndNoDownloadsPendingHandler variablesChangedAndNoDownloadsPending;

		private static event Leanplum.StartHandler started;

		public override event Leanplum.VariableChangedHandler VariablesChanged
		{
			add
			{
				LeanplumNative.variablesChanged = (Leanplum.VariableChangedHandler)Delegate.Combine(LeanplumNative.variablesChanged, value);
				if (VarCache.HasReceivedDiffs)
				{
					value();
				}
			}
			remove
			{
				LeanplumNative.variablesChanged = (Leanplum.VariableChangedHandler)Delegate.Remove(LeanplumNative.variablesChanged, value);
			}
		}

		public override event Leanplum.VariablesChangedAndNoDownloadsPendingHandler VariablesChangedAndNoDownloadsPending
		{
			add
			{
				LeanplumNative.variablesChangedAndNoDownloadsPending = (Leanplum.VariablesChangedAndNoDownloadsPendingHandler)Delegate.Combine(LeanplumNative.variablesChangedAndNoDownloadsPending, value);
				if (_hasStarted && VarCache.HasReceivedDiffs)
				{
					value();
				}
			}
			remove
			{
				LeanplumNative.variablesChangedAndNoDownloadsPending = (Leanplum.VariablesChangedAndNoDownloadsPendingHandler)Delegate.Remove(LeanplumNative.variablesChangedAndNoDownloadsPending, value);
			}
		}

		public override event Leanplum.StartHandler Started
		{
			add
			{
				LeanplumNative.started = (Leanplum.StartHandler)Delegate.Combine(LeanplumNative.started, value);
				if (_hasStarted)
				{
					value(startSuccessful);
				}
			}
			remove
			{
				LeanplumNative.started = (Leanplum.StartHandler)Delegate.Remove(LeanplumNative.started, value);
			}
		}

		private static void ValidateAttributes(IDictionary<string, object> attributes)
		{
			if (attributes == null)
			{
				return;
			}
			foreach (object value in attributes.Values)
			{
				if (!Util.IsNumber(value) && !(value is string) && !(value is bool?))
				{
					Util.MaybeThrow(new LeanplumException("userAttributes values must be of type string, number type, or bool."));
				}
			}
		}

		public override bool HasStarted()
		{
			return _hasStarted;
		}

		public override bool HasStartedAndRegisteredAsDeveloper()
		{
			return _HasStartedAndRegisteredAsDeveloper;
		}

		public override bool IsDeveloperModeEnabled()
		{
			return Constants.isDevelopmentModeEnabled;
		}

		public override void SetApiConnectionSettings(string hostName, string servletName = "api", bool useSSL = true)
		{
			Constants.API_HOST_NAME = hostName;
			Constants.API_SERVLET = servletName;
			Constants.API_SSL = useSSL;
		}

		public override void SetSocketConnectionSettings(string hostName, int port)
		{
			Constants.SOCKET_HOST = hostName;
			Constants.SOCKET_PORT = port;
		}

		public override void SetNetworkTimeout(int seconds, int downloadSeconds)
		{
			Constants.NETWORK_TIMEOUT_SECONDS = seconds;
			Constants.NETWORK_TIMEOUT_SECONDS_FOR_DOWNLOADS = downloadSeconds;
		}

		public override void SetAppIdForDevelopmentMode(string appId, string accessKey)
		{
			Constants.isDevelopmentModeEnabled = true;
			LeanplumRequest.SetAppId(appId, accessKey);
		}

		public override void SetAppIdForProductionMode(string appId, string accessKey)
		{
			Constants.isDevelopmentModeEnabled = false;
			LeanplumRequest.SetAppId(appId, accessKey);
		}

		public override void SetAppVersion(string version)
		{
			CompatibilityLayer.VersionName = version;
		}

		public override void SetDeviceId(string deviceId)
		{
			customDeviceId = deviceId;
		}

		public override void SetTestMode(bool testModeEnabled)
		{
			if (calledStart)
			{
				CompatibilityLayer.LogWarning("Leanplum was already started. Call SetTestMode before calling Start.");
			}
			Constants.isNoop = testModeEnabled;
		}

		public override void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled)
		{
			Constants.EnableRealtimeUpdatesInDevelopmentMode = enabled;
		}

		public override object ObjectForKeyPath(params object[] components)
		{
			return VarCache.GetMergedValueFromComponentArray(components);
		}

		public override object ObjectForKeyPathComponents(object[] pathComponents)
		{
			return VarCache.GetMergedValueFromComponentArray(pathComponents);
		}

		private static void OnStarted(bool success)
		{
			if (LeanplumNative.started != null)
			{
				LeanplumNative.started(success);
			}
		}

		internal static void OnHasStartedAndRegisteredAsDeveloper()
		{
			_HasStartedAndRegisteredAsDeveloper = true;
			VarCache.MaybeSendUpdatedValuesFromCode();
		}

		private static void OnVariablesChanged()
		{
			if (LeanplumNative.variablesChanged != null)
			{
				LeanplumNative.variablesChanged();
			}
		}

		private static void OnVariablesChangedAndNoDownloadsPending()
		{
			if (LeanplumNative.variablesChangedAndNoDownloadsPending != null)
			{
				LeanplumNative.variablesChangedAndNoDownloadsPending();
			}
		}

		public override void Start(string userId, IDictionary<string, object> attributes, Leanplum.StartHandler startResponseAction)
		{
			if (calledStart)
			{
				CompatibilityLayer.Log("Already called start");
				return;
			}
			if (string.IsNullOrEmpty(LeanplumRequest.AppId))
			{
				CompatibilityLayer.LogError("You cannot call Start without setting your app's API keys.");
				return;
			}
			if (CompatibilityLayer.GetPlatformName() == "Standalone")
			{
				Constants.API_SSL = false;
			}
			if (startResponseAction != null)
			{
				Started += startResponseAction;
			}
			if (Constants.isNoop)
			{
				_hasStarted = true;
				startSuccessful = true;
				OnVariablesChanged();
				OnVariablesChangedAndNoDownloadsPending();
				OnStarted(true);
				VarCache.ApplyVariableDiffs(null);
				return;
			}
			ValidateAttributes(attributes);
			calledStart = true;
			VarCache.IsSilent = true;
			VarCache.LoadDiffs();
			VarCache.IsSilent = false;
			VarCache.Update += delegate
			{
				OnVariablesChanged();
				if (LeanplumRequest.PendingDownloads == 0)
				{
					OnVariablesChangedAndNoDownloadsPending();
				}
			};
			LeanplumRequest.NoPendingDownloads += delegate
			{
				OnVariablesChangedAndNoDownloadsPending();
			};
			string userId2 = (LeanplumRequest.DeviceId = ((customDeviceId == null) ? CompatibilityLayer.GetDeviceId() : customDeviceId));
			if (!string.IsNullOrEmpty(userId))
			{
				LeanplumRequest.UserId = userId;
			}
			if (string.IsNullOrEmpty(LeanplumRequest.UserId))
			{
				LeanplumRequest.UserId = userId2;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["includeDefaults"] = false.ToString();
			dictionary["versionName"] = CompatibilityLayer.VersionName ?? string.Empty;
			dictionary["deviceName"] = CompatibilityLayer.GetDeviceName();
			dictionary["deviceModel"] = CompatibilityLayer.GetDeviceModel();
			dictionary["systemName"] = CompatibilityLayer.GetSystemName();
			dictionary["systemVersion"] = CompatibilityLayer.GetSystemVersion();
			TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
			if (currentTimeZone.IsDaylightSavingTime(DateTime.UtcNow))
			{
				dictionary["timezone"] = currentTimeZone.DaylightName;
			}
			else
			{
				dictionary["timezone"] = currentTimeZone.StandardName;
			}
			dictionary["timezoneOffsetSeconds"] = currentTimeZone.GetUtcOffset(DateTime.UtcNow).TotalSeconds.ToString();
			dictionary["country"] = "(detect)";
			dictionary["region"] = "(detect)";
			dictionary["city"] = "(detect)";
			dictionary["location"] = "(detect)";
			if (attributes != null)
			{
				dictionary["userAttributes"] = Json.Serialize(attributes);
			}
			LeanplumRequest leanplumRequest = LeanplumRequest.Post("start", dictionary);
			leanplumRequest.Response += delegate(object responsesObject)
			{
				IDictionary<string, object> dictionary2 = Util.GetLastResponse(responsesObject) as IDictionary<string, object>;
				IDictionary<string, object> diffs = (Util.GetValueOrDefault(dictionary2, "vars") as IDictionary<string, object>) ?? new Dictionary<string, object>();
				IDictionary<string, object> fileAttributes = (Util.GetValueOrDefault(dictionary2, "fileAttributes") as IDictionary<string, object>) ?? new Dictionary<string, object>();
				bool flag = (bool)Util.GetValueOrDefault(dictionary2, "isRegistered", false);
				LeanplumRequest.Token = (Util.GetValueOrDefault(dictionary2, "token") as string) ?? string.Empty;
				if (Constants.isDevelopmentModeEnabled)
				{
					VarCache.SetDevModeValuesFromServer(Util.GetValueOrDefault(dictionary2, "varsFromCode") as Dictionary<string, object>);
					if (Constants.EnableRealtimeUpdatesInDevelopmentMode && SocketUtilsFactory.Utils.AreSocketsAvailable)
					{
						leanplumSocket = new LeanplumSocket(delegate
						{
							VarCache.VarsNeedUpdate = true;
						});
					}
					if (flag)
					{
						string text2 = Util.GetValueOrDefault(dictionary2, "latestVersion") as string;
						if (text2 != null)
						{
							CompatibilityLayer.Log("Leanplum Unity SDK " + text2 + " available. Go to https://www.leanplum.com/dashboard to download it.");
						}
						OnHasStartedAndRegisteredAsDeveloper();
					}
				}
				VarCache.ApplyVariableDiffs(diffs, fileAttributes);
				_hasStarted = true;
				startSuccessful = true;
				OnStarted(true);
				CompatibilityLayer.Init();
			};
			leanplumRequest.Error += delegate
			{
				VarCache.ApplyVariableDiffs(null);
				_hasStarted = true;
				startSuccessful = false;
				OnStarted(false);
				CompatibilityLayer.Init();
			};
			leanplumRequest.SendIfConnected();
		}

		public override void TrackGooglePlayPurchase(string item, long priceMicros, string currencyCode, string purchaseData, string dataSignature, IDictionary<string, object> parameters)
		{
			IDictionary<string, object> dictionary = ((parameters != null) ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>());
			dictionary.Add("item", item);
			IDictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2["googlePlayPurchaseData"] = purchaseData;
			dictionary2["googlePlayPurchaseDataSignature"] = dataSignature;
			dictionary2["currencyCode"] = currencyCode;
			Track("Purchase", (double)priceMicros / 1000000.0, null, dictionary, dictionary2);
		}

		public override void TrackIOSInAppPurchase(string item, double unitPrice, int quantity, string currencyCode, string transactionIdentifier, string receiptData, IDictionary<string, object> parameters)
		{
			IDictionary<string, object> dictionary = ((parameters != null) ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>());
			dictionary.Add("item", item);
			dictionary.Add("quantity", quantity);
			IDictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2["iOSTransactionIdentifier"] = transactionIdentifier;
			dictionary2["iOSReceiptData"] = receiptData;
			dictionary2["iOSSandbox"] = ((!IsDeveloperModeEnabled()) ? "false" : "true");
			dictionary2["currencyCode"] = currencyCode;
			Track("Purchase", unitPrice * (double)quantity, null, dictionary, dictionary2);
		}

		public override void Track(string eventName, double value, string info, IDictionary<string, object> parameters)
		{
			Track(eventName, value, info, parameters, null);
		}

		public void Track(string eventName, double value, string info, IDictionary<string, object> parameters, IDictionary<string, string> arguments)
		{
			if (Constants.isNoop)
			{
				return;
			}
			if (!calledStart)
			{
				CompatibilityLayer.LogError("You cannot call Track before calling Start.");
				return;
			}
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["event"] = eventName;
			dictionary["value"] = value.ToString();
			if (info != null)
			{
				dictionary["info"] = info;
			}
			if (parameters != null)
			{
				dictionary["params"] = Json.Serialize(parameters);
			}
			if (arguments != null)
			{
				foreach (string key in arguments.Keys)
				{
					dictionary[key] = arguments[key];
				}
			}
			LeanplumRequest.Post("track", dictionary).Send();
		}

		public override void AdvanceTo(string state, string info, IDictionary<string, object> parameters)
		{
			if (Constants.isNoop)
			{
				return;
			}
			if (!calledStart)
			{
				CompatibilityLayer.LogError("You cannot call AdvanceTo before calling Start.");
				return;
			}
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["info"] = info;
			if (state != null)
			{
				dictionary["state"] = state;
			}
			if (parameters != null)
			{
				dictionary["params"] = Json.Serialize(parameters);
			}
			LeanplumRequest.Post("advance", dictionary).Send();
		}

		public override void SetUserAttributes(string newUserId, IDictionary<string, object> value)
		{
			if (!calledStart)
			{
				CompatibilityLayer.LogWarning("Start was not called. Set user ID and attributes as the arguments when calling Start.");
			}
			else
			{
				if (Constants.isNoop)
				{
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (value != null)
				{
					ValidateAttributes(value);
					dictionary["userAttributes"] = Json.Serialize(value);
				}
				if (!string.IsNullOrEmpty(newUserId))
				{
					dictionary["newUserId"] = newUserId;
					VarCache.SaveDiffs();
				}
				LeanplumRequest.Post("setUserAttributes", dictionary).Send();
				if (!string.IsNullOrEmpty(newUserId))
				{
					LeanplumRequest.UserId = newUserId;
					if (_hasStarted)
					{
						VarCache.SaveDiffs();
					}
				}
			}
		}

		public override void PauseState()
		{
			if (!Constants.isNoop)
			{
				if (!calledStart)
				{
					CompatibilityLayer.LogError("You cannot call PauseState before calling start.");
				}
				else
				{
					LeanplumRequest.Post("pauseState", new Dictionary<string, string>()).Send();
				}
			}
		}

		public override void ResumeState()
		{
			if (!Constants.isNoop)
			{
				if (!calledStart)
				{
					CompatibilityLayer.LogError("You cannot call ResumeState before calling start.");
				}
				else
				{
					LeanplumRequest.Post("resumeState", new Dictionary<string, string>()).Send();
				}
			}
		}

		public override void ForceContentUpdate()
		{
			ForceContentUpdate(null);
		}

		public override void ForceContentUpdate(Action callback)
		{
			VarCache.CheckVarsUpdate(callback);
		}

		internal static void Pause()
		{
			if (Constants.isNoop)
			{
				return;
			}
			if (!calledStart)
			{
				CompatibilityLayer.LogError("You cannot call Pause before calling start.");
			}
			else if (!isPaused)
			{
				isPaused = true;
				LeanplumRequest leanplumRequest = LeanplumRequest.Post("pauseSession", null);
				leanplumRequest.Response += delegate
				{
					CompatibilityLayer.FlushSavedSettings();
				};
				leanplumRequest.Error += delegate
				{
					CompatibilityLayer.FlushSavedSettings();
				};
				leanplumRequest.SendIfConnected();
			}
		}

		internal static void Resume()
		{
			if (!Constants.isNoop)
			{
				if (!calledStart)
				{
					CompatibilityLayer.LogError("You cannot call Resume before calling start.");
				}
				else if (isPaused)
				{
					isPaused = false;
					LeanplumRequest.Post("resumeSession", null).SendIfConnected();
				}
			}
		}

		internal static void Stop()
		{
			if (Constants.isNoop)
			{
				return;
			}
			if (!calledStart)
			{
				CompatibilityLayer.LogError("You cannot call Stop before calling start.");
				return;
			}
			if (leanplumSocket != null)
			{
				leanplumSocket.Close();
			}
			LeanplumRequest.Post("stop", null).SendIfConnected();
		}

		internal static void Reset()
		{
			if (calledStart)
			{
				Stop();
			}
			calledStart = false;
			_hasStarted = false;
			_HasStartedAndRegisteredAsDeveloper = false;
			startSuccessful = false;
			LeanplumNative.variablesChanged = null;
			LeanplumNative.variablesChangedAndNoDownloadsPending = null;
			LeanplumNative.started = null;
			LeanplumRequest.ClearNoPendingDownloads();
		}

		public override Var<U> Define<U>(string name, U defaultValue)
		{
			string text = null;
			if (defaultValue is int || defaultValue is long || defaultValue is short || defaultValue is char || defaultValue is sbyte || defaultValue is byte)
			{
				text = "integer";
			}
			else if (defaultValue is float || defaultValue is double || defaultValue is decimal)
			{
				text = "float";
			}
			else if (defaultValue is string)
			{
				text = "string";
			}
			else if (defaultValue is IList || defaultValue is Array)
			{
				text = "list";
			}
			else if (defaultValue is IDictionary)
			{
				text = "group";
			}
			else
			{
				if (!(defaultValue is bool))
				{
					Util.MaybeThrow(new LeanplumException("Default value for \"" + name + "\" not recognized or supported."));
					return null;
				}
				text = "bool";
			}
			NativeVar<U> nativeVar = DefineHelper(name, text, defaultValue);
			if (nativeVar != null)
			{
				nativeVar.defaultClonedContainer = DeepCopyContainer(defaultValue);
				nativeVar._defaultValue = defaultValue;
				VarCache.RegisterVariable(nativeVar);
				nativeVar.Update();
			}
			return nativeVar;
		}

		private static NativeVar<U> DefineHelper<U>(string name, string kind, U defaultValue)
		{
			NativeVar<U> nativeVar = (NativeVar<U>)VarCache.GetVariable<U>(name);
			if (nativeVar != null)
			{
				nativeVar.ClearValueChangedCallbacks();
				return nativeVar;
			}
			if (VarCache.HasVariable(name))
			{
				CompatibilityLayer.LogWarning("Failed to define variable: \"" + name + "\" refers to an existing Leanplum variable of a different type.");
				return null;
			}
			return new NativeVar<U>(name, kind, defaultValue, string.Empty);
		}

		private static object DeepCopyContainer(object container)
		{
			object obj = null;
			if (container is IDictionary)
			{
				obj = new Dictionary<object, object>();
				foreach (object key in ((IDictionary)container).Keys)
				{
					((IDictionary)obj).Add(key, DeepCopyContainer(((IDictionary)container)[key]));
				}
			}
			else if (container is IList)
			{
				obj = new List<object>();
				foreach (object item in (IList)container)
				{
					((IList)obj).Add(DeepCopyContainer(item));
				}
			}
			else
			{
				obj = container;
			}
			return obj;
		}

		public override Var<AssetBundle> DefineAssetBundle(string name, bool realtimeUpdating = true, string iosBundleName = "", string androidBundleName = "", string standaloneBundleName = "")
		{
			string platformName = CompatibilityLayer.GetPlatformName();
			string name2 = "__Unity Resources" + '.' + platformName + " Assets." + name;
			string filename = string.Empty;
			switch (platformName)
			{
			case "iOS":
				filename = iosBundleName;
				break;
			case "Android":
				filename = androidBundleName;
				break;
			case "Standalone":
				filename = standaloneBundleName;
				break;
			}
			NativeVar<AssetBundle> nativeVar = DefineHelper<AssetBundle>(name2, "file", null);
			if (nativeVar != null)
			{
				nativeVar.SetFilename(filename);
				nativeVar.fileReady = false;
				nativeVar.realtimeAssetUpdating = realtimeUpdating;
				VarCache.RegisterVariable(nativeVar);
				nativeVar.Update();
			}
			return nativeVar;
		}
	}
}
