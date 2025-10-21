using UnityEngine;

public static class i18nExtension
{
	public static string Localize(this string s)
	{
		return i18n.Get(s);
	}

	public static string GetGroupedNumberString(this string s)
	{
		if (i18n.Language == SystemLanguage.French)
		{
			for (int num = s.Length; num > 0; num -= 3)
			{
				s = s.Insert(num, " ");
			}
		}
		return s;
	}
}
