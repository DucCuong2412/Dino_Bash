using dinobash;

public class ShotAdapter : DinoShotUpgradeAdapter
{
	public ShotType shot;

	public override string name
	{
		get
		{
			return shot.ToString();
		}
	}

	public override int UpgradeCost
	{
		get
		{
			return Konfiguration.getShotUpgradeCost(shot);
		}
	}

	public override int AttackPower
	{
		get
		{
			return Konfiguration.getShotDamage(shot);
		}
	}

	public override int AttackPowerNextLevel
	{
		get
		{
			return Konfiguration.getShotDamage(shot, true);
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
			return Player.GetShotLevel(shot);
		}
	}

	public override bool CanLevelUp
	{
		get
		{
			return Konfiguration.canLevelUp(shot);
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
			return 1;
		}
	}

	public override bool isAvailable
	{
		get
		{
			return Player.AvailableShotTypes.Contains(shot);
		}
	}

	public ShotAdapter(ShotType shot)
	{
		this.shot = shot;
	}
}
