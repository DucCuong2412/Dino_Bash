using System;
using System.Collections.Generic;
//using LeanplumSDK;
using UnityEngine;
using dinobash;
using mixpanel.platform;

public static class RewardedVideosWrapper
{
	private const int Lives_reward = 2;

	private const int MegaBall_reward = 1;

	private const int Blizzard_reward = 1;

	private const int MeteorStorm_reward = 1;

	private const float rewardedVideos_result_coin_boost_factor = 0.25f;

	private const bool rewardedVideos_hide_for_paying_users = true;

	private const int rewardedVideos_start_level = 5;

	private const int videos_per_day = 10;

	private const int lives_videos_per_day = 10;

	private const int lives_threshold = 0;

	private const bool show_only_on_loose = true;

	private const int coin_boosts_per_day = 10;

	private const int gift_coin_videos_per_day = 10;

	private const int lower_end_gift_range = 5;

	private const int upper_end_gift_range = 15;

	private static Dictionary<RewardedVideoItems, int> rewards = new Dictionary<RewardedVideoItems, int>();

	private static float var_rewardedVideos_result_coin_boost_factor;

	private static bool var_rewardedVideos_hide_for_paying_users;

	private static int var_rewardedVideos_start_level;

	private static int var_videos_per_day;

	private static int var_lives_videos_per_day;

	private static List<object> show_on_lost = new List<object> { 2, 4, 7, 10 };

	private static List<object> var_show_on_lost;

	private static int var_lives_threshold;

	private static bool var_show_only_on_loose;

	private static int var_coin_boosts_per_day;

	private static int var_gift_coin_videos_per_day;

	private static int var_lower_end_gift_range;

	private static int var_upper_end_gift_range;

	private static System.Random randomValue = new System.Random();

	private static int currenRandomValue = -1;

	private static string appKey = "41b3cce5";

	private static bool initalized = false;

	private static Action onDoneCallback;

	public static float CoinBoostFactor
	{
		get
		{
			if (var_rewardedVideos_result_coin_boost_factor != null)
			{
				return var_rewardedVideos_result_coin_boost_factor;
			}
			Debug.LogError("var_rewardedVideos_result_coin_boost_factor is null");
			return 0f;
		}
	}

	public static int RewardedVideos_start_level
	{
		get
		{
			return var_rewardedVideos_start_level;
		}
	}

	public static int Videos_per_day
	{
		get
		{
			return var_videos_per_day;
		}
	}

	public static int Lives_videos_per_day
	{
		get
		{
			return var_lives_videos_per_day;
		}
	}

	public static List<int> Show_on_lost//cuongnd
	{
		get
		{
			//int[] collection = LeanplumHelper.toIntArray(var_show_on_lost);
			return null; // LeanplumHelper.toList<int>(collection);
		}
	}

	public static int Lives_threshold
	{
		get
		{
			return var_lives_threshold;
		}
	}

	public static bool Show_only_on_loose
	{
		get
		{
			return var_show_only_on_loose;
		}
	}

	public static int Coin_boosts_per_day
	{
		get
		{
			return var_coin_boosts_per_day;
		}
	}

	public static int Gift_coin_videos_per_day
	{
		get
		{
			return var_gift_coin_videos_per_day;
		}
	}

	public static bool hasVideo
	{
		get
		{
			if (Player.MaxLevelID < RewardedVideos_start_level)
			{
				return false;
			}
			if (Wallet.IsPayingUser && var_rewardedVideos_hide_for_paying_users)
			{
				return false;
			}
			return initalized && Supersonic.Agent.isRewardedVideoAvailable();
		}
	}

	public static int getRewardAmount(RewardedVideoItems item)
	{
		if (item == RewardedVideoItems.Coins)
		{
			if (App.State == App.States.Map)
			{
				return getCoinGift();
			}
			if (App.State == App.States.Game)
			{
				return Mathf.RoundToInt((float)Player.Instance.LevelCoins * var_rewardedVideos_result_coin_boost_factor);
			}
		}
		if (rewards.ContainsKey(item))
		{
			return rewards[item];
		}
		return 0;
	}

	private static void NextRandomForGift()
	{
		currenRandomValue = randomValue.Next(var_lower_end_gift_range, var_upper_end_gift_range);
	}

	private static int getCoinGift()
	{
		int maxLevelID = Player.MaxLevelID;
		int num = 0;
		for (int num2 = maxLevelID; num2 >= 0; num2--)
		{
			if (Konfiguration.levels[maxLevelID].level_coins > 0)
			{
				num = Konfiguration.levels[maxLevelID].Level_coins + Konfiguration.levels[maxLevelID].Kill_coins;
				break;
			}
		}
		float f = (float)num * (0.01f * (float)currenRandomValue);
		return Mathf.RoundToInt(f);
	}

	public static void Init()
	{
		var_videos_per_day = 10;
		var_lives_videos_per_day = 10;
		var_rewardedVideos_start_level = 5;
		var_lives_threshold = 0;
		var_show_on_lost = show_on_lost;
		var_show_only_on_loose = true;
		var_rewardedVideos_hide_for_paying_users = true;
		var_rewardedVideos_result_coin_boost_factor = 0.25f;
		var_coin_boosts_per_day = 10;
		var_gift_coin_videos_per_day = 10;
		var_lower_end_gift_range = 5;
		var_upper_end_gift_range = 15;
		int value = 2;
		rewards.Add(RewardedVideoItems.Lives, value);
		int value2 = 1;
		rewards.Add(RewardedVideoItems.MegaBall, value2);
		int value3 = 1;
		rewards.Add(RewardedVideoItems.Blizzard, value3);
		int	 value4 = 1;
		rewards.Add(RewardedVideoItems.MeteorStorm, value4);
		NextRandomForGift();
		GameObject gameObject = new GameObject("SupersonicEvents");
		gameObject.AddComponent<SupersonicEvents>();
		string distinct_id = MixpanelUnityPlatform.get_distinct_id();
		Supersonic.Agent.start();
		SupersonicConfig.Instance.setClientSideCallbacks(true);
		Supersonic.Agent.initRewardedVideo(appKey, distinct_id);
		SupersonicEvents.onRewardedVideoInitSuccessEvent += RewardedVideoInitSuccessEvent;
		SupersonicEvents.onRewardedVideoInitFailEvent += HandleonRewardedVideoInitFailEvent;
		SupersonicEvents.onRewardedVideoAdRewardedEvent += HandleonRewardedVideoAdRewardedEvent;
		SupersonicEvents.onRewardedVideoAdClosedEvent += HandleonRewardedVideoAdClosedEvent;
		SupersonicEvents.onVideoStartEvent += HandleonVideoStartEvent;
		SupersonicEvents.onVideoEndEvent += HandleonRewardedVideoAdClosedEvent;
		Debug.Log("SuperSonic init");
		initalized = true;
	}

	public static void Pause()
	{
		if (initalized)
		{
			Supersonic.Agent.onPause();
		}
	}

	public static void Resume()
	{
		if (initalized)
		{
			Supersonic.Agent.onResume();
		}
	}

	private static void HandleonRewardedVideoAdRewardedEvent(SupersonicPlacement placement)
	{
		if (!initalized)
		{
			return;
		}
		try
		{
			Debug.Log("SuperSonic placement:" + placement.getPlacementName());
			RewardedVideoItems rewardedVideoItems = (RewardedVideoItems)(int)Enum.Parse(typeof(RewardedVideoItems), placement.getRewardName());
			Debug.Log("SuperSonic reward:" + rewardedVideoItems);
			int amount = handleReward(rewardedVideoItems);
			RewardedVideoPopup screen = ScreenManager.GetScreen<RewardedVideoPopup>();
			screen.Show(rewardedVideoItems, amount);
			Player.Instance.WatchedRewardedVideo(rewardedVideoItems);
			Tracking.rewarded_video(Player.CurrentLevelID, Tracking.RewardedVideoAction.watched, rewardedVideoItems, amount);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception: " + ex.Message);
		}
	}

	public static int handleReward(RewardedVideoItems reward)
	{
		int rewardAmount = getRewardAmount(reward);
		Debug.Log("SuperSonic amount:" + rewardAmount);
		switch (reward)
		{
		case RewardedVideoItems.Lives:
			Player.Lives += rewardAmount;
			break;
		case RewardedVideoItems.Coins:
			Wallet.GiveCoins(getRewardAmount(reward));
			if (App.State == App.States.Map)
			{
				NextRandomForGift();
			}
			break;
		case RewardedVideoItems.XP:
			Player.XP += rewardAmount;
			break;
		default:
			if (Konfiguration.isConsumable((UnitType)reward))
			{
				Player.changeConsumableCount((UnitType)reward, rewardAmount);
				break;
			}
			Debug.LogError("no valid reward! " + reward);
			return -1;
		}
		return rewardAmount;
	}

	private static void HandleonVideoStartEvent()
	{
		Debug.Log("SuperSonic VideoAD start");
	}

	private static void HandleonRewardedVideoAdClosedEvent()
	{
		Debug.Log("SuperSonic VideoAD closed");
		if (onDoneCallback != null)
		{
			onDoneCallback();
		}
		onDoneCallback = null;
	}

	private static void RewardedVideoInitSuccessEvent()
	{
		Debug.Log("SuperSonic init complete");
	}

	public static bool ShowVideo(RewardedVideoItems reward, Action onDone)
	{
		if (!initalized)
		{
			return false;
		}
		if (hasVideo)
		{
			Debug.Log("SuperSonic show VideoAD: " + reward);
			onDoneCallback = onDone;
			Supersonic.Agent.showRewardedVideo(reward.ToString());
			return true;
		}
		Debug.Log("SuperSonic has no ad ready!");
		return false;
	}

	private static void HandleonRewardedVideoInitFailEvent(SupersonicError error)
	{
		Debug.Log("SuperSonic init errror, code :  " + error.getCode() + ", description : " + error.getDescription());
	}
}
