using System.Collections.Generic;
using UnityEngine;
using mixpanel.detail;
using mixpanel.platform;

namespace mixpanel
{
	public class Mixpanel : MonoBehaviour
	{
		public class People
		{
			private global::mixpanel.detail.Mixpanel mixpanel;

			private bool tracking_enabled;

			public string FirstName
			{
				set
				{
					if (tracking_enabled)
					{
						mixpanel.people.set_first_name(value);
					}
				}
			}

			public string LastName
			{
				set
				{
					if (tracking_enabled)
					{
						mixpanel.people.set_last_name(value);
					}
				}
			}

			public string Name
			{
				set
				{
					if (tracking_enabled)
					{
						mixpanel.people.set_name(value);
					}
				}
			}

			public string Email
			{
				set
				{
					if (tracking_enabled)
					{
						mixpanel.people.set_email(value);
					}
				}
			}

			public string Phone
			{
				set
				{
					if (tracking_enabled)
					{
						mixpanel.people.set_phone(value);
					}
				}
			}

			public string PushId
			{
				set
				{
					if (tracking_enabled)
					{
						mixpanel.people.set_push_id(value);
					}
				}
			}

			public People(global::mixpanel.detail.Mixpanel mixpanel, bool tracking_enabled)
			{
				this.mixpanel = mixpanel;
				this.tracking_enabled = tracking_enabled;
			}

			public void Set(string property, Value to)
			{
				if (tracking_enabled)
				{
					mixpanel.people.set(property, to);
				}
			}

			public void SetOnce(string property, Value to)
			{
				if (tracking_enabled)
				{
					mixpanel.people.set_once(property, to);
				}
			}

			public void Unset(Value properties)
			{
				if (tracking_enabled)
				{
					mixpanel.people.unset(properties);
				}
			}

			private void UnsetOne(string property)
			{
				if (tracking_enabled)
				{
					mixpanel.people.unset_one(property);
				}
			}

			public void Increment(string property, Value by)
			{
				if (tracking_enabled)
				{
					mixpanel.people.increment(property, by);
				}
			}

			public void Append(string listName, Value value)
			{
				if (tracking_enabled)
				{
					mixpanel.people.append(listName, value);
				}
			}

			public void Union(string listName, Value values)
			{
				if (tracking_enabled)
				{
					mixpanel.people.union_(listName, values);
				}
			}

			public void TrackCharge(double amount, Value properties)
			{
				if (tracking_enabled)
				{
					mixpanel.people.track_charge(amount, properties);
				}
			}

			public void TrackCharge(double amount)
			{
				if (tracking_enabled)
				{
					mixpanel.people.track_charge(amount, new Value());
				}
			}

			public void TrackChargeConverting(double amount, string currency_code, Value properties)
			{
				if (tracking_enabled)
				{
					mixpanel.people.track_charge_converting(amount, currency_code, properties);
				}
			}

			public void TrackChargeConverting(double amount, string currency_code)
			{
				if (tracking_enabled)
				{
					mixpanel.people.track_charge_converting(amount, currency_code, new Value());
				}
			}

			public void ClearCharges()
			{
				if (tracking_enabled)
				{
					mixpanel.people.clear_charges();
				}
			}

			public void DeleteUser()
			{
				if (tracking_enabled)
				{
					mixpanel.people.delete_user();
				}
			}
		}

		[Header("Account")]
		[Tooltip("The token of the Mixpanel project.")]
		public string token = "<your-token-here>";

		[Tooltip("Used when the DEBUG compile flag is set or when in the editor. Usefull if you want to use different tokens for test builds")]
		public string debugToken = "<your-debug-token-here>";

		[Tooltip("automatically emit a user_install event upon first start. And set the people property install_date. This will also attach a install_days property to each tracked event. See get_days_since_install() for details. Also a people property 'rooted' will be set if the device if jailbroken or rooted.")]
		[Header("Advanced Tracking")]
		public bool installTracking;

		[Tooltip("automatically emit session_start and session_end events. this will also attach a session_duration and session_count property to each event. it will also maintain session_count and session_length (total) as a peolple property.")]
		public bool sessionTracking;

		[Tooltip("if true a 'reachability' property of 'offline', 'cellular' or 'wifi' will be attached to each event.")]
		public bool rechabilityTracking;

		[Tooltip("if enabled a property called 'local_time' will be set to and iso formatted string representation of the local time.")]
		public bool localTimeTracking;

		[Tooltip("If enabled, Exception, Errors and failed Assertions will be reported to Mixpanel.")]
		public bool reportUnityErrors;

		[Header("Trouble Shooting")]
		[Tooltip("Also send out data when inside the Unity editor.")]
		public bool trackInEditor;

		[Tooltip("The minimul log level you're interested in. If set to LL_NONE, logging will be disabled.")]
		public mixpanel.detail.Mixpanel.LogEntry.Level minLogLevel = mixpanel.detail.Mixpanel.LogEntry.Level.LL_WARNING;

		[Header("Limits")]
		[Tooltip("If enabled no data will be send, if the device is only connected to a celluar network.")]
		public bool SendOnlyOnLan;

		[Tooltip("How long the app can be in background before the session expires.")]
		public int sessionTimeoutMinutes = 10;

		[Tooltip("Data will be discarded if the outgoing queue grows above this size.")]
		public int maxQueueSizeMB = 5;

		[Tooltip("The maximum number of exceptions, errors and failed assertions to report to the backend. Counter can be reset via ResetErrorCount().")]
		public int maxErrorsToReport = 5;

		[Tooltip("How often to send out data. if set to 0, the data will be send immediately.")]
		public int batchSendInterval;

		private static mixpanel.detail.Mixpanel mp_interface;

		private static bool tracking_enabled = true;

		private NetworkReachability reachability = NetworkReachability.ReachableViaLocalAreaNetwork;

		private static uint error_count;

		private static int max_errors;

		private static People people_;

		public static People people
		{
			get
			{
				if (people_ == null)
				{
					people_ = new People(instance, tracking_enabled);
				}
				return people_;
			}
		}

		public static uint SecondsSinceSessionStart
		{
			get
			{
				if (tracking_enabled && instance != null)
				{
					return instance.get_session_duration_in_seconds();
				}
				return 0u;
			}
		}

		public static uint SecondsSinceAppStart
		{
			get
			{
				if (tracking_enabled && instance != null)
				{
					return instance.get_seconds_since_app_start();
				}
				return 0u;
			}
		}

		public static uint DaysSinceInstall
		{
			get
			{
				if (tracking_enabled && instance != null)
				{
					return instance.get_days_since_install();
				}
				return 0u;
			}
		}

		private static mixpanel.detail.Mixpanel instance
		{
			get
			{
				return mp_interface;
			}
		}

		public static void Identify(string uniqueId)
		{
			if (tracking_enabled)
			{
				instance.identify(uniqueId);
			}
		}

		public static void Alias(string alias)
		{
			if (tracking_enabled)
			{
				instance.alias(alias);
			}
		}

		public static void Register(string key, Value value)
		{
			if (tracking_enabled)
			{
				instance.register_(key, value);
			}
		}

		public static bool RegisterOnce(string key, Value value)
		{
			if (tracking_enabled)
			{
				return instance.register_once(key, value);
			}
			return false;
		}

		public static bool Unregister(string key)
		{
			if (tracking_enabled)
			{
				return instance.unregister(key);
			}
			return false;
		}

		public static void Track(string eventName, Value properties)
		{
			if (tracking_enabled)
			{
				instance.track(eventName, properties);
			}
		}

		public static void Track(string eventName)
		{
			if (tracking_enabled)
			{
				instance.track(eventName, new Value());
			}
		}

		public static void FlushQueue()
		{
			if (tracking_enabled)
			{
				instance.flush_queue();
			}
		}

		public static double GetExchangeRate(string currency_code)
		{
			if (tracking_enabled)
			{
				return instance.get_exchange_rate(currency_code);
			}
			return 0.0;
		}

		public static bool StartTimedEvent(string eventName)
		{
			if (tracking_enabled)
			{
				return instance.start_timed_event(eventName);
			}
			return false;
		}

		public static bool StartTimedEventOnce(string eventName)
		{
			if (tracking_enabled)
			{
				return instance.start_timed_event_once(eventName);
			}
			return false;
		}

		public static bool ClearTimedEvent(string eventName)
		{
			if (tracking_enabled)
			{
				return instance.clear_timed_event(eventName);
			}
			return false;
		}

		public static void ClearTimedEvents()
		{
			if (tracking_enabled)
			{
				instance.clear_timed_events();
			}
		}

		public static void ClearSuperProperties()
		{
			if (tracking_enabled)
			{
				instance.clear_super_properties();
			}
		}

		public static void ClearSendQueues()
		{
			if (tracking_enabled)
			{
				instance.clear_send_queues();
			}
		}

		public static void Reset()
		{
			if (tracking_enabled)
			{
				instance.reset();
			}
		}

		public static void UpdateExchangeRates(Dictionary<string, double> currency_to_usd)
		{
			if (!tracking_enabled)
			{
				return;
			}
			ExchangeRates exchangeRates = new ExchangeRates();
			foreach (KeyValuePair<string, double> item in currency_to_usd)
			{
				exchangeRates.Add(item);
			}
			instance.update_exchange_rates(exchangeRates);
		}

		public static void ResetErrorCount()
		{
			error_count = 0u;
		}

		private void Awake()
		{
			Object.DontDestroyOnLoad(this);
			if (tracking_enabled && mp_interface == null)
			{
				mp_interface = new mixpanel.detail.Mixpanel(token, MixpanelUnityPlatform.get_distinct_id(), MixpanelUnityPlatform.get_storage_directory(), installTracking, sessionTracking, rechabilityTracking, localTimeTracking, true);
				mp_interface.set_minimum_log_level(minLogLevel);
				mp_interface.set_send_only_on_lan(SendOnlyOnLan);
				mp_interface.set_session_expiry_seconds((uint)(60 * sessionTimeoutMinutes));
				mp_interface.set_maximum_queue_size((uint)(maxQueueSizeMB * 1024 * 1024));
				max_errors = maxErrorsToReport;
				Register("$screen_width", Screen.width);
				Register("$screen_height", Screen.height);
				Register("$screen_dpi", Screen.dpi);
				Register("$app_version", MixpanelUnityPlatform.get_android_version_name());
				Register("$app_release", MixpanelUnityPlatform.get_android_version_code());
				people.Set("$android_app_version", MixpanelUnityPlatform.get_android_version_name());
				people.Set("$android_app_version_code", MixpanelUnityPlatform.get_android_version_code());
				if (batchSendInterval < 0)
				{
					Debug.LogError("batchSendInterval must be greater or equal zo zero");
					batchSendInterval = 0;
				}
				mp_interface.set_batch_send_interval((uint)batchSendInterval);
				if (reportUnityErrors)
				{
					Application.RegisterLogCallback(HandleLogMessage);
				}
			}
		}

		private void OnDestroy()
		{
			if (tracking_enabled)
			{
				mp_interface.Dispose();
			}
		}

		private void Update()
		{
			if (!tracking_enabled)
			{
				return;
			}
			mixpanel.detail.Mixpanel.LogEntry logEntry = new mixpanel.detail.Mixpanel.LogEntry();
			while (mp_interface.get_next_log_entry(logEntry))
			{
				string message = string.Format("Mixpanel[{0}]: {1}", logEntry.level, logEntry.message);
				switch (logEntry.level)
				{
				case mixpanel.detail.Mixpanel.LogEntry.Level.LL_ERROR:
					Debug.LogError(message);
					break;
				case mixpanel.detail.Mixpanel.LogEntry.Level.LL_WARNING:
					Debug.LogWarning(message);
					break;
				default:
					Debug.Log(message);
					break;
				}
			}
			if (reachability != Application.internetReachability)
			{
				reachability = Application.internetReachability;
				switch (reachability)
				{
				case NetworkReachability.NotReachable:
					mp_interface.on_reachability_changed(mixpanel.detail.Mixpanel.NetworkReachability.NotReachable);
					break;
				case NetworkReachability.ReachableViaCarrierDataNetwork:
					mp_interface.on_reachability_changed(mixpanel.detail.Mixpanel.NetworkReachability.ReachableViaCarrierDataNetwork);
					break;
				case NetworkReachability.ReachableViaLocalAreaNetwork:
					mp_interface.on_reachability_changed(mixpanel.detail.Mixpanel.NetworkReachability.ReachableViaLocalAreaNetwork);
					break;
				}
			}
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (tracking_enabled)
			{
				if (focusStatus)
				{
					mp_interface.on_entered_foreground();
				}
				else
				{
					mp_interface.on_entered_background();
				}
			}
		}

		private static void HandleLogMessage(string logString, string stackTrace, LogType type)
		{
			if (tracking_enabled && instance != null && error_count < max_errors && !logString.Contains("Mixpanel[LL_"))
			{
				switch (type)
				{
				case LogType.Error:
				case LogType.Assert:
				case LogType.Exception:
				{
					error_count++;
					Value value = new Value();
					value["severity"] = type.ToString();
					value["log"] = logString;
					value["stack"] = stackTrace;
					Track("error", value);
					break;
				}
				case LogType.Warning:
				case LogType.Log:
					break;
				}
			}
		}
	}
}
