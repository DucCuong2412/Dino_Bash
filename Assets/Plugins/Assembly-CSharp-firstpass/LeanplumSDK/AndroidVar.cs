using System;
using System.Collections;
using System.Linq;
using LeanplumSDK.MiniJSON;
using UnityEngine;

namespace LeanplumSDK
{
	internal class AndroidVar<T> : Var<T>
	{
		private bool registeredCallbackInAndroid;

		protected bool valueHasChanged;

		private VariableCallback valueChanged;

		private T currentValue;

		private T defaultValue;

		public override string[] NameComponents
		{
			get
			{
				string text = LeanplumAndroid.NativeSDK.CallStatic<string>("varNameComponents", new object[1] { base.Name });
				string[] array = new string[text.Count((char x) => x == ',') + 1];
				SharedUtil.FillInValues(Json.Deserialize(text), array);
				return array;
			}
		}

		public override T Value
		{
			get
			{
				if (base.Kind == "file")
				{
					string text = Json.Deserialize(LeanplumAndroid.NativeSDK.CallStatic<string>("fileValue", new object[1] { base.Name })) as string;
					if (text != base.FileName)
					{
						base.FileName = text;
					}
					return (T)Convert.ChangeType(AssetBundle.CreateFromFile(base.FileName), typeof(T));
				}
				string text2 = LeanplumAndroid.NativeSDK.CallStatic<string>("varValue", new object[1] { base.Name });
				if (text2 == Json.Serialize(currentValue))
				{
					return currentValue;
				}
				object obj = Json.Deserialize(text2);
				if (obj is IDictionary || obj is IList)
				{
					SharedUtil.FillInValues(obj, currentValue);
				}
				else if (obj == null)
				{
					currentValue = defaultValue;
				}
				else
				{
					currentValue = (T)Convert.ChangeType(obj, typeof(T));
				}
				return currentValue;
			}
		}

		public override event VariableCallback ValueChanged
		{
			add
			{
				valueChanged = (VariableCallback)Delegate.Combine(valueChanged, value);
				if (!registeredCallbackInAndroid)
				{
					registeredCallbackInAndroid = true;
					LeanplumAndroid.NativeSDK.CallStatic("registerVarCallback", base.Name);
				}
				if (valueHasChanged)
				{
					value();
				}
			}
			remove
			{
				valueChanged = (VariableCallback)Delegate.Remove(valueChanged, value);
			}
		}

		internal AndroidVar(string name, string kind, T DefaultValue, string filename = "")
		{
			base.Name = name;
			base.Kind = kind;
			base.FileName = filename;
			currentValue = DefaultValue;
			defaultValue = DefaultValue;
		}

		public override void OnValueChanged()
		{
			if (valueChanged != null)
			{
				valueChanged();
			}
			valueHasChanged = true;
		}

		public override object GetDefaultValue()
		{
			return defaultValue;
		}
	}
}
