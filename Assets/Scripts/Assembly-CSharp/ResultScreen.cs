using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class ResultScreen : BaseScreen
{
	private StandardButton boostCoinsButton;

	private int coin_balance;

	private StandardButton nextButton;

	private LocalizedText titleLabel;

	private LocalizedText nextButtonLabel;

	private tk2dTextMesh coinsValue;

	private tk2dTextMesh xpValue;

	private tk2dTextMesh highscoreValue;

	private tk2dTextMesh timeValue;

	private SpriteRenderer icon_coin;

	private SpriteRenderer icon_xp;

	private FlyElementTo[] fly_coin_icons;

	private FlyElementTo[] fly_xp_icons;

	private Transform winDino;

	private Transform looseDino;

	private ParticleSystem konfetti;

	private int player_maxlevel_id = -1;

	private bool show_endless_result;

	private float pre_play_record;

	private int iconsToSpawn = 20;

	private int playerLevelUps;

	private bool show_upgrade_screen;

	private bool userClosedGetLivesScreen;

	public int tweenCoins { get; set; }

	public int tweenXp { get; set; }

	private int get_xp_count
	{
		get
		{
			float num = (float)Player.Instance.LevelXP / (float)(Level.Instance.Config.kill_xp + Level.Instance.Config.level_xp);
			if (Player.Instance.LevelXP <= fly_xp_icons.Length)
			{
				return Player.Instance.LevelXP;
			}
			return Mathf.Min(Mathf.FloorToInt((float)fly_xp_icons.Length * num), iconsToSpawn);
		}
	}

	private int get_coin_count
	{
		get
		{
			float num = (float)Player.Instance.LevelCoins / (float)(Level.Instance.Config.Kill_coins + Level.Instance.Config.Level_coins);
			if (Player.Instance.LevelCoins <= fly_coin_icons.Length)
			{
				return Player.Instance.LevelCoins;
			}
			return Mathf.Min(Mathf.FloorToInt((float)fly_coin_icons.Length * num), iconsToSpawn);
		}
	}

	public bool show_GetLivesScreen { get; private set; }

	protected void Start()
	{
		winDino = base.transform.Search("dino_win");
		winDino.gameObject.SetActive(false);
		looseDino = base.transform.Search("dino_loose");
		looseDino.gameObject.SetActive(false);
		boostCoinsButton = base.transform.Search("btn_boost_coins").GetComponent<StandardButton>();
		boostCoinsButton.uiItem.OnClick += delegate
		{
			RewardedVideoScreen screen = ScreenManager.GetScreen<RewardedVideoScreen>();
			screen.Show(RewardedVideoItems.Coins);
			coin_balance = Wallet.Coins;
		};
		tk2dTextMesh componentInChildren = boostCoinsButton.GetComponentInChildren<tk2dTextMesh>();
		componentInChildren.text = "result_boost_coins".Localize();
		nextButton = base.transform.Search("btn_ok").GetComponent<StandardButton>();
		nextButton.clickSound = Sounds.main_continue_button;
		nextButtonLabel = nextButton.GetComponentInChildren<LocalizedText>();
		titleLabel = base.transform.Search("title_label").GetComponent<LocalizedText>();
		konfetti = base.transform.Search("Konfetti").GetComponent<ParticleSystem>();
		player_maxlevel_id = Player.MaxLevelID;
		show_endless_result = Level.Instance.Config.endless_mode;
		if (!show_endless_result)
		{
			base.transform.Search("timer").gameObject.SetActive(false);
			coinsValue = base.transform.Search("coins_value").GetComponent<tk2dTextMesh>();
			tweenCoins = 0;
			xpValue = base.transform.Search("xp_value").GetComponent<tk2dTextMesh>();
			tweenXp = 0;
			icon_coin = base.transform.Search("icon_coin").GetComponent<SpriteRenderer>();
			icon_xp = base.transform.Search("icon_xp").GetComponent<SpriteRenderer>();
			CloneIcons(icon_coin, out fly_coin_icons);
			CloneIcons(icon_xp, out fly_xp_icons);
			int sortingLayerID = ScreenManager.GetScreen<ResourceBarScreen>().SortingLayerID;
			FlyElementTo[] array = fly_xp_icons;
			foreach (FlyElementTo target in array)
			{
				SpriteTools.TargetSetSortingLayerID(target, sortingLayerID);
			}
			FlyElementTo[] array2 = fly_coin_icons;
			foreach (FlyElementTo target2 in array2)
			{
				SpriteTools.TargetSetSortingLayerID(target2, sortingLayerID);
			}
		}
		else
		{
			base.transform.Search("stats").gameObject.SetActive(false);
			timeValue = base.transform.Search("time_value_label").GetComponent<tk2dTextMesh>();
			timeValue.text = "0";
			highscoreValue = base.transform.Search("highscore_value_label").GetComponent<tk2dTextMesh>();
			pre_play_record = Player.getBestEndlessLevelScore(Level.Instance.levelid);
			highscoreValue.text = pre_play_record.ToString();
			boostCoinsButton.gameObject.SetActive(false);
			nextButton.transform.LocalPosX(0f);
		}
		Player.OnLevelUp += OnLevelUp;
		Level.Instance.OnLevelWon += OnWon;
		Level.Instance.OnLevelLost += OnLost;
		show_GetLivesScreen = false;
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public void handleCoinBoostReward()
	{
		int num = Wallet.Coins - coin_balance;
		tweenCoins = Player.Instance.LevelCoins;
		Go.to(this, 2f, new GoTweenConfig().intProp("tweenCoins", Player.Instance.LevelCoins + num).onUpdate(delegate
		{
			coinsValue.text = tweenCoins.ToString().GetGroupedNumberString();
			coinsValue.Commit();
		}));
		FlyElementTo[] array = fly_coin_icons;
		foreach (FlyElementTo flyElementTo in array)
		{
			flyElementTo.Stop();
			flyElementTo.transform.position = icon_coin.transform.position;
		}
		StartCoins(10);
		boostCoinsButton.uiItem.enabled = false;
		Go.to(boostCoinsButton.transform, 0.3f, new GoTweenConfig().scale(0f));
	}

	private void CloneIcons(SpriteRenderer sprite, out FlyElementTo[] icons)
	{
		icons = new FlyElementTo[iconsToSpawn];
		for (int i = 0; i < iconsToSpawn; i++)
		{
			SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate(sprite, sprite.transform.position, sprite.transform.rotation) as SpriteRenderer;
			spriteRenderer.sortingOrder += 1000;
			spriteRenderer.transform.parent = base.transform;
			FlyElementTo flyElementTo = spriteRenderer.AddOrGetComponent<FlyElementTo>();
			icons[i] = flyElementTo;
			flyElementTo.gameObject.SetActive(false);
		}
	}

	protected override void OnDestroy()
	{
		Player.OnLevelUp -= OnLevelUp;
		Level.Instance.OnLevelWon -= OnWon;
		Level.Instance.OnLevelLost -= OnLost;
		base.OnDestroy();
	}

	private void OnLevelUp()
	{
		playerLevelUps++;
	}

	private void OnWon()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(Show(true));
	}

	private void OnLost()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(Show(false));
	}

	private IEnumerator Show(bool playerHasWon)
	{
		yield return new WaitForSeconds(1f);
		base.Show();
		if (playerHasWon || show_endless_result)
		{
			winDino.gameObject.SetActive(true);
			AudioPlayer.PlayGuiSFX(Sounds.game_won_music, 0.5f);
			titleLabel.Key = ((!show_endless_result) ? "Yeah! You won!" : "result_screen_score");
			nextButtonLabel.Key = "Yeah";
		}
		else
		{
			looseDino.gameObject.SetActive(true);
			AudioPlayer.PlayGuiSFX(Sounds.game_lost_music, 0.5f);
			titleLabel.Key = "Oh no, you Lost!";
			nextButtonLabel.Key = "continue";
		}
		boostCoinsButton.gameObject.SetActive(RewardedVideosWrapper.hasVideo && Player.Instance.canWatchRewardedVideo(RewardedVideoItems.Coins) && RewardedVideosWrapper.getRewardAmount(RewardedVideoItems.Coins) > 0);
		boostCoinsButton.uiItem.enabled = false;
		if (!boostCoinsButton.gameObject.activeSelf)
		{
			nextButton.transform.LocalPosX(0f);
		}
		if (show_endless_result)
		{
			boostCoinsButton.gameObject.SetActive(false);
			int new_record = Player.getBestEndlessLevelScore(Level.Instance.levelid);
			highscoreValue.text = Mathf.RoundToInt(Mathf.Max(pre_play_record, new_record)).ToString();
		}
		ShowFrom(base.right);
		yield return new WaitForSeconds(0.3f);
		AudioPlayer.StopMusic();
		nextButton.isFocused = true;
		nextButton.uiItem.OnClick += nextButtonClick;
		ResourceBarScreen resource_bar = ScreenManager.GetScreen<ResourceBarScreen>();
		resource_bar.Show(true);
		if (playerHasWon || show_endless_result)
		{
			konfetti.Play();
		}
		if (!playerHasWon)
		{
			resource_bar.RemoveHeart();
		}
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		if (!show_endless_result)
		{
			Go.to(this, 2f, new GoTweenConfig().setDelay(1.5f).intProp("tweenCoins", Player.Instance.LevelCoins).onUpdate(delegate
			{
				coinsValue.text = tweenCoins.ToString().GetGroupedNumberString();
				coinsValue.Commit();
			}));
			Go.to(this, 2f, new GoTweenConfig().setDelay(1f).intProp("tweenXp", Player.Instance.LevelXP).onUpdate(delegate
			{
				xpValue.text = tweenXp.ToString().GetGroupedNumberString();
				xpValue.Commit();
			}));
			yield return new WaitForSeconds(1f);
			StartXP(get_xp_count);
			StartCoins(get_coin_count);
			boostCoinsButton.uiItem.enabled = true;
		}
		else
		{
			Go.to(this, 2f, new GoTweenConfig().setDelay(1f).intProp("tweenXp", Mathf.RoundToInt(Level.Instance.EndlessScore)).onUpdate(delegate
			{
				timeValue.text = tweenXp.ToString();
				timeValue.Commit();
			}));
		}
	}

	private void SpawnCoinsXP(int coin_count, int xp_count)
	{
		ResourceBarScreen screen = ScreenManager.GetScreen<ResourceBarScreen>();
		for (int i = 0; i < xp_count; i++)
		{
			fly_xp_icons[i].gameObject.SetActive(true);
			fly_xp_icons[i].GetComponent<SpriteRenderer>().sortingOrder = i;
			fly_xp_icons[i].transform.PosZ(screen.icon_xp.transform.position.z);
			fly_xp_icons[i].transform.position += new Vector3(UnityEngine.Random.value * 32f - 16f, UnityEngine.Random.value * 32f - 16f, 0f);
			fly_xp_icons[i].transform.localScale = Vector3.zero;
			Go.to(fly_xp_icons[i].transform, 0.3f, new GoTweenConfig().scale(Vector3.one).setDelay(1.5f + (float)i * 2f / (float)xp_count));
		}
		for (int j = 0; j < coin_count; j++)
		{
			fly_coin_icons[j].gameObject.SetActive(true);
			fly_coin_icons[j].GetComponent<SpriteRenderer>().sortingOrder = j;
			fly_coin_icons[j].transform.PosZ(screen.icon_coins.transform.position.z);
			fly_coin_icons[j].transform.position += new Vector3(UnityEngine.Random.value * 32f - 16f, UnityEngine.Random.value * 32f - 16f, 0f);
			fly_coin_icons[j].transform.localScale = Vector3.zero;
			Go.to(fly_coin_icons[j].transform, 0.3f, new GoTweenConfig().scale(Vector3.one).setDelay(1f + (float)j * 2f / (float)coin_count));
		}
	}

	private void StartXP(int xp_count)
	{
		ResourceBarScreen screen = ScreenManager.GetScreen<ResourceBarScreen>();
		for (int i = 0; i < xp_count; i++)
		{
			fly_xp_icons[i].gameObject.SetActive(true);
			fly_xp_icons[i].transform.PosZ(screen.icon_xp.transform.position.z);
			fly_xp_icons[i].Play(screen.icon_xp.transform.position, (float)i * (1f / (float)xp_count));
		}
	}

	private void StartCoins(int coin_count)
	{
		ResourceBarScreen screen = ScreenManager.GetScreen<ResourceBarScreen>();
		for (int i = 0; i < coin_count; i++)
		{
			fly_coin_icons[i].gameObject.SetActive(true);
			fly_coin_icons[i].transform.PosZ(screen.icon_coins.transform.position.z);
			fly_coin_icons[i].Play(screen.icon_coins.transform.position, 0.5f + (float)i * (1f / (float)coin_count));
		}
	}

	private void nextButtonClick()
	{
		StopAllCoroutines();
		ParticleSystem[] componentsInChildren = konfetti.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.enableEmission = false;
		}
		Tracking.level_001_complete_tutorial();
		Tracking.level_002_complete_tutorial();
		Tracking.level_003_complete_tutorial();
		if (Player.MaxLevelID > Tutorials.LevelID("BasicUnit_Tutorial"))
		{
			StartCoroutine(Hide(delegate
			{
				App.stateMap(show_upgrade_screen);
			}));
		}
		else
		{
			StartCoroutine(Hide(delegate
			{
				App.stateGame(Player.MaxLevelID);
			}));
		}
	}

	private IEnumerator Hide(Action callback)
	{
		base.Hide();
		if (!show_endless_result)
		{
			Array.ForEach(fly_coin_icons, delegate(FlyElementTo x)
			{
				x.Stop();
			});
			Array.ForEach(fly_xp_icons, delegate(FlyElementTo x)
			{
				x.Stop();
			});
			bool waiting = true;
			while (waiting)
			{
				if (Array.FindIndex(fly_coin_icons, (FlyElementTo x) => x.isFlying) == -1 && Array.FindIndex(fly_xp_icons, (FlyElementTo x) => x.isFlying) == -1)
				{
					waiting = false;
				}
				yield return new WaitForSeconds(0.1f);
			}
		}
		HideTo(base.left);
		UnlockScreen unlockScreen = ScreenManager.GetScreen<UnlockScreen>();
		LevelupScreen levelupscreen = ScreenManager.GetScreen<LevelupScreen>();
		ResourceBarScreen resource_bar = ScreenManager.GetScreen<ResourceBarScreen>();
		if (playerLevelUps > 0)
		{
			playerLevelUps = 0;
			yield return new WaitForSeconds(0.3f);
			levelupscreen.Show();
			resource_bar.AnimateDiamondsDifference();
			while (levelupscreen.isVisible)
			{
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(0.3f);
		}
		if (Player.MaxLevelID > RewardedVideosWrapper.RewardedVideos_start_level && (!RewardedVideosWrapper.Show_only_on_loose || Player.LooseCount > 0) && RewardedVideosWrapper.hasVideo)
		{
			RewardedVideoScreen screen3 = ScreenManager.GetScreen<RewardedVideoScreen>();
			if (Player.Lives <= RewardedVideosWrapper.Lives_threshold)
			{
				if (Player.Instance.canWatchRewardedVideo(RewardedVideoItems.Lives))
				{
					screen3.Show(RewardedVideoItems.Lives);
				}
			}
			else if (!RewardedVideosWrapper.Show_only_on_loose || RewardedVideosWrapper.Show_on_lost.Contains(Player.LooseCount))
			{
				List<UnitType> consumables = Konfiguration.getConsumables();
				consumables = consumables.FindAll((UnitType consumable) => Player.hasUnlockedConsumable(consumable));
				if (consumables.Count > 0)
				{
					RewardedVideoItems reward = (RewardedVideoItems)consumables[UnityEngine.Random.Range(0, consumables.Count)];
					if (Player.Instance.canWatchRewardedVideo(reward))
					{
						screen3.Show(reward);
					}
				}
			}
			while (screen3.isVisible)
			{
				yield return null;
			}
			yield return new WaitForSeconds(0.3f);
		}
		if (Player.Lives == 0)
		{
			show_GetLivesScreen = true;
			GetLivesScreen screen4 = ScreenManager.GetScreen<GetLivesScreen>();
			screen4.btn_close.uiItem.OnClick -= HandleGetLivesScreenCloseButton;
			screen4.btn_close.uiItem.OnClick += HandleGetLivesScreenCloseButton;
			screen4.Show_Delayed();
			while (Player.Lives == 0 && !userClosedGetLivesScreen)
			{
				yield return null;
			}
			show_GetLivesScreen = false;
			if (Player.Lives == 5)
			{
				yield return new WaitForSeconds(1f);
			}
			yield return new WaitForSeconds(0.3f);
		}
		resource_bar.Hide();
		yield return new WaitForSeconds(0.3f);
		if (Player.LooseCount == 0 && Player.CurrentLevelID == player_maxlevel_id)
		{
			UnitType unlocked_unit = Konfiguration.GetLevelData(Player.CurrentLevelID).unlockUnit;
			ShotType unlocked_shot = Konfiguration.GetLevelData(Player.CurrentLevelID).unlockShot;
			if (unlocked_unit != 0 && (Konfiguration.isDinoUnit(unlocked_unit) || Konfiguration.isUpgrade(unlocked_unit) || Konfiguration.isConsumable(unlocked_unit)))
			{
				yield return new WaitForSeconds(0.3f);
				unlockScreen.Show(unlocked_unit);
			}
			else if (unlocked_shot != ShotType.None)
			{
				yield return new WaitForSeconds(0.3f);
				unlockScreen.Show(unlocked_shot);
			}
			while (unlockScreen.isVisible)
			{
				yield return null;
			}
			show_upgrade_screen = unlockScreen.openUpUpgradeScreen;
			yield return new WaitForSeconds(0.3f);
		}
		List<UnitType> discount_items = new List<UnitType>(new UnitType[8]
		{
			UnitType.MegaBall,
			UnitType.AdditionalShotSlot,
			UnitType.Rocky,
			UnitType.Blizzard,
			UnitType.FastShotCooldown,
			UnitType.RaindowTrail,
			UnitType.TRex_Jr,
			UnitType.MeteorStorm
		});
		discount_items.RemoveAll(delegate(UnitType item)
		{
			if (Konfiguration.isDinoUnit(item))
			{
				return Player.AvailableUnitTypes.Contains(item);
			}
			if (Konfiguration.isUpgrade(item))
			{
				return Player.AvailiableUpgrades.Contains(item);
			}
			return !Konfiguration.isConsumable(item) || !Player.hasUnlockedConsumable(item);
		});
		LevelData level = Konfiguration.GetLevelData(Player.CurrentLevelID);
		List<LevelEnemy> premium_units = level.enemies.FindAll((LevelEnemy x) => Konfiguration.isPremium(x.unittype));
		premium_units.RemoveAll((LevelEnemy x) => Player.AvailableUnitTypes.Contains(x.unittype));
		Debug.Log("Possible discount offers:");
		foreach (UnitType item4 in discount_items)
		{
			Debug.Log(item4.ToString());
		}
		UpgradeInfoDinoScreen dino_screen = ScreenManager.GetScreen<UpgradeInfoDinoScreen>();
		UpgradeInfoDinoScreen upgrade_screen = ScreenManager.GetScreen<UpgradeInfoDinoScreen>();
		int display_level_name = short.Parse(Konfiguration.levels[Player.CurrentLevelID].display_name);
		if (Player.LooseCount == 0)
		{
			if (premium_units.Count > 0)
			{
				dino_screen.Show(premium_units[0].unittype, true);
			}
			else if (Player.CurrentLevelID == player_maxlevel_id && discount_items.Count > 0/* && display_level_name > Konfiguration.GameConfig.Special_offer_start_level*/ && display_level_name % 3 == 0)
			{
				UnitType item3 = discount_items[UnityEngine.Random.Range(0, discount_items.Count)];
				if (Konfiguration.isDinoUnit(item3) || Konfiguration.isConsumable(item3))
				{
					dino_screen.Show(item3, true);
				}
				else if (Konfiguration.isUpgrade(item3))
				{
					upgrade_screen.Show(item3, true);
				}
			}
		}
		//else if (/*Player.LooseCount % Konfiguration.GameConfig.Special_offer_loss_count == 0 &&*/ display_level_name > Konfiguration.GameConfig.Special_offer_start_level)
		//{
		//	List<UnitType> consumables3 = discount_items.FindAll((UnitType item) => Konfiguration.isConsumable(item));
		//	if (consumables3.Count > 0)
		//	{
		//		UnitType item2 = consumables3[UnityEngine.Random.Range(0, consumables3.Count)];
		//		dino_screen.Show(item2, true);
		//	}
		//}
		while (upgrade_screen.isVisible)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.3f);
		if (QuestManager.instance.activeQuests.Count > 0)
		{
			QuestScreen screen2 = ScreenManager.GetScreen<QuestScreen>();
			screen2.Show();
			while (screen2.isVisible)
			{
				yield return null;
			}
			yield return new WaitForSeconds(0.3f);
		}
		if (Player.LooseCount == 0 && Player.CurrentLevelID == player_maxlevel_id && Player.Instance.shouldShowRatePrompt)
		{
			Debug.Log("Showing App Rating Screen !!!");
			RateAppScreen screen = ScreenManager.GetScreen<RateAppScreen>();
			screen.Show();
			while (screen.isVisible)
			{
				yield return null;
			}
			yield return new WaitForSeconds(0.3f);
		}
		RewardedVideoPopup popup = ScreenManager.GetScreen<RewardedVideoPopup>();
		while (popup.isVisible)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.3f);
		callback();
	}

	private void HandleGetLivesScreenCloseButton()
	{
		if (!userClosedGetLivesScreen)
		{
			userClosedGetLivesScreen = true;
		}
	}
}
