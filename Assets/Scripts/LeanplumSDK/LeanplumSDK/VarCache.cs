using System;
using System.Collections;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
	internal static class VarCache
	{
		public delegate void updateEventHandler();

		private static readonly IDictionary<string, Var> vars = new Dictionary<string, Var>();

		private static readonly IDictionary<string, object> valuesFromClient = new Dictionary<string, object>();

		private static readonly IDictionary<string, string> defaultKinds = new Dictionary<string, string>();

		private static IDictionary<string, object> diffs = new Dictionary<string, object>();

		private static IDictionary<string, object> devModeValuesFromServer;

		private static IDictionary<string, object> fileAttributes = new Dictionary<string, object>();

		private static object merged;

		public static int downloadsPending;

		public static bool HasReceivedDiffs { get; private set; }

		public static bool IsSilent { get; set; }

		public static bool VarsNeedUpdate { get; set; }

		public static IDictionary<string, object> FileAttributes
		{
			get
			{
				return fileAttributes;
			}
			private set
			{
				fileAttributes = value;
			}
		}

		public static IDictionary<string, object> Diffs
		{
			get
			{
				return diffs;
			}
			private set
			{
				diffs = value;
			}
		}

		public static event updateEventHandler Update;

		public static void SetDevModeValuesFromServer(IDictionary<string, object> value)
		{
			devModeValuesFromServer = value;
		}

		private static object Traverse(object collection, object key, bool autoInsert)
		{
			if (collection == null)
			{
				return null;
			}
			if (collection is IDictionary)
			{
				object value = null;
				IDictionary<object, object> dictionary = collection as Dictionary<object, object>;
				if (dictionary != null)
				{
					dictionary.TryGetValue(key, out value);
					if (value == null && autoInsert)
					{
						value = new Dictionary<object, object>();
						((IDictionary<object, object>)collection)[key] = value;
					}
				}
				else
				{
					IDictionary<string, object> dictionary2 = collection as Dictionary<string, object>;
					dictionary2.TryGetValue((string)key, out value);
					if (value == null && autoInsert)
					{
						value = new Dictionary<string, object>();
						((IDictionary<string, object>)collection)[(string)key] = value;
					}
				}
				return value;
			}
			if (collection is IList)
			{
				IList<object> list = collection as IList<object>;
				int? num = key as int?;
				if (list != null && num.HasValue && num.HasValue && num.Value >= 0 && num.HasValue && num.Value < list.Count)
				{
					return list[num.Value];
				}
			}
			return null;
		}

		public static void RegisterVariable(Var lpVariable)
		{
			vars[lpVariable.Name] = lpVariable;
			if (lpVariable.GetDefaultValue() != null)
			{
				object obj = valuesFromClient;
				for (int i = 0; i < lpVariable.NameComponents.Length - 1; i++)
				{
					obj = Traverse(obj, lpVariable.NameComponents[i], true);
				}
				((IDictionary<string, object>)obj).Add(new KeyValuePair<string, object>(lpVariable.NameComponents[lpVariable.NameComponents.Length - 1], lpVariable.GetDefaultValue()));
			}
			defaultKinds[lpVariable.Name] = lpVariable.Kind;
			if (!Constants.isNoop && Leanplum.HasStartedAndRegisteredAsDeveloper)
			{
				MaybeSendUpdatedValuesFromCode();
			}
		}

		public static Var<T> GetVariable<T>(string name)
		{
			return (!HasVariable(name)) ? null : (vars[name] as Var<T>);
		}

		public static bool HasVariable(string name)
		{
			return vars.ContainsKey(name);
		}

		private static void ComputeMergedDictionary()
		{
			merged = MergeHelper(valuesFromClient, Diffs);
		}

		private static object MergeHelper(object vars, object diff)
		{
			if (Util.IsNumber(diff) || diff is bool? || diff is string || diff is char?)
			{
				return diff;
			}
			if (diff == null)
			{
				return vars;
			}
			if (vars is IList)
			{
				object obj;
				if (diff is IDictionary)
				{
					IDictionary dictionary = diff as IDictionary;
					obj = dictionary;
				}
				else
				{
					obj = null;
				}
				IDictionary dictionary2 = (IDictionary)obj;
				object obj2;
				if (dictionary2 != null)
				{
					IEnumerable keys = dictionary2.Keys;
					obj2 = keys;
				}
				else
				{
					obj2 = diff as IEnumerable;
				}
				IEnumerable enumerable = (IEnumerable)obj2;
				object obj3;
				if (vars is IList)
				{
					IList list = vars as IList;
					obj3 = list;
				}
				else
				{
					obj3 = null;
				}
				IList list2 = (IList)obj3;
				List<object> list3 = new List<object>();
				foreach (object item in list2)
				{
					list3.Add(MergeHelper(item, null));
				}
				{
					foreach (object item2 in enumerable)
					{
						string text = (string)item2;
						int num = Convert.ToInt32(text.Substring(1, text.Length - 1 - 1));
						object diff2 = ((!dictionary2.Contains(item2)) ? null : dictionary2[item2]);
						while (num >= list3.Count)
						{
							list3.Add(null);
						}
						list3[num] = MergeHelper(list3[num], diff2);
					}
					return list3;
				}
			}
			if (vars is IDictionary || diff is IDictionary)
			{
				object obj4;
				if (diff is IDictionary)
				{
					IDictionary dictionary = diff as IDictionary;
					obj4 = dictionary;
				}
				else
				{
					obj4 = null;
				}
				IDictionary dictionary3 = (IDictionary)obj4;
				object obj5;
				if (vars is IDictionary)
				{
					IDictionary dictionary = vars as IDictionary;
					obj5 = dictionary;
				}
				else
				{
					obj5 = null;
				}
				IDictionary dictionary4 = (IDictionary)obj5;
				object obj6;
				if (dictionary3 != null)
				{
					IEnumerable keys = dictionary3.Keys;
					obj6 = keys;
				}
				else
				{
					obj6 = diff as IEnumerable;
				}
				IEnumerable enumerable2 = (IEnumerable)obj6;
				object obj7;
				if (dictionary4 != null)
				{
					IEnumerable keys = dictionary4.Keys;
					obj7 = keys;
				}
				else
				{
					obj7 = vars as IEnumerable;
				}
				IEnumerable enumerable3 = (IEnumerable)obj7;
				Dictionary<object, object> dictionary5 = new Dictionary<object, object>();
				if (enumerable3 != null)
				{
					foreach (object item3 in enumerable3)
					{
						if (!dictionary3.Contains(item3))
						{
							dictionary5[item3] = MergeHelper((!dictionary4.Contains(item3)) ? null : dictionary4[item3], null);
						}
					}
				}
				{
					foreach (object item4 in enumerable2)
					{
						IDictionary<string, object> dictionary6 = Json.Deserialize(Json.Serialize(dictionary4)) as IDictionary<string, object>;
						if (dictionary6 == null)
						{
							dictionary5[item4] = MergeHelper((dictionary4 == null || !dictionary4.Contains(item4)) ? null : dictionary4[item4], (dictionary3 == null || !dictionary3.Contains(item4)) ? null : dictionary3[item4]);
						}
						else
						{
							dictionary5[item4] = MergeHelper((dictionary6 == null || !dictionary6.ContainsKey((string)item4)) ? null : dictionary6[(string)item4], (dictionary3 == null || !dictionary3.Contains(item4)) ? null : dictionary3[item4]);
						}
					}
					return dictionary5;
				}
			}
			return null;
		}

		public static object GetMergedValueFromComponentArray(object[] components)
		{
			object obj = merged ?? valuesFromClient;
			foreach (object key in components)
			{
				obj = Traverse(obj, key, false);
			}
			return obj;
		}

		public static void LoadDiffs()
		{
			if (Constants.isNoop)
			{
				return;
			}
			string savedString = LeanplumNative.CompatibilityLayer.GetSavedString("__leanplum_token");
			if (savedString != null)
			{
				LeanplumRequest.Token = savedString;
				string savedString2 = LeanplumNative.CompatibilityLayer.GetSavedString("__leanplum_variables", "{}");
				string savedString3 = LeanplumNative.CompatibilityLayer.GetSavedString("__leanplum_file_attributes", "{}");
				string savedString4 = LeanplumNative.CompatibilityLayer.GetSavedString("___leanplum_userid");
				if (!string.IsNullOrEmpty(savedString4))
				{
					LeanplumRequest.UserId = AESCrypt.Decrypt(savedString4, LeanplumRequest.Token);
				}
				ApplyVariableDiffs(Json.Deserialize((!(savedString2 == "{}")) ? AESCrypt.Decrypt(savedString2, LeanplumRequest.Token) : savedString2) as IDictionary<string, object>, Json.Deserialize((!(savedString3 == "{}")) ? AESCrypt.Decrypt(savedString3, LeanplumRequest.Token) : savedString3) as IDictionary<string, object>);
			}
		}

		public static void SaveDiffs()
		{
			if (!Constants.isNoop && !string.IsNullOrEmpty(LeanplumRequest.Token))
			{
				string plaintext = Json.Serialize(diffs);
				string plaintext2 = Json.Serialize(fileAttributes);
				LeanplumNative.CompatibilityLayer.StoreSavedString("__leanplum_variables", AESCrypt.Encrypt(plaintext, LeanplumRequest.Token));
				LeanplumNative.CompatibilityLayer.StoreSavedString("__leanplum_file_attributes", AESCrypt.Encrypt(plaintext2, LeanplumRequest.Token));
				if (!string.IsNullOrEmpty(LeanplumRequest.UserId))
				{
					LeanplumNative.CompatibilityLayer.StoreSavedString("___leanplum_userid", AESCrypt.Encrypt(LeanplumRequest.UserId, LeanplumRequest.Token));
				}
				LeanplumNative.CompatibilityLayer.StoreSavedString("__leanplum_token", LeanplumRequest.Token);
				LeanplumNative.CompatibilityLayer.FlushSavedSettings();
			}
		}

		public static void ApplyVariableDiffs(IDictionary<string, object> diffs, IDictionary<string, object> fileAttributes = null)
		{
			if (fileAttributes != null)
			{
				foreach (KeyValuePair<string, object> fileAttribute in fileAttributes)
				{
					FileAttributes[fileAttribute.Key] = fileAttribute.Value;
				}
			}
			if (diffs != null)
			{
				Diffs = diffs;
				ComputeMergedDictionary();
			}
			foreach (Var value in vars.Values)
			{
				value.Update();
			}
			if (!IsSilent)
			{
				SaveDiffs();
				HasReceivedDiffs = true;
				OnUpdate();
			}
		}

		public static void OnUpdate()
		{
			if (VarCache.Update != null)
			{
				VarCache.Update();
			}
		}

		internal static void CheckVarsUpdate()
		{
			CheckVarsUpdate(null);
		}

		internal static void CheckVarsUpdate(Action callback)
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["includeDefaults"] = false.ToString();
			LeanplumRequest leanplumRequest = LeanplumRequest.Post("getVars", dictionary);
			leanplumRequest.Response += delegate(object varsUpdate)
			{
				IDictionary<string, object> dictionary2 = Util.GetLastResponse(varsUpdate) as IDictionary<string, object>;
				IDictionary<string, object> dictionary3 = Util.GetValueOrDefault(dictionary2, "vars") as IDictionary<string, object>;
				IDictionary<string, object> dictionary4 = Util.GetValueOrDefault(dictionary2, "fileAttributes") as IDictionary<string, object>;
				if (!dictionary3.Equals(Diffs) || !dictionary4.Equals(FileAttributes))
				{
					ApplyVariableDiffs(dictionary3, dictionary4);
				}
				if (callback != null)
				{
					callback();
				}
			};
			leanplumRequest.Error += delegate
			{
				if (callback != null)
				{
					callback();
				}
			};
			leanplumRequest.SendNow();
			VarsNeedUpdate = false;
		}

		internal static void MaybeSendUpdatedValuesFromCode()
		{
			if (!Constants.isNoop && devModeValuesFromServer != null && valuesFromClient != devModeValuesFromServer)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["variables"] = Json.Serialize(valuesFromClient);
				dictionary["kinds"] = Json.Serialize(defaultKinds);
				LeanplumRequest.Post("setVars", dictionary).SendNow();
			}
		}
	}
}
