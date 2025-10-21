using System;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public static class Tutorials
{
	public const string asset_path = "Tutorial/Tutorial";

	public const string BasicShooting_Tutorial = "BasicShooting_Tutorial";

	public const string BasicUnit_Tutorial = "BasicUnit_Tutorial";

	public const string PanOverMap = "PanOverMap";

	public const string Bomb_Tutorial = "Bomb_Tutorial";

	public const string Ingame_Hints = "Ingame_Hints";

	public const string UpgradeTutorial = "UpgradeTutorial";

	public const string ThirdShot_Tutorial = "ThirdShot_Tutorial";

	public const string WaitForTank_Tutorial = "WaitForTank_Tutorial";

	public const string FriendGate_Tutorial = "FriendGate_Tutorial";

	public const string MegaBallTutorial = "MegaBallTutorial";

	public const string AppleUpgradeTutorial = "AppleUpgradeTutorial";

	public const string SpecialMissionTutorial = "SpecialMissionTutorial";

	public const string dialog_map2_end = "dialog_map2_end";

	public const string dialog_map3_start = "dialog_map3_start";

	public const string dialog_brachio_appears = "dialog_brachio_appears";

	public const string dialog_map3_end = "dialog_map3_end";

	public const string dialog_map4_start = "dialog_map4_start";

	public const string dialog_disguise_appears = "dialog_disguise_appears";

	public const string dialog_trex_appears = "dialog_trex_appears";

	public const string dialog_map4_end = "dialog_map4_end";

	public const string dialog_map5_start = "dialog_map5_start";

	private static List<Tutorial> entries;

	private static Dictionary<string, Func<BaseScreen>> tutorials = new Dictionary<string, Func<BaseScreen>>
	{
		{
			"BasicShooting_Tutorial",
			() => ScreenManager.Load<BasicShootingTutorial>("Tutorial/Tutorial")
		},
		{
			"BasicUnit_Tutorial",
			() => ScreenManager.Load<BasicUnitCombatTutorial>("Tutorial/Tutorial")
		},
		{
			"Ingame_Hints",
			() => ScreenManager.Load<IngameHinting>("Tutorial/Tutorial")
		},
		{
			"Bomb_Tutorial",
			() => ScreenManager.Load<BombShotTutorial>("Tutorial/Tutorial")
		},
		{
			"ThirdShot_Tutorial",
			() => ScreenManager.Load<ThirdShotTutorial>("Tutorial/Tutorial")
		},
		{
			"WaitForTank_Tutorial",
			() => ScreenManager.Load<WaitForTricerTutorial>("Tutorial/Tutorial")
		},
		{
			"MegaBallTutorial",
			() => ScreenManager.Load<MegaBallTutorial>("Tutorial/Tutorial")
		},
		{
			"AppleUpgradeTutorial",
			() => ScreenManager.Load<AppleLevelUpgradeTutorial>("Tutorial/Tutorial")
		},
		{
			"SpecialMissionTutorial",
			() => ScreenManager.Load<SpecialMissionTutorial>("Tutorial/Tutorial")
		},
		{
			"dialog_map2_end",
			() => ScreenManager.Load<GenericDialogue>("Tutorial/Tutorial")
		},
		{
			"dialog_map3_start",
			() => ScreenManager.Load<GenericDialogue>("Tutorial/Tutorial")
		},
		{
			"dialog_brachio_appears",
			() => ScreenManager.Load<GenericDialogue>("Tutorial/Tutorial")
		},
		{
			"dialog_map3_end",
			() => ScreenManager.Load<GenericDialogue>("Tutorial/Tutorial")
		},
		{
			"dialog_map4_start",
			() => ScreenManager.Load<GenericDialogue>("Tutorial/Tutorial")
		},
		{
			"dialog_disguise_appears",
			() => ScreenManager.Load<GenericDialogue>("Tutorial/Tutorial")
		},
		{
			"dialog_trex_appears",
			() => ScreenManager.Load<GenericDialogue>("Tutorial/Tutorial")
		},
		{
			"dialog_map4_end",
			() => ScreenManager.Load<GenericDialogue>("Tutorial/Tutorial")
		},
		{
			"dialog_map5_start",
			() => ScreenManager.Load<GenericDialogue>("Tutorial/Tutorial")
		}
	};

	public static void Init()
	{
		entries = Serializer.DeserializeFileOrTextAsset<List<Tutorial>>("XML/tutorials");
	}

	public static int LevelID(string name)
	{
		Tutorial tutorial = entries.Find((Tutorial tut) => tut.name == name);
		return Konfiguration.levels.FindIndex((LevelData level) => level.name == tutorial.level);
	}

	public static bool isTutorialLevel(int level_id)
	{
		bool on_map = App.State == App.States.Map;
		string level = Konfiguration.GetLevelData(level_id).name;
		Tutorial tutorial = entries.Find((Tutorial item) => level == item.level && item.onMap == on_map);
		bool flag = tutorial != null;
		if (flag && tutorial.name == "AppleUpgradeTutorial")
		{
			flag = true;
		}
		return Player.MaxLevelID == level_id && flag;
	}

	public static string LocaKeyForLevel(int level)
	{
		if (isTutorialLevel(level))
		{
			string name = Konfiguration.GetLevelData(level).name;
			bool on_map = App.State == App.States.Map;
			Tutorial tutorial = entries.Find((Tutorial item) => name == item.level && item.onMap == on_map);
			return tutorial.name;
		}
		return string.Empty;
	}

	public static BaseScreen LoadAndPush(int level_id, BaseScreen after_this_screen = null)
	{
		bool on_map = App.State == App.States.Map;
		string level = Konfiguration.GetLevelData(level_id).name;
		Tutorial tutorial = entries.Find((Tutorial tut) => level == tut.level && tut.onMap == on_map);
		if (tutorials.ContainsKey(tutorial.name))
		{
			BaseScreen baseScreen = tutorials[tutorial.name]();
			ScreenManager.Push(baseScreen, after_this_screen);
			return baseScreen;
		}
		Debug.LogError("Tutorial was not found: " + level);
		return null;
	}
}
