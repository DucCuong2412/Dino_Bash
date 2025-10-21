using System.Collections.Generic;
using UnityEngine;

public class i18n
{
	public class Texts : SerializableDictionary<SystemLanguage, SerializableDictionary<string, string>>
	{
	}

	private static SystemLanguage currentLanguage = SystemLanguage.English;

	private static Texts languages = new Texts();

	public static SystemLanguage Language
	{
		get
		{
			return currentLanguage;
		}
		set
		{
			SystemLanguage systemLanguage = currentLanguage;
			if (languages.ContainsKey(value))
			{
				currentLanguage = value;
			}
			else
			{
				currentLanguage = SystemLanguage.English;
			}
		}
	}

	public static SystemLanguage[] Languages
	{
		get
		{
			SystemLanguage[] array = new SystemLanguage[languages.Keys.Count];
			languages.Keys.CopyTo(array, 0);
			return array;
		}
	}

	private i18n()
	{
	}

	public static void Init()
	{
		languages = Serializer.DeserializeFileOrTextAsset<Texts>("XML/i18n");
		Debug.Log("localization loaded");
		Language = Application.systemLanguage;
		Debug.Log("I18n Language was set to " + currentLanguage);
	}

	public static string Get(string key)
	{
		if (languages[currentLanguage].ContainsKey(key))
		{
			return languages[currentLanguage][key];
		}
		Debug.LogError("No loca entry in " + currentLanguage.ToString() + " for key: #" + key + "#");
		Tracking.MissingLocaKey(key);
		return "KNF:" + key;
	}

	public static List<string> GetCurrentLanguageValues()
	{
		return new List<string>(languages[currentLanguage].Values);
	}
}
