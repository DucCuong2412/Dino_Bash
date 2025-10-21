using UnityEngine;
using dinobash;

public class UpgradeScreenListEntry : MonoBehaviour
{
	public Transform FX_Upgrade_Prefab;

	public Sprite premiumBg;

	private DinoShotUpgradeAdapter adapter;

	private UnitType unit_type;

	private ShotType shot_type = ShotType.None;

	private bool isDino;

	private bool isShot;

	private bool isUpgrade;

	private bool isPremium;

	private tk2dUIItem button_buy;

	private StandardButton bg_panel_button;

	private LocalizedText item_name;

	private LocalizedText item_level;

	private tk2dTextMesh item_price;

	private Transform upgrading_icon;

	private Transform locked_panel;

	private InfoContainer infoContainer;

	private SpriteRenderer portait;

	private int sortingLayerID;

	private GameObject active_timer_bg;

	private static Material _disabledMaterial;

	private static GoTweenConfig _portraitTweenConfig;

	private bool initialized;

	private SpriteRenderer icon_coins;

	private SpriteRenderer icon_diamonds;

	private Vector3 item_price_position;

	private Vector3 item_price_size;

	private static InfoContainer infoContainerDefinition;

	public UnitType Unit_Type
	{
		get
		{
			return unit_type;
		}
	}

	public ShotType Shot_Type
	{
		get
		{
			return shot_type;
		}
	}

	public int ListIndex { get; set; }

	public bool mark_as_new
	{
		set
		{
			base.transform.Search("new_marker").gameObject.SetActive(value);
		}
	}

	private bool is_upgrading
	{
		get
		{
			if (isDino)
			{
				return EntityTimers.is_upgrading(unit_type);
			}
			return false;
		}
	}

	private bool is_temporarily_unlocked
	{
		get
		{
			if (isUpgrade)
			{
				return EntityTimers.is_temporarily_unlocked(unit_type);
			}
			return false;
		}
	}

	private Material disabledMaterial
	{
		get
		{
			if (_disabledMaterial == null)
			{
				_disabledMaterial = Resources.Load<Material>("GUI/Disabled_Buttons");
			}
			return _disabledMaterial;
		}
	}

	private GoTweenConfig portraitTweenConfig
	{
		get
		{
			if (_portraitTweenConfig == null)
			{
				_portraitTweenConfig = new GoTweenConfig();
				_portraitTweenConfig.scale(Vector3.one * 0.9f);
				_portraitTweenConfig.loopType = GoLoopType.PingPong;
				_portraitTweenConfig.iterations = 2;
			}
			return _portraitTweenConfig;
		}
	}

	private void OnEnable()
	{
		if (initialized)
		{
			UpdateEntry();
		}
	}

	private void OnDestroy()
	{
		EntityTimers.onUpgradeComplete -= HandleonUpgradeComplete;
		EntityTimers.onUpgradeStarted -= HandleUpgradeStarted;
		EntityTimers.onTemporaryUpgradeOver -= HandleonTemporaryUpgradeOver;
		_portraitTweenConfig = null;
		_disabledMaterial = null;
	}

	private void Update()
	{
		if (initialized && is_upgrading)
		{
			item_level.textMesh.text = "upgrade_ready".Localize() + " " + EntityTimers.getUpgradeTime(unit_type).Humanize();
		}
		else if (initialized && isUpgrade && is_temporarily_unlocked)
		{
			item_level.textMesh.LineCount = 2;
			item_level.textMesh.text = "item_active_for".Localize() + " \n" + EntityTimers.getTempUnlockTime(unit_type).Humanize();
		}
	}

	private void HandleonUpgradeComplete(UnitType entity)
	{
		if (entity == unit_type && isDino && base.gameObject.activeInHierarchy)
		{
			UpdateEntry(true);
		}
	}

	private void HandleUpgradeStarted(UnitType entity)
	{
		if (entity == unit_type && isDino && base.gameObject.activeInHierarchy)
		{
			item_level.textMesh.text = "upgrade_ready".Localize() + " " + EntityTimers.getUpgradeTime(unit_type).Humanize();
			item_price.scale = item_price_size;
			item_price.text = "entity_training".Localize();
			upgrading_icon.gameObject.SetActive(true);
			SetCurrency();
		}
	}

	private void HandleonTemporaryUpgradeOver(UnitType item)
	{
		if (item == unit_type && isUpgrade && base.gameObject.activeInHierarchy)
		{
			UpdateEntry();
			ResetButtonBuy();
			active_timer_bg.SetActive(false);
		}
	}

	public void Init(DinoShotUpgradeAdapter adapter)
	{
		setItem(adapter);
		sortingLayerID = ScreenManager.GetScreen<UpgradeScreen>().SortingLayerID;
		portait = base.transform.FindChild("item_portrait").GetComponent<SpriteRenderer>();
		if (isUpgrade)
		{
			portait.transform.localScale = Vector3.one;
		}
		Sprite sprite = ((!isShot) ? SpriteRessources.getSpiteForUnitType(unit_type) : SpriteRessources.getShotBuySprite(shot_type));
		if (sprite != null)
		{
			portait.sprite = sprite;
		}
		SpawnUpgradeFX();
		if (isPremium)
		{
			base.transform.FindChild("bg_panel").GetComponent<SpriteRenderer>().sprite = premiumBg;
		}
		string key = ((!isDino && !isUpgrade) ? shot_type.ToString() : unit_type.ToString());
		item_name = base.transform.Search("item_name").GetComponent<LocalizedText>();
		item_level = base.transform.Search("item_level").GetComponent<LocalizedText>();
		infoContainer = spawnInfoContainer(base.transform.Find("infoContainer_position"));
		item_name.Key = key;
		locked_panel = base.transform.FindChild("locked_panel");
		button_buy = base.transform.Search("btn_buy").GetComponent<tk2dUIItem>();
		upgrading_icon = base.transform.Search("upgrading");
		upgrading_icon.gameObject.SetActive(false);
		bg_panel_button = base.transform.Search("bg_panel").GetComponent<StandardButton>();
		bg_panel_button.darken_on_down = 0.95f;
		bg_panel_button.tween.clearTweenProperties();
		bg_panel_button.uiItem.OnClick += onClickBuyButton;
		active_timer_bg = base.transform.Search("active_timer_bg").gameObject;
		active_timer_bg.SetActive(false);
		item_price = button_buy.transform.FindChild("item_price").GetComponent<tk2dTextMesh>();
		item_price_position = item_price.transform.localPosition;
		item_price_size = item_price.scale;
		mark_as_new = false;
		UpdateEntry();
		EntityTimers.onUpgradeStarted -= HandleUpgradeStarted;
		EntityTimers.onUpgradeStarted += HandleUpgradeStarted;
		EntityTimers.onUpgradeComplete -= HandleonUpgradeComplete;
		EntityTimers.onUpgradeComplete += HandleonUpgradeComplete;
		EntityTimers.onTemporaryUpgradeOver -= HandleonTemporaryUpgradeOver;
		EntityTimers.onTemporaryUpgradeOver += HandleonTemporaryUpgradeOver;
		if (EntityTimers.is_upgrading(unit_type))
		{
			HandleUpgradeStarted(unit_type);
		}
		initialized = true;
	}

	private void setItem(DinoShotUpgradeAdapter item_adapter)
	{
		adapter = item_adapter;
		if (adapter is DinoAdapter)
		{
			isDino = true;
			unit_type = ((DinoAdapter)adapter).dino;
		}
		if (adapter is ShotAdapter)
		{
			isShot = true;
			shot_type = ((ShotAdapter)adapter).shot;
		}
		if (adapter is UpgradeAdapter)
		{
			isUpgrade = true;
			unit_type = ((UpgradeAdapter)adapter).upgrade;
		}
		isPremium = Konfiguration.isPremium(shot_type) || Konfiguration.isPremium(unit_type);
	}

	private void onClickBuyButton()
	{
		if (isUpgrade && Player.ActiveUpgrades.Contains(unit_type) && unit_type == UnitType.RaindowTrail)
		{
			Player.SetUpgradeState(unit_type, !Player.GetUpgradeState(unit_type));
			UpdateEntry();
			return;
		}
		if (isDino)
		{
			ScreenManager.GetScreen<UpgradeInfoDinoScreen>().Show(unit_type);
		}
		if (isShot)
		{
			ScreenManager.GetScreen<UpgradeInfoShotScreen>().Show(shot_type);
		}
		if (isUpgrade)
		{
			ScreenManager.GetScreen<UpgradeInfoUpgradesScreen>().Show(unit_type);
		}
	}

	public void UpdateEntry(bool animate = false)
	{
		item_level.transform.PosX(item_name.transform.position.x);
		upgrading_icon.gameObject.SetActive(isDino && is_upgrading);
		SetCurrency();
		if (animate)
		{
			Go.to(portait.transform, 0.25f, portraitTweenConfig);
			ParticleSystem componentInChildren = portait.GetComponentInChildren<ParticleSystem>();
			componentInChildren.Play();
		}
		if (adapter.isAvailable)
		{
			locked_panel.gameObject.SetActive(false);
			if (isShot)
			{
				infoContainer.Set(shot_type, sortingLayerID, adapter.CanLevelUp, animate);
			}
			if (isDino)
			{
				infoContainer.Set(unit_type, sortingLayerID, adapter.CanLevelUp, animate);
			}
			if (isDino || isShot)
			{
				string text = string.Format(" {0}/{1}", adapter.Level + 1, 10);
				item_level.textMesh.text = "level".Localize() + text;
			}
			if (isUpgrade)
			{
				item_name.transform.PosY(item_name.transform.parent.position.y);
				if (is_temporarily_unlocked)
				{
					active_timer_bg.SetActive(true);
					item_level.textMesh.anchor = TextAnchor.MiddleCenter;
				}
				else
				{
					active_timer_bg.SetActive(false);
					item_level.textMesh.anchor = TextAnchor.MiddleLeft;
				}
				item_level.transform.position = button_buy.transform.position;
				item_level.gameObject.SetActive(is_temporarily_unlocked);
			}
			item_price.scale = item_price_size;
			if (adapter.CanLevelUp && !is_upgrading)
			{
				item_price.text = adapter.UpgradeCost.ToString().GetGroupedNumberString();
			}
			else if (is_upgrading)
			{
				item_price.text = "entity_training".Localize();
			}
			else if (is_temporarily_unlocked)
			{
				MaxOutButtonBuy(false);
			}
			else
			{
				MaxOutButtonBuy(!isUpgrade);
			}
			return;
		}
		Transform transform = locked_panel.FindChild("lock icon");
		transform.transform.PosX(item_level.transform.position.x + transform.renderer.bounds.extents.x);
		item_level.transform.position += new Vector3(transform.renderer.bounds.size.x * 1.1f, 0f, 0f);
		if (isPremium)
		{
			item_price.scale = item_price_size;
			item_price.text = adapter.PremiumUnlockCost.ToString().GetGroupedNumberString();
			item_level.Key = "Unlock for";
			item_level.Localize();
			locked_panel.transform.FindChild("label_level").gameObject.SetActive(false);
			locked_panel.transform.FindChild("bg").gameObject.SetActive(false);
			locked_panel.transform.FindChild("icon_game_level").gameObject.SetActive(false);
			if (isShot)
			{
				infoContainer.Set(shot_type, sortingLayerID, false);
			}
			if (isDino)
			{
				infoContainer.Set(unit_type, sortingLayerID, false);
			}
			if (isUpgrade && Player.ActiveUpgrades.Contains(unit_type))
			{
				item_name.transform.PosY(item_name.transform.parent.position.y);
				Renderer obj = transform.renderer;
				bool flag = false;
				item_level.renderer.enabled = flag;
				obj.enabled = flag;
				if (unit_type == UnitType.RaindowTrail)
				{
					SetRainbowTrailState();
				}
				else
				{
					MaxOutButtonBuy(false);
				}
			}
			return;
		}
		if (isShot)
		{
			infoContainer.Set(ShotType.None, sortingLayerID, false);
		}
		if (isDino)
		{
			infoContainer.Set(UnitType.None, sortingLayerID, false);
		}
		tk2dTextMesh component = locked_panel.Find("label_level").GetComponent<tk2dTextMesh>();
		if (isDino || isUpgrade)
		{
			component.text = Konfiguration.levels.Find((LevelData x) => x.unlockUnit == unit_type).display_name;
		}
		if (isShot)
		{
			component.text = Konfiguration.levels.Find((LevelData x) => x.unlockShot == shot_type).display_name;
		}
		item_level.Key = "Unlock at:";
		portait.material = disabledMaterial;
		MaxOutButtonBuy(false);
	}

	private void SetCurrency()
	{
		if (icon_coins == null)
		{
			icon_coins = base.transform.Search("icon_coin").GetComponent<SpriteRenderer>();
			icon_diamonds = base.transform.Search("icon_diamond").GetComponent<SpriteRenderer>();
		}
		icon_diamonds.enabled = isPremium;
		icon_coins.enabled = !icon_diamonds.enabled;
		if (is_upgrading || is_temporarily_unlocked)
		{
			SpriteRenderer spriteRenderer = icon_coins;
			bool flag = false;
			icon_diamonds.enabled = flag;
			spriteRenderer.enabled = flag;
		}
	}

	private void MaxOutButtonBuy(bool keepVisible = true)
	{
		bg_panel_button.GetComponent<Collider>().enabled = false;
		if (keepVisible)
		{
			button_buy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.4f);
			item_price.scale = item_price_size;
			item_price.text = "max".Localize();
			SetBuyButtonToTextOnly();
		}
		else
		{
			button_buy.gameObject.SetActive(false);
		}
	}

	private void ResetButtonBuy()
	{
		button_buy.gameObject.SetActive(true);
		item_price.anchor = TextAnchor.MiddleRight;
		item_price.transform.localPosition = item_price_position;
		bg_panel_button.GetComponent<Collider>().enabled = true;
		button_buy.GetComponent<SpriteRenderer>().color = Color.white;
		button_buy.transform.Find("icon_coin").gameObject.SetActive(true);
		button_buy.transform.Find("icon_diamond").gameObject.SetActive(true);
	}

	private void SetBuyButtonToTextOnly()
	{
		item_price.anchor = TextAnchor.MiddleCenter;
		item_price.transform.localPosition = Vector3.zero;
		button_buy.transform.Find("icon_coin").gameObject.SetActive(false);
		button_buy.transform.Find("icon_diamond").gameObject.SetActive(false);
	}

	private void SetRainbowTrailState()
	{
		SetBuyButtonToTextOnly();
		bool upgradeState = Player.GetUpgradeState(UnitType.RaindowTrail);
		item_price.scale = item_price_size;
		item_price.text = ((!upgradeState) ? "upgrade_off".Localize() : "upgrade_on".Localize());
	}

	private void SpawnUpgradeFX()
	{
		Transform transform = Object.Instantiate(FX_Upgrade_Prefab) as Transform;
		transform.parent = portait.transform;
		transform.transform.localPosition = Vector3.zero;
		SpriteTools.SetSortingLayerID(transform, sortingLayerID);
	}

	private InfoContainer spawnInfoContainer(Transform target)
	{
		if (isDino || isShot)
		{
			if (infoContainerDefinition == null)
			{
				infoContainerDefinition = InfoContainer.Load();
				infoContainerDefinition.gameObject.name = "containerInfoDefintion";
				infoContainerDefinition.gameObject.SetActive(false);
			}
			InfoContainer component = (Object.Instantiate(infoContainerDefinition.gameObject) as GameObject).GetComponent<InfoContainer>();
			component.gameObject.SetActive(true);
			component.transform.RepositionAndReparent(target);
			return component;
		}
		Vector3 vector = new Vector3(-400f, 0f, 0f);
		item_name.transform.localPosition += vector;
		item_name.textMesh.wordWrapWidth = 900;
		item_level.transform.localPosition += vector;
		return null;
	}
}
