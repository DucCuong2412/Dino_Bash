using System;
using System.Collections;
using System.Collections.Generic;
//using LeanplumSDK;
using UnityEngine;
using dinobash;

public static class Konfiguration
{
	public static List<PlayerXPLevelData> XpLevels;

	public static Dictionary<UnitType, EntityData> UnitData;

	private static Dictionary<UnitType, EntityDataVar> entityDataVars;

	public static Dictionary<ShotType, ShotData> ShotData;

	private static Dictionary<ShotType, ShotDataVar> shotDataVars;

	public static List<ChapterData> chapters;

	public static List<LevelData> levels;

	public static List<Quest> quests;

	public static KonfigurationData GameConfig { get; set; }

	public static int CoinBonusForTappingOnNotification
	{
		get
		{
			return GameConfig.CoinBonusForTappingOnNotification;
		}
	}

	public static string RateURL
	{
		get
		{
			return GameConfig.googlePlayRateURL;
		}
	}

	public static void Init()
	{
		if (GameConfig == null)
		{
			GameConfig = Serializer.DeserializeFileOrTextAsset<KonfigurationData>("XML/konfiguration");
			GameConfig.init();
		}
		quests = Serializer.DeserializeFileOrTextAsset<List<Quest>>("XML/quests");
		quests.ForEach(delegate(Quest quest)
		{
			quest.RegisterVars();
		});
		XpLevels = Serializer.DeserializeFileOrTextAsset<List<PlayerXPLevelData>>("XML/xplevels");
		chapters = Serializer.DeserializeFileOrTextAsset<List<ChapterData>>("XML/chapter");
		chapters.ForEach(delegate(ChapterData chapter)
		{
			chapter.RegisterVars();
		});
		LoadLevels();
		LoadEntityData(Serializer.DeserializeFileOrTextAsset<List<EntityData>>("XML/entitydata"));
		LoadShotData(Serializer.DeserializeFileOrTextAsset<List<ShotData>>("XML/shotdata"));
		//Leanplum.VariablesChanged += HandleVariablesChanged;
	}

	private static void HandleVariablesChanged()
	{
		foreach (KeyValuePair<UnitType, EntityDataVar> entityDataVar in entityDataVars)
		{
			EntityData entityData = UnitData[entityDataVar.Key];
			try
			{
				entityData = entityDataVar.Value.apply(entityData);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
			}
			UnitData[entityDataVar.Key] = entityData;
		}
		foreach (KeyValuePair<ShotType, ShotDataVar> shotDataVar in shotDataVars)
		{
			ShotData shotData = ShotData[shotDataVar.Key];
			try
			{
				shotData = shotDataVar.Value.apply(shotData);
			}
			catch (Exception ex2)
			{
				Debug.LogError(shotData.type);
				Debug.LogError(ex2.Message.ToString() + "\n" + ex2.StackTrace.ToString());
			}
			ShotData[shotDataVar.Key] = shotData;
		}
		quests.ForEach(delegate(Quest quest)
		{
			quest.ApplyVars();
		});
		chapters.ForEach(delegate(ChapterData chapter)
		{
			chapter.ApplyVars();
		});
	}

	private static void LoadEntityData(List<EntityData> pUnitdata)
	{
		entityDataVars = new Dictionary<UnitType, EntityDataVar>();
		UnitData = new Dictionary<UnitType, EntityData>();
		foreach (EntityData pUnitdatum in pUnitdata)
		{
			UnitData.Add(pUnitdatum.unit, pUnitdatum);
			entityDataVars.Add(pUnitdatum.unit, new EntityDataVar(pUnitdatum));
		}
	}

	private static void LoadShotData(List<ShotData> pShotdata)
	{
		shotDataVars = new Dictionary<ShotType, ShotDataVar>();
		ShotData = new Dictionary<ShotType, ShotData>();
		foreach (ShotData pShotdatum in pShotdata)
		{
			ShotData.Add(pShotdatum.type, pShotdatum);
			shotDataVars.Add(pShotdatum.type, new ShotDataVar(pShotdatum));
		}
	}

	private static void ValidateDataDict<T>(IDictionary dict)
	{
		string[] names = Enum.GetNames(typeof(T));
		foreach (string text in names)
		{
			bool flag = false;
			foreach (object key in dict.Keys)
			{
				if (key.ToString() == text)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			Debug.LogError("Did not find: " + text);
		}
	}

	private static void LoadLevels()
	{
		levels = new List<LevelData>();
		foreach (ChapterData chapter in chapters)
		{
			for (int i = 0; i < chapter.levelCount; i++)
			{
				levels.Add(LoadLevelData(chapter.levelnames[i]));
			}
		}
	}

	public static LevelData GetLevelData(int levelID)
	{
		int index = Mathf.Min(levels.Count - 1, levelID);
		return levels[index];
	}

	private static LevelData LoadLevelData(string levelname)
	{
		LevelData levelData = Serializer.DeserializeFileOrTextAsset<LevelData>("XML/levels/" + levelname);
		levelData.name = levelname;
		levelData.init();
		return levelData;
	}

	public static int ChapterForLevel(int level)
	{
		int num = 0;
		for (int i = 0; i < chapters.Count; i++)
		{
			num += chapters[i].levelCount;
			if (num > level)
			{
				return i;
			}
		}
		return chapters.Count - 1;
	}

	public static ChapterData GetChapterData(int level)
	{
		int num = ChapterForLevel(level);
		if (0 <= num && num < chapters.Count)
		{
			return chapters[num];
		}
		return null;
	}

	public static int LevelIndexForName(string name)
	{
		int num = levels.FindIndex((LevelData level) => level.name == name);
		if (num == -1)
		{
			Debug.LogError("level does not exists! Might be wrongly spelled: " + name);
		}
		return num;
	}

	public static string NameForLevelIndex(int index)
	{
		return levels[index].name;
	}

	public static HashSet<UnitType> GetUnknownNeanders()
	{
		HashSet<UnitType> hashSet = new HashSet<UnitType>(new UnitType[5]
		{
			UnitType.Neander_ShotShield,
			UnitType.Neander_FrontShield,
			UnitType.Neander_Healer,
			UnitType.Neander_Fire,
			UnitType.Neander_Disguise
		});
		foreach (UnitType discoveredNeander in Player.DiscoveredNeanders)
		{
			hashSet.Remove(discoveredNeander);
		}
		return hashSet;
	}

	public static bool isDinoUnit(UnitType entity)
	{
		return UnitType.LASTUPGRADE < entity && entity < UnitType.LASTDINO;
	}

	public static bool isNeander(UnitType entity)
	{
		return UnitType.FIRSTNEANDER < entity && entity < UnitType.LASTNEANDER;
	}

	public static bool isUpgrade(UnitType entity)
	{
		return UnitType.FIRSTUPGRADE < entity && entity < UnitType.LASTUPGRADE;
	}

	public static bool isConsumable(UnitType entity)
	{
		return UnitType.FIRSTCONSUMABLE < entity && entity < UnitType.LASTCONSUMABLE;
	}

	public static bool isCollectable(UnitType entity)
	{
		return UnitType.FIRSTCOLLECTABLE < entity && entity < UnitType.LASTCOLLECTABLE;
	}

	public static UnitType getPreviousUpgradeStage(UnitType entity)
	{
		if (entity == UnitType.Dynamite)
		{
			return UnitType.DynamiteSmall;
		}
		return UnitType.None;
	}

	public static List<UnitType> getUpgrades()
	{
		List<UnitType> list = new List<UnitType>();
		foreach (LevelData level in levels)
		{
			if (level.unlockUnit != 0 && isUpgrade(level.unlockUnit) && !list.Contains(level.unlockUnit))
			{
				list.Add(level.unlockUnit);
			}
		}
		return list;
	}

	public static List<ShotType> getShots()
	{
		List<ShotType> list = new List<ShotType>();
		list.Add(ShotType.Normal);
		foreach (LevelData level in levels)
		{
			if (level.unlockShot != ShotType.None)
			{
				list.Add(level.unlockShot);
			}
		}
		return list;
	}

	public static List<UnitType> getDinos()
	{
		List<UnitType> list = new List<UnitType>();
		list.Add(UnitType.DinoEgg);
		foreach (LevelData level in levels)
		{
			if (level.unlockUnit != 0 && !list.Contains(level.unlockUnit) && isDinoUnit(level.unlockUnit))
			{
				list.Add(level.unlockUnit);
			}
		}
		return list;
	}

	public static List<UnitType> getConsumables()
	{
		List<UnitType> list = new List<UnitType>();
		foreach (int value in Enum.GetValues(typeof(UnitType)))
		{
			if (isConsumable((UnitType)value))
			{
				list.Add((UnitType)value);
			}
		}
		return list;
	}

	public static int scaleConsumableWithPlayerProgress(int amount)
	{
		int num = Mathf.Max(1, ChapterForLevel(Player.MaxLevelID));
		return Mathf.RoundToInt((float)amount * ((float)num * 0.5f + 0.5f));
	}

	public static bool canLevelUp(UnitType entity)
	{
		if (isDinoUnit(entity))
		{
			int unitLevel = Player.GetUnitLevel(entity);
			return unitLevel + 1 < UnitData[entity].attackStrenghLevels.Length;
		}
		if (isUpgrade(entity))
		{
			return Player.AvailiableUpgrades.Contains(entity) && !Player.ActiveUpgrades.Contains(entity);
		}
		return false;
	}

	public static bool canLevelUp(ShotType shot)
	{
		int shotLevel = Player.GetShotLevel(shot);
		return shotLevel + 1 < ShotData[shot].damageLevels.Length;
	}

	public static int getEntityHealth(UnitType entity, bool nextLevel = false)
	{
		int num = Player.GetUnitLevel(entity);
		if (canLevelUp(entity) && nextLevel)
		{
			num++;
		}
		if (isDinoUnit(entity))
		{
			return UnitData[entity].healthpointslevels[num];
		}
		if (isNeander(entity))
		{
			return UnitData[entity].healthpointslevels[0];
		}
		Debug.LogError("No valid Unit type supplied:" + entity);
		return -1;
	}

	public static int getAttackPower(UnitType entity, bool nextLevel = false)
	{
		int num = Player.GetUnitLevel(entity);
		if (canLevelUp(entity) && nextLevel)
		{
			num++;
		}
		if (isDinoUnit(entity))
		{
			return UnitData[entity].attackStrenghLevels[num];
		}
		if (isNeander(entity))
		{
			return UnitData[entity].attackStrenghLevels[0];
		}
		Debug.LogError("No valid Unit type supplied:" + entity);
		return -1;
	}

	public static int getShotDamage(ShotType shot, bool nextLevel = false)
	{
		int num = Player.GetShotLevel(shot);
		if (canLevelUp(shot) && nextLevel)
		{
			num++;
		}
		return ShotData[shot].damageLevels[num];
	}

	public static int getDinoUpgradeCost(UnitType dino)
	{
		if (isDinoUnit(dino))
		{
			if (isPremium(dino) && !Player.AvailableUnitTypes.Contains(dino))
			{
				return UnitData[dino].premiumCost;
			}
			return UnitData[dino].costLevels[Player.GetUnitLevel(dino)];
		}
		if (isConsumable(dino) && isPremium(dino) && !Player.AvailableUnitTypes.Contains(dino))
		{
			return UnitData[dino].premiumCost;
		}
		Debug.LogError("No valid Unit type supplied:" + dino);
		return -1;
	}

	public static int getEntitiyAppleCost(UnitType entity)
	{
		if (isDinoUnit(entity))
		{
			return UnitData[entity].appleCost;
		}
		return -1;
	}

	public static int getShotUpgradeCost(ShotType shot)
	{
		return ShotData[shot].cost;
	}

	public static int getUpgradeBuyCost(UnitType upgrade)
	{
		if (isUpgrade(upgrade))
		{
			if (isPremium(upgrade) && !Player.AvailableUnitTypes.Contains(upgrade))
			{
				return UnitData[upgrade].premiumCost;
			}
			return UnitData[upgrade].costLevels[0];
		}
		return -1;
	}

	public static bool isPremium(UnitType entitiy)
	{
		if ((isUpgrade(entitiy) || isDinoUnit(entitiy) || isConsumable(entitiy)) && UnitData.ContainsKey(entitiy))
		{
			return UnitData[entitiy].premiumCost > 0;
		}
		return false;
	}

	public static bool isPremium(ShotType shot)
	{
		return false;
	}

	public static PlayerXPLevelData getXPLevelData(int xplevel)
	{
		return XpLevels[xplevel];
	}

	public static int GetUpgradeTime(UnitType unit, ShotType shot)
	{
		if (unit != 0 && UnitData.ContainsKey(unit))
		{
			return UnitData[unit].upgrade_duration;
		}
		Debug.LogError(string.Format("No upgrade time found for: {0} - {1}", unit, shot));
		return 3;
	}

	public static int GetUpgradeTrainingSkipCost(UnitType unit, ShotType shot)
	{
		if (unit != 0 && UnitData.ContainsKey(unit))
		{
			return UnitData[unit].upgrade_skip_cost;
		}
		Debug.LogError(string.Format("No upgrade skip cost found for: {0} - {1}", unit, shot));
		return 3;
	}

	public static int GetUpgradeRequirement(UnitType dino)
	{
		if (UnitData.ContainsKey(dino))
		{
			string level_requirement = UnitData[dino].level_requirement;
			return LevelIndexForName(level_requirement);
		}
		Debug.LogError("Unit not exist in collection? " + dino);
		return 0;
	}

	public static int GetUpgradeRequirement(ShotType shot)
	{
		if (ShotData.ContainsKey(shot))
		{
			string level_requirement = ShotData[shot].level_requirement;
			return LevelIndexForName(level_requirement);
		}
		Debug.LogError("Shot not exist in collection? " + shot);
		return 0;
	}
}
