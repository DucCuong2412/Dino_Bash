using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
	internal class NativeVar<T> : Var<T>
	{
		private static readonly Regex nameComponentPattern = new Regex("(?:[^\\.\\[.(\\\\]+|\\\\.)+");

		private VariableCallback valueChanged;

		public object defaultClonedContainer;

		internal T _defaultValue;

		private T _value;

		internal bool fileReady;

		internal bool realtimeAssetUpdating;

		internal string currentlyDownloadingFile;

		public override string[] NameComponents
		{
			get
			{
				MatchCollection matchCollection = nameComponentPattern.Matches(base.Name);
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

		public override T Value
		{
			get
			{
				return _value;
			}
		}

		public override event VariableCallback ValueChanged
		{
			add
			{
				valueChanged = (VariableCallback)Delegate.Combine(valueChanged, value);
				if (Leanplum.HasStarted && fileReady)
				{
					value();
				}
			}
			remove
			{
				valueChanged = (VariableCallback)Delegate.Remove(valueChanged, value);
			}
		}

		internal NativeVar(string name, string kind, T defaultValue, string filename = "")
		{
			base.Name = name;
			base.Kind = kind;
			base.FileName = filename;
			_value = (_defaultValue = defaultValue);
		}

		public void SetFilename(string fileName)
		{
			base.FileName = fileName;
		}

		public override void OnValueChanged()
		{
			VariableCallback variableCallback = valueChanged;
			if (variableCallback != null)
			{
				variableCallback();
			}
		}

		internal virtual void ClearValueChangedCallbacks()
		{
			valueChanged = null;
		}

		public override void Update()
		{
			//Discarded unreachable code: IL_026c
			object obj2 = VarCache.GetMergedValueFromComponentArray(NameComponents);
			if (obj2 == null)
			{
				obj2 = GetDefaultValue();
			}
			if (base.Kind == "file")
			{
				if (VarCache.IsSilent)
				{
					return;
				}
				string newFile = obj2.ToString();
				string text = null;
				if (string.IsNullOrEmpty(newFile))
				{
					return;
				}
				if (VarCache.FileAttributes != null && VarCache.FileAttributes.ContainsKey(newFile))
				{
					IDictionary<string, object> dictionary = (VarCache.FileAttributes[newFile] as IDictionary<string, object>)[string.Empty] as IDictionary<string, object>;
					if (dictionary.ContainsKey("url"))
					{
						text = ((VarCache.FileAttributes[newFile] as IDictionary<string, object>)[string.Empty] as IDictionary<string, object>)["url"] as string;
					}
				}
				if (!(currentlyDownloadingFile != newFile) || string.IsNullOrEmpty(text) || ((!(newFile != base.FileName) || !realtimeAssetUpdating || !fileReady) && (Value != null || !realtimeAssetUpdating)))
				{
					return;
				}
				VarCache.downloadsPending++;
				currentlyDownloadingFile = newFile;
				base.FileName = newFile;
				fileReady = false;
				LeanplumRequest leanplumRequest = LeanplumRequest.Get(text.Substring(1));
				leanplumRequest.Response += delegate(object obj)
				{
					_value = (T)obj;
					if (newFile == base.FileName && !fileReady)
					{
						fileReady = true;
						OnValueChanged();
						currentlyDownloadingFile = null;
					}
					VarCache.downloadsPending--;
				};
				leanplumRequest.Error += delegate(Exception obj)
				{
					if (newFile == base.FileName && !fileReady)
					{
						LeanplumNative.CompatibilityLayer.LogError("Error downloading assetbundle \"" + base.FileName + "\". " + obj.ToString());
						currentlyDownloadingFile = null;
					}
					VarCache.downloadsPending--;
				};
				leanplumRequest.DownloadAssetNow();
			}
			else
			{
				if (!(Json.Serialize(obj2) != Json.Serialize(Value)))
				{
					return;
				}
				try
				{
					if (obj2 is IDictionary || obj2 is IList)
					{
						SharedUtil.FillInValues(obj2, Value);
					}
					else
					{
						_value = (T)Convert.ChangeType(obj2, typeof(T));
					}
					if (!VarCache.IsSilent)
					{
						OnValueChanged();
					}
				}
				catch (Exception ex)
				{
					Util.MaybeThrow(new LeanplumException("Error parsing values from server. " + ex.ToString()));
				}
			}
		}

		public override object GetDefaultValue()
		{
			if (_defaultValue is IDictionary || _defaultValue is IList)
			{
				return defaultClonedContainer;
			}
			if (base.FileName != null && base.FileName != string.Empty)
			{
				return base.FileName;
			}
			return _defaultValue;
		}

		public static implicit operator T(NativeVar<T> var)
		{
			return var.Value;
		}
	}
}
