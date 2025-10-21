using System;
using System.Xml.Serialization;
//using LeanplumSDK;
using UnityEngine;

public class ChapterData
{
	public readonly string name;

	public readonly LevelTheme theme;

	public float shop_amount_multiplier;

	public readonly string[] levelnames;


	private float var_shop_amount_multiplier;

	public int levelCount
	{
		get
		{
			return levelnames.Length;
		}
	}

	public void RegisterVars()
	{
		var_shop_amount_multiplier = shop_amount_multiplier;

    }

	public void ApplyVars()
	{
		try
		{
			shop_amount_multiplier = var_shop_amount_multiplier;
		}
		catch (Exception ex)
		{
			Debug.LogError("Quest Leanplum input error in chapter:" + name);
			Debug.LogError(ex.Message + "\n" + ex.StackTrace);
		}
	}
}
