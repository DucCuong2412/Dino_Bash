using dinobash;

public class UpgradeAdapter : DinoShotUpgradeAdapter
{
	public UnitType upgrade;

	public override string name
	{
		get
		{
			return upgrade.ToString();
		}
	}

	public override int UpgradeCost
	{
		get
		{
			return Konfiguration.getUpgradeBuyCost(upgrade);
		}
	}

	public override int AttackPower
	{
		get
		{
			return -1;
		}
	}

	public override int AttackPowerNextLevel
	{
		get
		{
			return -1;
		}
	}

	public override int Health
	{
		get
		{
			return -1;
		}
	}

	public override int HealthNextLevel
	{
		get
		{
			return -1;
		}
	}

	public override int Level
	{
		get
		{
			return -1;
		}
	}

	public override bool CanLevelUp
	{
		get
		{
			return Konfiguration.canLevelUp(upgrade);
		}
	}

	public override int ApplePrice
	{
		get
		{
			return -1;
		}
	}

	public override int PremiumUnlockCost
	{
		get
		{
			return Konfiguration.UnitData[upgrade].premiumCost;
		}
	}

	public override bool isAvailable
	{
		get
		{
			return Player.AvailiableUpgrades.Contains(upgrade);
		}
	}

	public UpgradeAdapter(UnitType upgrade)
	{
		this.upgrade = upgrade;
	}
}
