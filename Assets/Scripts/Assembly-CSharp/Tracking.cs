using System;
using System.Collections;
using System.Collections.Generic;
using LeanplumSDK;
using UnityEngine;
using dinobash;
using mixpanel;
using mixpanel.platform;

public class Tracking
{
	public enum Location
	{
		invalid = 0,
		install = 1,
		tutorial = 2,
		quest = 3,
		upgrades = 4,
		store = 5,
		level = 6,
		map = 7,
		startscreen = 8,
		error = 9
	}

	public enum RewardedVideoAction
	{
		offer = 0,
		closed = 1,
		cancelled = 2,
		watched = 3
	}

	public enum GateMethod
	{
		paid = 0,
		waited = 1,
		friends = 2
	}

	public enum LevelAction
	{
		start = 0,
		complete = 1,
		win = 2,
		loose = 3,
		quit = 4
	}

	private static Var<bool> trackingEnabled;

	private static bool jb_evt_sent;

	private static uint last_session_duration = 1000000000u;

	private static bool enabled
	{
		get
		{
			if (trackingEnabled == null)
			{
				return true;
			}
			return trackingEnabled.Value;
		}
	}

	private static uint StepDuration
	{
		get
		{
			uint secondsSinceSessionStart = Mixpanel.SecondsSinceSessionStart;
			if (last_session_duration > secondsSinceSessionStart)
			{
				last_session_duration = secondsSinceSessionStart;
			}
			return secondsSinceSessionStart - last_session_duration;
		}
	}

	public static int items_bought_count { get; private set; }

	private static void track(string event_name, string event_tag, Location location, Dictionary<string, object> properties = null)
	{
		if (!enabled)
		{
			return;
		}
		Value value = new Value();
		value["EventTag"] = event_tag;
		value["Location"] = location.ToString();
		if (properties != null)
		{
			foreach (KeyValuePair<string, object> property in properties)
			{
				if (property.Value is string)
				{
					value[property.Key] = property.Value.ToString();
				}
				else if (property.Value is int)
				{
					value[property.Key] = (int)property.Value;
				}
				else if (property.Value is uint)
				{
					value[property.Key] = (uint)property.Value;
				}
				else if (property.Value is float)
				{
					value[property.Key] = (float)property.Value;
				}
				else if (property.Value is double)
				{
					value[property.Key] = (float)property.Value;
				}
				else if (property.Value is bool)
				{
					value[property.Key] = (bool)property.Value;
				}
				else
				{
					Debug.LogError("Unhandled Type: " + property.ToString() + " : " + property.Value.ToString());
				}
			}
		}
		Mixpanel.Track(event_name, value);
	}

	public static void Initialize()
	{
		trackingEnabled = Var.Define("feature.TrackingEnabled", true);
		items_bought_count = 0;
		App.OnStateChange += delegate
		{
			Mixpanel.ResetErrorCount();
		};
		Apsalar.SetFBAppId("527007970745796");
		Mixpanel.people.Set("device_os", SystemInfo.operatingSystem);
		Mixpanel.people.Set("client_version", App.VERSION_CODE);
		Mixpanel.people.Set("game_language", i18n.Language.ToString());
		Mixpanel.people.Set("test_id", Player.Instance.splitTestingRandomId);
		Mixpanel.people.Set("test_group", Player.Instance.splitTestingGroupName);
		set_facebook_login();
		Mixpanel.people.Set("android_aid", MixpanelUnityPlatform.get_android_advertising_id());
		Mixpanel.people.Set("android_id", MixpanelUnityPlatform.get_android_id());
	}

	private static bool IsJailbroken()
	{
		return false;
	}

	private static bool CheckJailBroken()
	{
		if (IsJailbroken())
		{
			if (!jb_evt_sent)
			{
				jb_evt_sent = true;
			}
			return true;
		}
		return false;
	}

	private static string getLevelName(int levelid)
	{
		return Konfiguration.GetLevelData(levelid).name;
	}

	public static void social_gaming_login(string social_id)
	{
		if (!string.IsNullOrEmpty(social_id))
		{
			Mixpanel.people.Set("social_gaming", social_id);
			Mixpanel.people.Set("googleplay_id", social_id);
		}
		track("User: Google Play", "google_play_login", Location.install);
	}

	public static void welcomescreen_visible_tutorial()
	{
		if (Player.tutorial_reached("welcomescreen_visible_tutorial"))
		{
			track("Tut: Welcome", "welcomescreen_visible_tutorial", Location.tutorial, new Dictionary<string, object> { { "step_duration ", StepDuration } });
		}
	}

	public static void play_button_pressed_tutorial()
	{
		if (Player.tutorial_reached("play_button_pressed_tutorial"))
		{
			track("Tut: Press Play", "play_button_pressed_tutorial", Location.tutorial, new Dictionary<string, object> { { "step_duration ", StepDuration } });
		}
	}

	public static void loading_screen_complete_tutorial(float load_time)
	{
		if (Player.tutorial_reached("loading_screen_complete_tutorial"))
		{
			track("Tut: Loading Screen", "loading_screen_complete_tutorial", Location.tutorial, new Dictionary<string, object>
			{
				{ "step_duration ", StepDuration },
				{ "load_time ", load_time }
			});
		}
	}

	public static void finish_intro_cutscene_tutorial()
	{
		if (Player.tutorial_reached("finish_intro_cutscene_tutorial"))
		{
			track("Tut: Intro Cutscene End", "finish_intro_cutscene_tutorial", Location.tutorial, new Dictionary<string, object> { { "step_duration ", StepDuration } });
		}
	}

	public static void start_intro_cutscene_tutorial()
	{
		if (Player.tutorial_reached("start_intro_cutscene_tutorial"))
		{
			track("Tut: Intro Cutscene Start", "start_intro_cutscene_tutorial", Location.tutorial, new Dictionary<string, object> { { "step_duration ", StepDuration } });
		}
	}

	public static void level_001_complete_tutorial()
	{
		if (Player.tutorial_reached("level_001_complete_tutorial"))
		{
			track("Tut: Tutorial Level 1 Complete", "level_001_complete_tutorial", Location.tutorial);
		}
	}

	public static void unlock_snappy()
	{
		if (Player.tutorial_reached("unlocked_snappy"))
		{
			track("Tut: Unlocked Snappy", "unlocked_snappy", Location.tutorial);
		}
	}

	public static void buy_snappy()
	{
		if (Player.tutorial_reached("buy_snappy"))
		{
			track("Tut: Buy a Snappy", "buy_snappy", Location.tutorial);
		}
	}

	public static void level_002_start_tutorial()
	{
		if (Player.tutorial_reached("level_002_start_tutorial"))
		{
			track("Tut: Tutorial Start Level 2", "level_002_start_tutorial", Location.tutorial);
		}
	}

	public static void level_002_complete_tutorial()
	{
		if (Player.tutorial_reached("level_002_complete_tutorial"))
		{
			track("Tut: Tutorial Level 2 Complete", "level_002_complete_tutorial", Location.tutorial);
		}
	}

	public static void unlocked_volcano_bomb()
	{
		if (Player.tutorial_reached("unlocked_volcano_bomb"))
		{
			track("Tut: Unlocked Volcano Bomb", "unlocked_volcano_bomb", Location.tutorial);
		}
	}

	public static void fire_volcano_bomb()
	{
		if (Player.tutorial_reached("fire_volcano_bomb"))
		{
			track("Tut: Fire Volcano Bomb", "fire_volcano_bomb", Location.tutorial);
		}
	}

	public static void drop_volcano_bomb()
	{
		if (Player.tutorial_reached("drop_volcano_bomb"))
		{
			track("Tut: Drop Volcano Bomb", "drop_volcano_bomb", Location.tutorial);
		}
	}

	public static void detonate_volcano_bomb()
	{
		if (Player.tutorial_reached("detonate_volcano_bomb"))
		{
			track("Tut: Detonate Volcano Bomb", "detonate_volcano_bomb", Location.tutorial);
		}
	}

	public static void level_003_start_tutorial()
	{
		if (Player.tutorial_reached("level_003_start_tutorial"))
		{
			track("Tut: Tutorial Start Level 3", "level_003_start_tutorial", Location.tutorial);
		}
	}

	public static void level_003_complete_tutorial()
	{
		if (Player.tutorial_reached("level_003_complete_tutorial"))
		{
			track("Tut: Tutorial Level 3 Complete", "level_003_complete_tutorial", Location.tutorial);
		}
	}

	public static void unlocked_raptor()
	{
		if (Player.tutorial_reached("unlocked_raptor"))
		{
			track("Tut: Unlocked Raptor", "unlocked_raptor", Location.tutorial);
		}
	}

	public static void level_004_start_tutorial()
	{
		if (Player.tutorial_reached("level_004_start_tutorial"))
		{
			track("Tut: Tutorial Start Level 4 in World Map", "level_004_start_tutorial", Location.tutorial);
		}
	}

	public static void welcomescreen_visible()
	{
		track("Welcome Screen Visible", "welcomescreen_visible", Location.startscreen);
	}

	public static void show_faq()
	{
		Location location = Location.map;
		if (App.State == App.States.StartScreen)
		{
			location = Location.startscreen;
		}
		track("User open FAQ", "faq_open", location);
	}

	public static void quest_list_open()
	{
		track("Open Objective List", "objective_list_open", Location.quest);
	}

	public static void quest_redeemed(string quest)
	{
		track("Collect Objective Reward", "objective_redeemed", Location.quest, new Dictionary<string, object> { { "objective ", quest } });
	}

	public static void quest_list_closed()
	{
		track("Objective List Exit", "objective_list_closed", Location.quest);
	}

	public static void quest_completed(string quest, string reward, int amount)
	{
		track("Objective Completed", "objective_completed", Location.quest, new Dictionary<string, object>
		{
			{ "objective ", quest },
			{ "reward ", reward },
			{ "amount ", amount }
		});
	}

	public static void upgrades_open()
	{
		track("Open Upgrades", "upgrades_open", Location.upgrades);
	}

	public static void bought_upgrade_after_loss(int buy_item_count)
	{
		if (Player.LooseCount > 0)
		{
			bool flag = buy_item_count < items_bought_count;
			track("Bougt Upgrade after Loss", "bought_upgrade_after_loss", Location.upgrades, new Dictionary<string, object>
			{
				{ "bought_something", flag },
				{
					"lost_games",
					Player.LooseCount
				}
			});
		}
	}

	public static void buy_item(UnitType item = UnitType.None, ShotType shot = ShotType.None, string misc = "", int spent = 0, string origin = "", bool skip_training = false)
	{
		if (CheckJailBroken() || !enabled)
		{
			return;
		}
		items_bought_count++;
		string value = "upgrade_" + item;
		bool flag = Konfiguration.isPremium(item);
		if (item != 0 && Konfiguration.isDinoUnit(item))
		{
			value = "dino_" + item;
			value = value + "_" + Player.GetUnitLevel(item);
		}
		else if (item != 0 && Konfiguration.isConsumable(item))
		{
			value = "consumable_" + item;
			set_total_consumables_availiable();
		}
		else if (shot != ShotType.None)
		{
			value = "shot_" + shot;
			value = value + "_" + Player.GetShotLevel(shot);
			flag = Konfiguration.isPremium(shot);
		}
		if (!string.IsNullOrEmpty(misc))
		{
			if (misc == "live")
			{
				value = misc;
				flag = true;
			}
			else if (misc == "dinorage")
			{
				value = misc;
				flag = true;
			}
		}
		int maxLevelID = Player.MaxLevelID;
		string levelName = getLevelName(maxLevelID);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("item", value);
		dictionary.Add("level", maxLevelID);
		dictionary.Add("level_name", levelName);
		dictionary.Add("diamonds_spent", flag ? spent : 0);
		dictionary.Add("coins_spent", (!flag) ? spent : 0);
		dictionary.Add("where", origin);
		dictionary.Add("bought_skip_training", skip_training);
		Dictionary<string, object> dictionary2 = dictionary;
		track("Buy Item", "buy_item", Location.store, dictionary2);
		dictionary2["EventTag"] = "buy_item";
		dictionary2["Location"] = Location.store.ToString();
		Leanplum.Track("Buy Item", dictionary2);
	}

	public static void total_coins_earned(int amount)
	{
		if (!CheckJailBroken())
		{
			Mixpanel.people.Increment("total_coins_earned", amount);
		}
	}

	public static void total_coins_spent(int amount)
	{
		if (!CheckJailBroken())
		{
			Mixpanel.people.Increment("total_coins_spent", amount);
		}
	}

	public static void total_diamonds_earned(int amount)
	{
		if (!CheckJailBroken())
		{
			Mixpanel.people.Increment("total_gems_earned", amount);
		}
	}

	public static void total_diamonds_spent(int amount)
	{
		if (!CheckJailBroken())
		{
			Mixpanel.people.Increment("total_gems_spent", amount);
		}
	}

	public static void store_open(Wallet.Currency currency, string origin, string origin_item)
	{
		int num = ((App.State != App.States.Game) ? Player.MaxLevelID : Player.CurrentLevelID);
		string levelName = getLevelName(num);
		track("Open Store", "store_open", Location.store, new Dictionary<string, object>
		{
			{
				"store",
				currency.ToString()
			},
			{ "where", origin },
			{ "level", num },
			{ "level_name", levelName },
			{ "origin_item", origin_item }
		});
	}

	public static void store_purchase_failed(Wallet.Currency currency)
	{
		track("IAP: Buy Cancel", "store_purchase_failed", Location.store, new Dictionary<string, object> { 
		{
			"store",
			currency.ToString()
		} });
	}

	public static void OnBuyPack(ShopItems.Pack pack, PurchasableItem item, string receipt)
	{
		App.Instance.StartCoroutine(OnBuyPackValidated(pack, item, receipt));
	}

	private static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
	{
		for (int i = 0; i < str.Length; i += maxChunkSize)
		{
			yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
		}
	}

	private static IEnumerator OnBuyPackValidated(ShopItems.Pack pack, PurchasableItem item, string receipt)
	{
		if (CheckJailBroken() || !enabled || Application.isEditor)
		{
			yield break;
		}
		string currency_code = pack.isoCurrencyCode;
		double amount_usd = pack.priceInUSD;
		int level = Player.MaxLevelID;
		string level_name = getLevelName(level);
		string virtual_currency = pack.currency.ToString() + "received";
		Value properties = new Value();
		properties["EventTag"] = "real_purchase";
		properties["Location"] = Location.store.ToString();
		properties["sku"] = pack.id;
		properties["currency_spent"] = amount_usd;
		properties["user_currency_spent"] = pack.priceInLocalCurrency;
		properties["user_currency"] = CurrencyHelper.getCurrencyCode();
		properties[virtual_currency] = pack.amount;
		properties["level"] = level;
		properties["level_name"] = level_name;
		properties["device_currency_code"] = CurrencyHelper.getCurrencyCode();
		properties["store_currency_code"] = item.isoCurrencySymbol;
		if (ShopPromotions.is_sale && !string.IsNullOrEmpty(ShopPromotions.promotion_id))
		{
			properties["promotion_id"] = ShopPromotions.promotion_id;
		}
		else
		{
			properties["promotion_id"] = "default";
		}
		foreach (string snip in ChunksUpto(receipt, 255))
		{
			properties["receipt"].append(snip);
		}
		string facebook_id = PlayerPrefs.GetString("facebook_id", string.Empty);
		WWWForm form = new WWWForm();
		form.AddField("device_id", MixpanelUnityPlatform.get_distinct_id());
		form.AddField("amount_original", pack.priceInLocalCurrency.ToString("0.00"));
		form.AddField("amount_usd", pack.priceInUSD.ToString("0.00"));
		form.AddField("currency_code", currency_code);
		form.AddField("facebook_id", facebook_id);
		form.AddField("receipt", receipt);
		WWW www = new WWW(Konfiguration.GameConfig.VERIFY_URL_ANDROID, form);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogWarning("Failed to verify receipt via " + www.url + ", error: " + www.error);
			yield break;
		}
		if (www.text != "true")
		{
			Debug.LogError("IAP-Validation failed: text=" + www.text);
			Mixpanel.people.Set("IAPThiefe", true);
			Mixpanel.people.Increment("CheatedIAPAmount", amount_usd);
			properties["EventTag"] = "fake_purchase";
			Mixpanel.Track("IAP: Steal", properties);
			yield break;
		}
		Mixpanel.people.Set("IAPThiefe", false);
		Mixpanel.people.Set("total_spend", string.Format("{0}({1})", Wallet.Total_spent, CurrencyHelper.getCurrencyCode()));
		Apsalar.SendEvent(new Dictionary<string, object>
		{
			{ "ps", "TiltingPoint_PokokoStudio" },
			{ "pk", pack.id },
			{ "pn", pack.id },
			{ "pc", "Consumables" },
			{ "pcc", pack.isoCurrencyCode },
			{ "pq", 1 },
			{ "pp", pack.priceInLocalCurrency },
			{ "r", pack.priceInLocalCurrency }
		}, "__iap__");
		Leanplum.Track("Purchase", amount_usd);
		Mixpanel.people.TrackChargeConverting(pack.priceInLocalCurrency, pack.isoCurrencyCode, properties);
		Mixpanel.Track("IAP: Buy", properties);
		Wallet.IsPayingUser = true;
		yield return null;
	}

	public static void rewarded_video(int level, RewardedVideoAction action, RewardedVideoItems reward, int amount)
	{
		if (enabled)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["action"] = action.ToString();
			dictionary["reward"] = reward.ToString();
			dictionary["amount"] = amount;
			dictionary["loose_count"] = Player.LooseCount;
			dictionary["level"] = level;
			dictionary["level_name"] = getLevelName(level);
			dictionary["watched_count"] = Player.Instance.PlayerData.watchedRewardedVideos;
			dictionary["videos_per_day"] = RewardedVideosWrapper.Videos_per_day;
			dictionary["lives videos_per_day"] = RewardedVideosWrapper.Lives_videos_per_day;
			track("Rewarded Video", "rewarded_video", Location.level, dictionary);
			dictionary["EventTag"] = "level";
			dictionary["Location"] = Location.level.ToString();
			Leanplum.Track("Rewarded Video", dictionary);
		}
	}

	public static void pass_gate(GateMethod method, int level)
	{
		if (!CheckJailBroken() || method != 0)
		{
			track("Pass Gate", "pass_gate", Location.level, new Dictionary<string, object>
			{
				{
					"method",
					method.ToString()
				},
				{ "level", level },
				{
					"level_name",
					getLevelName(level)
				}
			});
		}
	}

	public static void dino_rage_offer(int level, float progress)
	{
		track("Dino Rage Offer", "dino_rage", Location.level, new Dictionary<string, object>
		{
			{ "level", level },
			{
				"level_name",
				getLevelName(level)
			},
			{ "progress", progress }
		});
	}

	public static void consumable_used(UnitType item, int level)
	{
		if (!CheckJailBroken())
		{
			track("Used Consumable", "used_consumable", Location.level, new Dictionary<string, object>
			{
				{
					"item",
					item.ToString()
				},
				{ "level", level },
				{
					"level_name",
					getLevelName(level)
				},
				{
					"progress",
					Level.Instance.getProgess()
				}
			});
			Mixpanel.people.Increment("total_consumables_used", 1);
			set_total_consumables_availiable();
		}
	}

	public static void set_total_consumables_availiable()
	{
		if (CheckJailBroken() || !enabled)
		{
			return;
		}
		int num = 0;
		foreach (int value in Enum.GetValues(typeof(UnitType)))
		{
			if (Konfiguration.isConsumable((UnitType)value))
			{
				num += Player.getConsumableCount((UnitType)value);
			}
		}
		Mixpanel.people.Set("total_consumables_available", num);
	}

	public static void pickup_consumable(string item, int level, int amount = 1)
	{
		if (enabled)
		{
			Mixpanel.people.Increment("total_free_consumables_received", amount);
			track("Pickup Consumable", "pickup_consumable", Location.level, new Dictionary<string, object>
			{
				{ "item", item },
				{ "level", level },
				{
					"level_name",
					getLevelName(level)
				}
			});
		}
	}

	public static void set_player_level()
	{
		Mixpanel.people.Set("player_star_level", Player.XPLevel);
	}

	public static void level(int level, LevelAction action, int attempts, float cleared, int coins_recieved, int xp_recieved)
	{
		if (enabled)
		{
			Dictionary<string, object> dictionary;
			if (action == LevelAction.complete)
			{
				Mixpanel.people.Set("highest_level_reached", Player.MaxLevelID);
				dictionary = new Dictionary<string, object>();
				dictionary.Add("highest_level_reached", Player.MaxLevelID);
				Leanplum.SetUserAttributes(dictionary);
			}
			Mixpanel.people.Set("current_level", Player.CurrentLevelID);
			Mixpanel.people.Set("total_lives_available", Player.Lives);
			Mixpanel.people.Set("total_lives_spent", Player.Instance.PlayerData.totalLooseCount);
			bool flag = false;
			if (App.State == App.States.Game)
			{
				flag = EntityFactory.Dino_Egg.player_used_dinorage;
			}
			bool flag2 = false;
			if (Level.Instance != null)
			{
				flag2 = Level.Instance.usedConsumableCount > 0;
			}
			bool flag3 = Player.MaxLevelID == level && !Player.HasPlayedMaxLevelID;
			dictionary = new Dictionary<string, object>();
			dictionary.Add("action", action.ToString());
			dictionary.Add("level", level);
			dictionary.Add("level_name", getLevelName(level));
			dictionary.Add("attempts", attempts + 1);
			dictionary.Add("cleared", cleared);
			dictionary.Add("first_try", flag3);
			dictionary.Add("used_dinorage", flag);
			dictionary.Add("used_consumable", flag2);
			dictionary.Add("coins_recieved", coins_recieved);
			dictionary.Add("xp_recieved", xp_recieved);
			Dictionary<string, object> dictionary2 = dictionary;
			track("level", "level", Location.level, dictionary2);
			dictionary2["EventTag"] = "level";
			dictionary2["Location"] = Location.level.ToString();
			Leanplum.Track("level", dictionary2);
		}
	}

	public static void set_facebook_login()
	{
		Mixpanel.people.Set("facebook_connect", App.Instance.facebookManager.isLoggedIn);
	}

	public static void rewarded_facebook_login(string message)
	{
		track("User:Facebook:RewardedLogin", "rewarded_facebook_login", Location.map, new Dictionary<string, object> { { "type", message } });
	}

	public static void FBConnect(FacebookManager.Profile profile)
	{
		set_facebook_login();
		Mixpanel.people.FirstName = profile.first_name;
		Mixpanel.people.LastName = profile.last_name;
		Mixpanel.people.Email = profile.email;
		Mixpanel.people.Set("facebook_id", profile.id);
		PlayerPrefs.SetString("facebook_id", profile.id);
		PlayerPrefs.Save();
		track("User:Facebook", "facebook_login", Location.install);
	}

	public static void FBAppRequest(FacebookManager.AppRequestType request_type)
	{
		if (request_type != 0)
		{
			track("User:Facebook:Requests", "facebook_request", Location.map, new Dictionary<string, object>
			{
				{
					"type",
					request_type.ToString()
				},
				{
					"level",
					Player.CurrentLevelID
				},
				{
					"level_name",
					getLevelName(Player.CurrentLevelID)
				}
			});
		}
	}

	public static void rate_button()
	{
		Location location = ((App.State != App.States.StartScreen) ? Location.level : Location.startscreen);
		track("Rate Button", "rate_button", location);
	}

	public static void moregames_button()
	{
		track("Spotlight_MGames_Click", "spotlight_more_games_click", Location.startscreen);
	}

	public static void TimeManipulationDetected()
	{
		track("Cheat: TimeManipulation", "time_manipulation", Location.map);
	}

	public static void CorruptedSaveGame(string corrupted, string backup)
	{
		track("Error: Corrupted Save Game", "corrupt_savegame", Location.error, new Dictionary<string, object>
		{
			{ "corrupted", corrupted },
			{ "backup", backup }
		});
	}

	public static void MissingLocaKey(string key)
	{
		track("Error: Missing Loca Key", "missing_loca_key", Location.error, new Dictionary<string, object> { { "key", key } });
	}
}
