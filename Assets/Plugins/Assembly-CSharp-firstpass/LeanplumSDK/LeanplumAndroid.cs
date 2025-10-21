using System;
using System.Collections;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;
using UnityEngine;

namespace LeanplumSDK
{
	public class LeanplumAndroid : LeanplumSDKObject
	{
		private static AndroidJavaClass nativeSdk = null;

		private Dictionary<int, Action> ForceContentUpdateCallbackDictionary = new Dictionary<int, Action>();

		private static int DictionaryKey = 0;

		protected static IDictionary<string, Var> AndroidVarCache = new Dictionary<string, Var>();

		internal static AndroidJavaClass NativeSDK
		{
			get
			{
				if (nativeSdk == null)
				{
					AndroidJNI.AttachCurrentThread();
					nativeSdk = new AndroidJavaClass("com.leanplum.UnityBridge");
				}
				return nativeSdk;
			}
			set
			{
				nativeSdk = value;
			}
		}

		public override string LeanplumGcmSenderId
		{
			get
			{
				return NativeSDK.GetStatic<string>("LEANPLUM_SENDER_ID");
			}
		}

		public override event Leanplum.VariableChangedHandler VariablesChanged;

		public override event Leanplum.VariablesChangedAndNoDownloadsPendingHandler VariablesChangedAndNoDownloadsPending;

		public override event Leanplum.StartHandler Started;

		public override bool HasStarted()
		{
			return NativeSDK.CallStatic<bool>("hasStarted", new object[0]);
		}

		public override bool HasStartedAndRegisteredAsDeveloper()
		{
			return NativeSDK.CallStatic<bool>("hasStartedAndRegisteredAsDeveloper", new object[0]);
		}

		public override bool IsDeveloperModeEnabled()
		{
			return NativeSDK.CallStatic<bool>("isDeveloperModeEnabled", new object[0]);
		}

		public override void SetApiConnectionSettings(string hostName, string servletName = "api", bool useSSL = true)
		{
			NativeSDK.CallStatic("setApiConnectionSettings", hostName, servletName, useSSL);
		}

		public override void SetSocketConnectionSettings(string hostName, int port)
		{
			NativeSDK.CallStatic("setSocketConnectionSettings", hostName, port);
		}

		public override void SetNetworkTimeout(int seconds, int downloadSeconds)
		{
			NativeSDK.CallStatic("setNetworkTimeout", seconds, downloadSeconds);
		}

		public override void SetAppIdForDevelopmentMode(string appId, string accessKey)
		{
			NativeSDK.CallStatic("setAppIdForDevelopmentMode", appId, accessKey);
		}

		public override void SetAppIdForProductionMode(string appId, string accessKey)
		{
			NativeSDK.CallStatic("setAppIdForProductionMode", appId, accessKey);
		}

		public override void SetAppVersion(string version)
		{
		}

		public override void SetDeviceId(string deviceId)
		{
			NativeSDK.CallStatic("setDeviceId", deviceId);
		}

		public override void SetTestMode(bool testModeEnabled)
		{
			NativeSDK.CallStatic("setTestModeEnabled", testModeEnabled);
		}

		public override void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled)
		{
			NativeSDK.CallStatic("setFileUploadingEnabledInDevelopmentMode", enabled);
		}

		public override void SetGcmSenderId(string senderId)
		{
			NativeSDK.CallStatic("setGcmSenderId", senderId);
		}

		public override void SetGcmSenderIds(string[] senderIds)
		{
			NativeSDK.CallStatic("setGcmSenderIds", Json.Serialize(senderIds));
		}

		public override object ObjectForKeyPath(params object[] components)
		{
			string json = NativeSDK.CallStatic<string>("objectForKeyPath", new object[1] { Json.Serialize(components) });
			return Json.Deserialize(json);
		}

		public override object ObjectForKeyPathComponents(object[] pathComponents)
		{
			string json = NativeSDK.CallStatic<string>("objectForKeyPathComponents", new object[1] { Json.Serialize(pathComponents) });
			return Json.Deserialize(json);
		}

		public override void Start(string userId, IDictionary<string, object> attributes, Leanplum.StartHandler startResponseAction)
		{
			Started = (Leanplum.StartHandler)Delegate.Combine(Started, startResponseAction);
			NativeSDK.CallStatic("start", userId, Json.Serialize(attributes), LeanplumUnityHelper.Instance.gameObject.name, "1.2.11", SystemInfo.deviceUniqueIdentifier);
		}

		public override void Track(string eventName, double value, string info, IDictionary<string, object> parameters)
		{
			NativeSDK.CallStatic("track", eventName, value, info, Json.Serialize(parameters));
		}

		public override void AdvanceTo(string state, string info, IDictionary<string, object> parameters)
		{
			NativeSDK.CallStatic("advanceTo", state, info, Json.Serialize(parameters));
		}

		public override void SetUserAttributes(string newUserId, IDictionary<string, object> value)
		{
			NativeSDK.CallStatic("setUserAttributes", newUserId, Json.Serialize(value));
		}

		public override void PauseState()
		{
			NativeSDK.CallStatic("pauseState");
		}

		public override void ResumeState()
		{
			NativeSDK.CallStatic("resumeState");
		}

		public override void ForceContentUpdate()
		{
			NativeSDK.CallStatic("forceContentUpdate");
		}

		public override void ForceContentUpdate(Action callback)
		{
			int num = DictionaryKey++;
			ForceContentUpdateCallbackDictionary.Add(num, callback);
			NativeSDK.CallStatic("forceContentUpdateWithCallback", num);
		}

		public override void NativeCallback(string message)
		{
			if (message.StartsWith("VariablesChanged:"))
			{
				if (VariablesChanged != null)
				{
					VariablesChanged();
				}
			}
			else if (message.StartsWith("VariablesChangedAndNoDownloadsPending:"))
			{
				if (VariablesChangedAndNoDownloadsPending != null)
				{
					VariablesChangedAndNoDownloadsPending();
				}
			}
			else if (message.StartsWith("Started:"))
			{
				if (Started != null)
				{
					bool success = message.EndsWith("true") || message.EndsWith("True");
					Started(success);
				}
			}
			else if (message.StartsWith("VariableValueChanged:"))
			{
				VariableValueChanged(message.Substring(21));
			}
			else if (message.StartsWith("ForceContentUpdateWithCallback:"))
			{
				int key = Convert.ToInt32(message.Substring(31));
				Action value;
				if (ForceContentUpdateCallbackDictionary.TryGetValue(key, out value))
				{
					value();
					ForceContentUpdateCallbackDictionary.Remove(key);
				}
			}
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
					Debug.LogError("Leanplum Error: Default value for \"" + name + "\" not recognized or supported.");
					return null;
				}
				text = "bool";
			}
			if (AndroidVarCache.ContainsKey(name))
			{
				if (AndroidVarCache[name].Kind != text)
				{
					Debug.LogError("Leanplum Error: \"" + name + "\" was already defined with a different kind");
					return null;
				}
				return (Var<U>)AndroidVarCache[name];
			}
			NativeSDK.CallStatic("defineVar", name, text, Json.Serialize(defaultValue));
			Var<U> var = new AndroidVar<U>(name, text, defaultValue, string.Empty);
			AndroidVarCache[name] = var;
			return var;
		}

		public override Var<AssetBundle> DefineAssetBundle(string name, bool realtimeUpdating = true, string iosBundleName = "", string androidBundleName = "", string standaloneBundleName = "")
		{
			string text = "file";
			string text2 = "__Unity Resources.Android.Assets." + name;
			string text3 = "assets/" + androidBundleName;
			if (AndroidVarCache.ContainsKey(text2))
			{
				if (AndroidVarCache[text2].Kind != text)
				{
					Debug.LogError("Leanplum Error: \"" + name + "\" was already defined with a different kind");
					return null;
				}
				return (Var<AssetBundle>)AndroidVarCache[text2];
			}
			Var<AssetBundle> var = new AndroidVar<AssetBundle>(text2, text, null, text3);
			AndroidVarCache[text2] = var;
			NativeSDK.CallStatic("defineVar", text2, "file", text3);
			return var;
		}

		public static void VariableValueChanged(string name)
		{
			Var var = AndroidVarCache[name];
			if (var != null)
			{
				var.OnValueChanged();
			}
		}
	}
}
