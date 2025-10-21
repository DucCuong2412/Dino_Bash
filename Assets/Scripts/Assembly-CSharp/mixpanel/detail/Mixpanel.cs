using System;
using System.Runtime.InteropServices;

namespace mixpanel.detail
{
	public class Mixpanel : IDisposable
	{
		public class People : IDisposable
		{
			private HandleRef swigCPtr;

			protected bool swigCMemOwn;

			internal People(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = new HandleRef(this, cPtr);
			}

			internal static HandleRef getCPtr(People obj)
			{
				return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
			}

			~People()
			{
				Dispose();
			}

			public virtual void Dispose()
			{
				lock (this)
				{
					if (swigCPtr.Handle != IntPtr.Zero)
					{
						if (swigCMemOwn)
						{
							swigCMemOwn = false;
							MixpanelSDKPINVOKE.CSharp_delete_Mixpanel_People(swigCPtr);
						}
						swigCPtr = new HandleRef(null, IntPtr.Zero);
					}
					GC.SuppressFinalize(this);
				}
			}

			public void set(string property, Value to)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_set(swigCPtr, property, Value.getCPtr(to));
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void set_once(string property, Value to)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_set_once(swigCPtr, property, Value.getCPtr(to));
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void unset(Value properties)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_unset(swigCPtr, Value.getCPtr(properties));
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void unset_one(string property)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_unset_one(swigCPtr, property);
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void increment(string property, Value by)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_increment(swigCPtr, property, Value.getCPtr(by));
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void append(string list_name, Value value)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_append(swigCPtr, list_name, Value.getCPtr(value));
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void union_(string list_name, Value values)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_union_(swigCPtr, list_name, Value.getCPtr(values));
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void track_charge(double amount, Value properties)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_track_charge__SWIG_0(swigCPtr, amount, Value.getCPtr(properties));
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void track_charge(double amount)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_track_charge__SWIG_1(swigCPtr, amount);
			}

			public void track_charge_converting(double amount, string currency_code, Value properties)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_track_charge_converting__SWIG_0(swigCPtr, amount, currency_code, Value.getCPtr(properties));
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void track_charge_converting(double amount, string currency_code)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_track_charge_converting__SWIG_1(swigCPtr, amount, currency_code);
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void clear_charges()
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_clear_charges(swigCPtr);
			}

			public void delete_user()
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_delete_user(swigCPtr);
			}

			public void set_push_id(string token)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_set_push_id(swigCPtr, token);
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void set_first_name(string to)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_set_first_name(swigCPtr, to);
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void set_last_name(string to)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_set_last_name(swigCPtr, to);
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void set_name(string to)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_set_name(swigCPtr, to);
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void set_email(string to)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_set_email(swigCPtr, to);
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}

			public void set_phone(string to)
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_People_set_phone(swigCPtr, to);
				if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
				{
					throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public class LogEntry : IDisposable
		{
			public enum Level
			{
				LL_TRACE = 0,
				LL_DEBUG = 1,
				LL_INFO = 2,
				LL_WARNING = 3,
				LL_ERROR = 4,
				LL_NONE = 5
			}

			private HandleRef swigCPtr;

			protected bool swigCMemOwn;

			public Level level
			{
				get
				{
					return (Level)MixpanelSDKPINVOKE.CSharp_Mixpanel_LogEntry_level_get(swigCPtr);
				}
				set
				{
					MixpanelSDKPINVOKE.CSharp_Mixpanel_LogEntry_level_set(swigCPtr, (int)value);
				}
			}

			public string message
			{
				get
				{
					//string result = MixpanelSDKPINVOKE.CSharp_Mixpanel_LogEntry_message_get(swigCPtr);
					//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
					//{
					//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
					//}
					return string.Empty;
				}
				set
				{
					//MixpanelSDKPINVOKE.CSharp_Mixpanel_LogEntry_message_set(swigCPtr, value);
					//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
					//{
					//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
					//}
				}
			}

			internal LogEntry(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = new HandleRef(this, cPtr);
			}

			public LogEntry()
				//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel_LogEntry(), true)
			{
			}

			internal static HandleRef getCPtr(LogEntry obj)
			{
				return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
			}

			~LogEntry()
			{
				Dispose();
			}

			public virtual void Dispose()
			{
				lock (this)
				{
					if (swigCPtr.Handle != IntPtr.Zero)
					{
						if (swigCMemOwn)
						{
							swigCMemOwn = false;
							MixpanelSDKPINVOKE.CSharp_delete_Mixpanel_LogEntry(swigCPtr);
						}
						swigCPtr = new HandleRef(null, IntPtr.Zero);
					}
					GC.SuppressFinalize(this);
				}
			}
		}

		public enum NetworkReachability
		{
			NotReachable = 0,
			ReachableViaCarrierDataNetwork = 1,
			ReachableViaLocalAreaNetwork = 2
		}

		private HandleRef swigCPtr;

		protected bool swigCMemOwn;

		public People people
		{
			get
			{
				//IntPtr intPtr = MixpanelSDKPINVOKE.CSharp_Mixpanel_people_get(swigCPtr);
				return null;// (!(intPtr == IntPtr.Zero)) ? new People(intPtr, false) : null;
			}
			set
			{
				MixpanelSDKPINVOKE.CSharp_Mixpanel_people_set(swigCPtr, People.getCPtr(value));
			}
		}

		internal Mixpanel(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = new HandleRef(this, cPtr);
		}

		public Mixpanel(string token, bool enable_install_tracking, bool enable_session_tracking, bool enable_reachability_tracking, bool enable_local_time_tracking, bool enable_log_queue)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_0(token, enable_install_tracking, enable_session_tracking, enable_reachability_tracking, enable_local_time_tracking, enable_log_queue), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, bool enable_install_tracking, bool enable_session_tracking, bool enable_reachability_tracking, bool enable_local_time_tracking)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_1(token, enable_install_tracking, enable_session_tracking, enable_reachability_tracking, enable_local_time_tracking), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, bool enable_install_tracking, bool enable_session_tracking, bool enable_reachability_tracking)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_2(token, enable_install_tracking, enable_session_tracking, enable_reachability_tracking), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, bool enable_install_tracking, bool enable_session_tracking)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_3(token, enable_install_tracking, enable_session_tracking), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, bool enable_install_tracking)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_4(token, enable_install_tracking), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_5(token), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, string distinct_id, string storage_directory, bool enable_install_tracking, bool enable_session_tracking, bool enable_reachability_tracking, bool enable_local_time_tracking, bool enable_log_queue)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_6(token, distinct_id, storage_directory, enable_install_tracking, enable_session_tracking, enable_reachability_tracking, enable_local_time_tracking, enable_log_queue), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, string distinct_id, string storage_directory, bool enable_install_tracking, bool enable_session_tracking, bool enable_reachability_tracking, bool enable_local_time_tracking)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_7(token, distinct_id, storage_directory, enable_install_tracking, enable_session_tracking, enable_reachability_tracking, enable_local_time_tracking), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, string distinct_id, string storage_directory, bool enable_install_tracking, bool enable_session_tracking, bool enable_reachability_tracking)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_8(token, distinct_id, storage_directory, enable_install_tracking, enable_session_tracking, enable_reachability_tracking), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, string distinct_id, string storage_directory, bool enable_install_tracking, bool enable_session_tracking)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_9(token, distinct_id, storage_directory, enable_install_tracking, enable_session_tracking), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, string distinct_id, string storage_directory, bool enable_install_tracking)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_10(token, distinct_id, storage_directory, enable_install_tracking), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Mixpanel(string token, string distinct_id, string storage_directory)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Mixpanel__SWIG_11(token, distinct_id, storage_directory), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		internal static HandleRef getCPtr(Mixpanel obj)
		{
			return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
		}

		~Mixpanel()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			lock (this)
			{
				if (swigCPtr.Handle != IntPtr.Zero)
				{
					if (swigCMemOwn)
					{
						swigCMemOwn = false;
						MixpanelSDKPINVOKE.CSharp_delete_Mixpanel(swigCPtr);
					}
					swigCPtr = new HandleRef(null, IntPtr.Zero);
				}
				GC.SuppressFinalize(this);
			}
		}

		public void identify(string unique_id)
		{
			MixpanelSDKPINVOKE.CSharp_Mixpanel_identify(swigCPtr, unique_id);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
		}

		public void alias(string alias)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_alias(swigCPtr, alias);
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
		}

		public void register_(string key, Value value)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_register_(swigCPtr, key, Value.getCPtr(value));
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
		}

		public bool register_once(string key, Value value)
		{
			bool result = MixpanelSDKPINVOKE.CSharp_Mixpanel_register_once(swigCPtr, key, Value.getCPtr(value));
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public bool unregister(string key)
		{
			bool result = MixpanelSDKPINVOKE.CSharp_Mixpanel_unregister(swigCPtr, key);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public Value get_super_properties()
		{
			return new Value(MixpanelSDKPINVOKE.CSharp_Mixpanel_get_super_properties(swigCPtr), true);
		}

		public void clear_super_properties()
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_clear_super_properties(swigCPtr);
		}

		public void clear_send_queues()
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_clear_send_queues(swigCPtr);
		}

		public bool start_timed_event(string event_name)
		{
			bool result = MixpanelSDKPINVOKE.CSharp_Mixpanel_start_timed_event(swigCPtr, event_name);
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
			return result;
		}

		public bool start_timed_event_once(string event_name)
		{
			bool result = MixpanelSDKPINVOKE.CSharp_Mixpanel_start_timed_event_once(swigCPtr, event_name);
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
			return result;
		}

		public bool clear_timed_event(string event_name)
		{
			bool result = MixpanelSDKPINVOKE.CSharp_Mixpanel_clear_timed_event(swigCPtr, event_name);
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
			return result;
		}

		public void clear_timed_events()
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_clear_timed_events(swigCPtr);
		}

		public void reset()
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_reset(swigCPtr);
		}

		public void track(string arg0, Value properties)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_track__SWIG_0(swigCPtr, arg0, Value.getCPtr(properties));
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
		}

		public void track(string arg0)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_track__SWIG_1(swigCPtr, arg0);
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
		}

		public void update_exchange_rates(ExchangeRates currency_to_usd)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_update_exchange_rates(swigCPtr, ExchangeRates.getCPtr(currency_to_usd));
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
		}

		public static string get_currency_code()
		{
			return MixpanelSDKPINVOKE.CSharp_Mixpanel_get_currency_code();
		}

		public double get_exchange_rate(string currency_code)
		{
			double result = MixpanelSDKPINVOKE.CSharp_Mixpanel_get_exchange_rate(swigCPtr, currency_code);
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
			return result;
		}

		public void set_minimum_log_level(LogEntry.Level level)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_set_minimum_log_level(swigCPtr, (int)level);
		}

		public bool get_next_log_entry(LogEntry entry)
		{
			//bool result = MixpanelSDKPINVOKE.CSharp_Mixpanel_get_next_log_entry(swigCPtr, LogEntry.getCPtr(entry));
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	//throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
			return false;
		}

		public void on_entered_foreground()
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_on_entered_foreground(swigCPtr);
		}

		public void on_entered_background()
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_on_entered_background(swigCPtr);
		}

		public void set_session_expiry_seconds(uint seconds)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_set_session_expiry_seconds(swigCPtr, seconds);
		}

		public static string utc_now()
		{
			return string.Empty;// MixpanelSDKPINVOKE.CSharp_Mixpanel_utc_now();
        }

		public static string local_now()
		{
			return string.Empty;// MixpanelSDKPINVOKE.CSharp_Mixpanel_local_now();
        }

		public uint get_seconds_since_app_start()
		{
			return 0;// MixpanelSDKPINVOKE.CSharp_Mixpanel_get_seconds_since_app_start(swigCPtr);
		}

		public uint get_session_duration_in_seconds()
		{
			return 0;//MixpanelSDKPINVOKE.CSharp_Mixpanel_get_session_duration_in_seconds(swigCPtr);
        }

		public uint get_days_since_install()
		{
			return 0;// MixpanelSDKPINVOKE.CSharp_Mixpanel_get_days_since_install(swigCPtr);
        }

		public void on_reachability_changed(NetworkReachability network_reachability)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_on_reachability_changed(swigCPtr, (int)network_reachability);
		}

		public void set_send_only_on_lan(bool send_only_on_lan)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_set_send_only_on_lan(swigCPtr, send_only_on_lan);
		}

		public void set_maximum_queue_size(uint maximum_size)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_set_maximum_queue_size(swigCPtr, maximum_size);
		}

		public static bool is_rooted()
		{
			return MixpanelSDKPINVOKE.CSharp_Mixpanel_is_rooted();
		}

		public void set_batch_send_interval(uint seconds)
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_set_batch_send_interval(swigCPtr, seconds);
		}

		public void flush_queue()
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_flush_queue(swigCPtr);
		}

		private void SwigDirectorConnect()
		{
			//MixpanelSDKPINVOKE.CSharp_Mixpanel_director_connect(swigCPtr);
		}
	}
}
