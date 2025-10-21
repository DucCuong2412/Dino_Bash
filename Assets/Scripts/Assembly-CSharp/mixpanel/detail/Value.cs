using System;
using System.Runtime.InteropServices;

namespace mixpanel.detail
{
	public class Value : IDisposable
	{
		private HandleRef swigCPtr;

		protected bool swigCMemOwn;

		public Value this[int idx]
		{
			get
			{
				return at(idx);
			}
			set
			{
				at(idx).set(value);
			}
		}

		public Value this[string idx]
		{
			get
			{
				return at(idx);
			}
			set
			{
				at(idx).set(value);
			}
		}

		internal Value(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = new HandleRef(this, cPtr);
		}

		public Value()
			//: this(MixpanelSDKPINVOKE.CSharp_new_Value__SWIG_0(), true)
		{
		}

		public Value(int value)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Value__SWIG_1(value), true)
		{
		}

		public Value(double value)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Value__SWIG_2(value), true)
		{
		}

		public Value(float value)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Value__SWIG_3(value), true)
		{
		}

		public Value(string value)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Value__SWIG_4(value), true)
		{
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
		}

		public Value(bool value)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Value__SWIG_5(value), true)
		{
		}

		public Value(Value other)
			//: this(MixpanelSDKPINVOKE.CSharp_new_Value__SWIG_6(getCPtr(other)), true)
		{
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
		}

		internal static HandleRef getCPtr(Value obj)
		{
			return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
		}

		~Value()
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
						MixpanelSDKPINVOKE.CSharp_delete_Value(swigCPtr);
					}
					swigCPtr = new HandleRef(null, IntPtr.Zero);
				}
				GC.SuppressFinalize(this);
			}
		}

		public Value get(uint index, Value defaultValue)
		{
			Value result = new Value(MixpanelSDKPINVOKE.CSharp_Value_get__SWIG_0(swigCPtr, index, getCPtr(defaultValue)), true);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public bool isValidIndex(uint index)
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isValidIndex(swigCPtr, index);
		}

		public Value append(Value value)
		{
			Value result = new Value(MixpanelSDKPINVOKE.CSharp_Value_append(swigCPtr, getCPtr(value)), false);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public Value get(string key, Value defaultValue)
		{
			Value result = new Value(MixpanelSDKPINVOKE.CSharp_Value_get__SWIG_1(swigCPtr, key, getCPtr(defaultValue)), true);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public Value removeMember(string key)
		{
			Value result = new Value(MixpanelSDKPINVOKE.CSharp_Value_removeMember(swigCPtr, key), true);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public bool isMember(string key)
		{
			bool result = MixpanelSDKPINVOKE.CSharp_Value_isMember(swigCPtr, key);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public string toStyledString()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_toStyledString(swigCPtr);
		}

		public string asString()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_asString(swigCPtr);
		}

		public int asInt()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_asInt(swigCPtr);
		}

		public float asFloat()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_asFloat(swigCPtr);
		}

		public double asDouble()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_asDouble(swigCPtr);
		}

		public bool asBool()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_asBool(swigCPtr);
		}

		public bool isNull()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isNull(swigCPtr);
		}

		public bool isBool()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isBool(swigCPtr);
		}

		public bool isInt()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isInt(swigCPtr);
		}

		public bool isIntegral()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isIntegral(swigCPtr);
		}

		public bool isDouble()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isDouble(swigCPtr);
		}

		public bool isNumeric()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isNumeric(swigCPtr);
		}

		public bool isString()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isString(swigCPtr);
		}

		public bool isArray()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isArray(swigCPtr);
		}

		public bool isObject()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_isObject(swigCPtr);
		}

		public uint size()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_size(swigCPtr);
		}

		public bool empty()
		{
			return MixpanelSDKPINVOKE.CSharp_Value_empty(swigCPtr);
		}

		public void clear()
		{
			MixpanelSDKPINVOKE.CSharp_Value_clear(swigCPtr);
		}

		public void resize(uint size)
		{
			MixpanelSDKPINVOKE.CSharp_Value_resize(swigCPtr, size);
		}

		public Value at(string key)
		{
			//Value result = new Value(MixpanelSDKPINVOKE.CSharp_Value_at__SWIG_0(swigCPtr, key), false);
			//if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			//{
			//	throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			//}
			return 0;
		}

		public Value at(int index)
		{
			return 0;// new Value(MixpanelSDKPINVOKE.CSharp_Value_at__SWIG_1(swigCPtr, index), false);
		}

		public void set(int x)
		{
			MixpanelSDKPINVOKE.CSharp_Value_set__SWIG_0(swigCPtr, x);
		}

		public void set(string x)
		{
			MixpanelSDKPINVOKE.CSharp_Value_set__SWIG_1(swigCPtr, x);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
		}

		public void set(double x)
		{
			//MixpanelSDKPINVOKE.CSharp_Value_set__SWIG_2(swigCPtr, x);
		}

		public void set(float x)
		{
			//MixpanelSDKPINVOKE.CSharp_Value_set__SWIG_3(swigCPtr, x);
		}

		public void set(Value x)
		{
			//MixpanelSDKPINVOKE.CSharp_Value_set__SWIG_4(swigCPtr, getCPtr(x));
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
		}

		public static implicit operator string(Value v)
		{
			return v.asString();
		}

		public static implicit operator Value(string v)
		{
			return new Value(v);
		}

		public static implicit operator int(Value v)
		{
			return v.asInt();
		}

		public static implicit operator Value(int v)
		{
			return new Value(v);
		}

		public static implicit operator double(Value v)
		{
			return v.asDouble();
		}

		public static implicit operator Value(double v)
		{
			return new Value(v);
		}

		public static implicit operator float(Value v)
		{
			return v.asFloat();
		}

		public static implicit operator Value(float v)
		{
			return new Value(v);
		}

		public static implicit operator bool(Value v)
		{
			return v.asBool();
		}

		public static implicit operator Value(bool v)
		{
			return new Value(v);
		}
	}
}
