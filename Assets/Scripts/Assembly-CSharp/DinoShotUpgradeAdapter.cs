public abstract class DinoShotUpgradeAdapter
{
	public abstract string name { get; }

	public abstract int UpgradeCost { get; }

	public abstract int AttackPower { get; }

	public abstract int AttackPowerNextLevel { get; }

	public abstract int Health { get; }

	public abstract int HealthNextLevel { get; }

	public abstract int Level { get; }

	public abstract bool CanLevelUp { get; }

	public abstract int ApplePrice { get; }

	public abstract int PremiumUnlockCost { get; }

	public abstract bool isAvailable { get; }
}
