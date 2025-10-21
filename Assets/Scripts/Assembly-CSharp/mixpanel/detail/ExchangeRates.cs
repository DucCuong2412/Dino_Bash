using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace mixpanel.detail
{
	public class ExchangeRates : IDisposable, IEnumerable, IDictionary<string, double>, ICollection<KeyValuePair<string, double>>, IEnumerable<KeyValuePair<string, double>>
	{
		public sealed class ExchangeRatesEnumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<string, double>>
		{
			private ExchangeRates collectionRef;

			private IList<string> keyCollection;

			private int currentIndex;

			private object currentObject;

			private int currentSize;

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public KeyValuePair<string, double> Current
			{
				get
				{
					if (currentIndex == -1)
					{
						throw new InvalidOperationException("Enumeration not started.");
					}
					if (currentIndex > currentSize - 1)
					{
						throw new InvalidOperationException("Enumeration finished.");
					}
					if (currentObject == null)
					{
						throw new InvalidOperationException("Collection modified.");
					}
					return (KeyValuePair<string, double>)currentObject;
				}
			}

			public ExchangeRatesEnumerator(ExchangeRates collection)
			{
				collectionRef = collection;
				keyCollection = new List<string>(collection.Keys);
				currentIndex = -1;
				currentObject = null;
				currentSize = collectionRef.Count;
			}

			public bool MoveNext()
			{
				int count = collectionRef.Count;
				bool flag = currentIndex + 1 < count && count == currentSize;
				if (flag)
				{
					currentIndex++;
					string key = keyCollection[currentIndex];
					currentObject = new KeyValuePair<string, double>(key, collectionRef[key]);
				}
				else
				{
					currentObject = null;
				}
				return flag;
			}

			public void Reset()
			{
				currentIndex = -1;
				currentObject = null;
				if (collectionRef.Count != currentSize)
				{
					throw new InvalidOperationException("Collection modified.");
				}
			}

			public void Dispose()
			{
				currentIndex = -1;
				currentObject = null;
			}
		}

		private HandleRef swigCPtr;

		protected bool swigCMemOwn;

		public double this[string key]
		{
			get
			{
				return getitem(key);
			}
			set
			{
				setitem(key, value);
			}
		}

		public int Count
		{
			get
			{
				return (int)size();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				ICollection<string> collection = new List<string>();
				int count = Count;
				if (count > 0)
				{
					IntPtr swigiterator = create_iterator_begin();
					for (int i = 0; i < count; i++)
					{
						collection.Add(get_next_key(swigiterator));
					}
					destroy_iterator(swigiterator);
				}
				return collection;
			}
		}

		public ICollection<double> Values
		{
			get
			{
				ICollection<double> collection = new List<double>();
				using (ExchangeRatesEnumerator exchangeRatesEnumerator = GetEnumerator())
				{
					while (exchangeRatesEnumerator.MoveNext())
					{
						collection.Add(exchangeRatesEnumerator.Current.Value);
					}
					return collection;
				}
			}
		}

		internal ExchangeRates(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = new HandleRef(this, cPtr);
		}

		public ExchangeRates()
			: this(MixpanelSDKPINVOKE.CSharp_new_ExchangeRates__SWIG_0(), true)
		{
		}

		public ExchangeRates(ExchangeRates other)
			: this(MixpanelSDKPINVOKE.CSharp_new_ExchangeRates__SWIG_1(getCPtr(other)), true)
		{
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
		}

		IEnumerator<KeyValuePair<string, double>> IEnumerable<KeyValuePair<string, double>>.GetEnumerator()
		{
			return new ExchangeRatesEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ExchangeRatesEnumerator(this);
		}

		internal static HandleRef getCPtr(ExchangeRates obj)
		{
			return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
		}

		~ExchangeRates()
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
						MixpanelSDKPINVOKE.CSharp_delete_ExchangeRates(swigCPtr);
					}
					swigCPtr = new HandleRef(null, IntPtr.Zero);
				}
				GC.SuppressFinalize(this);
			}
		}

		public bool TryGetValue(string key, out double value)
		{
			if (ContainsKey(key))
			{
				value = this[key];
				return true;
			}
			value = 0.0;
			return false;
		}

		public void Add(KeyValuePair<string, double> item)
		{
			Add(item.Key, item.Value);
		}

		public bool Remove(KeyValuePair<string, double> item)
		{
			if (Contains(item))
			{
				return Remove(item.Key);
			}
			return false;
		}

		public bool Contains(KeyValuePair<string, double> item)
		{
			if (this[item.Key] == item.Value)
			{
				return true;
			}
			return false;
		}

		public void CopyTo(KeyValuePair<string, double>[] array)
		{
			CopyTo(array, 0);
		}

		public void CopyTo(KeyValuePair<string, double>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "Value is less than zero");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("Multi dimensional array.", "array");
			}
			if (arrayIndex + Count > array.Length)
			{
				throw new ArgumentException("Number of elements to copy is too large.");
			}
			IList<string> list = new List<string>(Keys);
			for (int i = 0; i < list.Count; i++)
			{
				string key = list[i];
				array.SetValue(new KeyValuePair<string, double>(key, this[key]), arrayIndex + i);
			}
		}

		public ExchangeRatesEnumerator GetEnumerator()
		{
			return new ExchangeRatesEnumerator(this);
		}

		private uint size()
		{
			return MixpanelSDKPINVOKE.CSharp_ExchangeRates_size(swigCPtr);
		}

		public bool empty()
		{
			return MixpanelSDKPINVOKE.CSharp_ExchangeRates_empty(swigCPtr);
		}

		public void Clear()
		{
			MixpanelSDKPINVOKE.CSharp_ExchangeRates_Clear(swigCPtr);
		}

		private double getitem(string key)
		{
			double result = MixpanelSDKPINVOKE.CSharp_ExchangeRates_getitem(swigCPtr, key);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private void setitem(string key, double x)
		{
			MixpanelSDKPINVOKE.CSharp_ExchangeRates_setitem(swigCPtr, key, x);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
		}

		public bool ContainsKey(string key)
		{
			bool result = MixpanelSDKPINVOKE.CSharp_ExchangeRates_ContainsKey(swigCPtr, key);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void Add(string key, double val)
		{
			MixpanelSDKPINVOKE.CSharp_ExchangeRates_Add(swigCPtr, key, val);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
		}

		public bool Remove(string key)
		{
			bool result = MixpanelSDKPINVOKE.CSharp_ExchangeRates_Remove(swigCPtr, key);
			if (MixpanelSDKPINVOKE.SWIGPendingException.Pending)
			{
				throw MixpanelSDKPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private IntPtr create_iterator_begin()
		{
			return MixpanelSDKPINVOKE.CSharp_ExchangeRates_create_iterator_begin(swigCPtr);
		}

		private string get_next_key(IntPtr swigiterator)
		{
			return MixpanelSDKPINVOKE.CSharp_ExchangeRates_get_next_key(swigCPtr, swigiterator);
		}

		private void destroy_iterator(IntPtr swigiterator)
		{
			MixpanelSDKPINVOKE.CSharp_ExchangeRates_destroy_iterator(swigCPtr, swigiterator);
		}
	}
}
