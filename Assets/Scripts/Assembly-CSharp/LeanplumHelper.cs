using System.Collections;
using System.Collections.Generic;
using LeanplumSDK;
using UnityEngine;

public static class LeanplumHelper
{
	public static List<T> toList<T>(IEnumerable collection)
	{
		List<T> list = new List<T>();
		if (collection != null)
		{
			foreach (object item in collection)
			{
				list.Add((T)item);
			}
		}
		return list;
	}

	public static List<object> toListObject(IEnumerable collection)
	{
		List<object> list = new List<object>();
		foreach (object item in collection)
		{
			list.Add(item);
		}
		return list;
	}

	public static Dictionary<string, object> toDictionary(IDictionary dict)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		IDictionaryEnumerator enumerator = dict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			dictionary.Add(enumerator.Key.ToString(), enumerator.Value);
		}
		return dictionary;
	}

	public static int[] toIntArray(Var<List<object>> item)
	{
		int[] array = new int[item.Value.Count];
		for (int i = 0; i < item.Value.Count; i++)
		{
			int result = -1;
			float result2 = -1f;
			if (int.TryParse(item.Value[i].ToString(), out result))
			{
				array[i] = result;
			}
			else if (float.TryParse(item.Value[i].ToString(), out result2))
			{
				array[i] = Mathf.RoundToInt(result2);
			}
			else
			{
				array[i] = (int)item.Value[i];
			}
		}
		return array;
	}

	public static float[] toFloatArray(Var<List<object>> item)
	{
		float[] array = new float[item.Value.Count];
		for (int i = 0; i < item.Value.Count; i++)
		{
			float result = -1f;
			if (float.TryParse(item.Value[i].ToString(), out result))
			{
				array[i] = result;
			}
			else
			{
				array[i] = (float)item.Value[i];
			}
		}
		return array;
	}

	public static string[] toStringArray(Var<List<object>> item)
	{
		string[] array = new string[item.Value.Count];
		for (int i = 0; i < item.Value.Count; i++)
		{
			array[i] = item.Value[i].ToString();
		}
		return array;
	}
}
