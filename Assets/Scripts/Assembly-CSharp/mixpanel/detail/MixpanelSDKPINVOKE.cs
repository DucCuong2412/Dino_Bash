using System;
using System.IO;
using System.Runtime.InteropServices;
using AOT;

namespace mixpanel.detail
{
	internal class MixpanelSDKPINVOKE
	{
		protected class SWIGExceptionHelper
		{
			public delegate void ExceptionDelegate(string message);

			public delegate void ExceptionArgumentDelegate(string message, string paramName);

			private static ExceptionDelegate applicationDelegate;

			private static ExceptionDelegate arithmeticDelegate;

			private static ExceptionDelegate divideByZeroDelegate;

			private static ExceptionDelegate indexOutOfRangeDelegate;

			private static ExceptionDelegate invalidCastDelegate;

			private static ExceptionDelegate invalidOperationDelegate;

			private static ExceptionDelegate ioDelegate;

			private static ExceptionDelegate nullReferenceDelegate;

			private static ExceptionDelegate outOfMemoryDelegate;

			private static ExceptionDelegate overflowDelegate;

			private static ExceptionDelegate systemDelegate;

			private static ExceptionArgumentDelegate argumentDelegate;

			private static ExceptionArgumentDelegate argumentNullDelegate;

			private static ExceptionArgumentDelegate argumentOutOfRangeDelegate;

			static SWIGExceptionHelper()
			{
				applicationDelegate = SetPendingApplicationException;
				arithmeticDelegate = SetPendingArithmeticException;
				divideByZeroDelegate = SetPendingDivideByZeroException;
				indexOutOfRangeDelegate = SetPendingIndexOutOfRangeException;
				invalidCastDelegate = SetPendingInvalidCastException;
				invalidOperationDelegate = SetPendingInvalidOperationException;
				ioDelegate = SetPendingIOException;
				nullReferenceDelegate = SetPendingNullReferenceException;
				outOfMemoryDelegate = SetPendingOutOfMemoryException;
				overflowDelegate = SetPendingOverflowException;
				systemDelegate = SetPendingSystemException;
				argumentDelegate = SetPendingArgumentException;
				argumentNullDelegate = SetPendingArgumentNullException;
				argumentOutOfRangeDelegate = SetPendingArgumentOutOfRangeException;
				SWIGRegisterExceptionCallbacks_MixpanelSDK(applicationDelegate, arithmeticDelegate, divideByZeroDelegate, indexOutOfRangeDelegate, invalidCastDelegate, invalidOperationDelegate, ioDelegate, nullReferenceDelegate, outOfMemoryDelegate, overflowDelegate, systemDelegate);
				SWIGRegisterExceptionArgumentCallbacks_MixpanelSDK(argumentDelegate, argumentNullDelegate, argumentOutOfRangeDelegate);
			}

			[DllImport("MixpanelSDK")]
			public static extern void SWIGRegisterExceptionCallbacks_MixpanelSDK(ExceptionDelegate applicationDelegate, ExceptionDelegate arithmeticDelegate, ExceptionDelegate divideByZeroDelegate, ExceptionDelegate indexOutOfRangeDelegate, ExceptionDelegate invalidCastDelegate, ExceptionDelegate invalidOperationDelegate, ExceptionDelegate ioDelegate, ExceptionDelegate nullReferenceDelegate, ExceptionDelegate outOfMemoryDelegate, ExceptionDelegate overflowDelegate, ExceptionDelegate systemExceptionDelegate);

			[DllImport("MixpanelSDK")]
			public static extern void SWIGRegisterExceptionArgumentCallbacks_MixpanelSDK(ExceptionArgumentDelegate argumentDelegate, ExceptionArgumentDelegate argumentNullDelegate, ExceptionArgumentDelegate argumentOutOfRangeDelegate);

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingApplicationException(string message)
			{
				SWIGPendingException.Set(new ApplicationException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingArithmeticException(string message)
			{
				SWIGPendingException.Set(new ArithmeticException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingDivideByZeroException(string message)
			{
				SWIGPendingException.Set(new DivideByZeroException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingIndexOutOfRangeException(string message)
			{
				SWIGPendingException.Set(new IndexOutOfRangeException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingInvalidCastException(string message)
			{
				SWIGPendingException.Set(new InvalidCastException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingInvalidOperationException(string message)
			{
				SWIGPendingException.Set(new InvalidOperationException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingIOException(string message)
			{
				SWIGPendingException.Set(new IOException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingNullReferenceException(string message)
			{
				SWIGPendingException.Set(new NullReferenceException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingOutOfMemoryException(string message)
			{
				SWIGPendingException.Set(new OutOfMemoryException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingOverflowException(string message)
			{
				SWIGPendingException.Set(new OverflowException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingSystemException(string message)
			{
				SWIGPendingException.Set(new SystemException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionArgumentDelegate))]
			private static void SetPendingArgumentException(string message, string paramName)
			{
				SWIGPendingException.Set(new ArgumentException(message, paramName, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionArgumentDelegate))]
			private static void SetPendingArgumentNullException(string message, string paramName)
			{
				Exception ex = SWIGPendingException.Retrieve();
				if (ex != null)
				{
					message = message + " Inner Exception: " + ex.Message;
				}
				SWIGPendingException.Set(new ArgumentNullException(paramName, message));
			}

			[MonoPInvokeCallback(typeof(ExceptionArgumentDelegate))]
			private static void SetPendingArgumentOutOfRangeException(string message, string paramName)
			{
				Exception ex = SWIGPendingException.Retrieve();
				if (ex != null)
				{
					message = message + " Inner Exception: " + ex.Message;
				}
				SWIGPendingException.Set(new ArgumentOutOfRangeException(paramName, message));
			}
		}

		public class SWIGPendingException
		{
			[ThreadStatic]
			private static Exception pendingException;

			private static int numExceptionsPending;

			public static bool Pending
			{
				get
				{
					bool result = false;
					if (numExceptionsPending > 0 && pendingException != null)
					{
						result = true;
					}
					return result;
				}
			}

			public static void Set(Exception e)
			{
				if (pendingException != null)
				{
					throw new ApplicationException("FATAL: An earlier pending exception from unmanaged code was missed and thus not thrown (" + pendingException.ToString() + ")", e);
				}
				pendingException = e;
				lock (typeof(MixpanelSDKPINVOKE))
				{
					numExceptionsPending++;
				}
			}

			public static Exception Retrieve()
			{
				Exception result = null;
				if (numExceptionsPending > 0 && pendingException != null)
				{
					result = pendingException;
					pendingException = null;
					lock (typeof(MixpanelSDKPINVOKE))
					{
						numExceptionsPending--;
					}
				}
				return result;
			}
		}

		protected class SWIGStringHelper
		{
			public delegate string SWIGStringDelegate(string message);

			private static SWIGStringDelegate stringDelegate;

			static SWIGStringHelper()
			{
				stringDelegate = CreateString;
				SWIGRegisterStringCallback_MixpanelSDK(stringDelegate);
			}

			[DllImport("MixpanelSDK")]
			public static extern void SWIGRegisterStringCallback_MixpanelSDK(SWIGStringDelegate stringDelegate);

			[MonoPInvokeCallback(typeof(SWIGStringDelegate))]
			private static string CreateString(string cString)
			{
				return cString;
			}
		}

		protected static SWIGExceptionHelper swigExceptionHelper;

		protected static SWIGStringHelper swigStringHelper;

		static MixpanelSDKPINVOKE()
		{
			swigExceptionHelper = new SWIGExceptionHelper();
			swigStringHelper = new SWIGStringHelper();
		}

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_ExchangeRates__SWIG_0();

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_ExchangeRates__SWIG_1(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern uint CSharp_ExchangeRates_size(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_ExchangeRates_empty(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_ExchangeRates_Clear(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern double CSharp_ExchangeRates_getitem(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_ExchangeRates_setitem(HandleRef jarg1, string jarg2, double jarg3);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_ExchangeRates_ContainsKey(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_ExchangeRates_Add(HandleRef jarg1, string jarg2, double jarg3);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_ExchangeRates_Remove(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_ExchangeRates_create_iterator_begin(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern string CSharp_ExchangeRates_get_next_key(HandleRef jarg1, IntPtr jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_ExchangeRates_destroy_iterator(HandleRef jarg1, IntPtr jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_delete_ExchangeRates(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Value__SWIG_0();

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Value__SWIG_1(int jarg1);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Value__SWIG_2(double jarg1);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Value__SWIG_3(float jarg1);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Value__SWIG_4(string jarg1);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Value__SWIG_5(bool jarg1);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Value__SWIG_6(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_delete_Value(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_Value_get__SWIG_0(HandleRef jarg1, uint jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isValidIndex(HandleRef jarg1, uint jarg2);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_Value_append(HandleRef jarg1, HandleRef jarg2);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_Value_get__SWIG_1(HandleRef jarg1, string jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_Value_removeMember(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isMember(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern string CSharp_Value_toStyledString(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern string CSharp_Value_asString(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern int CSharp_Value_asInt(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern float CSharp_Value_asFloat(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern double CSharp_Value_asDouble(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_asBool(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isNull(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isBool(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isInt(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isIntegral(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isDouble(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isNumeric(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isString(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isArray(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_isObject(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern uint CSharp_Value_size(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Value_empty(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Value_clear(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Value_resize(HandleRef jarg1, uint jarg2);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_Value_at__SWIG_0(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_Value_at__SWIG_1(HandleRef jarg1, int jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Value_set__SWIG_0(HandleRef jarg1, int jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Value_set__SWIG_1(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Value_set__SWIG_2(HandleRef jarg1, double jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Value_set__SWIG_3(HandleRef jarg1, float jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Value_set__SWIG_4(HandleRef jarg1, HandleRef jarg2);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_0(string jarg1, bool jarg2, bool jarg3, bool jarg4, bool jarg5, bool jarg6);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_1(string jarg1, bool jarg2, bool jarg3, bool jarg4, bool jarg5);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_2(string jarg1, bool jarg2, bool jarg3, bool jarg4);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_3(string jarg1, bool jarg2, bool jarg3);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_4(string jarg1, bool jarg2);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_5(string jarg1);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_6(string jarg1, string jarg2, string jarg3, bool jarg4, bool jarg5, bool jarg6, bool jarg7, bool jarg8);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_7(string jarg1, string jarg2, string jarg3, bool jarg4, bool jarg5, bool jarg6, bool jarg7);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_8(string jarg1, string jarg2, string jarg3, bool jarg4, bool jarg5, bool jarg6);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_9(string jarg1, string jarg2, string jarg3, bool jarg4, bool jarg5);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_10(string jarg1, string jarg2, string jarg3, bool jarg4);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel__SWIG_11(string jarg1, string jarg2, string jarg3);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_delete_Mixpanel(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_identify(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_alias(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_register_(HandleRef jarg1, string jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Mixpanel_register_once(HandleRef jarg1, string jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Mixpanel_unregister(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_Mixpanel_get_super_properties(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_clear_super_properties(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_clear_send_queues(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Mixpanel_start_timed_event(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Mixpanel_start_timed_event_once(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Mixpanel_clear_timed_event(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_clear_timed_events(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_reset(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_track__SWIG_0(HandleRef jarg1, string jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_track__SWIG_1(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_update_exchange_rates(HandleRef jarg1, HandleRef jarg2);

		[DllImport("MixpanelSDK")]
		public static extern string CSharp_Mixpanel_get_currency_code();

		[DllImport("MixpanelSDK")]
		public static extern double CSharp_Mixpanel_get_exchange_rate(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_set(HandleRef jarg1, string jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_set_once(HandleRef jarg1, string jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_unset(HandleRef jarg1, HandleRef jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_unset_one(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_increment(HandleRef jarg1, string jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_append(HandleRef jarg1, string jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_union_(HandleRef jarg1, string jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_track_charge__SWIG_0(HandleRef jarg1, double jarg2, HandleRef jarg3);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_track_charge__SWIG_1(HandleRef jarg1, double jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_track_charge_converting__SWIG_0(HandleRef jarg1, double jarg2, string jarg3, HandleRef jarg4);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_track_charge_converting__SWIG_1(HandleRef jarg1, double jarg2, string jarg3);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_clear_charges(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_delete_user(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_set_push_id(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_set_first_name(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_set_last_name(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_set_name(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_set_email(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_People_set_phone(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_delete_Mixpanel_People(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_people_set(HandleRef jarg1, HandleRef jarg2);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_Mixpanel_people_get(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_LogEntry_level_set(HandleRef jarg1, int jarg2);

		[DllImport("MixpanelSDK")]
		public static extern int CSharp_Mixpanel_LogEntry_level_get(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_LogEntry_message_set(HandleRef jarg1, string jarg2);

		[DllImport("MixpanelSDK")]
		public static extern string CSharp_Mixpanel_LogEntry_message_get(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern IntPtr CSharp_new_Mixpanel_LogEntry();

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_delete_Mixpanel_LogEntry(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_set_minimum_log_level(HandleRef jarg1, int jarg2);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Mixpanel_get_next_log_entry(HandleRef jarg1, HandleRef jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_on_entered_foreground(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_on_entered_background(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_set_session_expiry_seconds(HandleRef jarg1, uint jarg2);

		[DllImport("MixpanelSDK")]
		public static extern string CSharp_Mixpanel_utc_now();

		[DllImport("MixpanelSDK")]
		public static extern string CSharp_Mixpanel_local_now();

		[DllImport("MixpanelSDK")]
		public static extern uint CSharp_Mixpanel_get_seconds_since_app_start(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern uint CSharp_Mixpanel_get_session_duration_in_seconds(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern uint CSharp_Mixpanel_get_days_since_install(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_on_reachability_changed(HandleRef jarg1, int jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_set_send_only_on_lan(HandleRef jarg1, bool jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_set_maximum_queue_size(HandleRef jarg1, uint jarg2);

		[DllImport("MixpanelSDK")]
		public static extern bool CSharp_Mixpanel_is_rooted();

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_set_batch_send_interval(HandleRef jarg1, uint jarg2);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_flush_queue(HandleRef jarg1);

		[DllImport("MixpanelSDK")]
		public static extern void CSharp_Mixpanel_director_connect(HandleRef jarg1);
	}
}
