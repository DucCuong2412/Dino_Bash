using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class UpgradeScreen : BaseScreen
{
	public enum Tab
	{
		invalid = -1,
		dinos = 0,
		shots = 1,
		special = 2
	}

	private tk2dUIScrollableArea dinoScrollView;

	private tk2dUIScrollableArea shotScrollView;

	private tk2dUIScrollableArea upgradeScrollView;

	private List<tk2dUIItem> tab_titles = new List<tk2dUIItem>();

	private tk2dUIItem dinoTabTitle;

	private tk2dUIItem shotTabTitle;

	private tk2dUIItem upgradeTabTitle;

	private StandardButton closeButton;

	private float listOffset = -260f;

	private float tabYStartPosition;

	private float tabYOffset = 60f;

	private Color tabTitleLabelColor;

	[SerializeField]
	private Transform FX_Upgrade_Prefab;

	[SerializeField]
	private Sprite premiumBg;

	private UpgradeScreenListEntry entryDefinition;

	private Dictionary<tk2dUIItem, Tab> tabButtonToTab = new Dictionary<tk2dUIItem, Tab>();

	private Tab tabState = Tab.invalid;

	private bool is_init;

	public List<UpgradeScreenListEntry> upgrade_list_entries = new List<UpgradeScreenListEntry>();

	private List<GoTween> blink_tweens = new List<GoTween>();

	private Vector3 tab_hide_offset = new Vector3(0f, 200f, 0f);

	private int buy_item_count;

	public Tab CurrentOpenTab
	{
		get
		{
			return tabState;
		}
	}

	public event Action<Tab> OnTabChanged;

	protected void Start()
	{
		if (!is_init)
		{
			is_init = true;
			closeButton = base.transform.Search("btn_close").GetComponent<StandardButton>();
			closeButton.uiItem.OnClick += OnCloseClick;
			closeButton.clickSound = Sounds.main_close_popup;
			dinoScrollView = base.transform.FindChild("MiddleCenter/Dinos/ScrollableArea").GetComponent<tk2dUIScrollableArea>();
			shotScrollView = base.transform.FindChild("MiddleCenter/Shots/ScrollableArea").GetComponent<tk2dUIScrollableArea>();
			upgradeScrollView = base.transform.FindChild("MiddleCenter/Upgrades/ScrollableArea").GetComponent<tk2dUIScrollableArea>();
			dinoTabTitle = base.transform.FindChild("MiddleCenter/Dinos/Tab_Root/Tab").GetComponent<tk2dUIItem>();
			dinoTabTitle.OnClickUIItem += OnTabClick;
			tabButtonToTab.Add(dinoTabTitle, Tab.dinos);
			tab_titles.Add(dinoTabTitle);
			shotTabTitle = base.transform.FindChild("MiddleCenter/Shots/Tab_Root/Tab").GetComponent<tk2dUIItem>();
			shotTabTitle.OnClickUIItem += OnTabClick;
			tabButtonToTab.Add(shotTabTitle, Tab.shots);
			tab_titles.Add(shotTabTitle);
			upgradeTabTitle = base.transform.FindChild("MiddleCenter/Upgrades/Tab_Root/Tab").GetComponent<tk2dUIItem>();
			upgradeTabTitle.OnClickUIItem += OnTabClick;
			tabButtonToTab.Add(upgradeTabTitle, Tab.special);
			tab_titles.Add(upgradeTabTitle);
			tabYStartPosition = dinoTabTitle.transform.localPosition.y;
			tabTitleLabelColor = dinoTabTitle.transform.FindChild("title").GetComponent<tk2dTextMesh>().color;
			entryDefinition = base.transform.FindChild("MiddleCenter/UpgradeListEntryWidget").GetComponent<UpgradeScreenListEntry>();
			SetupDinoEntries();
			SetupShotEntries();
			SetupUpgradeEntries();
			entryDefinition.gameObject.SetActive(false);
			base.transform.localPosition += base.right;
			base.gameObject.SetActive(false);
		}
	}

	private void OnCloseClick()
	{
		Hide();
		ScreenManager.GetScreen<MapScreen>().Show();
	}

	public UpgradeScreenListEntry getListEntry(UnitType unit_type = UnitType.None, ShotType shot_type = ShotType.None)
	{
		if (unit_type != 0)
		{
			return upgrade_list_entries.Find((UpgradeScreenListEntry x) => x.Unit_Type == unit_type);
		}
		if (shot_type != ShotType.None)
		{
			return upgrade_list_entries.Find((UpgradeScreenListEntry x) => x.Shot_Type == shot_type);
		}
		Debug.LogError(string.Format("entry not found: {0} - {1}", unit_type, shot_type));
		return null;
	}

	private void SetupDinoEntries()
	{
		Transform parent = dinoScrollView.transform.FindChild("Content");
		ArrayList arrayList = new ArrayList(Konfiguration.getDinos());
		arrayList.Insert(3, UnitType.TRex_Jr);
		arrayList.Insert(6, UnitType.Rocky);
		for (int i = 0; i < arrayList.Count; i++)
		{
			DinoShotUpgradeAdapter adapter = getAdapter(arrayList[i]);
			UpgradeScreenListEntry upgradeScreenListEntry = UnityEngine.Object.Instantiate(entryDefinition, entryDefinition.transform.position + new Vector3(0f, (float)i * listOffset, 0f), Quaternion.identity) as UpgradeScreenListEntry;
			upgradeScreenListEntry.transform.parent = parent;
			upgradeScreenListEntry.gameObject.name = adapter.name;
			upgradeScreenListEntry.FX_Upgrade_Prefab = FX_Upgrade_Prefab;
			upgradeScreenListEntry.premiumBg = premiumBg;
			upgradeScreenListEntry.ListIndex = i;
			upgradeScreenListEntry.Init(adapter);
			upgrade_list_entries.Add(upgradeScreenListEntry);
		}
		dinoScrollView.ContentLength = (float)arrayList.Count * Mathf.Abs(listOffset);
	}

	private void SetupShotEntries()
	{
		Transform parent = shotScrollView.transform.FindChild("Content");
		ArrayList arrayList = new ArrayList(Konfiguration.getShots());
		arrayList.Insert(2, UnitType.AdditionalShotSlot);
		arrayList.Insert(4, UnitType.FastShotCooldown);
		arrayList.Insert(7, UnitType.RaindowTrail);
		for (int i = 0; i < arrayList.Count; i++)
		{
			DinoShotUpgradeAdapter adapter = getAdapter(arrayList[i]);
			UpgradeScreenListEntry upgradeScreenListEntry = UnityEngine.Object.Instantiate(entryDefinition, entryDefinition.transform.position + new Vector3(0f, (float)i * listOffset, 0f), Quaternion.identity) as UpgradeScreenListEntry;
			upgradeScreenListEntry.transform.parent = parent;
			upgradeScreenListEntry.gameObject.name = adapter.name;
			upgradeScreenListEntry.FX_Upgrade_Prefab = FX_Upgrade_Prefab;
			upgradeScreenListEntry.premiumBg = premiumBg;
			upgradeScreenListEntry.ListIndex = i;
			upgradeScreenListEntry.Init(adapter);
			upgrade_list_entries.Add(upgradeScreenListEntry);
		}
		shotScrollView.ContentLength = (float)arrayList.Count * Mathf.Abs(listOffset);
	}

	private void SetupUpgradeEntries()
	{
		Transform parent = upgradeScrollView.transform.FindChild("Content");
		ArrayList arrayList = new ArrayList(Konfiguration.getUpgrades());
		arrayList.Insert(2, UnitType.Stone);
		arrayList.Insert(3, UnitType.CoinDoubler);
		arrayList.Add(UnitType.XpDoubler);
		for (int i = 0; i < arrayList.Count; i++)
		{
			DinoShotUpgradeAdapter adapter = getAdapter(arrayList[i]);
			UpgradeScreenListEntry upgradeScreenListEntry = UnityEngine.Object.Instantiate(entryDefinition, entryDefinition.transform.position + new Vector3(0f, (float)i * listOffset, 0f), Quaternion.identity) as UpgradeScreenListEntry;
			upgradeScreenListEntry.transform.parent = parent;
			upgradeScreenListEntry.gameObject.name = adapter.name;
			upgradeScreenListEntry.FX_Upgrade_Prefab = FX_Upgrade_Prefab;
			upgradeScreenListEntry.premiumBg = premiumBg;
			upgradeScreenListEntry.ListIndex = i;
			upgradeScreenListEntry.Init(adapter);
			upgrade_list_entries.Add(upgradeScreenListEntry);
		}
		upgradeScrollView.ContentLength = (float)arrayList.Count * Mathf.Abs(listOffset);
	}

	private DinoShotUpgradeAdapter getAdapter(object item)
	{
		UnitType unitType = UnitType.None;
		ShotType shotType = ShotType.None;
		if (item is UnitType)
		{
			unitType = (UnitType)(int)item;
		}
		else if (item is ShotType)
		{
			shotType = (ShotType)(int)item;
		}
		if (Konfiguration.isDinoUnit(unitType))
		{
			return new DinoAdapter(unitType);
		}
		if (Konfiguration.isUpgrade(unitType))
		{
			return new UpgradeAdapter(unitType);
		}
		if (shotType != ShotType.None)
		{
			return new ShotAdapter(shotType);
		}
		return null;
	}

	private void OnTabClick(tk2dUIItem uiItem)
	{
		AudioPlayer.PlayGuiSFX(Sounds.main_play_button, 0f);
		ShowTab(tabButtonToTab[uiItem]);
	}

	private void ShowTab(Tab tab)
	{
		if (tabState != tab)
		{
			ClearNewElementsInTab(tabState);
			tabState = tab;
			blink_tweens.ForEach(delegate(GoTween t)
			{
				t.destroy();
			});
			blink_tweens.Clear();
			switch (tabState)
			{
			case Tab.dinos:
				Player.UserHasSeenUpgradeTabs[0] = true;
				SetTabTitle(Tab.dinos, true);
				dinoScrollView.gameObject.SetActive(true);
				SetTabTitle(Tab.shots, false);
				shotScrollView.gameObject.SetActive(false);
				SetTabTitle(Tab.special, false);
				upgradeScrollView.gameObject.SetActive(false);
				break;
			case Tab.shots:
				SetTabTitle(Tab.dinos, false);
				dinoScrollView.gameObject.SetActive(false);
				Player.UserHasSeenUpgradeTabs[1] = true;
				SetTabTitle(Tab.shots, true);
				shotScrollView.gameObject.SetActive(true);
				SetTabTitle(Tab.special, false);
				upgradeScrollView.gameObject.SetActive(false);
				break;
			case Tab.special:
				SetTabTitle(Tab.dinos, false);
				dinoScrollView.gameObject.SetActive(false);
				SetTabTitle(Tab.shots, false);
				shotScrollView.gameObject.SetActive(false);
				Player.UserHasSeenUpgradeTabs[2] = true;
				SetTabTitle(Tab.special, true);
				upgradeScrollView.gameObject.SetActive(true);
				break;
			}
			if (this.OnTabChanged != null)
			{
				this.OnTabChanged(tabState);
			}
		}
	}

	private void SetTabTitle(Tab tab, bool active)
	{
		tk2dUIItem tk2dUIItem2 = tab_titles[(int)tab];
		float duration = 0.15f;
		Color color = new Color(0.7f, 0.7f, 0.7f, 1f);
		tk2dTextMesh component = tk2dUIItem2.transform.FindChild("title").GetComponent<tk2dTextMesh>();
		SpriteRenderer component2 = tk2dUIItem2.GetComponent<SpriteRenderer>();
		tk2dBaseSprite component3 = tk2dUIItem2.transform.FindChild("bg").GetComponent<tk2dBaseSprite>();
		Transform transform = tk2dUIItem2.transform.Search("FX_UpgradeTabBling(Clone)");
		transform.gameObject.SetActive(!Player.UserHasSeenUpgradeTabs[(int)tab]);
		if (active)
		{
			Go.to(tk2dUIItem2.transform, duration, new GoTweenConfig().localPosition(tk2dUIItem2.transform.localPosition.SetY(tabYStartPosition)));
			Go.to(component.transform, duration, new GoTweenConfig().scale(Vector3.one));
			Go.to(component2, duration, new GoTweenConfig().colorProp("color", Color.white));
			Go.to(component3, duration, new GoTweenConfig().colorProp("color", Color.white));
			Go.to(component, duration, new GoTweenConfig().colorProp("color", tabTitleLabelColor));
		}
		else if (component2.color != color)
		{
			Go.to(tk2dUIItem2.transform, duration, new GoTweenConfig().localPosition(tk2dUIItem2.transform.localPosition.SetY(tabYStartPosition - tabYOffset)));
			Go.to(component.transform, duration, new GoTweenConfig().scale(new Vector3(0.8f, 0.8f, 0.8f)));
			GoTweenConfig goTweenConfig = new GoTweenConfig();
			goTweenConfig.loopType = GoLoopType.PingPong;
			if (Player.UserHasSeenUpgradeTabs[(int)tab])
			{
				goTweenConfig.iterations = 1;
			}
			else
			{
				goTweenConfig.iterations = -1;
				duration = 0.5f;
			}
			blink_tweens.Add(Go.to(component2, duration, goTweenConfig.colorProp("color", color)));
			goTweenConfig.clearProperties();
			blink_tweens.Add(Go.to(component3, duration, goTweenConfig.colorProp("color", color)));
			goTweenConfig.clearProperties();
			blink_tweens.Add(Go.to(component, duration, goTweenConfig.colorProp("color", component.color * 0.7f)));
		}
	}

	private void SetScrollViewToElement(DinoShotUpgradeAdapter focus_element)
	{
		if (focus_element is DinoAdapter)
		{
			int listIndex = upgrade_list_entries.Find((UpgradeScreenListEntry x) => x.Unit_Type == (focus_element as DinoAdapter).dino).ListIndex;
			float num = dinoScrollView.contentContainer.transform.childCount;
			dinoScrollView.Value = (float)listIndex / num;
		}
		else if (focus_element is ShotAdapter)
		{
			int listIndex2 = upgrade_list_entries.Find((UpgradeScreenListEntry x) => x.Shot_Type == (focus_element as ShotAdapter).shot).ListIndex;
			float num2 = shotScrollView.contentContainer.transform.childCount;
			shotScrollView.Value = (float)listIndex2 / num2;
		}
		else if (focus_element is UpgradeAdapter)
		{
			int listIndex3 = upgrade_list_entries.Find((UpgradeScreenListEntry x) => x.Unit_Type == (focus_element as UpgradeAdapter).upgrade).ListIndex;
			float num3 = upgradeScrollView.contentContainer.transform.childCount;
			upgradeScrollView.Value = (float)listIndex3 / num3;
		}
	}

	private void MarkElementAsNew(DinoShotUpgradeAdapter element, bool is_new)
	{
		if (element is DinoAdapter)
		{
			getListEntry((element as DinoAdapter).dino).mark_as_new = is_new;
		}
		else if (element is ShotAdapter)
		{
			getListEntry(UnitType.None, (element as ShotAdapter).shot).mark_as_new = is_new;
		}
		else if (element is UpgradeAdapter)
		{
			getListEntry((element as UpgradeAdapter).upgrade).mark_as_new = is_new;
		}
	}

	private void ClearNewElementsInTab(Tab tab)
	{
		if (tab == Tab.invalid)
		{
			return;
		}
		SetTabNewElementCounter(tab, 0);
		switch (tab)
		{
		case Tab.dinos:
			Player.getNewUpgradePossibilites.ForEach(delegate(DinoShotUpgradeAdapter element)
			{
				if (element is DinoAdapter)
				{
					Player.RemoveFromNewUpgradePossibilites(element);
					MarkElementAsNew(element, false);
				}
			});
			break;
		case Tab.shots:
			Player.getNewUpgradePossibilites.ForEach(delegate(DinoShotUpgradeAdapter element)
			{
				if (element is ShotAdapter)
				{
					Player.RemoveFromNewUpgradePossibilites(element);
					MarkElementAsNew(element, false);
				}
			});
			break;
		case Tab.special:
			Player.getNewUpgradePossibilites.ForEach(delegate(DinoShotUpgradeAdapter element)
			{
				if (element is UpgradeAdapter)
				{
					Player.RemoveFromNewUpgradePossibilites(element);
					MarkElementAsNew(element, false);
				}
			});
			break;
		}
		ScreenManager.GetScreen<MapScreen>().UpdateUpgradePossibilitesLabel();
	}

	private void SetTabNewElementCounter(Tab tab, int count)
	{
		string text = "label_new";
		count = Mathf.Clamp(count, 0, 9);
		tk2dUIItem tk2dUIItem2 = tab_titles[(int)tab];
		tk2dTextMesh label = tk2dUIItem2.transform.FindChild(text).GetComponent<tk2dTextMesh>();
		try
		{
			if (count == 0)
			{
				Go.to(label.transform, 0.25f, new GoTweenConfig().scale(Vector3.zero).onComplete(delegate
				{
					label.gameObject.SetActive(false);
				}));
				return;
			}
			label.text = count.ToString();
			label.Commit();
			label.gameObject.SetActive(true);
			label.transform.localScale = Vector3.one;
		}
		catch (NullReferenceException ex)
		{
			Debug.LogError(ex.Message + ex.StackTrace);
		}
	}

	private Tab getLatestUnlock()
	{
		LevelData levelData = Konfiguration.GetLevelData(Mathf.Max(0, Player.MaxLevelID - 1));
		Tab result = Tab.invalid;
		DinoShotUpgradeAdapter scrollViewToElement = null;
		if (levelData.unlockShot != ShotType.None)
		{
			result = Tab.shots;
			scrollViewToElement = new ShotAdapter(levelData.unlockShot);
		}
		else if (Konfiguration.isDinoUnit(levelData.unlockUnit))
		{
			result = Tab.dinos;
			scrollViewToElement = new DinoAdapter(levelData.unlockUnit);
		}
		else if (Konfiguration.isUpgrade(levelData.unlockUnit))
		{
			result = Tab.special;
			scrollViewToElement = new UpgradeAdapter(levelData.unlockUnit);
		}
		SetScrollViewToElement(scrollViewToElement);
		return result;
	}

	public void Show(Tab open_tap, bool present_last_upgrade = false)
	{
		Start();
		base.gameObject.SetActive(true);
		buy_item_count = Tracking.items_bought_count;
		ShowFrom(base.right);
		OnEscapeUp = delegate
		{
			if (!ScreenManager.GetScreen<UpgradeInfoDinoScreen>().isVisible && !ScreenManager.GetScreen<UpgradeInfoShotScreen>().isVisible && !ScreenManager.GetScreen<UpgradeInfoUpgradesScreen>().isVisible && !ScreenManager.GetScreen<ShopScreenCoins>().isVisible && !ScreenManager.GetScreen<ShopScreenDiamonds>().isVisible)
			{
				OnCloseClick();
			}
		};
		float num = 0.22500001f;
		foreach (tk2dUIItem key in tabButtonToTab.Keys)
		{
			Vector3 localPosition = key.transform.parent.localPosition;
			localPosition -= tab_hide_offset;
			key.transform.parent.localPosition = localPosition;
			localPosition += tab_hide_offset;
			Go.to(key.transform.parent, 0.35f, new GoTweenConfig().setDelay(num).localPosition(localPosition).setEaseType(GoEaseType.CubicOut));
			num += 0.1f;
		}
		if (present_last_upgrade)
		{
			open_tap = getLatestUnlock();
		}
		ShowTab(open_tap);
		int dino_upgrades = 0;
		int shot_upgrades = 0;
		int special = 0;
		Player.getNewUpgradePossibilites.ForEach(delegate(DinoShotUpgradeAdapter element)
		{
			if (element is DinoAdapter)
			{
				dino_upgrades++;
			}
			else if (element is ShotAdapter)
			{
				shot_upgrades++;
			}
			else if (element is UpgradeAdapter)
			{
				special++;
			}
			MarkElementAsNew(element, true);
		});
		SetTabNewElementCounter(Tab.dinos, dino_upgrades);
		SetTabNewElementCounter(Tab.shots, shot_upgrades);
		SetTabNewElementCounter(Tab.special, special);
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		base.Show();
	}

	public override void Hide()
	{
		base.Interactive = false;
		Tracking.bought_upgrade_after_loss(buy_item_count);
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		HideTo(base.left, delegate
		{
			ClearNewElementsInTab(tabState);
			base.Hide();
			base.gameObject.SetActive(false);
		});
		App.Instance.cloudSaveGameManager.checkAndPrompt(false);
	}
}
