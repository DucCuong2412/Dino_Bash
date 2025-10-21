using System;
using UnityEngine;
using dinobash;

public class UpgradeInfoDinoScreen : UpgradeInfoScreen<UnitType>
{
	private bool use_upgrade_queue
	{
		get
		{
			return Konfiguration.GameConfig.Use_upgrade_timers;
		}
	}

	private bool at_max_level
	{
		get
		{
			return Player.GetUnitLevel(item) == 9;
		}
	}

	protected override int getCost()
	{
		if (isUpgrading())
		{
			return Konfiguration.GetUpgradeTrainingSkipCost(item, ShotType.None);
		}
		if ((isPremium() && isUnlock()) || Konfiguration.isConsumable(item))
		{
			return Konfiguration.UnitData[item].premiumCost;
		}
		return Konfiguration.getDinoUpgradeCost(item);
	}

	protected override void OnClickedUpgrade()
	{
		int cost = 0;
		if (!Debit(out cost))
		{
			return;
		}
		upgradeButton.OnClick -= OnClickedUpgrade;
		ReenableBuyButton();
		if (Konfiguration.isDinoUnit(item))
		{
			if (isUnlock() && item != UnitType.DinoEgg)
			{
				Player.UnlockDino(item);
			}
			else if (!isUpgrading() && use_upgrade_queue)
			{
				Player.UpgradeUnit(item);
				EntityTimers.onUpgradeComplete -= HandleonUpgradeComplete;
				EntityTimers.onUpgradeComplete += HandleonUpgradeComplete;
				SetAnimation();
			}
			else
			{
				Player.UpgradeUnit(item, false);
				if (use_upgrade_queue)
				{
					HandleonUpgradeComplete(item);
				}
			}
			int spent = cost;
			string origin = get_tracking_origin(cost);
			Tracking.buy_item(item, ShotType.None, string.Empty, spent, origin, isUpgrading());
		}
		else if (Konfiguration.isConsumable(item) && Player.hasUnlockedConsumable(item))
		{
			Player.changeConsumableCount(item, Player.getConsumableRefillAmount(item));
			int spent = cost;
			string origin = get_tracking_origin(cost);
			Tracking.buy_item(item, ShotType.None, string.Empty, spent, origin);
		}
		UpdateEntries();
		if (!isUpgrading())
		{
			FX_onBuy.gameObject.SetActive(true);
		}
		hide_wait_duration = 0.75f;
		if (!use_upgrade_queue || at_max_level || App.State == App.States.Game)
		{
			Hide();
		}
	}

	private void UpdateEntries()
	{
		setInfoContainer();
		setTexts();
		if (App.State == App.States.Map)
		{
			if (!isUpgrading())
			{
				UpgradeScreen screen = ScreenManager.GetScreen<UpgradeScreen>();
				if (screen != null && screen.isVisible)
				{
					screen.getListEntry(item).UpdateEntry(true);
				}
			}
			CheckForAchievement();
			EntityUpgradeSetter[] componentsInChildren = itemPortrait.GetComponentsInChildren<EntityUpgradeSetter>(true);
			EntityUpgradeSetter[] array = componentsInChildren;
			foreach (EntityUpgradeSetter entityUpgradeSetter in array)
			{
				entityUpgradeSetter.ForceUpdate();
			}
		}
		TestPlayerProgressRequirement();
	}

	private void ReenableBuyButton()
	{
		WaitThenRealtime(0.5f, delegate
		{
			if (base.Interactive && !at_max_level)
			{
				upgradeButton.OnClick += OnClickedUpgrade;
			}
		});
	}

	private void setTexts()
	{
		if (isUpgrading())
		{
			label_upgrade.Key = "skip";
			return;
		}
		TimeSpan timeSpan = new TimeSpan(0, 0, Konfiguration.GetUpgradeTime(item, ShotType.None));
		label_upgrade_duration.textMesh.text = string.Format("training_duration".Localize(), timeSpan.Humanize());
		label_upgrade_duration.textMesh.Commit();
		label_upgrade_duration.gameObject.SetActive(Konfiguration.GetUpgradeTime(item, ShotType.None) != 0 && use_upgrade_queue);
		description.Key = item.ToString() + ".description";
		description.Localize();
		label_upgrade.Key = ((!isUnlock()) ? "upgrade" : "Buy");
	}

	private void setInfoContainer()
	{
		infoContainer.Set(item, base.SortingLayerID, !isUnlock(), false, true);
	}

	private void SetAnimation()
	{
		Animator componentInChildren = itemPortrait.GetComponentInChildren<Animator>();
		if (componentInChildren != null)
		{
			string stateName = ((!isUpgrading()) ? "stand" : "move");
			componentInChildren.CrossFade(stateName, 0.3f);
		}
	}

	protected override void setInfos(bool isSpecialOffer)
	{
		itemPortrait = UnityEngine.Object.Instantiate(Resources.Load<Transform>("GUI/Unlocks/Dino_" + item)) as Transform;
		SetAnimation();
		setInfoContainer();
		setTexts();
		FX_Upgrade.GetComponent<Renderer>().enabled = !isUnlock() && !Konfiguration.isConsumable(item);
		upgradeButton.transform.parent.parent.LocalPosX((!isSpecialOffer) ? upgrade_button_offset : 0f);
		stats_bg.gameObject.SetActive(!isSpecialOffer);
		if (isUpgrading())
		{
			EntityTimers.onUpgradeComplete -= HandleonUpgradeComplete;
			EntityTimers.onUpgradeComplete += HandleonUpgradeComplete;
		}
		if (Konfiguration.isConsumable(item))
		{
			cosumable_count_label.transform.parent.gameObject.SetActive(true);
			cosumable_count_label.text = Player.getConsumableRefillAmount(item).ToString();
		}
	}

	private void OnDisable()
	{
		EntityTimers.onUpgradeComplete -= HandleonUpgradeComplete;
	}

	protected override void Update()
	{
		base.Update();
		if (isUpgrading())
		{
			description.textMesh.text = string.Format("entity_is_ready_in".Localize(), item.ToString().Localize(), EntityTimers.getUpgradeTime(item).Humanize());
			description.textMesh.Commit();
			Color color = label_upgrade_duration.textMesh.color;
			if (color != Colors.Invisible && label_upgrade_duration.gameObject.activeSelf)
			{
				label_upgrade_duration.textMesh.color = Color.Lerp(color, Colors.Invisible, Time.deltaTime * 5f);
			}
		}
		else
		{
			Color color2 = label_upgrade_duration.textMesh.color;
			if (color2 != Colors.GuiBrown1 && label_upgrade_duration.gameObject.activeSelf)
			{
				label_upgrade_duration.textMesh.color = Color.Lerp(color2, Colors.GuiBrown1, Time.deltaTime * 5f);
			}
		}
	}

	private void HandleonUpgradeComplete(UnitType unit)
	{
		if (unit == item)
		{
			UpdateEntries();
			FX_onBuy.gameObject.SetActive(true);
			SetAnimation();
		}
	}

	protected override int getUpgradeLevelRequirement()
	{
		return Konfiguration.GetUpgradeRequirement(item);
	}

	protected override bool isUpgrading()
	{
		if (use_upgrade_queue)
		{
			return EntityTimers.is_upgrading(item);
		}
		return false;
	}

	protected override bool isPremium()
	{
		return Konfiguration.isPremium(item);
	}

	protected override bool isUnlock()
	{
		return !Player.AvailableUnitTypes.Contains(item) && item != UnitType.DinoEgg;
	}
}
