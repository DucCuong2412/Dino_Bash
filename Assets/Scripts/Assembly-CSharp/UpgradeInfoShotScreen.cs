using UnityEngine;
using dinobash;

public class UpgradeInfoShotScreen : UpgradeInfoScreen<ShotType>
{
	protected override int getCost()
	{
		return Konfiguration.getShotUpgradeCost(item);
	}

	protected override void OnClickedUpgrade()
	{
		int cost = 0;
		if (!Debit(out cost))
		{
			return;
		}
		upgradeButton.OnClick -= OnClickedUpgrade;
		if (isUnlock())
		{
			Player.UnlockShot(item);
		}
		else
		{
			Player.UpgradeShot(item);
		}
		ShotType shot = item;
		string origin = get_tracking_origin(cost);
		Tracking.buy_item(UnitType.None, shot, string.Empty, cost, origin);
		if (App.State == App.States.Map)
		{
			UpgradeScreen screen = ScreenManager.GetScreen<UpgradeScreen>();
			if (screen != null && screen.isVisible)
			{
				shot = item;
				screen.getListEntry(UnitType.None, shot).UpdateEntry(true);
			}
			CheckForAchievement();
		}
		FX_onBuy.gameObject.SetActive(true);
		hide_wait_duration = 0.75f;
		Hide();
	}

	protected override void setInfos(bool isSpecialOffer)
	{
		description.Key = item.ToString() + ".description";
		itemPortrait = Object.Instantiate(Resources.Load<Transform>("GUI/Unlocks/Shot_" + item)) as Transform;
		label_upgrade.Key = ((!isUnlock()) ? "upgrade" : "Buy");
		FX_Upgrade.GetComponent<Renderer>().enabled = !isUnlock();
		infoContainer.Set(item, base.SortingLayerID, !isUnlock(), false, true);
		upgradeButton.transform.parent.parent.LocalPosX((!isSpecialOffer) ? upgrade_button_offset : 0f);
		stats_bg.gameObject.SetActive(!isSpecialOffer);
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
		return !Player.AvailableShotTypes.Contains(item);
	}
}
