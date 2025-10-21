using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class HudScreen : BaseScreen
{
	public enum State
	{
		none = 0,
		movingIn = 1,
		In = 2,
		movingOut = 3,
		Out = 4
	}

	private const float min_bar_width = 35f;

	private const float max_bar_width = 525f;

	private const float applebar_height = 90f;

	private const float barAdaptSpeed = 2.5f;

	private const float MAXSHOTBAROUT = 5f;

	private tk2dUIItem pauseButton;

	private StandardButton appleLevelUpgradeButton;

	private List<tk2dCameraAnchor> button_anchors = new List<tk2dCameraAnchor>();

	private SpriteRenderer apple_sprite;

	private tk2dSlicedSprite appleCountBar;

	private tk2dSlicedSprite appleLevelBar;

	private tk2dTextMesh apple_upgrade_cost;

	private SpriteRenderer apple_bar_marker;

	private ParticleSystem appleUpgradeFx;

	private tk2dTextMesh apple_level_label;

	private tk2dTextMesh skipIntroLabel;

	private GoTween labelBlink;

	private GoTweenFlow tweenFlow;

	private Vector3 smallButtonSpacing = new Vector3(195f, 0f, 0f);

	private Vector3 largeButtonSpacing = new Vector3(240f, 0f, 0f);

	private Animator hit_flash;

	private tk2dTextMesh timer_label;

	private GoTween consumables_bar_tween;

	private GoTween apple_bar_tween;

	private GoTweenFlow shot_buttons_flow;

	private float duration = -1f;

	private bool is_showing_hitflash;

	private float shotButtonYPos;

	private int maxUpgradeCost;

	private int next_score_update;

	private GoTween apple_wobble_tween;

	private GoTweenConfig wooble_config;

	private float lastHideShotbar;

	private IEnumerator verfiy_routine;

	public State state { get; private set; }

	public List<BuyUnitButton> UnitButtons { get; private set; }

	public List<BuyUnitButton> ConsumableButtons { get; private set; }

	public List<ShootButton> ShotButtons { get; private set; }

	private int CurrentUpgradeCost
	{
		get
		{
			if (Player.Instance.AppleCollectLevel == Konfiguration.GameConfig.AppleUpgradeCost.Length)
			{
				return Konfiguration.GameConfig.AppleUpgradeCost[Player.Instance.AppleCollectLevel - 1];
			}
			return Konfiguration.GameConfig.AppleUpgradeCost[Player.Instance.AppleCollectLevel];
		}
	}

	public int ScoreTweenCount { get; set; }

	private float levelBarWidth
	{
		get
		{
			return Mathf.Lerp(35f, 525f, (float)(CurrentUpgradeCost + 20) / (float)maxUpgradeCost);
		}
	}

	public State shotbarState { get; private set; }

	protected virtual void Start()
	{
		if (state != 0)
		{
			return;
		}
		state = State.Out;
		shotbarState = State.In;
		skipIntroLabel = base.transform.Search("skipIntroLabel").GetComponent<tk2dTextMesh>();
		timer_label = base.transform.Search("label_timer").GetComponent<tk2dTextMesh>();
		timer_label.transform.parent.gameObject.SetActive(Level.Instance.Config.endless_mode);
		pauseButton = base.transform.Search("Menu_Button").GetComponent<tk2dUIItem>();
		pauseButton.OnClick += delegate
		{
			ScreenManager.GetScreen<PauseScreen>().Show();
		};
		apple_sprite = base.transform.Search("applebar_apple").GetComponent<SpriteRenderer>();
		appleLevelUpgradeButton = base.transform.Search("Upgrade_Button").GetComponent<StandardButton>();
		appleLevelUpgradeButton.uiItem.OnClick += UpgradeAppleLevel;
		appleUpgradeFx = base.transform.Search("UpgradeFX").GetComponent<ParticleSystem>();
		apple_level_label = base.transform.Search("apple_level_label").GetComponent<tk2dTextMesh>();
		apple_level_label.text = "apple_level_rank".Localize() + " " + (Player.Instance.AppleCollectLevel + 1);
		hit_flash = FindChildComponent<Animator>("hitflash");
		EntityFactory.Dino_Egg.OnEntityHit += OnEggHit;
		SetupShotButtons();
		SetupBuyUnitButtons();
		SetupConsumableButtons();
		ScoreTweenCount = 0;
		maxUpgradeCost = Konfiguration.GameConfig.AppleUpgradeCost[Konfiguration.GameConfig.AppleUpgradeCost.Length - 1] + 20;
		apple_bar_marker = base.transform.Search("applebar_marker").GetComponent<SpriteRenderer>();
		apple_upgrade_cost = base.transform.Search("apple_upgrade_cost").GetComponent<tk2dTextMesh>();
		apple_upgrade_cost.text = Konfiguration.GameConfig.AppleUpgradeCost[0].ToString();
		appleLevelBar = base.transform.Search("applebar_level").GetComponent<tk2dSlicedSprite>();
		appleLevelBar.dimensions = new Vector2(levelBarWidth, 90f);
		appleCountBar = base.transform.Search("applebar").GetComponent<tk2dSlicedSprite>();
		appleCountBar.dimensions = new Vector2(35f, 90f);
		SetAnchors();
		SetButtonAnchors();
		SetupTweens();
		if (Level.Instance.Config.override_shots_selection || Level.Instance.Config.override_shots_selection)
		{
			appleCountBar.transform.parent.parent.gameObject.SetActive(false);
		}
		if (!Level.Instance.tutorialMode)
		{
			GoTweenConfig goTweenConfig = new GoTweenConfig().colorProp("color", Colors.Invisible).setIterations(-1).onUpdate(delegate
			{
				skipIntroLabel.Commit();
			});
			goTweenConfig.loopType = GoLoopType.PingPong;
			labelBlink = Go.to(skipIntroLabel, 0.5f, goTweenConfig);
		}
		else
		{
			skipIntroLabel.renderer.enabled = false;
		}
		base.Interactive = false;
	}

	private void SetupTweens()
	{
		GoTweenConfig goTweenConfig = new GoTweenConfig().vector2Prop("AnchorOffsetPixels", new Vector2(0f, 512f)).setEaseType(GoEaseType.CubicIn).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate);
		apple_bar_tween = Go.to(button_anchors[0], 0.3f, goTweenConfig);
		apple_bar_tween.autoRemoveOnComplete = false;
		apple_bar_tween.pause();
		consumables_bar_tween = Go.to(button_anchors[1], 0.3f, goTweenConfig);
		consumables_bar_tween.autoRemoveOnComplete = false;
		consumables_bar_tween.pause();
		tweenFlow = new GoTweenFlow(new GoTweenCollectionConfig().setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
		tweenFlow.autoRemoveOnComplete = false;
		goTweenConfig.clearProperties();
		goTweenConfig.vector2Prop("AnchorOffsetPixels", Vector2.zero);
		goTweenConfig.setEaseType(GoEaseType.CubicOut);
		float num = 0.1f;
		for (int i = 0; i < button_anchors.Count; i++)
		{
			GoTween goTween = Go.to(button_anchors[i], 0.3f, goTweenConfig);
			goTween.autoRemoveOnComplete = false;
			goTween.pause();
			tweenFlow.insert((float)i * num, goTween);
		}
	}

	private void SetShootButtonOutTween()
	{
		shot_buttons_flow = new GoTweenFlow(new GoTweenCollectionConfig().setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
		shot_buttons_flow.autoRemoveOnComplete = false;
		float num = 0f;
		List<ShootButton> shotButtons = ShotButtons;
		shotButtons.Reverse();
		foreach (ShootButton item in shotButtons)
		{
			GoTweenConfig config = new GoTweenConfig().setEaseType(GoEaseType.CubicIn).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate).localPosition(item.transform.parent.localPosition.SetY(-256f))
				.setDelay(num);
			GoTween goTween = Go.to(item.transform.parent, 0.2f, config);
			goTween.autoRemoveOnComplete = false;
			shot_buttons_flow.insert(num, goTween);
			num += 0.1f;
		}
	}

	private void SetButtonAnchors()
	{
		button_anchors.Add(FindChildComponent<tk2dCameraAnchor>("UpperLeft"));
		button_anchors.Add(FindChildComponent<tk2dCameraAnchor>("UpperCenter"));
		button_anchors.Add(FindChildComponent<tk2dCameraAnchor>("UpperRight"));
		button_anchors.Add(FindChildComponent<tk2dCameraAnchor>("LowerLeft"));
		button_anchors.Add(FindChildComponent<tk2dCameraAnchor>("LowerRight"));
		button_anchors.ForEach(delegate(tk2dCameraAnchor anchor)
		{
			anchor.AnchorOffsetPixels = GetAnchorOutPosition(anchor);
		});
	}

	private Vector2 GetAnchorOutPosition(tk2dCameraAnchor anchor)
	{
		float width = ScreenManager.Camera.GetComponent<tk2dCamera>().ScreenExtents.width;
		switch (anchor.AnchorPoint)
		{
		case tk2dBaseSprite.Anchor.LowerLeft:
		case tk2dBaseSprite.Anchor.UpperLeft:
			return new Vector2((0f - width) * 0.75f, 0f);
		case tk2dBaseSprite.Anchor.LowerRight:
		case tk2dBaseSprite.Anchor.UpperRight:
			return new Vector2(width * 1.5f, 0f);
		case tk2dBaseSprite.Anchor.UpperCenter:
			return new Vector2(0f, 1536f);
		default:
			return Vector2.zero;
		}
	}

	private void OnEggHit(BaseEntity dino_egg)
	{
		if (!is_showing_hitflash)
		{
			hit_flash.Play("hit");
			if (duration == -1f)
			{
				duration = hit_flash.GetCurrentAnimationClipState(0)[0].clip.length;
			}
			WaitThen(duration, delegate
			{
				is_showing_hitflash = false;
			});
		}
	}

	private void SetupBuyUnitButtons()
	{
		BuyUnitButton component = base.transform.Search("btnBuyUnitDefinition").GetComponent<BuyUnitButton>();
		UnitButtons = new List<BuyUnitButton>();
		int num = 0;
		foreach (UnitType availableDino in Level.Instance.AvailableDinos)
		{
			if (availableDino == UnitType.None)
			{
				break;
			}
			BuyUnitButton buyUnitButton = UnityEngine.Object.Instantiate(component) as BuyUnitButton;
			buyUnitButton.transform.parent = component.transform.parent;
			buyUnitButton.transform.localPosition = component.transform.localPosition + num * largeButtonSpacing;
			buyUnitButton.name = availableDino.ToString();
			buyUnitButton.Init(availableDino);
			UnitButtons.Add(buyUnitButton);
			num++;
		}
		UnityEngine.Object.Destroy(component.gameObject);
	}

	private void SetupConsumableButtons()
	{
		BuyUnitButton component = base.transform.Search("btnConsumableDefintion").GetComponent<BuyUnitButton>();
		ConsumableButtons = new List<BuyUnitButton>();
		List<UnitType> consumables = Konfiguration.getConsumables();
		consumables = consumables.FindAll((UnitType consumable) => Player.hasUnlockedConsumable(consumable));
		for (int i = 0; i < consumables.Count; i++)
		{
			BuyUnitButton buyUnitButton = UnityEngine.Object.Instantiate(component) as BuyUnitButton;
			buyUnitButton.transform.parent = component.transform.parent;
			buyUnitButton.transform.localPosition = component.transform.localPosition - i * smallButtonSpacing;
			buyUnitButton.name = consumables[i].ToString();
			buyUnitButton.Init(consumables[i]);
			ConsumableButtons.Add(buyUnitButton);
		}
		UnityEngine.Object.Destroy(component.gameObject);
	}

	private void SetupShotButtons()
	{
		Transform parent = GetComponentInChildren<ShootButton>().transform.parent;
		shotButtonYPos = parent.transform.localPosition.y;
		Vector3 vector = smallButtonSpacing;
		float num = (float)Screen.width / (float)Screen.height;
		if (num > 1.3333334f)
		{
			parent.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			vector = largeButtonSpacing;
		}
		int num2 = 0;
		ShotButtons = new List<ShootButton>();
		List<ShotType> list = new List<ShotType>(Level.Instance.AvailableShots);
		list.Reverse();
		foreach (ShotType item in list)
		{
			if (item == ShotType.None)
			{
				break;
			}
			ShootButton componentInChildren = (UnityEngine.Object.Instantiate(parent) as Transform).GetComponentInChildren<ShootButton>();
			componentInChildren.transform.parent.parent = parent.transform.parent;
			componentInChildren.transform.parent.localPosition = parent.transform.localPosition - num2 * vector;
			componentInChildren.name = item.ToString();
			componentInChildren.Init(item);
			ShotButtons.Add(componentInChildren);
			num2++;
		}
		UnityEngine.Object.Destroy(parent.gameObject);
	}

	private void OnIntroEnd()
	{
		if (!Level.Instance.tutorialMode)
		{
			labelBlink.destroy();
			Go.to(skipIntroLabel, 0.5f, new GoTweenConfig().colorProp("color", Colors.Invisible).onUpdate(delegate
			{
				skipIntroLabel.Commit();
			}).onComplete(delegate
			{
				skipIntroLabel.renderer.enabled = false;
			}));
		}
	}

	private void OnEnable()
	{
		Level.Instance.OnLevelPlay += Show;
		Level.Instance.OnLevelLost += OnLevelEnd;
		Level.Instance.OnLevelWon += OnLevelEnd;
		Level.Instance.OnLevelAbort += OnLevelEnd;
		ShotFactory.OnShotStart = (Action)Delegate.Combine(ShotFactory.OnShotStart, new Action(HideShotBar));
		ShotFactory.OnShotEnd = (Action)Delegate.Combine(ShotFactory.OnShotEnd, new Action(ShowShotBar));
	}

	private void OnDisable()
	{
		Level.Instance.OnLevelPlay -= Show;
		Level.Instance.OnLevelLost -= Hide;
		Level.Instance.OnLevelWon -= Hide;
		Level.Instance.OnLevelAbort -= OnLevelEnd;
		ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(HideShotBar));
		ShotFactory.OnShotEnd = (Action)Delegate.Remove(ShotFactory.OnShotEnd, new Action(ShowShotBar));
	}

	private void OnLevelEnd()
	{
		ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(HideShotBar));
		ShotFactory.OnShotEnd = (Action)Delegate.Remove(ShotFactory.OnShotEnd, new Action(ShowShotBar));
		EntityFactory.Dino_Egg.OnEntityHit -= OnEggHit;
		Hide();
	}

	private void Update()
	{
		if (shotbarState == State.Out && Time.time > lastHideShotbar + 5f)
		{
			ShowShotBar();
		}
		if (Level.Instance.Config.endless_mode && Level.Instance.state == Level.State.playing)
		{
			int endlessScore = Level.Instance.EndlessScore;
			if (endlessScore >= next_score_update)
			{
				UpdateScoreLabel(endlessScore);
				next_score_update = endlessScore + 50;
			}
		}
		UpdateAppleLevel();
	}

	private void UpdateScoreLabel(int score)
	{
		Go.to(this, 0.2f, new GoTweenConfig().intProp("ScoreTweenCount", score).onUpdate(delegate
		{
			timer_label.text = ScoreTweenCount.ToString();
		}));
	}

	private void UpdateAppleLevel()
	{
		appleLevelBar.dimensions = Vector2.Lerp(appleLevelBar.dimensions, new Vector2(levelBarWidth, 90f), Time.deltaTime * 2.5f);
		float x = Helper.MapRange(0f, CurrentUpgradeCost + 20, 35f, levelBarWidth, Player.Instance.Apples);
		appleCountBar.dimensions = Vector2.Lerp(appleCountBar.dimensions, new Vector2(x, 90f), Time.deltaTime * 2.5f);
		float to = Mathf.Lerp(35f, 525f, (float)CurrentUpgradeCost / (float)maxUpgradeCost);
		apple_bar_marker.transform.localPosition = apple_bar_marker.transform.LocalPosX(Mathf.Lerp(apple_bar_marker.transform.localPosition.x, to, Time.deltaTime * 2.5f));
		bool flag = Player.Instance.Apples >= CurrentUpgradeCost && Player.Instance.AppleCollectLevel < Konfiguration.GameConfig.AppleUpgradeCost.Length - 1;
		if (Player.Instance.AppleCollectLevel != Konfiguration.GameConfig.AppleCollectRate.Length - 1)
		{
			appleLevelUpgradeButton.Enabled = flag;
		}
	}

	public void UpgradeAppleLevel()
	{
		Start();
		Player.Instance.Apples -= CurrentUpgradeCost;
		Player.Instance.AppleCollectLevel++;
		if (apple_wobble_tween == null)
		{
			wooble_config = new GoTweenConfig();
			wooble_config.loopType = GoLoopType.PingPong;
			wooble_config.iterations = 2;
			wooble_config.easeType = GoEaseType.CubicOut;
			apple_wobble_tween = Go.to(apple_sprite.transform, 0.25f, wooble_config.scale(1.2f));
		}
		apple_wobble_tween.play();
		appleUpgradeFx.Play();
		WaitThen(appleUpgradeFx.duration, appleUpgradeFx.Stop);
		apple_level_label.text = "apple_level_rank".Localize() + " " + (Player.Instance.AppleCollectLevel + 1);
		wooble_config.setDelay(0.05f);
		Go.to(apple_level_label.transform, 0.2f, wooble_config.scale(1.1f));
		if (Player.Instance.AppleCollectLevel == Konfiguration.GameConfig.AppleCollectRate.Length - 1)
		{
			apple_bar_marker.enabled = false;
			appleLevelUpgradeButton.uiItem.enabled = false;
			appleLevelUpgradeButton.uiItem.OnClick -= UpgradeAppleLevel;
			apple_upgrade_cost.text = "max".Localize();
			apple_upgrade_cost.anchor = TextAnchor.MiddleCenter;
			apple_upgrade_cost.transform.LocalPosX(0f);
			apple_upgrade_cost.transform.parent.FindChild("icon_apple").gameObject.SetActive(false);
			SocialGamingManager.Instance.ReportProgress(AchievementIds.COMPLETELY_UPGRADE_YOUR_APPLE_COUNTER, 1);
		}
		else
		{
			apple_upgrade_cost.text = Konfiguration.GameConfig.AppleUpgradeCost[Player.Instance.AppleCollectLevel].ToString();
		}
	}

	private void ShowShotBar()
	{
		if (shotbarState == State.Out)
		{
			shotbarState = State.In;
			shot_buttons_flow.complete();
			shot_buttons_flow.playBackwards();
			apple_bar_tween.complete();
			apple_bar_tween.playBackwards();
			consumables_bar_tween.complete();
			consumables_bar_tween.playBackwards();
			verfiy_routine = VerfiyShotButtonsPosition();
			StartCoroutine(verfiy_routine);
		}
	}

	private IEnumerator VerfiyShotButtonsPosition()
	{
		yield return new WaitForSeconds((float)ShotButtons.Count * 0.1f + 0.1f);
		ShotButtons.ForEach(delegate(ShootButton shot)
		{
			shot.transform.parent.transform.LocalPosY(shotButtonYPos);
		});
	}

	private void HideShotBar()
	{
		if (!ShotFactory.Current.isAutoShot && shotbarState == State.In)
		{
			shotbarState = State.Out;
			if (shot_buttons_flow == null)
			{
				SetShootButtonOutTween();
				shot_buttons_flow.playForward();
			}
			else
			{
				shot_buttons_flow.complete();
				shot_buttons_flow.playForward();
			}
			apple_bar_tween.playForward();
			consumables_bar_tween.playForward();
			lastHideShotbar = Time.time;
			if (verfiy_routine != null)
			{
				StopCoroutine(verfiy_routine);
			}
		}
	}

	public override void Show()
	{
		if (state == State.In || state == State.movingIn)
		{
			return;
		}
		if (state == State.none)
		{
			Start();
		}
		state = State.movingIn;
		OnEscapeUp = delegate
		{
			if (state == State.In && !ScreenManager.GetScreen<PauseScreen>().isVisible && !ScreenManager.GetScreen<RefillConsumablesScreen>().isVisible && !ScreenManager.GetScreen<DinoRageScreen>().isVisible && !ScreenManager.GetScreen<ShopScreen>().isVisible && !ScreenManager.GetScreen<QuitLevelRequestScreen>().isVisible)
			{
				ScreenManager.GetScreen<PauseScreen>().Show();
			}
		};
		base.Show();
		tweenFlow.setOnCompleteHandler(delegate
		{
			state = State.In;
			base.Interactive = true;
		});
		tweenFlow.playForward();
		OnIntroEnd();
	}

	public override void Hide()
	{
		if (state != State.Out && state != State.movingOut)
		{
			state = State.movingOut;
			base.Hide();
			base.Interactive = false;
			tweenFlow.setOnCompleteHandler(delegate
			{
				state = State.Out;
			});
			tweenFlow.playBackwards();
		}
	}
}
