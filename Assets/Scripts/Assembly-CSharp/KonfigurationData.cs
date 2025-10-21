using System;
using System.Collections.Generic;
using System.Xml.Serialization;
//using LeanplumSDK;
using UnityEngine;
using dinobash;

public class KonfigurationData
{
	public readonly float levelstart = -512f;

	public int Start_Coins;

	public int Start_Diamonds;

	[XmlIgnore]
	private List<object> var_appleCollectRate;

	public float[] AppleCollectRate;

	[XmlIgnore]
	private List<object> var_appleUpgradeCost;

	public int[] AppleUpgradeCost;

	public int AppleCollectReward;

	public float AppleCollectTimer;

	public int appleBoostAmount = 25;

	[XmlIgnore]
	private int var_appleBoostAmount;

	public int consumable_unlock_fill = 3;

	[XmlIgnore]
	private int var_consumable_unlock_fill;

	public SerializableDictionary<UnitType, int> ConsumableRefillValues;

	[XmlIgnore]
	private Dictionary<UnitType, int> var_ConsumableRefillValues = new Dictionary<UnitType, int>();

	public int CoinBonusForTappingOnNotification = 5000;

	public string terms_of_use_url = "http://www.tilting-point.com/terms-of-use/";

	public string privacy_policy_url = "http://www.tilting-point.com/privacy-policy/";

	public string faq_url = "http://www.pokokostudio.com/faq/";

	private bool use_skip_intro;

	[XmlIgnore]
	private bool var_use_skip_intro;

	public bool use_dragshot_feature = true;

	[XmlIgnore]
	private bool var_use_dragshot_feature;

	public bool use_upgrade_timers = true;

	[XmlIgnore]
	private bool var_use_upgrade_timers;

	public bool use_upgrade_locks;

	[XmlIgnore]
	private bool var_use_upgrade_locks;

	private bool use_temp_upgrades;

	[XmlIgnore]
	private bool var_use_temp_upgrades;

	private bool use_ShotSlot_upsell = true;

	[XmlIgnore]
	private bool var_use_ShotSlot_upsell;

	[XmlIgnore]
	private int var_maxClientLevel;

	public bool show_update_prompt;

	[XmlIgnore]
	private bool var_show_app_update_prompt;

	public bool force_app_update;

	[XmlIgnore]
	private bool var_force_app_update;

	[XmlIgnore]
	private int facebook_connect_interval = 3;

	[XmlIgnore]
	private int var_facebook_connect_interval;

	[XmlIgnore]
	private int facebook_connect_reward = 1000;

	[XmlIgnore]
	private int var_facebook_connect_reward;

	public int hours_to_next_quest = 24;

	[XmlIgnore]
	private int var_hours_to_next_quest;

	public int first_quest_level = 7;

	[XmlIgnore]
	private int var_first_quest_level;

	public int special_offer_start_level = 16;

	[XmlIgnore]
	private int var_special_offer_start_level;

	public int special_offer_loss_count;

	[XmlIgnore]
	private int var_special_offer_loss_count;

	public float specialOfferSaveRate = 0.5f;

	[XmlIgnore]
	private float var_specialOfferSaveRate;

	public int friendGateDiamondCost;

	[XmlIgnore]
	private int var_friendGateDiamondCost;

	public int refillLivesDiamondCost;

	[XmlIgnore]
	private int var_refillLivesDiamondCost;

	public int lifeRefillIntervall = 1800;

	[XmlIgnore]
	private int var_lifeRefillIntervall;

	public float neanderRushInterval = 8f;

	[XmlIgnore]
	private float var_neanderRushInterval;

	public int neanderRushUnits = 4;

	[XmlIgnore]
	private int var_neanderRushUnits;

	public float replayFactor = 0.05f;

	[XmlIgnore]
	private float var_replayFactor;

	public float win_loss_influce = 0.05f;

	[XmlIgnore]
	private float var_win_loss_influce;

	public int win_loss_maxCount = 6;

	[XmlIgnore]
	private int var_win_loss_maxCount;

	private int level_grind_spread = 3;

	[XmlIgnore]
	private int var_level_grind_spread;

	private float level_grind_factor = 0.3f;

	[XmlIgnore]
	private float var_level_grind_factor;

	public float dino_rage_threshold = 0.75f;

	[XmlIgnore]
	private float var_dino_rage_threshold;

	public int dino_rage_cost = 5;

	[XmlIgnore]
	private int var_dino_rage_cost;

	public int dino_rage_start_level = 18;

	[XmlIgnore]
	private int var_dino_rage_start_level;

	public float dino_rage_duration = 12f;

	[XmlIgnore]
	private float var_dino_rage_duration;

	public int dino_rage_apples = 50;

	[XmlIgnore]
	private int var_dino_rage_apples;

	public int dino_rage_loose_count = 2;

	[XmlIgnore]
	private int var_dino_rage_loose_count;

	public int show_rate_prompt_level = 6;

	[XmlIgnore]
	private int var_show_rate_prompt_level;

	public int show_rate_prompt_frequency = 4;

	[XmlIgnore]
	private int var_show_rate_prompt_frequency;

	public string iOSPre7RateURL = "itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=895462868";

	[XmlIgnore]
	private string var_iOSPre7RateURL;

	public string iOSPost7RateURL = "itms-apps://itunes.apple.com/app/id895462868";

	[XmlIgnore]
	private string var_iOSPost7RateURL;

	public string googlePlayRateURL = "market://details?id=com.pokokostudio.dinobash";

	[XmlIgnore]
	private string var_googlePlayRateURL;

	[XmlIgnore]
	private string moregamesURL_iOS = "http://spot.tilting-point.com/dinobash-ios/?idfa=";

	[XmlIgnore]
	private string var_moregamesURL_iOS;

	[XmlIgnore]
	private string moregamesURL_Android = "http://spot.tilting-point.com/dinobash-android/?&andi=";

	[XmlIgnore]
	private string var_moregamesURL_Android;

	[XmlIgnore]
	private string verfiy_url_android = "https://gameservices.dinobash.com/iapvalidate/android/";

	[XmlIgnore]
	private string var_verfiy_url_android;

	[XmlIgnore]
	private string verfiy_url_ios = "https://gameservices.dinobash.com/iapvalidate/ios/";

	[XmlIgnore]
	private string var_verfiy_url_ios;

	public int AppleBoostAmount
	{
		get
		{
			return var_appleBoostAmount;
		}
	}

	public bool Use_skip_intro
	{
		get
		{
			return var_use_skip_intro;
		}
	}

	public bool Use_dragshot_feature
	{
		get
		{
			return var_use_dragshot_feature;
		}
	}

	public bool Use_upgrade_timers
	{
		get
		{
			return var_use_upgrade_timers;
		}
	}

	public bool Use_upgrade_locks
	{
		get
		{
			if (Use_upgrade_timers)
			{
				return var_use_upgrade_locks;
			}
			return false;
		}
	}

	public bool Use_temp_upgrades
	{
		get
		{
			return var_use_temp_upgrades;
		}
	}

	public bool Use_ShotSlot_upsell
	{
		get
		{
			return var_use_ShotSlot_upsell;
		}
	}

	public int MaxClientLevel
	{
		get
		{
			return var_maxClientLevel;
		}
	}

	public bool Show_update_prompt
	{
		get
		{
			return var_show_app_update_prompt;
		}
	}

	public bool Force_App_Update
	{
		get
		{
			return var_force_app_update;
		}
	}

	public int Facebook_connect_interval
	{
		get
		{
			return var_facebook_connect_interval;
		}
	}

	public int Facebook_connect_reward
	{
		get
		{
			return Mathf.RoundToInt((float)var_facebook_connect_reward * Konfiguration.GetChapterData(Player.MaxLevelID).shop_amount_multiplier);
		}
	}

	public int Hours_to_next_quest
	{
		get
		{
			return var_hours_to_next_quest;
		}
	}

	public int First_quest_level
	{
		get
		{
			return var_first_quest_level;
		}
	}

	public int Special_offer_start_level
	{
		get
		{
			return var_special_offer_start_level;
		}
	}

	public int Special_offer_loss_count
	{
		get
		{
			return var_special_offer_loss_count;
		}
	}

	public float SpecialOfferSaveRate
	{
		get
		{
			return var_specialOfferSaveRate;
		}
	}

	public int FriendGateDiamondCost
	{
		get
		{
			return var_friendGateDiamondCost;
		}
	}

	public int RefillLivesDiamondCost
	{
		get
		{
			return var_refillLivesDiamondCost;
		}
	}

	public int LifeRefillIntervall
	{
		get
		{
			return var_lifeRefillIntervall;
		}
	}

	public float NeanderRushInterval
	{
		get
		{
			return var_neanderRushInterval;
		}
	}

	public int NeanderRushUnits
	{
		get
		{
			return var_neanderRushUnits;
		}
	}

	public float ReplayFactor
	{
		get
		{
			return var_replayFactor;
		}
	}

	public float Win_loss_influce
	{
		get
		{
			return var_win_loss_influce;
		}
	}

	public int Win_loss_maxCount
	{
		get
		{
			return var_win_loss_maxCount;
		}
	}

	public int Level_grind_spread
	{
		get
		{
			return var_level_grind_spread;
		}
	}

	public float Level_grind_factor
	{
		get
		{
			return var_level_grind_factor;
		}
	}

	public float Dino_rage_threshold
	{
		get
		{
			return var_dino_rage_threshold;
		}
	}

	public int Dino_rage_cost
	{
		get
		{
			return var_dino_rage_cost;
		}
	}

	public int Dino_rage_start_level
	{
		get
		{
			return var_dino_rage_start_level;
		}
	}

	public float Dino_rage_duration
	{
		get
		{
			return var_dino_rage_duration;
		}
	}

	public int Dino_rage_apples
	{
		get
		{
			return var_dino_rage_apples;
		}
	}

	public int Dino_rage_loose_count
	{
		get
		{
			return var_dino_rage_loose_count;
		}
	}

	public int Show_rate_prompt_level
	{
		get
		{
			return var_show_rate_prompt_level;
		}
	}

	public int Show_rate_prompt_frequency
	{
		get
		{
			return var_show_rate_prompt_frequency;
		}
	}

	public string IOSPre7RateURL
	{
		get
		{
			return var_iOSPre7RateURL;
		}
	}

	public string IOSPost7RateURL
	{
		get
		{
			return var_iOSPost7RateURL;
		}
	}

	public string GooglePlayRateURL
	{
		get
		{
			return var_googlePlayRateURL;
		}
	}

	public string MoregamesURL_iOS
	{
		get
		{
			return var_moregamesURL_iOS;
		}
	}

	public string MoregamesURL_Android
	{
		get
		{
			return var_moregamesURL_Android;
		}
	}

	public string VERIFY_URL_ANDROID
	{
		get
		{
			return var_verfiy_url_android;
		}
	}

	public string VERIFY_URL_IOS
	{
		get
		{
			return var_verfiy_url_ios;
		}
	}

    public void init()
    {
        var_use_skip_intro = use_skip_intro;
        var_use_dragshot_feature = use_dragshot_feature;
        var_use_upgrade_timers = use_upgrade_timers;
        var_use_upgrade_locks = use_upgrade_locks;
        var_use_temp_upgrades = use_temp_upgrades;
        var_use_ShotSlot_upsell = use_ShotSlot_upsell;
        var_maxClientLevel = 81;
        var_show_app_update_prompt = show_update_prompt;
        var_force_app_update = force_app_update;
        var_hours_to_next_quest = hours_to_next_quest;
        var_first_quest_level = first_quest_level;
        var_special_offer_start_level = special_offer_start_level;
        var_special_offer_loss_count = special_offer_loss_count;
        var_specialOfferSaveRate = specialOfferSaveRate;
        var_facebook_connect_reward = facebook_connect_reward;
        var_facebook_connect_interval = facebook_connect_interval;
        var_friendGateDiamondCost = friendGateDiamondCost;
        var_refillLivesDiamondCost = refillLivesDiamondCost;
        var_lifeRefillIntervall = lifeRefillIntervall;
        var_neanderRushUnits = neanderRushUnits;
        var_neanderRushInterval = neanderRushInterval;
        var_level_grind_factor = level_grind_factor;
        var_level_grind_spread = level_grind_spread;
        var_replayFactor = replayFactor;
        var_win_loss_influce = win_loss_influce;
        var_win_loss_maxCount = win_loss_maxCount;
        var_appleBoostAmount = appleBoostAmount;

        // XÓA 2 dòng này - không cần convert nữa
        // var_appleCollectRate = LeanplumHelper.toListObject(AppleCollectRate);
        // var_appleUpgradeCost = LeanplumHelper.toListObject(AppleUpgradeCost);

        var_consumable_unlock_fill = consumable_unlock_fill;

        foreach (KeyValuePair<UnitType, int> consumableRefillValue in ConsumableRefillValues)
        {
            // Dùng trực tiếp giá trị mặc định thay vì Var.Define
            var_ConsumableRefillValues.Add(consumableRefillValue.Key, consumableRefillValue.Value);
        }

        var_dino_rage_threshold = dino_rage_threshold;
        var_dino_rage_cost = dino_rage_cost;
        var_dino_rage_start_level = dino_rage_start_level;
        var_dino_rage_duration = dino_rage_duration;
        var_dino_rage_apples = dino_rage_apples;
        var_dino_rage_loose_count = dino_rage_loose_count;
        var_show_rate_prompt_level = show_rate_prompt_level;
        var_show_rate_prompt_frequency = show_rate_prompt_frequency;
        var_iOSPre7RateURL = iOSPre7RateURL;
        var_iOSPost7RateURL = iOSPost7RateURL;
        var_googlePlayRateURL = googlePlayRateURL;
        var_moregamesURL_iOS = moregamesURL_iOS;
        var_moregamesURL_Android = moregamesURL_Android;
        var_verfiy_url_android = verfiy_url_android;
        var_verfiy_url_ios = verfiy_url_ios;

        // XÓA dòng này - không còn Leanplum callback
        // Leanplum.VariablesChanged += HandleVariablesChanged;
    }
 //   private void HandleVariablesChanged()
	//{
	//	try
	//	{
	//		AppleCollectRate = LeanplumHelper.toFloatArray(var_appleCollectRate);
	//		AppleUpgradeCost = LeanplumHelper.toIntArray(var_appleUpgradeCost);
	//		consumable_unlock_fill = var_consumable_unlock_fill;
	//		foreach (KeyValuePair<UnitType,int> var_ConsumableRefillValue in var_ConsumableRefillValues)
	//		{
	//			if (ConsumableRefillValues.ContainsKey(var_ConsumableRefillValue.Key))
	//			{
	//				ConsumableRefillValues[var_ConsumableRefillValue.Key] = var_ConsumableRefillValue.Value;
	//			}
	//		}
	//	}
	//	catch (Exception ex)
	//	{
	//		Debug.LogError(ex.Message + "\n" + ex.StackTrace);
	//	}
	//}
}
