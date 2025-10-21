using UnityEngine;
using dinobash;

public struct EntityData
{
	public readonly UnitType unit;

	public bool isUnique;

	public int[] healthpointslevels;

	public float walkspeed;

	public int attackRange;

	public int appleCost;

	public int premiumCost;

	public int[] costLevels;

	public float buildcooldown;

	public float dropChance;

	public int[] attackStrenghLevels;

	public int override_unit_level;

	private int _level;

	public string command;

	public int[] upgrade_skip_costs;

	public int[] upgrade_durations;

	public int minutes_available;

	public string[] level_requirements;

	public bool isFriendly
	{
		get
		{
			return !Konfiguration.isNeander(unit);
		}
	}

	public int healthpoints
	{
		get
		{
			return healthpointslevels[level];
		}
	}

	public int cost
	{
		get
		{
			return costLevels[level];
		}
	}

	public int level
	{
		get
		{
			if (override_unit_level > 0)
			{
				return override_unit_level;
			}
			if (isFriendly)
			{
				if (App.State == App.States.Game && Level.Instance.UnitLevels.ContainsKey(unit))
				{
					return Level.Instance.UnitLevels[unit];
				}
				return Player.GetUnitLevel(unit);
			}
			return _level;
		}
		set
		{
			if (!isFriendly)
			{
				_level = Mathf.Clamp(value, 0, attackStrenghLevels.Length - 1);
			}
		}
	}

	public int attackStrengh
	{
		get
		{
			return attackStrenghLevels[level];
		}
	}

	public int upgrade_skip_cost
	{
		get
		{
			return upgrade_skip_costs[level];
		}
	}

	public int upgrade_duration
	{
		get
		{
			return upgrade_durations[level];
		}
	}

	public int Minutes_available
	{
		get
		{
			if (Konfiguration.GameConfig.Use_temp_upgrades)
			{
				return minutes_available;
			}
			return 0;
		}
	}

	public string level_requirement
	{
		get
		{
			int num = Mathf.Clamp(level, 0, level_requirements.Length);
			return level_requirements[num];
		}
	}
}
