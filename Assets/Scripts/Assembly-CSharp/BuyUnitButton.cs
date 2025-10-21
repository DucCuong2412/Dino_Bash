using UnityEngine;
using dinobash;

public class BuyUnitButton : StandardButton
{
	private int cost;

	private float timer;

	private float cooldown;

	private bool is_unique;

	private Transform loaderSprite;

	private SpriteRenderer unitSprite;

	private bool isInitalized;

	private tk2dTextMesh cost_label;

	private SpriteRenderer apple_icon;

	private tk2dTextMesh max_label;

	private tk2dTextMesh icon_plus;

	private GoTween cosumable_buy_hint;

	private GoTweenFlow apple_cost_tween;

	private tk2dTextMesh apple_cost_label;

	public UnitType Unit { get; private set; }

	public bool disable { get; set; }

	public bool ignoreCommand { get; set; }

	public void Init(UnitType unit)
	{
		if (isInitalized)
		{
			return;
		}
		Unit = unit;
		base.uiItem.OnClick += OnClick;
		disable = false;
		ignoreCommand = false;
		is_unique = Konfiguration.UnitData[unit].isUnique;
		cost = Konfiguration.getEntitiyAppleCost(unit);
		cost_label = FindChildComponent<tk2dTextMesh>("label");
		cost_label.text = cost.ToString();
		if (Konfiguration.isConsumable(unit))
		{
			Transform transform = base.transform.Find("dinobuy_panel");
			GoTweenConfig goTweenConfig = new GoTweenConfig();
			goTweenConfig.scale(1.5f);
			goTweenConfig.localRotation(new Vector3(0f, 0f, -15f));
			goTweenConfig.setIterations(2);
			goTweenConfig.setEaseType(GoEaseType.CubicOut);
			goTweenConfig.loopType = GoLoopType.PingPong;
			cosumable_buy_hint = Go.to(transform, 0.5f, goTweenConfig);
			cosumable_buy_hint.autoRemoveOnComplete = false;
			cosumable_buy_hint.pause();
			icon_plus = FindChildComponent<tk2dTextMesh>("dinobuy_panel/icon_plus");
			icon_plus.GetComponent<Renderer>().enabled = false;
			cost_label.transform.parent = transform;
			cost_label.text = Player.getConsumableCount(unit).ToString();
			Player.OnConsumableRefill += HandleOnConsumableRefill;
			SetConsumableLabel();
		}
		else
		{
			apple_icon = base.transform.Find("dinobuy_panel/icon_apple").GetComponent<SpriteRenderer>();
			max_label = base.transform.Find("dinobuy_panel/label_max").GetComponent<tk2dTextMesh>();
			max_label.GetComponent<Renderer>().enabled = false;
			Transform apple_cost_panel = base.transform.Find("apple_cost");
			apple_cost_label = FindChildComponent<tk2dTextMesh>("apple_cost/label");
			SpriteRenderer target = FindChildComponent<SpriteRenderer>("apple_cost/icon_apple");
			GoTweenConfig config = new GoTweenConfig().colorProp("color", Colors.Invisible);
			apple_cost_tween = new GoTweenFlow();
			apple_cost_tween.autoRemoveOnComplete = false;
			apple_cost_tween.setOnBeginHandler(delegate
			{
				apple_cost_panel.gameObject.SetActive(true);
			});
			apple_cost_tween.setOnCompleteHandler(delegate
			{
				apple_cost_panel.gameObject.SetActive(false);
			});
			apple_cost_tween.insert(0f, Go.to(apple_cost_panel, 1f, new GoTweenConfig().localPosition(new Vector3(0f, 220f)).setEaseType(GoEaseType.CubicOut)));
			apple_cost_tween.insert(0.8f, Go.to(apple_cost_label, 0.2f, config));
			apple_cost_tween.insert(0.8f, Go.to(target, 0.2f, config));
			apple_cost_panel.gameObject.SetActive(false);
		}
		cooldown = Konfiguration.UnitData[unit].buildcooldown;
		unitSprite = GetComponent<SpriteRenderer>();
		Sprite spiteForUnitType = SpriteRessources.getSpiteForUnitType(Unit);
		if (unitSprite != null)
		{
			_normal = spiteForUnitType;
			_pressed = _normal;
			_disabled = _normal;
			sprite.sprite = _normal;
		}
		loaderSprite = base.transform.Find("cooldown");
		loaderSprite.localScale = new Vector3(1f, 0f, 1f);
		clickSound = Sounds.None;
		isInitalized = true;
	}

	private void HandleOnConsumableRefill(UnitType consumable)
	{
		if (consumable == Unit)
		{
			cost_label.text = Player.getConsumableCount(consumable).ToString();
			SetConsumableLabel();
		}
	}

	private void OnDestroy()
	{
		Player.OnConsumableRefill -= HandleOnConsumableRefill;
	}

	protected void Update()
	{
		if (!isInitalized)
		{
			return;
		}
		bool flag = Konfiguration.isConsumable(Unit) || Player.Instance.Apples >= cost;
		if (is_unique)
		{
			bool flag2 = EntityFactory.OnStageCount[Unit] > 0;
			if (flag2)
			{
				flag = false;
			}
			SetLabelActive(!flag2);
		}
		bool flag3 = Player.getConsumableCount(Unit) == 0;
		bool flag4 = (flag && !disable && Time.time > timer) || flag3;
		if (base.Enabled != flag4)
		{
			base.Enabled = flag4;
		}
		if (loaderSprite.gameObject.activeSelf)
		{
			if (Time.time < timer)
			{
				float y = Mathf.Clamp01(Helper.MapRange(cooldown, 0f, 1f, 0f, timer - Time.time));
				loaderSprite.localScale = new Vector3(1f, y, 1f);
			}
			else
			{
				loaderSprite.gameObject.SetActive(false);
			}
		}
	}

	private void SetConsumableLabel()
	{
		int consumableCount = Player.getConsumableCount(Unit);
		bool flag = consumableCount == 0;
		cost_label.GetComponent<Renderer>().enabled = !flag;
		icon_plus.GetComponent<Renderer>().enabled = flag;
		if (consumableCount == 0)
		{
			cosumable_buy_hint.restart();
		}
	}

	private void OnClick()
	{
		if (!ignoreCommand && base.Enabled)
		{
			if (!Konfiguration.isConsumable(Unit))
			{
				AudioPlayer.PlayGuiSFX(AudioResources.GetDinoSelectSound(Unit), 0f);
				Player.Instance.Apples -= cost;
				apple_cost_label.text = (-cost).ToString();
				apple_cost_tween.restart();
				Spawn();
				QuestManager.instance.ReportProgress(QuestObjective.Deploy_Dino, 1);
				UpdateAchievements();
			}
			else if (Player.getConsumableCount(Unit) > 0)
			{
				AudioPlayer.PlayGuiSFX(AudioResources.GetDinoSelectSound(Unit), 0f);
				Player.changeConsumableCount(Unit, -1);
				cost_label.text = Player.getConsumableCount(Unit).ToString();
				SetConsumableLabel();
				CameraShake.Shake(CameraShake.Intensity.strong);
				Spawn();
				Level.Instance.usedConsumableCount++;
				Tracking.consumable_used(Unit, Player.CurrentLevelID);
				UpdateAchievements();
			}
			else
			{
				ScreenManager.GetScreen<HudScreen>().Hide();
				ScreenManager.GetScreen<RefillConsumablesScreen>().Show(Unit);
			}
		}
	}

	private void SetLabelActive(bool state)
	{
		cost_label.GetComponent<Renderer>().enabled = state;
		apple_icon.enabled = state;
		max_label.GetComponent<Renderer>().enabled = !state;
	}

	private void Spawn()
	{
		EntityFactory.Create(Unit);
		timer = Time.time + cooldown;
		loaderSprite.gameObject.SetActive(true);
	}

	private void UpdateAchievements()
	{
		if (Unit == UnitType.MegaBall && Player.MaxLevelID > Tutorials.LevelID("MegaBallTutorial"))
		{
			SocialGamingManager.Instance.ReportProgress(AchievementIds.RATE_DINO_BASH, 1);
			SocialGamingManager.Instance.ReportProgress(AchievementIds.USE_FIVE_MEGABALLS, 1, 5);
			SocialGamingManager.Instance.ReportProgress(AchievementIds.USE_20_MEGABALLS, 1, 20);
		}
		if (Konfiguration.isDinoUnit(Unit))
		{
			SocialGamingManager.Instance.ReportProgress(AchievementIds.DEPLOY_500_DINOSAURS, 1, 500);
		}
		if (Unit == UnitType.Raptor)
		{
			SocialGamingManager.Instance.ReportProgress(AchievementIds.DEPLOY_50_RAPTORS, 1, 50);
		}
		if (Unit == UnitType.Brachio)
		{
			SocialGamingManager.Instance.ReportProgress(AchievementIds.DEPLOY_50_BRACHIOS, 1, 50);
		}
	}
}
