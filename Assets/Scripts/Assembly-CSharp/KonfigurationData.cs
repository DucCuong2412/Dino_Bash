using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LeanplumSDK;
using UnityEngine;
using dinobash;

public class KonfigurationData
{
	public readonly float levelstart = -512f;

	public int Start_Coins;

	public int Start_Diamonds;

	[XmlIgnore]
	private Var<List<object>> var_appleCollectRate;

	public float[] AppleCollectRate;

	[XmlIgnore]
	private Var<List<object>> var_appleUpgradeCost;

	public int[] AppleUpgradeCost;

	public int AppleCollectReward;

	public float AppleCollectTimer;

	public int appleBoostAmount = 25;

	[XmlIgnore]
	private Var<int> var_appleBoostAmount;

	public int consumable_unlock_fill = 3;

	[XmlIgnore]
	private Var<int> var_consumable_unlock_fill;

	public SerializableDictionary<UnitType, int> ConsumableRefillValues;

	[XmlIgnore]
	private Dictionary<UnitType, Var<int>> var_ConsumableRefillValues = new Dictionary<UnitType, Var<int>>();

	public int CoinBonusForTappingOnNotification = 5000;

	public string terms_of_use_url = "http://www.tilting-point.com/terms-of-use/";

	public string privacy_policy_url = "http://www.tilting-point.com/privacy-policy/";

	public string faq_url = "http://www.pokokostudio.com/faq/";

	private bool use_skip_intro;

	[XmlIgnore]
	private Var<bool> var_use_skip_intro;

	public bool use_dragshot_feature = true;

	[XmlIgnore]
	private Var<bool> var_use_dragshot_feature;

	public bool use_upgrade_timers = true;

	[XmlIgnore]
	private Var<bool> var_use_upgrade_timers;

	public bool use_upgrade_locks;

	[XmlIgnore]
	private Var<bool> var_use_upgrade_locks;

	private bool use_temp_upgrades;

	[XmlIgnore]
	private Var<bool> var_use_temp_upgrades;

	private bool use_ShotSlot_upsell = true;

	[XmlIgnore]
	private Var<bool> var_use_ShotSlot_upsell;

	[XmlIgnore]
	private Var<int> var_maxClientLevel;

	public bool show_update_prompt;

	[XmlIgnore]
	private Var<bool> var_show_app_update_prompt;

	public bool force_app_update;

	[XmlIgnore]
	private Var<bool> var_force_app_update;

	[XmlIgnore]
	private int facebook_connect_interval = 3;

	[XmlIgnore]
	private Var<int> var_facebook_connect_interval;

	[XmlIgnore]
	private int facebook_connect_reward = 1000;

	[XmlIgnore]
	private Var<int> var_facebook_connect_reward;

	public int hours_to_next_quest = 24;

	[XmlIgnore]
	private Var<int> var_hours_to_next_quest;

	public int first_quest_level = 7;

	[XmlIgnore]
	private Var<int> var_first_quest_level;

	public int special_offer_start_level = 16;

	[XmlIgnore]
	private Var<int> var_special_offer_start_level;

	public int special_offer_loss_count;

	[XmlIgnore]
	private Var<int> var_special_offer_loss_count;

	public float specialOfferSaveRate = 0.5f;

	[XmlIgnore]
	private Var<float> var_specialOfferSaveRate;

	public int friendGateDiamondCost;

	[XmlIgnore]
	private Var<int> var_friendGateDiamondCost;

	public int refillLivesDiamondCost;

	[XmlIgnore]
	private Var<int> var_refillLivesDiamondCost;

	public int lifeRefillIntervall = 1800;

	[XmlIgnore]
	private Var<int> var_lifeRefillIntervall;

	public float neanderRushInterval = 8f;

	[XmlIgnore]
	private Var<float> var_neanderRushInterval;

	public int neanderRushUnits = 4;

	[XmlIgnore]
	private Var<int> var_neanderRushUnits;

	public float replayFactor = 0.05f;

	[XmlIgnore]
	private Var<float> var_replayFactor;

	public float win_loss_influce = 0.05f;

	[XmlIgnore]
	private Var<float> var_win_loss_influce;

	public int win_loss_maxCount = 6;

	[XmlIgnore]
	private Var<int> var_win_loss_maxCount;

	private int level_grind_spread = 3;

	[XmlIgnore]
	private Var<int> var_level_grind_spread;

	private float level_grind_factor = 0.3f;

	[XmlIgnore]
	private Var<float> var_level_grind_factor;

	public float dino_rage_threshold = 0.75f;

	[XmlIgnore]
	private Var<float> var_dino_rage_threshold;

	public int dino_rage_cost = 5;

	[XmlIgnore]
	private Var<int> var_dino_rage_cost;

	public int dino_rage_start_level = 18;

	[XmlIgnore]
	private Var<int> var_dino_rage_start_level;

	public float dino_rage_duration = 12f;

	[XmlIgnore]
	private Var<float> var_dino_rage_duration;

	public int dino_rage_apples = 50;

	[XmlIgnore]
	private Var<int> var_dino_rage_apples;

	public int dino_rage_loose_count = 2;

	[XmlIgnore]
	private Var<int> var_dino_rage_loose_count;

	public int show_rate_prompt_level = 6;

	[XmlIgnore]
	private Var<int> var_show_rate_prompt_level;

	public int show_rate_prompt_frequency = 4;

	[XmlIgnore]
	private Var<int> var_show_rate_prompt_frequency;

	public string iOSPre7RateURL = "itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=895462868";

	[XmlIgnore]
	private Var<string> var_iOSPre7RateURL;

	public string iOSPost7RateURL = "itms-apps://itunes.apple.com/app/id895462868";

	[XmlIgnore]
	private Var<string> var_iOSPost7RateURL;

	public string googlePlayRateURL = "market://details?id=com.pokokostudio.dinobash";

	[XmlIgnore]
	private Var<string> var_googlePlayRateURL;

	[XmlIgnore]
	private string moregamesURL_iOS = "http://spot.tilting-point.com/dinobash-ios/?idfa=";

	[XmlIgnore]
	private Var<string> var_moregamesURL_iOS;

	[XmlIgnore]
	private string moregamesURL_Android = "http://spot.tilting-point.com/dinobash-android/?&andi=";

	[XmlIgnore]
	private Var<string> var_moregamesURL_Android;

	[XmlIgnore]
	private string verfiy_url_android = "https://gameservices.dinobash.com/iapvalidate/android/";

	[XmlIgnore]
	private Var<string> var_verfiy_url_android;

	[XmlIgnore]
	private string verfiy_url_ios = "https://gameservices.dinobash.com/iapvalidate/ios/";

	[XmlIgnore]
	private Var<string> var_verfiy_url_ios;

	public int AppleBoostAmount
	{
		get
		{
			return var_appleBoostAmount.Value;
		}
	}

	public bool Use_skip_intro
	{
		get
		{
			return var_use_skip_intro.Value;
		}
	}

	public bool Use_dragshot_feature
	{
		get
		{
			return var_use_dragshot_feature.Value;
		}
	}

	public bool Use_upgrade_timers
	{
		get
		{
			return var_use_upgrade_timers.Value;
		}
	}

	public bool Use_upgrade_locks
	{
		get
		{
			if (Use_upgrade_timers)
			{
				return var_use_upgrade_locks.Value;
			}
			return false;
		}
	}

	public bool Use_temp_upgrades
	{
		get
		{
			return var_use_temp_upgrades.Value;
		}
	}

	public bool Use_ShotSlot_upsell
	{
		get
		{
			return var_use_ShotSlot_upsell.Value;
		}
	}

	public int MaxClientLevel
	{
		get
		{
			return var_maxClientLevel.Value;
		}
	}

	public bool Show_update_prompt
	{
		get
		{
			return var_show_app_update_prompt.Value;
		}
	}

	public bool Force_App_Update
	{
		get
		{
			return var_force_app_update.Value;
		}
	}

	public int Facebook_connect_interval
	{
		get
		{
			return var_facebook_connect_interval.Value;
		}
	}

	public int Facebook_connect_reward
	{
		get
		{
			return Mathf.RoundToInt((float)var_facebook_connect_reward.Value * Konfiguration.GetChapterData(Player.MaxLevelID).shop_amount_multiplier);
		}
	}

	public int Hours_to_next_quest
	{
		get
		{
			return var_hours_to_next_quest.Value;
		}
	}

	public int First_quest_level
	{
		get
		{
			return var_first_quest_level.Value;
		}
	}

	public int Special_offer_start_level
	{
		get
		{
			return var_special_offer_start_level.Value;
		}
	}

	public int Special_offer_loss_count
	{
		get
		{
			return var_special_offer_loss_count.Value;
		}
	}

	public float SpecialOfferSaveRate
	{
		get
		{
			return var_specialOfferSaveRate.Value;
		}
	}

	public int FriendGateDiamondCost
	{
		get
		{
			return var_friendGateDiamondCost.Value;
		}
	}

	public int RefillLivesDiamondCost
	{
		get
		{
			return var_refillLivesDiamondCost.Value;
		}
	}

	public int LifeRefillIntervall
	{
		get
		{
			return var_lifeRefillIntervall.Value;
		}
	}

	public float NeanderRushInterval
	{
		get
		{
			return var_neanderRushInterval.Value;
		}
	}

	public int NeanderRushUnits
	{
		get
		{
			return var_neanderRushUnits.Value;
		}
	}

	public float ReplayFactor
	{
		get
		{
			return var_replayFactor.Value;
		}
	}

	public float Win_loss_influce
	{
		get
		{
			return var_win_loss_influce.Value;
		}
	}

	public int Win_loss_maxCount
	{
		get
		{
			return var_win_loss_maxCount.Value;
		}
	}

	public int Level_grind_spread
	{
		get
		{
			return var_level_grind_spread.Value;
		}
	}

	public float Level_grind_factor
	{
		get
		{
			return var_level_grind_factor.Value;
		}
	}

	public float Dino_rage_threshold
	{
		get
		{
			return var_dino_rage_threshold.Value;
		}
	}

	public int Dino_rage_cost
	{
		get
		{
			return var_dino_rage_cost.Value;
		}
	}

	public int Dino_rage_start_level
	{
		get
		{
			return var_dino_rage_start_level.Value;
		}
	}

	public float Dino_rage_duration
	{
		get
		{
			return var_dino_rage_duration.Value;
		}
	}

	public int Dino_rage_apples
	{
		get
		{
			return var_dino_rage_apples.Value;
		}
	}

	public int Dino_rage_loose_count
	{
		get
		{
			return var_dino_rage_loose_count.Value;
		}
	}

	public int Show_rate_prompt_level
	{
		get
		{
			return var_show_rate_prompt_level.Value;
		}
	}

	public int Show_rate_prompt_frequency
	{
		get
		{
			return var_show_rate_prompt_frequency.Value;
		}
	}

	public string IOSPre7RateURL
	{
		get
		{
			return var_iOSPre7RateURL.Value;
		}
	}

	public string IOSPost7RateURL
	{
		get
		{
			return var_iOSPost7RateURL.Value;
		}
	}

	public string GooglePlayRateURL
	{
		get
		{
			return var_googlePlayRateURL.Value;
		}
	}

	public string MoregamesURL_iOS
	{
		get
		{
			return var_moregamesURL_iOS.Value;
		}
	}

	public string MoregamesURL_Android
	{
		get
		{
			return var_moregamesURL_Android.Value;
		}
	}

	public string VERIFY_URL_ANDROID
	{
		get
		{
			return var_verfiy_url_android.Value;
		}
	}

	public string VERIFY_URL_IOS
	{
		get
		{
			return var_verfiy_url_ios.Value;
		}
	}

	public void init()
	{
		var_use_skip_intro = Var.Define("feature.skip_intro_enabled", use_skip_intro);
		var_use_dragshot_feature = Var.Define("feature.dragshot_enabled", use_dragshot_feature);
		var_use_upgrade_timers = Var.Define("feature.use_upgrade_timers", use_upgrade_timers);
		var_use_upgrade_locks = Var.Define("feature.use_upgrade_locks", use_upgrade_locks);
		var_use_temp_upgrades = Var.Define("feature.use_temp_upgrades", use_temp_upgrades);
		var_use_ShotSlot_upsell = Var.Define("feature.use_ShotSlot_upsell", use_ShotSlot_upsell);
		var_maxClientLevel = Var.Define("game.maxClientLevel", 81);
		var_show_app_update_prompt = Var.Define("game.show_update_prompt", show_update_prompt);
		var_force_app_update = Var.Define("game.force_app_update", force_app_update);
		var_hours_to_next_quest = Var.Define("quest.hours_to_next_quest", hours_to_next_quest);
		var_first_quest_level = Var.Define("quest.first_quest_level", first_quest_level);
		var_special_offer_start_level = Var.Define("special_offer.start_level", special_offer_start_level);
		var_special_offer_loss_count = Var.Define("special_offer.loss_count", special_offer_loss_count);
		var_specialOfferSaveRate = Var.Define("special_offer.save_rate", specialOfferSaveRate);
		var_facebook_connect_reward = Var.Define("facebook.login_reward", facebook_connect_reward);
		var_facebook_connect_interval = Var.Define("facebook.show_reward_login_interval", facebook_connect_interval);
		var_friendGateDiamondCost = Var.Define("friendgate.DiamondCost", friendGateDiamondCost);
		var_refillLivesDiamondCost = Var.Define("lives.refill_cost", refillLivesDiamondCost);
		var_lifeRefillIntervall = Var.Define("lives.refill_interval_in_seconds", lifeRefillIntervall);
		var_neanderRushUnits = Var.Define("level.CavemanRushUnits", neanderRushUnits);
		var_neanderRushInterval = Var.Define("level.CavemanRushInterval", neanderRushInterval);
		var_level_grind_factor = Var.Define("level.grind_factor", level_grind_factor);
		var_level_grind_spread = Var.Define("level.grind_spread", level_grind_spread);
		var_replayFactor = Var.Define("level.replayFactor", replayFactor);
		var_win_loss_influce = Var.Define("level.win_loss_influce", win_loss_influce);
		var_win_loss_maxCount = Var.Define("level.win_loss_maxCount", win_loss_maxCount);
		var_appleBoostAmount = Var.Define("level.appleBoostAmount", appleBoostAmount);
		var_appleCollectRate = Var.Define("level.appleCollectRate", LeanplumHelper.toListObject(AppleCollectRate));
		var_appleUpgradeCost = Var.Define("level.appleUpgradeCost", LeanplumHelper.toListObject(AppleUpgradeCost));
		var_consumable_unlock_fill = Var.Define("level.consumable_unlock_fill", consumable_unlock_fill);
		foreach (KeyValuePair<UnitType, int> consumableRefillValue in ConsumableRefillValues)
		{
			string name = string.Format("level.consumable_refill_values.{0}.", consumableRefillValue.Key);
			Var<int> value = Var.Define(name, consumableRefillValue.Value);
			var_ConsumableRefillValues.Add(consumableRefillValue.Key, value);
		}
		var_dino_rage_threshold = Var.Define("dinorage.dino_rage_threshold", dino_rage_threshold);
		var_dino_rage_cost = Var.Define("dinorage.dino_rage_cost", dino_rage_cost);
		var_dino_rage_start_level = Var.Define("dinorage.dino_rage_start_level", dino_rage_start_level);
		var_dino_rage_duration = Var.Define("dinorage.dino_rage_duration", dino_rage_duration);
		var_dino_rage_apples = Var.Define("dinorage.dino_rage_apples", dino_rage_apples);
		var_dino_rage_loose_count = Var.Define("dinorage.dino_rage_loose_count", dino_rage_loose_count);
		var_show_rate_prompt_level = Var.Define("rate_prompt.show_rate_prompt_level", show_rate_prompt_level);
		var_show_rate_prompt_frequency = Var.Define("rate_prompt.popup_frequency", show_rate_prompt_frequency);
		var_iOSPre7RateURL = Var.Define("rate_prompt.url_ios_pre7", iOSPre7RateURL);
		var_iOSPost7RateURL = Var.Define("rate_prompt.url_ios_post7", iOSPost7RateURL);
		var_googlePlayRateURL = Var.Define("rate_prompt.url_google_play", googlePlayRateURL);
		var_moregamesURL_iOS = Var.Define("more_games.ios_url", moregamesURL_iOS);
		var_moregamesURL_Android = Var.Define("more_games.android_url", moregamesURL_Android);
		var_verfiy_url_android = Var.Define("iap_validate.android_url", verfiy_url_android);
		var_verfiy_url_ios = Var.Define("iap_validate.ios_url", verfiy_url_ios);
		Leanplum.VariablesChanged += HandleVariablesChanged;
	}

	private void HandleVariablesChanged()
	{
		try
		{
			AppleCollectRate = LeanplumHelper.toFloatArray(var_appleCollectRate);
			AppleUpgradeCost = LeanplumHelper.toIntArray(var_appleUpgradeCost);
			consumable_unlock_fill = var_consumable_unlock_fill.Value;
			foreach (KeyValuePair<UnitType, Var<int>> var_ConsumableRefillValue in var_ConsumableRefillValues)
			{
				if (ConsumableRefillValues.ContainsKey(var_ConsumableRefillValue.Key))
				{
					ConsumableRefillValues[var_ConsumableRefillValue.Key] = var_ConsumableRefillValue.Value;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message + "\n" + ex.StackTrace);
		}
	}
}
