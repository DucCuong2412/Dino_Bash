using System;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class PlayerData
{
	[Serializable]
	public class AchievementStates : SerializableList<SocialGamingManager.AchievementState>
	{
	}

	public const int version = 2;

	[XmlAttribute("SaveGameVersion")]
	public int saveGameVersion = 1;

	public int splitTestingRandomId = UnityEngine.Random.Range(0, 1000);

	public string splitTestingGroupName = App.VERSION_CODE;

	public string splitTestingDataSet = string.Empty;

	public DateTime time_of_install = DateTime.Today;

	public DateTime timeOfLostLive = DateTime.UtcNow;

	public bool timeManipulationPenalty;

	public DateTime? friendGateStartTime;

	public int numberOfFriendGateHelpers;

	public bool shouldShowRatePrompt = true;

	public int Lives = 5;

	public int XP;

	public SerializableDictionary<UnitType, int> consumables = new SerializableDictionary<UnitType, int>();

	public int CurrentLevelID;

	public int MaxLevelID;

	public bool[] CollectableDropProgress = new bool[0];

	public bool hasPlayedMaxLevel;

	public int looseCount;

	public int totalLooseCount;

	public int winCount;

	public int session;

	public int total_coins_collected;

	public bool bird_focus_on_visible_neanders;

	public SerializableDictionary<string, float> endless_level_times = new SerializableDictionary<string, float>();

	public SerializableList<string> tutorial_progress = new SerializableList<string>();

	public bool watched_upgrade_tutorial;

	public bool watched_survivalmission_tutorial;

	public bool[] user_has_seen_upgrade_tabs = new bool[3];

	public SerializableList<UnitType> discovered_neanders = new SerializableList<UnitType>();

	public SerializableList<ShotType> newShotTypes = new SerializableList<ShotType>();

	public SerializableList<UnitType> newUnitTypes = new SerializableList<UnitType>();

	public SerializableList<ShotType> availiableShotTypes = new SerializableList<ShotType>();

	public SerializableList<UnitType> availiableUnitTypes = new SerializableList<UnitType>();

	public SerializableDictionary<UnitType, int> unitLevels = new SerializableDictionary<UnitType, int>();

	public SerializableList<UnitType> selectedUnitTypes = new SerializableList<UnitType>();

	public SerializableList<ShotType> selectedShotTypes = new SerializableList<ShotType>();

	public SerializableDictionary<ShotType, int> shotLevels = new SerializableDictionary<ShotType, int>();

	public SerializableList<UnitType> availiableUpgrades = new SerializableList<UnitType>();

	public SerializableList<UnitType> activeUpgrades = new SerializableList<UnitType>();

	public SerializableDictionary<UnitType, bool> activeUpgradesState = new SerializableDictionary<UnitType, bool>();

	public bool hasReceivedCoinsForTappingOnNotification;

	public DateTime lastTimeInApp = DateTime.UtcNow;

	public AchievementStates achievementStates = new AchievementStates();

	public bool wasLoggedIntoSocialGamingProvider = true;

	public bool userRecievedLoginReward;

	public DateTime lastQuestCreated = DateTime.Now;

	public SerializableList<Quest> activeQuests = new SerializableList<Quest>();

	public SerializableList<TimerItem> temp_unlocks = new SerializableList<TimerItem>();

	public SerializableList<TimerItem> upgrade_queue = new SerializableList<TimerItem>();

	public SerializableList<UnitType> newly_trained_dinos = new SerializableList<UnitType>();

	public DateTime bundleOfferEnd = DateTime.UtcNow;

	public string bundle_id = "default";

	public bool offer_bundle;

	public DateTime promotionEnd = DateTime.UtcNow;

	public string promotion_id = "default";

	public bool price_promo;

	public bool quantity_promo;

	public DateTime lastRewardedVideo = DateTime.UtcNow;

	public int watchedRewardedVideos;

	public int watchedRewardedVideosLifes;

	public int watchedRewardedVideosCoinBoost;

	public int watchedRewardedVideosGiftCoins;
}
