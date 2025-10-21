using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dinobash
{
	public class Player : MonoBase
	{
		public const int megaball_start_count = 3;

		public const int MAXLEVEL = 100;

		public const int MAXLIFES = 5;

		public const int dinoUnitSlots = 4;

		private const string watched_upgrade_tutorial = "watched_upgrade_tutorial";

		private const string Watched_upgrade_tutorial_2nd = "Watched_upgrade_tutorial_2nd";

		private const string playerDataKey = "playerData";

		private const string playerDataBackupKey = "playerDataBackup";

		public const int appleCapOnTop = 20;

		private PlayerData playerData;

		private EntityTimers entity_timers;

		private static readonly DateTime referenceTime = new DateTime(1970, 1, 9, 0, 0, 0);

		private float lastUpdate;

		private int _apples;

		private int appleCollectLevel;

		private float appleCollectRate;

		public static Player Instance { get; private set; }

		public PlayerData PlayerData
		{
			get
			{
				return Instance.playerData;
			}
		}

		public static int CurrentLevelID
		{
			get
			{
				return Instance.playerData.CurrentLevelID;
			}
			set
			{
				if (value <= Instance.playerData.MaxLevelID)
				{
					Instance.playerData.CurrentLevelID = value;
					SavePlayer();
				}
			}
		}

		public static int MaxLevelID
		{
			get
			{
				return Instance.playerData.MaxLevelID;
			}
		}

		public static bool HasPlayedMaxLevelID
		{
			get
			{
				return Instance.playerData.hasPlayedMaxLevel;
			}
			set
			{
				Instance.playerData.hasPlayedMaxLevel = value;
				SavePlayer();
			}
		}

		public static bool[] CollectableDropProgress
		{
			get
			{
				return Instance.playerData.CollectableDropProgress;
			}
			private set
			{
				Instance.playerData.CollectableDropProgress = value;
			}
		}

		public static int LooseCount
		{
			get
			{
				return Instance.playerData.looseCount;
			}
			set
			{
				int num = value - Instance.playerData.looseCount;
				Instance.playerData.looseCount = value;
				if (num > 0)
				{
					Instance.playerData.totalLooseCount += num;
				}
				SavePlayer();
			}
		}

		public static int WinCount
		{
			get
			{
				return Instance.playerData.winCount;
			}
			set
			{
				Instance.playerData.winCount = value;
				SavePlayer();
			}
		}

		public static bool CompletedAllLevels
		{
			get
			{
				return Instance.playerData.MaxLevelID >= Konfiguration.levels.Count - 1;
			}
		}

		public static int XP
		{
			get
			{
				return Instance.playerData.XP;
			}
			set
			{
				if (value < 0)
				{
					return;
				}
				int xPLevel = XPLevel;
				Instance.playerData.XP = value;
				if (XPLevel > xPLevel)
				{
					Debug.Log("Level up! Now level: " + XPLevel);
					PlayerXPLevelData xPLevelData = Konfiguration.getXPLevelData(XPLevel);
					Wallet.GiveDiamonds(xPLevelData.diamonds_reward);
					Tracking.set_player_level();
					SavePlayer();
					if (Player.OnLevelUp != null)
					{
						Player.OnLevelUp();
					}
				}
			}
		}

		public static int XPLevel
		{
			get
			{
				int num = 0;
				int num2 = 0;
				for (int i = 1; i < Konfiguration.XpLevels.Count; i++)
				{
					num2 += Konfiguration.XpLevels[i].xp;
					if (num2 <= XP && i < 100)
					{
						num++;
						continue;
					}
					return num;
				}
				throw new Exception("Ups, Error in XP Level Calculation");
			}
		}

		public static int Lives
		{
			get
			{
				return Instance.playerData.Lives;
			}
			set
			{
				int lives = Instance.playerData.Lives;
				Instance.playerData.Lives = Mathf.Clamp(value, 0, 5);
				if (lives > value && lives == 5)
				{
					Instance.playerData.timeOfLostLive = DateTime.UtcNow;
				}
				else if (lives < 5 && Player.OnGenerateLife != null)
				{
					Player.OnGenerateLife();
				}
			}
		}

		public static int shotSlots
		{
			get
			{
				return (!ActiveUpgrades.Contains(UnitType.AdditionalShotSlot)) ? 3 : 4;
			}
		}

		public static List<DinoShotUpgradeAdapter> getNewUpgradePossibilites
		{
			get
			{
				List<DinoShotUpgradeAdapter> newlist = new List<DinoShotUpgradeAdapter>();
				Instance.playerData.newShotTypes.ForEach(delegate(ShotType shot)
				{
					newlist.Add(new ShotAdapter(shot));
				});
				Instance.playerData.newUnitTypes.ForEach(delegate(UnitType entitiy)
				{
					if (Konfiguration.isDinoUnit(entitiy))
					{
						newlist.Add(new DinoAdapter(entitiy));
					}
					else if (Konfiguration.isUpgrade(entitiy))
					{
						newlist.Add(new UpgradeAdapter(entitiy));
					}
				});
				return newlist;
			}
		}

		public static SerializableList<UnitType> AvailableUnitTypes
		{
			get
			{
				return Instance.playerData.availiableUnitTypes;
			}
		}

		public static SerializableList<ShotType> AvailableShotTypes
		{
			get
			{
				return Instance.playerData.availiableShotTypes;
			}
		}

		public static SerializableList<UnitType> SelectedUnitTypes
		{
			get
			{
				return Instance.playerData.selectedUnitTypes;
			}
			set
			{
				Instance.playerData.selectedUnitTypes = value;
			}
		}

		public static SerializableList<ShotType> SelectedShotTypes
		{
			get
			{
				return Instance.playerData.selectedShotTypes;
			}
			set
			{
				Instance.playerData.selectedShotTypes = value;
			}
		}

		public static SerializableList<UnitType> AvailiableUpgrades
		{
			get
			{
				return Instance.playerData.availiableUpgrades;
			}
			set
			{
				Instance.playerData.availiableUpgrades = value;
			}
		}

		public static SerializableList<UnitType> ActiveUpgrades
		{
			get
			{
				return Instance.playerData.activeUpgrades;
			}
			set
			{
				Instance.playerData.activeUpgrades = value;
			}
		}

		public static bool WatchedUpgradeTutorial
		{
			get
			{
				return Instance.playerData.tutorial_progress.Contains("watched_upgrade_tutorial");
			}
			set
			{
				if (value && !Instance.playerData.tutorial_progress.Contains("watched_upgrade_tutorial"))
				{
					Instance.playerData.tutorial_progress.Add("watched_upgrade_tutorial");
				}
			}
		}

		public static bool WatchedUpgradeTutorial2nd
		{
			get
			{
				return Instance.playerData.tutorial_progress.Contains("Watched_upgrade_tutorial_2nd");
			}
			set
			{
				if (value && !Instance.playerData.tutorial_progress.Contains("Watched_upgrade_tutorial_2nd"))
				{
					Instance.playerData.tutorial_progress.Add("Watched_upgrade_tutorial_2nd");
				}
			}
		}

		public static bool[] UserHasSeenUpgradeTabs
		{
			get
			{
				return Instance.playerData.user_has_seen_upgrade_tabs;
			}
			set
			{
				Instance.playerData.user_has_seen_upgrade_tabs = value;
			}
		}

		public static SerializableList<UnitType> DiscoveredNeanders
		{
			get
			{
				return Instance.playerData.discovered_neanders;
			}
		}

		public static int LifeRespawnInterval
		{
			get
			{
				if (Instance.PlayerData.timeManipulationPenalty)
				{
					return 86400;
				}
				return Konfiguration.GameConfig.LifeRefillIntervall;
			}
		}

		public int LevelCoins { get; set; }

		public int LevelXP { get; set; }

		public int Apples
		{
			get
			{
				return _apples;
			}
			set
			{
				_apples = Mathf.Clamp(value, 0, get_max_apples());
			}
		}

		public int AppleCollectLevel
		{
			get
			{
				return appleCollectLevel;
			}
			set
			{
				appleCollectLevel = Mathf.Clamp(value, 0, Konfiguration.GameConfig.AppleUpgradeCost.Length - 1);
				appleCollectRate = Konfiguration.GameConfig.AppleCollectRate[appleCollectLevel];
				Debug.Log("apple collec level: " + appleCollectLevel + " | apple collect rate: " + appleCollectRate);
			}
		}

		public int numberOfFriendGateHelpers
		{
			get
			{
				return playerData.numberOfFriendGateHelpers;
			}
		}

		public int splitTestingRandomId
		{
			get
			{
				playerData.splitTestingRandomId = ((playerData.splitTestingRandomId <= 999) ? playerData.splitTestingRandomId : UnityEngine.Random.Range(0, 1000));
				return playerData.splitTestingRandomId;
			}
		}

		public string splitTestingGroupName
		{
			get
			{
				return playerData.splitTestingGroupName;
			}
			set
			{
				playerData.splitTestingGroupName = value;
			}
		}

		public string splitTestingDataSet
		{
			get
			{
				return playerData.splitTestingDataSet;
			}
			set
			{
				playerData.splitTestingDataSet = value;
			}
		}

		public PlayerData.AchievementStates achievementStates
		{
			get
			{
				return playerData.achievementStates;
			}
			set
			{
				playerData.achievementStates = value;
			}
		}

		public bool wasLoggedIntoSocialGamingProvider
		{
			get
			{
				return playerData.wasLoggedIntoSocialGamingProvider;
			}
			set
			{
				playerData.wasLoggedIntoSocialGamingProvider = value;
			}
		}

		public bool hasReceivedCoinsForTappingOnNotification
		{
			get
			{
				return playerData.hasReceivedCoinsForTappingOnNotification;
			}
		}

		public bool shouldShowRatePrompt
		{
			get
			{
				return playerData.shouldShowRatePrompt;
			}
			set
			{
				playerData.shouldShowRatePrompt = value;
			}
		}

		public DateTime lastTimeInApp
		{
			get
			{
				return playerData.lastTimeInApp;
			}
		}

		public DateTime lastQuestCreated
		{
			get
			{
				return playerData.lastQuestCreated;
			}
			set
			{
				playerData.lastQuestCreated = value;
			}
		}

		public SerializableList<Quest> activeQuests
		{
			get
			{
				return playerData.activeQuests;
			}
		}

		public bool bird_focus_on_visible_neanders
		{
			get
			{
				return playerData.bird_focus_on_visible_neanders;
			}
			set
			{
				playerData.bird_focus_on_visible_neanders = value;
			}
		}

		public static event Action OnLevelUp;

		public static event Action OnGenerateLife;

		public static event Action<UnitType> OnConsumableRefill;

		public static Player Create()
		{
			Instance = App.Instance.AddOrGetComponent<Player>();
			return Instance;
		}

		public static void CompletedLevel(int level)
		{
			if (level == Instance.playerData.MaxLevelID && !CompletedAllLevels)
			{
				Instance.playerData.MaxLevelID++;
				HasPlayedMaxLevelID = false;
				CollectableDropProgress = new bool[Konfiguration.GetLevelData(MaxLevelID).enemies.FindAll((LevelEnemy x) => Konfiguration.isCollectable(x.unittype)).Count];
				ShotType unlockShot = Konfiguration.GetLevelData(level).unlockShot;
				if (unlockShot != ShotType.None)
				{
					UnlockShot(unlockShot);
				}
				UnitType unlockUnit = Konfiguration.GetLevelData(level).unlockUnit;
				if (unlockUnit != 0)
				{
					if (Konfiguration.isDinoUnit(unlockUnit))
					{
						UnlockDino(unlockUnit);
					}
					if (Konfiguration.isUpgrade(unlockUnit))
					{
						Instance.playerData.newUnitTypes.Add(unlockUnit);
						if (!AvailiableUpgrades.Contains(unlockUnit))
						{
							AvailiableUpgrades.Add(unlockUnit);
						}
					}
					if (Konfiguration.isConsumable(unlockUnit) && !Instance.playerData.consumables.ContainsKey(unlockUnit))
					{
						if (unlockUnit == UnitType.MegaBall)
						{
							Instance.playerData.consumables.Add(unlockUnit, 3);
						}
						else
						{
							Instance.playerData.consumables.Add(unlockUnit, Konfiguration.GameConfig.consumable_unlock_fill);
						}
						Tracking.pickup_consumable(unlockUnit.ToString(), level, getConsumableCount(unlockUnit));
					}
				}
				if (level == Konfiguration.GameConfig.First_quest_level)
				{
					Debug.Log("Quest - Starting Quests for game!");
					QuestManager.instance.createQuest(QuestDuration.medium_term | QuestDuration.long_term);
				}
				UpdateAchievements(level);
				Tracking.level(level, Tracking.LevelAction.complete, LooseCount, 1f, 0, 0);
			}
			Tracking.level(level, Tracking.LevelAction.win, LooseCount, 1f, Level.coins_recieved, Level.xp_recieved);
			Lives++;
			if (!CompletedAllLevels && Konfiguration.GetLevelData(MaxLevelID).endless_mode && Konfiguration.GetLevelData(MaxLevelID - 1).is_friend_gate)
			{
				CompletedLevel(MaxLevelID);
			}
			SavePlayer();
		}

		private static void UpdateAchievements(int level)
		{
			if (Konfiguration.GetLevelData(level).is_friend_gate)
			{
				int num = Konfiguration.ChapterForLevel(level);
				if (num == 0)
				{
					SocialGamingManager.Instance.ReportProgress(AchievementIds.COMPLETED_FIRST_ISLAND, 1);
				}
				if (num == 1)
				{
					SocialGamingManager.Instance.ReportProgress(AchievementIds.COMPLETED_THE_SECOND_ISLAND, 1);
				}
				if (num == 2)
				{
					SocialGamingManager.Instance.ReportProgress(AchievementIds.COMPLETED_THE_THIRD_ISLAND, 1);
				}
				if (num == 3)
				{
					SocialGamingManager.Instance.ReportProgress(AchievementIds.COMPLETED_THE_FOURTH_ISLAND, 1);
				}
			}
			else if (CompletedAllLevels)
			{
				SocialGamingManager.Instance.ReportProgress(AchievementIds.COMPLETED_THE_FIFTH_ISLAND, 1);
			}
		}

		public static void RecordEndlessLevel(int levelid, float time)
		{
			LevelData levelData = Konfiguration.GetLevelData(levelid);
			if (Instance.playerData.endless_level_times.ContainsKey(levelData.name))
			{
				Instance.playerData.endless_level_times[levelData.name] = Mathf.Max(Instance.playerData.endless_level_times[levelData.name], time);
			}
			else
			{
				Instance.playerData.endless_level_times.Add(levelData.name, time);
			}
		}

		public static int getBestEndlessLevelScore(int levelid)
		{
			LevelData levelData = Konfiguration.GetLevelData(levelid);
			if (Instance.playerData.endless_level_times.ContainsKey(levelData.name))
			{
				float time = Instance.playerData.endless_level_times[levelData.name];
				return Level.calculateEndlessScore(time);
			}
			return -1;
		}

		public static bool hasUnlockedConsumable(UnitType consumable)
		{
			if (Konfiguration.isConsumable(consumable))
			{
				return Instance.playerData.consumables.ContainsKey(consumable);
			}
			return false;
		}

		public static int getConsumableCount(UnitType consumable)
		{
			if (Konfiguration.isConsumable(consumable) && Instance.playerData.consumables.ContainsKey(consumable))
			{
				return Instance.playerData.consumables[consumable];
			}
			return -1;
		}

		public static int getConsumableRefillAmount(UnitType consumable)
		{
			if (Konfiguration.isConsumable(consumable) && Konfiguration.GameConfig.ConsumableRefillValues.ContainsKey(consumable))
			{
				return Konfiguration.GameConfig.ConsumableRefillValues[consumable];
			}
			return 0;
		}

		public static void changeConsumableCount(UnitType consumable, int amount)
		{
			if (Konfiguration.isConsumable(consumable) && Instance.playerData.consumables.ContainsKey(consumable))
			{
				int consumableCount = getConsumableCount(consumable);
				Instance.playerData.consumables[consumable] = Mathf.Max(0, amount + consumableCount);
				SavePlayer();
				if (consumableCount < Instance.playerData.consumables[consumable] && Player.OnConsumableRefill != null)
				{
					Player.OnConsumableRefill(consumable);
				}
			}
		}

		public static void RemoveFromNewUpgradePossibilites(DinoShotUpgradeAdapter adapter)
		{
			if (adapter is DinoAdapter)
			{
				UnitType dino = (adapter as DinoAdapter).dino;
				if (Instance.playerData.newUnitTypes.Contains(dino))
				{
					Instance.playerData.newUnitTypes.Remove(dino);
				}
			}
			if (adapter is UpgradeAdapter)
			{
				UnitType upgrade = (adapter as UpgradeAdapter).upgrade;
				if (Instance.playerData.newUnitTypes.Contains(upgrade))
				{
					Instance.playerData.newUnitTypes.Remove(upgrade);
				}
			}
			else if (adapter is ShotAdapter)
			{
				ShotType shot = (adapter as ShotAdapter).shot;
				if (Instance.playerData.newShotTypes.Contains(shot))
				{
					Instance.playerData.newShotTypes.Remove(shot);
				}
			}
		}

		public static int GetUnitLevel(UnitType unit)
		{
			if (Instance.playerData.unitLevels.ContainsKey(unit))
			{
				return Instance.playerData.unitLevels[unit];
			}
			return 0;
		}

		public static void UnlockShot(ShotType shot)
		{
			if (!Instance.playerData.availiableShotTypes.Contains(shot))
			{
				Debug.Log("unlocking: " + shot);
				Instance.playerData.availiableShotTypes.Add(shot);
				Instance.playerData.shotLevels.Add(shot, 0);
				Instance.playerData.newShotTypes.Add(shot);
				if (Instance.playerData.selectedShotTypes.Count < shotSlots && !Instance.playerData.selectedShotTypes.Contains(shot))
				{
					Instance.playerData.selectedShotTypes.Add(shot);
				}
				SavePlayer();
			}
		}

		public static void UnlockDino(UnitType dino)
		{
			if (!Instance.playerData.availiableUnitTypes.Contains(dino) && Konfiguration.isDinoUnit(dino))
			{
				Debug.Log("unlocking: " + dino);
				Instance.playerData.newUnitTypes.Add(dino);
				if (dino != UnitType.DinoEgg)
				{
					Instance.playerData.availiableUnitTypes.Add(dino);
				}
				if (!Instance.playerData.unitLevels.ContainsKey(dino))
				{
					Instance.playerData.unitLevels.Add(dino, 0);
				}
				if (Instance.playerData.selectedUnitTypes.Count < 4 && Instance.playerData.availiableUnitTypes.Contains(dino))
				{
					Instance.playerData.selectedUnitTypes.Add(dino);
				}
				SavePlayer();
			}
		}

		public static void ActivateUpgrade(UnitType upgrade)
		{
			if (Konfiguration.isUpgrade(upgrade) && Instance.playerData.availiableUpgrades.Contains(upgrade) && !Instance.playerData.activeUpgrades.Contains(upgrade))
			{
				Debug.Log("unlocking: " + upgrade);
				Instance.playerData.activeUpgrades.Add(upgrade);
				Instance.entity_timers.AddTempUnlock(upgrade);
				if (!Instance.playerData.activeUpgradesState.ContainsKey(upgrade))
				{
					Instance.playerData.activeUpgradesState.Add(upgrade, true);
				}
				else
				{
					Instance.playerData.activeUpgradesState[upgrade] = true;
				}
				UnitType previousUpgradeStage = Konfiguration.getPreviousUpgradeStage(upgrade);
				if (previousUpgradeStage != 0)
				{
					ActivateUpgrade(previousUpgradeStage);
				}
				SavePlayer();
			}
		}

		public static void DisableUpgrade(UnitType upgrade)
		{
			if (Konfiguration.isUpgrade(upgrade) && Instance.playerData.activeUpgrades.Contains(upgrade))
			{
				Debug.Log("locking: " + upgrade);
				Instance.playerData.activeUpgrades.Remove(upgrade);
				Instance.playerData.activeUpgradesState.Remove(upgrade);
				SavePlayer();
			}
		}

		public static bool GetUpgradeState(UnitType upgrade)
		{
			if (Konfiguration.isUpgrade(upgrade) && Instance.playerData.activeUpgradesState.ContainsKey(upgrade))
			{
				return Instance.playerData.activeUpgradesState[upgrade];
			}
			Debug.LogError(string.Format("upgrade: {0} does not exist", upgrade));
			return false;
		}

		public static void SetUpgradeState(UnitType upgrade, bool state)
		{
			if (Konfiguration.isUpgrade(upgrade) && Instance.playerData.activeUpgradesState.ContainsKey(upgrade))
			{
				Instance.playerData.activeUpgradesState[upgrade] = state;
			}
		}

		public static void UpgradeUnit(UnitType unit, bool add_to_queue = true)
		{
			if (add_to_queue)
			{
				Instance.entity_timers.Add(unit);
				return;
			}
			if (EntityTimers.is_upgrading(unit))
			{
				Instance.entity_timers.Remove(unit);
			}
			if (unit == UnitType.DinoEgg || (Konfiguration.isDinoUnit(unit) && Instance.playerData.availiableUnitTypes.Contains(unit) && Konfiguration.canLevelUp(unit)))
			{
				if (!Instance.playerData.unitLevels.ContainsKey(unit))
				{
					Instance.playerData.unitLevels.Add(unit, 0);
				}
				SerializableDictionary<UnitType, int> unitLevels;
				SerializableDictionary<UnitType, int> serializableDictionary = (unitLevels = Instance.playerData.unitLevels);
				UnitType key;
				UnitType key2 = (key = unit);
				int num = unitLevels[key];
				serializableDictionary[key2] = num + 1;
				SavePlayer();
			}
		}

		public static void UpgradeShot(ShotType shot, bool add_to_queue = true)
		{
			if (Instance.playerData.availiableShotTypes.Contains(shot) && Konfiguration.canLevelUp(shot))
			{
				if (!Instance.playerData.shotLevels.ContainsKey(shot))
				{
					Instance.playerData.shotLevels.Add(shot, 0);
				}
				SerializableDictionary<ShotType, int> shotLevels;
				SerializableDictionary<ShotType, int> serializableDictionary = (shotLevels = Instance.playerData.shotLevels);
				ShotType key;
				ShotType key2 = (key = shot);
				int num = shotLevels[key];
				serializableDictionary[key2] = num + 1;
				SavePlayer();
			}
		}

		public static int GetShotLevel(ShotType shot)
		{
			if (Instance.playerData.shotLevels.ContainsKey(shot))
			{
				return Instance.playerData.shotLevels[shot];
			}
			return 0;
		}

		public static bool tutorial_reached(string event_tag)
		{
			if (Instance.playerData.tutorial_progress.Contains(event_tag))
			{
				return false;
			}
			Instance.playerData.tutorial_progress.Add(event_tag);
			return true;
		}

		public static void ResetPlayer()
		{
			Instance.ResetPlayerData();
		}

		public static void LoadPlayer()
		{
			try
			{
				Instance.playerData = Serializer.DeserializeFromPlayerPrefs<PlayerData>("playerData");
				UpdateSaveGame();
				Debug.Log("load Player");
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message + ex.StackTrace);
				string @string = PlayerPrefs.GetString("playerDataBackup", null);
				if (!string.IsNullOrEmpty(@string))
				{
					string string2 = PlayerPrefs.GetString("playerData", null);
					Debug.Log("**** trying to restore backup");
					Tracking.CorruptedSaveGame(string2, @string);
					PlayerPrefs.SetString("playerDataBackup", null);
					PlayerPrefs.SetString("playerData", @string);
					LoadPlayer();
					return;
				}
				Debug.Log("**** No backup found");
				Instance.ResetPlayerData();
			}
			if (Instance.playerData == null)
			{
				Instance.ResetPlayerData();
			}
			Instance.entity_timers = Instance.AddOrGetComponent<EntityTimers>();
			Instance.playerData.session++;
			ValidatePlayer();
		}

		private static void UpdateSaveGame()
		{
			if (Instance.playerData.saveGameVersion < 2 && Instance.playerData.saveGameVersion == 1)
			{
				List<float> list = new List<float>(Instance.playerData.endless_level_times.Values);
				List<string> list2 = new List<string>(Instance.playerData.endless_level_times.Keys);
				for (int i = 0; i < list2.Count; i++)
				{
					float num = list[i];
					float num2 = Level.oldScoreToTime(num);
					float num3 = Level.calculateEndlessScore(num2);
					Debug.Log(string.Format("Updating Savegame - level:{0} from {1} to {2} => {3} ", list2[i], num, num2, num3));
					Instance.playerData.endless_level_times[list2[i]] = num2;
				}
				Instance.playerData.saveGameVersion = 2;
			}
		}

		public static void SavePlayer()
		{
			Debug.Log("save Player");
			string @string = PlayerPrefs.GetString("playerData", null);
			if (!string.IsNullOrEmpty(@string))
			{
				Debug.Log("**** Backing up current save game");
				PlayerPrefs.SetString("playerDataBackup", @string);
			}
			Instance.playerData.lastTimeInApp = DateTime.UtcNow;
			Serializer.SerializeToPlayerPrefs(Instance.playerData, "playerData");
		}

		public static void ApplyCloudSaveGame(CloudSaveGames.CloudSaveGame csg)
		{
			Instance.playerData = csg.player_data;
			Wallet.ApplyCloudSaveGame(csg);
			SavePlayer();
			switch (App.State)
			{
			case App.States.Map:
				App.stateMap(false, true);
				break;
			case App.States.StartScreen:
				App.stateStartScreen();
				break;
			case App.States.Menu:
				break;
			}
		}

		public static void ValidatePlayer()
		{
			Instance.AddPremiumItems();
			Instance.AddItemsFromGameProgress();
			Instance.UpdateUpgradeStates();
			Instance.CheckCollectableProgress();
			CheckForTimeManipulation();
			SavePlayer();
		}

		private void ResetPlayerData()
		{
			StopAllCoroutines();
			Debug.Log("reset Player");
			playerData = new PlayerData();
			Wallet.Reset();
			Wallet.GiveCoins(Konfiguration.GameConfig.Start_Coins);
			Wallet.GiveDiamonds(Konfiguration.GameConfig.Start_Diamonds);
			playerData.availiableShotTypes.Add(ShotType.Normal);
			playerData.shotLevels.Add(ShotType.Normal, 0);
			playerData.selectedShotTypes.Add(ShotType.Normal);
			AddPremiumItems();
			playerData.unitLevels.Add(UnitType.DinoEgg, 0);
		}

		private void UpdateUpgradeStates()
		{
			foreach (UnitType activeUpgrade in ActiveUpgrades)
			{
				if (!Instance.playerData.activeUpgradesState.ContainsKey(activeUpgrade))
				{
					Instance.playerData.activeUpgradesState.Add(activeUpgrade, true);
				}
			}
		}

		private void AddPremiumItems()
		{
			foreach (int value in Enum.GetValues(typeof(UnitType)))
			{
				if (Konfiguration.isUpgrade((UnitType)value) && Konfiguration.isPremium((UnitType)value))
				{
					if (!AvailiableUpgrades.Contains((UnitType)value))
					{
						AvailiableUpgrades.Add((UnitType)value);
					}
					if (!Instance.playerData.activeUpgradesState.ContainsKey((UnitType)value))
					{
						Instance.playerData.activeUpgradesState.Add((UnitType)value, false);
					}
				}
			}
			if (!playerData.shotLevels.ContainsKey(ShotType.Meteor))
			{
				playerData.shotLevels.Add(ShotType.Meteor, 0);
			}
		}

		private void AddItemsFromGameProgress()
		{
			for (int i = 0; i < Konfiguration.levels.Count && i < MaxLevelID; i++)
			{
				LevelData levelData = Konfiguration.levels[i];
				if (!AvailableUnitTypes.Contains(levelData.unlockUnit) && Konfiguration.isDinoUnit(levelData.unlockUnit))
				{
					UnlockDino(levelData.unlockUnit);
				}
				if (!AvailableShotTypes.Contains(levelData.unlockShot) && levelData.unlockShot != ShotType.None)
				{
					UnlockShot(levelData.unlockShot);
				}
				if (!AvailiableUpgrades.Contains(levelData.unlockUnit) && Konfiguration.isUpgrade(levelData.unlockUnit))
				{
					ActivateUpgrade(levelData.unlockUnit);
				}
			}
		}

		private void CheckCollectableProgress()
		{
			int count = Konfiguration.GetLevelData(MaxLevelID).enemies.FindAll((LevelEnemy x) => Konfiguration.isCollectable(x.unittype)).Count;
			if (CollectableDropProgress.Length != count)
			{
				CollectableDropProgress = new bool[count];
			}
		}

		public static void CheckForTimeManipulation()
		{
			if (!(Instance == null) && (TimeCheatDetector.time_cheat_detected || Instance.lastTimeInApp > DateTime.UtcNow))
			{
				TimeCheatDetector.time_cheat_detected = false;
				Instance.PlayerData.timeManipulationPenalty = true;
				Instance.playerData.timeOfLostLive = DateTime.UtcNow;
				DateTime? friendGateStartTime = Instance.playerData.friendGateStartTime;
				if (friendGateStartTime.HasValue)
				{
					Instance.playerData.friendGateStartTime = DateTime.Now;
				}
				Tracking.TimeManipulationDetected();
			}
		}

		private void UpdateLives()
		{
			if (playerData == null)
			{
				Debug.Log("playerdata is null");
				return;
			}
			if (playerData.Lives >= 5)
			{
				playerData.Lives = 5;
				return;
			}
			double totalSeconds = DateTime.UtcNow.Subtract(referenceTime).TotalSeconds;
			double totalSeconds2 = playerData.timeOfLostLive.Subtract(referenceTime).TotalSeconds;
			int value = (int)Math.Floor((totalSeconds - totalSeconds2) / (double)LifeRespawnInterval);
			value = Mathf.Clamp(value, 0, 5);
			playerData.Lives = Mathf.Clamp(playerData.Lives + value, 0, 5);
			playerData.timeOfLostLive = playerData.timeOfLostLive.AddSeconds(value * LifeRespawnInterval);
			if (value > 0)
			{
				Instance.PlayerData.timeManipulationPenalty = false;
				SavePlayer();
				Debug.Log("Player receives a life " + playerData.Lives + "/" + 5);
				if (Player.OnGenerateLife != null)
				{
					Player.OnGenerateLife();
				}
			}
		}

		public static double GetSecondsToNextLive()
		{
			if (Instance.playerData.Lives >= 5)
			{
				return 0.0;
			}
			double totalSeconds = DateTime.UtcNow.Subtract(referenceTime).TotalSeconds;
			double totalSeconds2 = Instance.playerData.timeOfLostLive.Subtract(referenceTime).TotalSeconds;
			double d = totalSeconds2 + (double)LifeRespawnInterval - totalSeconds;
			return Math.Floor(d);
		}

		public static double GetSecondsToFullLives()
		{
			if (Lives == 5)
			{
				return 0.0;
			}
			return (double)((5 - Lives - 1) * LifeRespawnInterval) + GetSecondsToNextLive();
		}

		private void Update()
		{
			if (Time.realtimeSinceStartup > lastUpdate)
			{
				lastUpdate = Time.realtimeSinceStartup + 0.5f;
				UpdateLives();
			}
		}

		public void LevelStart()
		{
			Level.Instance.OnLevelStart += OnLevelStart;
			Level.Instance.OnLevelPlay += OnLevelPlay;
			Level.Instance.OnLevelWon += OnlevelEnd;
			Level.Instance.OnLevelLost += OnlevelEnd;
			Level.Instance.OnLevelAbort += OnlevelEnd;
		}

		public void LevelEnd()
		{
			Level.Instance.OnLevelStart -= OnLevelStart;
			Level.Instance.OnLevelPlay -= OnLevelPlay;
			Level.Instance.OnLevelWon -= OnlevelEnd;
			Level.Instance.OnLevelLost -= OnlevelEnd;
			Level.Instance.OnLevelAbort -= OnlevelEnd;
		}

		public int get_max_apples()
		{
			return Konfiguration.GameConfig.AppleUpgradeCost[appleCollectLevel] + 20;
		}

		private IEnumerator GenerateApples()
		{
			while (true)
			{
				yield return new WaitForSeconds(appleCollectRate);
				if (Level.Instance.state == Level.State.playing || Level.Instance.state == Level.State.tutorial)
				{
					Apples++;
				}
			}
		}

		private void OnlevelEnd()
		{
			StopAllCoroutines();
		}

		private void OnLevelPlay()
		{
			StopAllCoroutines();
			StartCoroutine(GenerateApples());
		}

		private void OnLevelStart()
		{
			StopAllCoroutines();
			AppleCollectLevel = 0;
			Apples = 0;
			LevelCoins = 0;
			LevelXP = 0;
		}

		public bool getFriendGateDuration(int level_id, out TimeSpan time_span)
		{
			LevelData levelData = Konfiguration.GetLevelData(level_id);
			time_span = new TimeSpan(0, 0, levelData.FriendGateDurationInSeconds) - (DateTime.Now - playerData.friendGateStartTime.GetValueOrDefault());
			if (levelData.is_friend_gate)
			{
				DateTime? friendGateStartTime = playerData.friendGateStartTime;
				if (friendGateStartTime.HasValue)
				{
					return true;
				}
			}
			return false;
		}

		public void resetFriendGate()
		{
			playerData.friendGateStartTime = null;
			playerData.numberOfFriendGateHelpers = 0;
		}

		public void startFriendgateTimer()
		{
			playerData.friendGateStartTime = DateTime.Now;
		}

		public void addFriendGateHelper()
		{
			playerData.numberOfFriendGateHelpers++;
		}

		public void giveCoinsForTappingOnNotification()
		{
			if (!hasReceivedCoinsForTappingOnNotification)
			{
				Wallet.GiveCoins(Konfiguration.CoinBonusForTappingOnNotification);
			}
			playerData.hasReceivedCoinsForTappingOnNotification = true;
		}

		public void RefillRewardedVideos()
		{
			if (DateTime.UtcNow.Date > playerData.lastRewardedVideo.Date)
			{
				playerData.watchedRewardedVideos = 0;
				playerData.watchedRewardedVideosLifes = 0;
				playerData.watchedRewardedVideosGiftCoins = 0;
				playerData.watchedRewardedVideosCoinBoost = 0;
			}
		}

		public void WatchedRewardedVideo(RewardedVideoItems item)
		{
			playerData.lastRewardedVideo = DateTime.UtcNow;
			switch (item)
			{
			case RewardedVideoItems.Lives:
				playerData.watchedRewardedVideosLifes++;
				return;
			case RewardedVideoItems.Coins:
				if (App.State == App.States.Map)
				{
					playerData.watchedRewardedVideosGiftCoins++;
					return;
				}
				break;
			}
			if (item == RewardedVideoItems.Coins && App.State == App.States.Game)
			{
				playerData.watchedRewardedVideosCoinBoost++;
			}
			else
			{
				playerData.watchedRewardedVideos++;
			}
		}

		public bool canWatchRewardedVideo(RewardedVideoItems item)
		{
			if (App.State == App.States.Map && item == RewardedVideoItems.Coins)
			{
				return playerData.watchedRewardedVideosGiftCoins < RewardedVideosWrapper.Gift_coin_videos_per_day;
			}
			if (App.State == App.States.Game && item == RewardedVideoItems.Coins)
			{
				return playerData.watchedRewardedVideosCoinBoost < RewardedVideosWrapper.Coin_boosts_per_day;
			}
			if (item == RewardedVideoItems.Lives)
			{
				return playerData.watchedRewardedVideosLifes < RewardedVideosWrapper.Lives_videos_per_day;
			}
			return playerData.watchedRewardedVideos < RewardedVideosWrapper.Videos_per_day;
		}
	}
}
