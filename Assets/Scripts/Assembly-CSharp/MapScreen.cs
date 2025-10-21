using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class MapScreen : BaseScreen
{
	public Material disabled_material;

	public Material enabled_material;

	private LocalizedText center_message;

	private tk2dUIItem backButton;

	private StandardButton upgradeButton;

	private StandardButton quickBuck_Button;

	private StandardButton btn_bundle_offer;

	public Parallaxer parallaxer;

	private List<BaseScreen> inputLockerScreens = new List<BaseScreen>();

	private tk2dUIItem training_notification_close_button;

	private tk2dTextMesh training_notification_label;

	private Vector3 training_notification_endpos;

	private UnitType current_dino;

	private SpriteRenderer icon_portrait;

	private Transform upgrade_fx;

	private UpgradeScreen upgrade_screen;

	private static int player_max_levelid = -1;

	private List<LevelButton> level_buttons = new List<LevelButton>();

	private bool EnableScrolling
	{
		get
		{
			return parallaxer.HandleInput;
		}
		set
		{
			if (parallaxer.HandleInput != value)
			{
				parallaxer.HandleInput = value;
			}
		}
	}

	public int upgrade_possibilities { get; set; }

	protected void Start()
	{
		training_notification_close_button = base.transform.Search("training_notification_close_button").GetComponent<tk2dUIItem>();
		training_notification_label = base.transform.Search("training_notification_label").GetComponent<tk2dTextMesh>();
		training_notification_endpos = training_notification_label.transform.parent.localPosition;
		training_notification_label.transform.parent.localPosition += base.right;
		upgrade_screen = ScreenManager.GetScreen<UpgradeScreen>();
		icon_portrait = base.transform.Search("icon_portrait").GetComponent<SpriteRenderer>();
		upgrade_fx = base.transform.Search("FX_Stars");
		upgrade_fx.gameObject.SetActive(false);
		center_message = base.transform.Search("label_message").GetComponent<LocalizedText>();
		center_message.transform.localPosition += base.top;
		backButton = base.transform.Search("Back_Button").GetComponent<tk2dUIItem>();
		backButton.OnClick += onBackButtonClick;
		upgradeButton = base.transform.Search("Upgrade_Button").GetComponent<StandardButton>();
		upgradeButton.clickSound = Sounds.map_upgrade_btn;
		upgradeButton.uiItem.OnClick += onUpgradeButtonClick;
		quickBuck_Button = base.transform.Search("QuickBuck_Button").GetComponent<StandardButton>();
		quickBuck_Button.uiItem.OnClick += delegate
		{
			RewardedVideoScreen screen = ScreenManager.GetScreen<RewardedVideoScreen>();
			screen.Show(RewardedVideoItems.Coins);
		};
		StandardButton component = base.transform.Search("Quest_Button").GetComponent<StandardButton>();
		component.uiItem.OnClick += delegate
		{
			Tracking.quest_list_open();
			ScreenManager.GetScreen<QuestScreen>().Show();
		};
		BundleOffer.offerEnded += HandleofferEnded;
		btn_bundle_offer = base.transform.Search("btn_bundle_offer").GetComponent<StandardButton>();
		btn_bundle_offer.GetComponentInChildren<tk2dTextMesh>().text = BundlePromotionScreen.getTitle();
		btn_bundle_offer.uiItem.OnClick += delegate
		{
			ScreenManager.GetScreen<BundlePromotionScreen>().Show();
		};
		if (BundleOffer.time_remaining.Seconds < 10)
		{
			btn_bundle_offer.gameObject.SetActive(false);
		}
		StandardButton standardButton = FindChildComponent<StandardButton>("LowerLeft/FAQ_Button");
		if (HelpShiftWapper.instance != null)
		{
			standardButton.uiItem.OnClick += HelpShiftWapper.instance.ShowFAQ;
		}
		else
		{
			standardButton.uiItem.OnClick += delegate
			{
				Application.OpenURL(Konfiguration.GameConfig.faq_url);
			};
		}
		UpdateUpgradePossibilitesLabel();
		parallaxer = GetComponentInChildren<Parallaxer>();
		initChaptersAndLevels();
		parallaxer.Init();
		EnableScrolling = true;
		focusLevelButton(Player.CurrentLevelID);
		Show();
		if (Konfiguration.GameConfig.Use_upgrade_timers)
		{
			StartCoroutine(ShowTrainingCompleteNotifications());
		}
		Player.Instance.RefillRewardedVideos();
		UpdateGiftCoinsButtonState();
		if (CheckFriendGateUnlock(Player.MaxLevelID))
		{
			center_message.Key = "map.friendgate.unlocked";
			StartCoroutine(ShowAfterLoading(delegate
			{
				Go.to(center_message.transform, 1f, new GoTweenConfig().localPosition(center_message.transform.localPosition - base.top).setEaseType(GoEaseType.CubicOut).onComplete(delegate
				{
					WaitThen(3f, delegate
					{
						Go.to(center_message.transform, 1f, new GoTweenConfig().localPosition(center_message.transform.localPosition + base.top).setEaseType(GoEaseType.CubicIn));
					});
				}));
			}));
			Tracking.pass_gate(Tracking.GateMethod.waited, Player.MaxLevelID);
			StartCoroutine(ShowAfterLoading(delegate
			{
				unlockFriendGate(Player.MaxLevelID);
			}));
		}
		else if (Player.CompletedAllLevels && !MoreLevelsScreen.shownInSession)
		{
			StartCoroutine(ShowAfterLoading(delegate
			{
				ScreenManager.GetScreen<MoreLevelsScreen>().Show();
			}));
		}
		else if ((Player.MaxLevelID == Konfiguration.LevelIndexForName("level_13") || (Player.MaxLevelID > Konfiguration.LevelIndexForName("level_13") && Player.Instance.PlayerData.session % Konfiguration.GameConfig.Facebook_connect_interval == 0)) && !RewardedFBLoginScreen.shown_in_session && !Player.Instance.PlayerData.userRecievedLoginReward && App.Instance.InternetConnectivity && !App.Instance.facebookManager.isLoggedIn)
		{
			StartCoroutine(ShowAfterLoading(delegate
			{
				ScreenManager.GetScreen<RewardedFBLoginScreen>().Show();
			}));
		}
		else if (QuestManager.instance.newQuests.Count > 0)
		{
			StartCoroutine(ShowAfterLoading(delegate
			{
				ScreenManager.GetScreen<QuestScreen>().Show();
			}));
		}
		else if (Player.LooseCount > 0)
		{
			UpgradeTip();
		}
		App.Instance.facebookManager.Init(delegate
		{
			if (App.Instance.facebookManager.isLoggedIn)
			{
				App.Instance.cloudSaveGameManager.checkAndPrompt();
				StartCoroutine(PollAppRequests());
			}
		});
	}

	private void HandleofferEnded()
	{
		btn_bundle_offer.gameObject.SetActive(false);
	}

	public void UpdateGiftCoinsButtonState()
	{
		bool flag = Player.Instance.canWatchRewardedVideo(RewardedVideoItems.Coins) && RewardedVideosWrapper.hasVideo && RewardedVideosWrapper.getRewardAmount(RewardedVideoItems.Coins) > 0;
		quickBuck_Button.gameObject.SetActive(flag);
	}

	private IEnumerator PollAppRequests()
	{
		while (true)
		{
			GetAppRequests();
			yield return new WaitForSeconds(30f);
		}
	}

	private void GetAppRequests()
	{
		if (!App.Instance.facebookManager.isLoggedIn)
		{
			return;
		}
		App.Instance.facebookManager.getAppRequests(delegate(List<FacebookManager.AppRequest> app_requests)
		{
			if (app_requests != null && app_requests.Count > 0 && !Tutorials.isTutorialLevel(Player.MaxLevelID))
			{
				SelectFriendsScreen screen = ScreenManager.GetScreen<SelectFriendsScreen>();
				MapScreen screen2 = ScreenManager.GetScreen<MapScreen>();
				if (App.State == App.States.Map && screen2 != null && !screen.isVisible && EnableScrolling)
				{
					ScreenManager.GetScreen<MessageCenterScreen>().Show();
				}
				else
				{
					Debug.Log("Not Showing Message Center, because not in map screen or SelectFriendsScreen is open");
				}
			}
		});
	}

	private IEnumerator ShowTrainingCompleteNotifications()
	{
		while (Loader.instance.isVisible)
		{
			yield return null;
		}
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (current_dino == UnitType.None)
			{
				CheckForTrainingComplete();
			}
		}
	}

	private void CheckForTrainingComplete()
	{
		UnitType newlyTrainedDino = EntityTimers.GetNewlyTrainedDino();
		if (newlyTrainedDino != 0 && !upgrade_screen.Interactive)
		{
			current_dino = newlyTrainedDino;
			string text = string.Format("dino_completed_training_notification".Localize(), current_dino.ToString().Localize());
			training_notification_label.text = text;
			training_notification_label.maxChars = text.Length;
			training_notification_label.Commit();
			tk2dSlicedSprite component = training_notification_label.transform.parent.GetComponent<tk2dSlicedSprite>();
			component.dimensions = new Vector2(training_notification_label.GetComponent<Renderer>().bounds.size.x + 160f, component.dimensions.y);
			if (component.dimensions.x > 2000f)
			{
				Debug.LogError("Training Notification Localization is too long:\n" + training_notification_label.text);
			}
			Vector3 localPosition = training_notification_close_button.transform.localPosition;
			localPosition.x = component.dimensions.x * 0.5f - 12f;
			training_notification_close_button.transform.localPosition = localPosition;
			icon_portrait.sprite = SpriteRessources.getDinoBuySprite(current_dino);
			icon_portrait.transform.localPosition = icon_portrait.transform.localPosition.SetX(0f - localPosition.x - 60f);
			Go.to(training_notification_close_button.transform.parent, 0.3f, new GoTweenConfig().localPosition(training_notification_endpos).setEaseType(GoEaseType.CubicOut).onComplete(delegate
			{
				training_notification_close_button.OnClick += HandleOnNotificationClose;
				upgrade_fx.gameObject.SetActive(true);
			}));
		}
	}

	private void HandleOnNotificationClose()
	{
		training_notification_close_button.OnClick -= HandleOnNotificationClose;
		EntityTimers.RemoveFromNewlyTrainedList(current_dino);
		Go.to(training_notification_close_button.transform.parent, 0.3f, new GoTweenConfig().setEaseType(GoEaseType.CubicIn).localPosition(training_notification_endpos + base.left)).setOnCompleteHandler(delegate
		{
			training_notification_close_button.transform.parent.localPosition = training_notification_endpos + base.right;
			current_dino = UnitType.None;
			upgrade_fx.gameObject.SetActive(false);
			CheckForTrainingComplete();
		});
	}

	private IEnumerator ShowAfterLoading(Action callback)
	{
		while (Loader.instance.isVisible)
		{
			yield return null;
		}
		callback();
	}

	private void ShowRewardedVideo()
	{
		WaitThen(1f, delegate
		{
			RewardedVideoScreen screen = ScreenManager.GetScreen<RewardedVideoScreen>();
			if (!screen.isVisible)
			{
				if (Player.Lives <= RewardedVideosWrapper.Lives_threshold)
				{
					screen.Show(RewardedVideoItems.Lives);
				}
				else if (RewardedVideosWrapper.Show_on_lost.Contains(Player.LooseCount))
				{
					List<UnitType> consumables = Konfiguration.getConsumables();
					consumables = consumables.FindAll((UnitType consumable) => Player.hasUnlockedConsumable(consumable));
					if (consumables.Count > 0)
					{
						screen.Show((RewardedVideoItems)consumables[UnityEngine.Random.Range(0, consumables.Count)]);
					}
				}
			}
		});
	}

	private void UpgradeTip()
	{
		int cheapest_upgrade = int.MaxValue;
		Player.AvailableUnitTypes.FindAll((UnitType x) => !Konfiguration.isPremium(x)).ForEach(delegate(UnitType dino)
		{
			cheapest_upgrade = Mathf.Min(cheapest_upgrade, Konfiguration.getDinoUpgradeCost(dino));
		});
		Player.AvailableShotTypes.FindAll((ShotType x) => !Konfiguration.isPremium(x)).ForEach(delegate(ShotType shot)
		{
			cheapest_upgrade = Mathf.Min(cheapest_upgrade, Konfiguration.getShotUpgradeCost(shot));
		});
		Player.AvailiableUpgrades.FindAll((UnitType x) => !Konfiguration.isPremium(x)).ForEach(delegate(UnitType upgrade)
		{
			cheapest_upgrade = Mathf.Min(cheapest_upgrade, Konfiguration.getUpgradeBuyCost(upgrade));
		});
		bool flag = Tutorials.LevelID("UpgradeTutorial") < Player.MaxLevelID && Player.MaxLevelID < 18;
		if (Wallet.Coins <= cheapest_upgrade || !flag)
		{
			return;
		}
		WaitThen(1f, delegate
		{
			UpgradeScreen screen = ScreenManager.GetScreen<UpgradeScreen>();
			if (!screen.isVisible)
			{
				ScreenManager.GetScreen<UpgradeReminderScreen>().Show();
			}
		});
	}

	public void UpdateUpgradePossibilitesLabel()
	{
		tk2dTextMesh upgrade_new_label = upgradeButton.transform.Find("label_new").GetComponent<tk2dTextMesh>();
		int count = Player.getNewUpgradePossibilites.Count;
		Go.to(this, 0.2f, new GoTweenConfig().intProp("upgrade_possibilities", count).onUpdate(delegate
		{
			upgrade_new_label.text = upgrade_possibilities.ToString();
		}));
		if (count == 0)
		{
			Go.to(upgrade_new_label.transform, 0.2f, new GoTweenConfig().scale(Vector3.zero));
		}
	}

	public void SetInputLockerScreens()
	{
		inputLockerScreens.Clear();
		inputLockerScreens.Add(ScreenManager.GetScreen<UpgradeScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<ShopScreenCoins>());
		inputLockerScreens.Add(ScreenManager.GetScreen<ShopScreenDiamonds>());
		inputLockerScreens.Add(ScreenManager.GetScreen<GetLivesScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<SelectDinoScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<SelectShotScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<FriendGateScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<SelectFriendsScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<FacebookLoginRequestScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<UpgradeReminderScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<QuestScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<ErrorMessageScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<RewardedVideoScreen>());
		inputLockerScreens.Add(ScreenManager.GetScreen<BundlePromotionScreen>());
	}

	private void Update()
	{
		if (base.isVisible)
		{
			EnableScrolling = inputLockerScreens.FindIndex((BaseScreen x) => x.isVisible) == -1;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		base.Hide();
		StopAllCoroutines();
		upgradeButton.uiItem.OnClick -= onUpgradeButtonClick;
		backButton.OnClick -= onBackButtonClick;
	}

	private void OnClickedLevel(tk2dUIItem uiitem)
	{
		LevelButton component = uiitem.transform.parent.parent.GetComponent<LevelButton>();
		LevelData levelData = Konfiguration.GetLevelData(component.Level_ID);
		if (levelData.is_friend_gate)
		{
			ScreenManager.GetScreen<FriendGateScreen>().Show(component.Level_ID);
		}
		else if (Player.Lives > 0)
		{
			ScreenManager.GetScreen<ResourceBarScreen>().Hide();
			if (Player.AvailableUnitTypes.Count > 4 && !levelData.override_dino_selection)
			{
				ScreenManager.GetScreen<CoverScreen>(ScreenManager.GetScreen<SelectDinoScreen>()).Show();
				ScreenManager.GetScreen<SelectDinoScreen>().Show(component.Level_ID, base.right);
				return;
			}
			if (Player.AvailableShotTypes.Count > Player.shotSlots && !levelData.override_shots_selection)
			{
				Player.SelectedUnitTypes = new SerializableList<UnitType>(Player.AvailableUnitTypes);
				ScreenManager.GetScreen<CoverScreen>(ScreenManager.GetScreen<SelectShotScreen>()).Show();
				ScreenManager.GetScreen<SelectShotScreen>().Show(component.Level_ID, base.right);
				return;
			}
			if (!levelData.override_shots_selection)
			{
				Player.SelectedShotTypes = new SerializableList<ShotType>(Player.AvailableShotTypes);
			}
			if (!levelData.override_dino_selection)
			{
				Player.SelectedUnitTypes = new SerializableList<UnitType>(Player.AvailableUnitTypes);
			}
			App.stateGame(component.Level_ID);
		}
		else
		{
			ScreenManager.GetScreen<GetLivesScreen>().Show();
		}
	}

	private void onUpgradeButtonClick()
	{
		Tracking.upgrades_open();
		ScreenManager.GetScreen<UpgradeScreen>().Show(UpgradeScreen.Tab.dinos);
	}

	private void onBackButtonClick()
	{
		base.Hide();
		App.stateStartScreen();
	}

	public override void Show()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(true);
		}
		OnEscapeUp = delegate
		{
			if (EnableScrolling)
			{
				onBackButtonClick();
			}
		};
		base.Show();
	}

	public override void Hide()
	{
		base.Hide();
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(false);
		}
	}

	private IEnumerator ShowLevelUnlockProgress(float delay)
	{
		if (Player.MaxLevelID >= 1 && Player.MaxLevelID != player_max_levelid && !Player.CompletedAllLevels && !Konfiguration.levels[Player.MaxLevelID - 1].is_friend_gate)
		{
			player_max_levelid = Player.MaxLevelID;
			LevelButton lb = level_buttons[Player.MaxLevelID];
			LevelButton lb_prev = level_buttons[Player.MaxLevelID - 1];
			lb_prev.transform.Find("top/LevelButtonWin").GetComponent<Animator>().Play("level_button_no_haekchen");
			yield return new WaitForSeconds(delay);
			lb_prev.transform.Find("top/LevelButtonWin").GetComponent<Animator>().Play("win in");
			Vector3 start_pos = lb_prev.selection.position;
			Vector3 end_pos = lb.selection.position;
			lb.selection.position = start_pos;
			Go.to(lb.selection, 0.5f, new GoTweenConfig().position(end_pos).setEaseType(GoEaseType.CubicInOut));
		}
	}

	public void focusLevelButton(int level_id, bool animated = false)
	{
		level_buttons.ForEach(delegate(LevelButton lb)
		{
			lb.focused = lb.Level_ID == Player.MaxLevelID;
		});
		if (level_id < level_buttons.Count)
		{
			LevelButton levelButton = level_buttons[level_id];
			levelButton.clickable = true;
			ParallaxLayer component = levelButton.transform.parent.GetComponent<ParallaxLayer>();
			if (animated)
			{
				parallaxer.CamXAnimationTarget = parallaxer.CamX - levelButton.transform.position.x / component.speed;
			}
			else
			{
				parallaxer.CamX = (0f - levelButton.transform.position.x) / component.speed;
			}
		}
		else
		{
			Debug.LogWarning("button for level " + level_id + " not found");
		}
	}

	private IEnumerator fadeIsland(SpriteRenderer sr)
	{
		float duration = 5f;
		float t = duration;
		while (t > 0f)
		{
			t -= Time.deltaTime;
			float r = t / duration;
			sr.material.SetFloat("EffectFade", r);
			yield return null;
		}
	}

	private bool CheckFriendGateUnlock(int level_id)
	{
		if (!Konfiguration.GetLevelData(level_id).is_friend_gate)
		{
			return false;
		}
		TimeSpan time_span;
		if (!Player.Instance.getFriendGateDuration(level_id, out time_span))
		{
			Player.Instance.startFriendgateTimer();
			return false;
		}
		if (time_span.TotalMilliseconds <= 0.0)
		{
			return true;
		}
		return false;
	}

	public void unlockFriendGate(int level_id)
	{
		Player.Instance.resetFriendGate();
		Player.CompletedLevel(level_id);
		Player.CurrentLevelID = Player.MaxLevelID;
		getLevelButton(level_id).Level_ID++;
		getLevelButton(level_id).clickable = true;
		int num = 2;
		getLevelButton(level_id + num).clickable = true;
		focusLevelButton(level_id + num, true);
		getLevelButton(level_id + num).levelComplete = false;
		ParallaxLayer alignedTo = getLevelButton(level_id + num).transform.parent.GetComponent<ParallaxLayer>().alignedTo;
		SpriteRenderer[] componentsInChildren = alignedTo.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sr in componentsInChildren)
		{
			StartCoroutine(fadeIsland(sr));
		}
		Transform transform = base.transform.Find("Map");
		Transform transform2 = transform.Find("bg_islands/" + alignedTo.name);
		SpriteRenderer[] componentsInChildren2 = transform2.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sr2 in componentsInChildren2)
		{
			StartCoroutine(fadeIsland(sr2));
		}
		MapTutorial dialogue_screen = ScreenManager.GetScreen<MapTutorial>();
		getFriendGate(level_id).unlock(delegate
		{
			StartCoroutine(dialogue_screen.StartDialogue(Player.MaxLevelID));
		});
	}

	private FriendGate getFriendGate(int level_id)
	{
		string text = Konfiguration.GetLevelData(level_id).name;
		Transform transform = base.transform.Find("Map");
		return transform.Find("friendgates/" + text).GetComponent<FriendGate>();
	}

	private void setFriendGateState(int level_id, bool locked)
	{
		getFriendGate(level_id).locked = locked;
	}

	public LevelButton getLevelButton(int level_id)
	{
		return level_buttons[level_id];
	}

	private void initChaptersAndLevels()
	{
		Transform transform = base.transform.Find("Map");
		Transform transform2 = transform.Find("buttons");
		Transform transform3 = transform2.Find("LevelButton");
		int num = 0;
		int num2 = 0;
		for (int i = 0; i != Konfiguration.chapters.Count; i++)
		{
			ChapterData chapterData = Konfiguration.chapters[i];
			Transform transform4 = transform.Find("chapters/chapter_" + i);
			num2 += chapterData.levelCount;
			GameObject gameObject = new GameObject();
			gameObject.name = transform4.name;
			float num3 = transform4.GetComponent<ParallaxLayer>().bounds.extents.x * 2f;
			float num4 = num3 / (float)chapterData.levelCount;
			num4 += 300f;
			gameObject.transform.parent = transform2.transform;
			gameObject.transform.localPosition = Vector3.zero;
			for (int j = 0; j != chapterData.levelCount; j++)
			{
				if (Konfiguration.GetLevelData(num).endless_mode && i < Konfiguration.chapters.Count - 1)
				{
					level_buttons.Add(level_buttons[j - 1]);
					num++;
					continue;
				}
				Transform transform5 = UnityEngine.Object.Instantiate(transform3) as Transform;
				transform5.name = "level_button_" + num;
				transform5.LocalPosX((float)j * num4);
				transform5.LocalPosY((j % 2 != 0) ? (-290) : (-120));
				transform5.transform.parent = gameObject.transform;
				transform5.LocalPosZ(0f);
				LevelButton component = transform5.GetComponent<LevelButton>();
				level_buttons.Add(component);
				component.Start();
				component.button.uiItem.OnClickUIItem += OnClickedLevel;
				if (Konfiguration.GetLevelData(num).is_friend_gate)
				{
					setFriendGateState(num, Player.MaxLevelID <= num);
					if (Player.MaxLevelID > num)
					{
						component.Level_ID = num + 1;
						num++;
						continue;
					}
				}
				component.Level_ID = num;
				num++;
			}
			ParallaxLayer parallaxLayer = gameObject.AddComponent<ParallaxLayer>();
			parallaxLayer.allignTo(transform4.GetComponent<ParallaxLayer>());
		}
		transform3.gameObject.SetActive(false);
	}
}
