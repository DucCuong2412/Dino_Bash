using System;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
	public static string ToPrettyString<T>(this IList<T> list)
	{
		string[] array = new string[list.Count];
		for (int i = 0; i != list.Count; i++)
		{
			array[i] = list[i].ToString();
		}
		return "[" + string.Join(", ", array) + "]";
	}

	public static string ToPrettyString<KT, VT>(this IDictionary<KT, VT> dict)
	{
		string[] array = new string[dict.Count];
		int num = 0;
		foreach (KeyValuePair<KT, VT> item in dict)
		{
			array[num] = item.Key.ToString() + " : " + item.Value.ToString();
			num++;
		}
		return "{" + string.Join(", ", array) + "}";
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = UnityEngine.Random.Range(0, num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static string Humanize(this TimeSpan timeSpan)
	{
		List<string> list = new List<string>();
		if (timeSpan.Days != 0 || list.Count != 0)
		{
			list.Add(timeSpan.Days + "Days_Format".Localize());
		}
		if (timeSpan.Hours != 0 || list.Count != 0)
		{
			list.Add(timeSpan.Hours + "Hours_Format".Localize());
		}
		if (timeSpan.Minutes != 0 || list.Count != 0)
		{
			list.Add(timeSpan.Minutes + "Minutes_Format".Localize());
		}
		if (timeSpan.Seconds != 0 || list.Count != 0)
		{
			list.Add(timeSpan.Seconds + "Seconds_Format".Localize());
		}
		return string.Join(" ", list.ToArray());
	}
}
