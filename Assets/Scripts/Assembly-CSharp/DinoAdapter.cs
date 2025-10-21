using System;
using dinobash;

public class DinoAdapter : DinoShotUpgradeAdapter
{
	public UnitType dino;

	public override string name
	{
		get
		{
			return dino.ToString();
		}
	}

	public override int UpgradeCost
	{
		get
		{
			return Konfiguration.getDinoUpgradeCost(dino);
		}
	}

	public override int AttackPower
	{
		get
		{
			return Konfiguration.getAttackPower(dino);
		}
	}

	public override int AttackPowerNextLevel
	{
		get
		{
			return Konfiguration.getAttackPower(dino, true);
		}
	}

	public override int Health
	{
		get
		{
			return Konfiguration.getEntityHealth(dino);
		}
	}

	public override int HealthNextLevel
	{
		get
		{
			return Konfiguration.getEntityHealth(dino, true);
		}
	}

	public override int Level
	{
		get
		{
			return Player.GetUnitLevel(dino);
		}
	}

	public override bool CanLevelUp
	{
		get
		{
			return Konfiguration.canLevelUp(dino);
		}
	}

	public override int ApplePrice
	{
		get
		{
			return Konfiguration.getEntitiyAppleCost(dino);
		}
	}

	public override int PremiumUnlockCost
	{
		get
		{
			return Konfiguration.UnitData[dino].premiumCost;
		}
	}

	public override bool isAvailable
	{
		get
		{
			return Player.AvailableUnitTypes.Contains(dino) || dino == UnitType.DinoEgg;
		}
	}

	public DinoAdapter(UnitType unit)
	{
		if (Konfiguration.isDinoUnit(unit))
		{
			dino = unit;
			return;
		}
		throw new Exception("not a dino unit: " + unit);
	}
}
