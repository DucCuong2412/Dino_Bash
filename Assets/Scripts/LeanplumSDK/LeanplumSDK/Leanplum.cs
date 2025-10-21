using System;
using System.Collections.Generic;

namespace LeanplumSDK
{
	public static class Leanplum
	{
		public delegate void StartHandler(bool success);

		public delegate void VariableChangedHandler();

		public delegate void VariablesChangedAndNoDownloadsPendingHandler();

		public const string PURCHASE_EVENT_NAME = "Purchase";

		public static string LeanplumGcmSenderId
		{
			get
			{
				return LeanplumFactory.SDK.LeanplumGcmSenderId;
			}
		}

		public static bool HasStarted
		{
			get
			{
				return LeanplumFactory.SDK.HasStarted();
			}
		}

		public static bool HasStartedAndRegisteredAsDeveloper
		{
			get
			{
				return LeanplumFactory.SDK.HasStartedAndRegisteredAsDeveloper();
			}
		}

		public static bool IsDeveloperModeEnabled
		{
			get
			{
				return LeanplumFactory.SDK.IsDeveloperModeEnabled();
			}
		}

		public static event VariableChangedHandler VariablesChanged
		{
			add
			{
				LeanplumFactory.SDK.VariablesChanged += value;
			}
			remove
			{
				LeanplumFactory.SDK.VariablesChanged -= value;
			}
		}

		public static event VariablesChangedAndNoDownloadsPendingHandler VariablesChangedAndNoDownloadsPending
		{
			add
			{
				LeanplumFactory.SDK.VariablesChangedAndNoDownloadsPending += value;
			}
			remove
			{
				LeanplumFactory.SDK.VariablesChangedAndNoDownloadsPending -= value;
			}
		}

		public static event StartHandler Started
		{
			add
			{
				LeanplumFactory.SDK.Started += value;
			}
			remove
			{
				LeanplumFactory.SDK.Started -= value;
			}
		}

		public static void SetApiConnectionSettings(string hostName, string servletName = "api", bool useSSL = true)
		{
			LeanplumFactory.SDK.SetApiConnectionSettings(hostName, servletName, useSSL);
		}

		public static void SetSocketConnectionSettings(string hostName, int port)
		{
			LeanplumFactory.SDK.SetSocketConnectionSettings(hostName, port);
		}

		public static void SetNetworkTimeout(int seconds, int downloadSeconds)
		{
			LeanplumFactory.SDK.SetNetworkTimeout(seconds, downloadSeconds);
		}

		public static void SetAppIdForDevelopmentMode(string appId, string accessKey)
		{
			LeanplumFactory.SDK.SetAppIdForDevelopmentMode(appId, accessKey);
		}

		public static void SetAppIdForProductionMode(string appId, string accessKey)
		{
			LeanplumFactory.SDK.SetAppIdForProductionMode(appId, accessKey);
		}

		public static void SetAppVersion(string version)
		{
			LeanplumFactory.SDK.SetAppVersion(version);
		}

		public static void SetDeviceId(string deviceId)
		{
			LeanplumFactory.SDK.SetDeviceId(deviceId);
		}

		public static void SetTestMode(bool testModeEnabled)
		{
			LeanplumFactory.SDK.SetTestMode(testModeEnabled);
		}

		public static void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled)
		{
			LeanplumFactory.SDK.SetRealtimeUpdatesInDevelopmentModeEnabled(enabled);
		}

		public static void SetGcmSenderId(string senderId)
		{
			LeanplumFactory.SDK.SetGcmSenderId(senderId);
		}

		public static void SetGcmSenderIds(params string[] senderIds)
		{
			LeanplumFactory.SDK.SetGcmSenderIds(senderIds);
		}

		public static void RegisterForIOSRemoteNotifications()
		{
			LeanplumFactory.SDK.RegisterForIOSRemoteNotifications();
		}

		public static object ObjectForKeyPath(params object[] components)
		{
			return LeanplumFactory.SDK.ObjectForKeyPath(components);
		}

		public static object ObjectForKeyPathComponents(object[] pathComponents)
		{
			return LeanplumFactory.SDK.ObjectForKeyPathComponents(pathComponents);
		}

		public static void Start()
		{
			LeanplumFactory.SDK.Start(null, null, null);
		}

		public static void Start(StartHandler callback)
		{
			LeanplumFactory.SDK.Start(null, null, callback);
		}

		public static void Start(IDictionary<string, object> userAttributes)
		{
			LeanplumFactory.SDK.Start(null, userAttributes, null);
		}

		public static void Start(string userId)
		{
			LeanplumFactory.SDK.Start(userId, null, null);
		}

		public static void Start(string userId, StartHandler callback)
		{
			LeanplumFactory.SDK.Start(userId, null, callback);
		}

		public static void Start(string userId, IDictionary<string, object> userAttributes)
		{
			LeanplumFactory.SDK.Start(userId, userAttributes, null);
		}

		public static void Start(string userId, IDictionary<string, object> attributes, StartHandler startResponseAction)
		{
			LeanplumFactory.SDK.Start(userId, attributes, startResponseAction);
		}

		public static void TrackGooglePlayPurchase(string item, long priceMicros, string currencyCode, string purchaseData, string dataSignature, IDictionary<string, object> parameters)
		{
			LeanplumFactory.SDK.TrackGooglePlayPurchase(item, priceMicros, currencyCode, purchaseData, dataSignature, parameters);
		}

		public static void TrackIOSInAppPurchases()
		{
			LeanplumFactory.SDK.TrackIOSInAppPurchases();
		}

		public static void TrackIOSInAppPurchase(string item, double unitPrice, int quantity, string currencyCode, string transactionIdentifier, string receiptData, IDictionary<string, object> parameters)
		{
			LeanplumFactory.SDK.TrackIOSInAppPurchase(item, unitPrice, quantity, currencyCode, transactionIdentifier, receiptData, parameters);
		}

		public static void Track(string eventName, double value, string info, IDictionary<string, object> parameters)
		{
			LeanplumFactory.SDK.Track(eventName, value, info, parameters);
		}

		public static void Track(string eventName)
		{
			LeanplumFactory.SDK.Track(eventName, 0.0, string.Empty, null);
		}

		public static void Track(string eventName, double value)
		{
			LeanplumFactory.SDK.Track(eventName, value, string.Empty, null);
		}

		public static void Track(string eventName, string info)
		{
			LeanplumFactory.SDK.Track(eventName, 0.0, info, null);
		}

		public static void Track(string eventName, Dictionary<string, object> parameters)
		{
			LeanplumFactory.SDK.Track(eventName, 0.0, string.Empty, parameters);
		}

		public static void Track(string eventName, double value, Dictionary<string, object> parameters)
		{
			LeanplumFactory.SDK.Track(eventName, value, string.Empty, parameters);
		}

		public static void Track(string eventName, double value, string info)
		{
			LeanplumFactory.SDK.Track(eventName, value, info, null);
		}

		public static void AdvanceTo(string state, string info, IDictionary<string, object> parameters)
		{
			LeanplumFactory.SDK.AdvanceTo(state, info, parameters);
		}

		public static void AdvanceTo(string state)
		{
			LeanplumFactory.SDK.AdvanceTo(state, string.Empty, null);
		}

		public static void AdvanceTo(string state, string info)
		{
			LeanplumFactory.SDK.AdvanceTo(state, info, null);
		}

		public static void AdvanceTo(string state, Dictionary<string, object> parameters)
		{
			LeanplumFactory.SDK.AdvanceTo(state, string.Empty, parameters);
		}

		public static void SetUserId(string newUserId)
		{
			LeanplumFactory.SDK.SetUserId(newUserId);
		}

		public static void SetUserAttributes(IDictionary<string, object> value)
		{
			LeanplumFactory.SDK.SetUserAttributes(null, value);
		}

		public static void SetUserAttributes(string newUserId, IDictionary<string, object> value)
		{
			LeanplumFactory.SDK.SetUserAttributes(newUserId, value);
		}

		public static void PauseState()
		{
			LeanplumFactory.SDK.PauseState();
		}

		public static void ResumeState()
		{
			LeanplumFactory.SDK.ResumeState();
		}

		public static void ForceContentUpdate()
		{
			LeanplumFactory.SDK.ForceContentUpdate();
		}

		public static void ForceContentUpdate(Action callback)
		{
			LeanplumFactory.SDK.ForceContentUpdate(callback);
		}
	}
}
