using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LeanplumSDK
{
	public abstract class Var
	{
		public delegate void VariableCallback();

		private static readonly Regex nameComponentPattern = new Regex("(?:[^\\.\\[.(\\\\]+|\\\\.)+");

		public string Kind { get; protected set; }

		public string Name { get; protected set; }

		public string FileName { get; protected set; }

		public virtual string[] NameComponents
		{
			get
			{
				MatchCollection matchCollection = nameComponentPattern.Matches(Name);
				string[] array = new string[matchCollection.Count];
				int num = 0;
				foreach (object item in matchCollection)
				{
					array[num] = item.ToString();
					num++;
				}
				return array;
			}
		}

		public virtual event VariableCallback ValueChanged;

		public static Var<int> Define(string name, int defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<long> Define(string name, long defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<short> Define(string name, short defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<byte> Define(string name, byte defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<bool> Define(string name, bool defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<float> Define(string name, float defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<double> Define(string name, double defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<string> Define(string name, string defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<List<object>> Define(string name, List<object> defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<List<string>> Define(string name, List<string> defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<Dictionary<string, object>> Define(string name, Dictionary<string, object> defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<Dictionary<string, string>> Define(string name, Dictionary<string, string> defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}

		public static Var<AssetBundle> DefineAssetBundle(string name, bool realtimeUpdating = true, string iosBundleName = "", string androidBundleName = "", string standaloneBundleName = "")
		{
			return LeanplumFactory.SDK.DefineAssetBundle(name, realtimeUpdating, iosBundleName, androidBundleName, standaloneBundleName);
		}

		public abstract void OnValueChanged();

		public abstract object GetDefaultValue();

		public virtual void Update()
		{
		}
	}
	public abstract class Var<T> : Var
	{
		public abstract T Value { get; }

		public static implicit operator T(Var<T> var)
		{
			return var.Value;
		}
	}
}
