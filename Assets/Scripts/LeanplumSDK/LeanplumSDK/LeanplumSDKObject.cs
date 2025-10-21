using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeanplumSDK
{
	public abstract class LeanplumSDKObject
	{
		public const string PURCHASE_EVENT_NAME = "Purchase";

		public virtual string LeanplumGcmSenderId
		{
			get
			{
				return string.Empty;
			}
		}

		public abstract event Leanplum.VariableChangedHandler VariablesChanged;

		public abstract event Leanplum.VariablesChangedAndNoDownloadsPendingHandler VariablesChangedAndNoDownloadsPending;

		public abstract event Leanplum.StartHandler Started;

		public abstract bool HasStarted();

		public abstract bool HasStartedAndRegisteredAsDeveloper();

		public abstract bool IsDeveloperModeEnabled();

		public abstract void SetApiConnectionSettings(string hostName, string servletName = "api", bool useSSL = true);

		public abstract void SetSocketConnectionSettings(string hostName, int port);

		public abstract void SetNetworkTimeout(int seconds, int downloadSeconds);

		public abstract void SetAppIdForDevelopmentMode(string appId, string accessKey);

		public abstract void SetAppIdForProductionMode(string appId, string accessKey);

		public abstract void SetAppVersion(string version);

		public abstract void SetDeviceId(string deviceId);

		public abstract void SetTestMode(bool testModeEnabled);

		public abstract void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled);

		public virtual void SetGcmSenderId(string senderId)
		{
		}

		public virtual void SetGcmSenderIds(string[] senderIds)
		{
		}

		public virtual void RegisterForIOSRemoteNotifications()
		{
		}

		public abstract object ObjectForKeyPath(params object[] components);

		public abstract object ObjectForKeyPathComponents(object[] pathComponents);

		public abstract void Start(string userId, IDictionary<string, object> attributes, Leanplum.StartHandler startResponseAction);

		public virtual void TrackIOSInAppPurchases()
		{
		}

		public virtual void TrackGooglePlayPurchase(string item, long priceMicros, string currencyCode, string purchaseData, string dataSignature, IDictionary<string, object> parameters)
		{
			Debug.LogError("TrackGooglePlayPurchase is not supported on the current platform. Call the method in the platform's native SDK.");
		}

		public virtual void TrackIOSInAppPurchase(string item, double unitPrice, int quantity, string currencyCode, string transactionIdentifier, string receiptData, IDictionary<string, object> parameters)
		{
			Debug.LogError("TrackIOSInAppPurchase is not supported on the current platform. Call the method in the platform's native SDK.");
		}

		public abstract void Track(string eventName, double value, string info, IDictionary<string, object> parameters);

		public abstract void AdvanceTo(string state, string info, IDictionary<string, object> parameters);

		public void SetUserId(string newUserId)
		{
			SetUserAttributes(newUserId, null);
		}

		public void SetUserAttributes(IDictionary<string, object> value)
		{
			SetUserAttributes(null, value);
		}

		public abstract void SetUserAttributes(string newUserId, IDictionary<string, object> value);

		public abstract void PauseState();

		public abstract void ResumeState();

		public abstract void ForceContentUpdate();

		public abstract void ForceContentUpdate(Action callback);

		public virtual void NativeCallback(string message)
		{
		}

		public virtual Var<int> Define(string name, int defaultValue)
		{
			return this.Define<int>(name, defaultValue);
		}

		public virtual Var<long> Define(string name, long defaultValue)
		{
			return this.Define<long>(name, defaultValue);
		}

		public virtual Var<short> Define(string name, short defaultValue)
		{
			return this.Define<short>(name, defaultValue);
		}

		public virtual Var<byte> Define(string name, byte defaultValue)
		{
			return this.Define<byte>(name, defaultValue);
		}

		public virtual Var<bool> Define(string name, bool defaultValue)
		{
			return this.Define<bool>(name, defaultValue);
		}

		public virtual Var<float> Define(string name, float defaultValue)
		{
			return this.Define<float>(name, defaultValue);
		}

		public virtual Var<double> Define(string name, double defaultValue)
		{
			return this.Define<double>(name, defaultValue);
		}

		public virtual Var<string> Define(string name, string defaultValue)
		{
			return this.Define<string>(name, defaultValue);
		}

		public virtual Var<List<object>> Define(string name, List<object> defaultValue)
		{
			return this.Define<List<object>>(name, defaultValue);
		}

		public virtual Var<List<string>> Define(string name, List<string> defaultValue)
		{
			return this.Define<List<string>>(name, defaultValue);
		}

		public virtual Var<Dictionary<string, object>> Define(string name, Dictionary<string, object> defaultValue)
		{
			return this.Define<Dictionary<string, object>>(name, defaultValue);
		}

		public virtual Var<Dictionary<string, string>> Define(string name, Dictionary<string, string> defaultValue)
		{
			return this.Define<Dictionary<string, string>>(name, defaultValue);
		}

		public virtual Var<U> Define<U>(string name, U defaultValue)
		{
			return null;
		}

		public abstract Var<AssetBundle> DefineAssetBundle(string name, bool realtimeUpdating = true, string iosBundleName = "", string androidBundleName = "", string standaloneBundleName = "");
	}
}
