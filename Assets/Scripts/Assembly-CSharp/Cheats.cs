using UnityEngine;
using dinobash;

public static class Cheats
{
	public static bool UnlockDinosAndShots { get; set; }

	public static bool UnlockLevels { get; set; }

	public static bool ShowLevelDuration { get; set; }

	public static bool ShowUnitNames { get; set; }

	public static int LabelCount { get; set; }

	public static bool PreselectUnits { get; set; }

	public static void UnlockAllUnits()
	{
		foreach (UnitType dino in Konfiguration.getDinos())
		{
			if (dino != 0 && Konfiguration.isDinoUnit(dino) && !Konfiguration.isUpgrade(dino))
			{
				Player.UnlockDino(dino);
			}
		}
	}

	public static void UnlockAllShots()
	{
		foreach (ShotType shot in Konfiguration.getShots())
		{
			if (shot != ShotType.None && !Player.AvailableShotTypes.Contains(shot))
			{
				Player.UnlockShot(shot);
			}
		}
	}

	public static void UnlockAllUpgrades()
	{
		foreach (UnitType upgrade in Konfiguration.getUpgrades())
		{
			if (Konfiguration.isUpgrade(upgrade) && !Player.AvailiableUpgrades.Contains(upgrade))
			{
				Player.AvailiableUpgrades.Add(upgrade);
				Debug.Log("adding: " + upgrade);
			}
		}
	}
}
