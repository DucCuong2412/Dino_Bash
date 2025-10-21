using UnityEngine;
using dinobash;

public class UpgradeInfoUpgradesScreen : UpgradeInfoScreen<UnitType>
{
	private bool is_temporal_item
	{
		get
		{
			return Konfiguration.UnitData[item].Minutes_available > 0;
		}
	}

	protected override int getCost()
	{
		if (isPremium() && isUnlock())
		{
			return Konfiguration.UnitData[item].premiumCost;
		}
		return Konfiguration.getUpgradeBuyCost(item);
	}

	protected override void OnClickedUpgrade()
	{
		int cost = 0;
		if (!Debit(out cost))
		{
			return;
		}
		upgradeButton.OnClick -= OnClickedUpgrade;
		Player.ActivateUpgrade(item);
		int spent = cost;
		string origin = get_tracking_origin(cost);
		Tracking.buy_item(item, ShotType.None, string.Empty, spent, origin);
		if (App.State == App.States.Map)
		{
			CheckForAchievement();
			UpgradeScreen screen = ScreenManager.GetScreen<UpgradeScreen>();
			if (screen != null && screen.isVisible)
			{
				screen.getListEntry(item).UpdateEntry(true);
			}
			UnitType previousUpgradeStage = Konfiguration.getPreviousUpgradeStage(item);
			if (previousUpgradeStage != 0 && screen != null && screen.isVisible)
			{
				screen.getListEntry(previousUpgradeStage).UpdateEntry();
			}
		}
		FX_onBuy.gameObject.SetActive(true);
		hide_wait_duration = 0.75f;
		Hide();
	}

	protected override void setInfos(bool isSpecialOffer)
	{
		string text = item.ToString() + ".description";
		if (item == UnitType.AppleStartBonus)
		{
			description.textMesh.text = string.Format(text.Localize(), Konfiguration.GameConfig.AppleBoostAmount);
		}
		else
		{
			description.Key = text;
		}
		itemPortrait = Object.Instantiate(Resources.Load<Transform>("GUI/Unlocks/Upgrade_" + item)) as Transform;
		label_upgrade.Key = "Buy";
		if (is_temporal_item)
		{
			label_upgrade_duration.gameObject.SetActive(true);
			label_upgrade.Key = "activate_item";
			setTexts();
		}
		else
		{
			label_upgrade_duration.gameObject.SetActive(false);
		}
		upgradeButton.transform.parent.parent.LocalPosX(0f);
		stats_bg.gameObject.SetActive(false);
	}

	private void setTexts()
	{
		if (is_temporal_item)
		{
			int minutes_available = Konfiguration.UnitData[item].Minutes_available;
			label_upgrade_duration.textMesh.text = string.Format("item_boost_duration".Localize(), Mathf.RoundToInt((float)minutes_available / 60f / 24f));
		}
	}

	protected override void Update()
	{
		base.Update();
		setTexts();
	}

	protected override int getUpgradeLevelRequirement()
	{
		return Konfiguration.GetUpgradeRequirement(item);
	}

	protected override bool isUpgrading()
	{
		return EntityTimers.is_upgrading(item);
	}

	protected override bool isPremium()
	{
		return Konfiguration.isPremium(item);
	}

	protected override bool isUnlock()
	{
		return !Player.AvailiableUpgrades.Contains(item);
	}
}
