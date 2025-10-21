using UnityEngine;
using dinobash;

public struct ShotData
{
	public ShotType type;

	public int[] damageLevels;

	public float[] cooldownLevels;

	public int[] costLevels;

	public float stunDuration;

	public string[] level_requirements;

	private int level
	{
		get
		{
			if (App.State != App.States.Game)
			{
				return Player.GetShotLevel(type);
			}
			return Level.Instance.ShotLevels[type];
		}
	}

	public int damage
	{
		get
		{
			if (damageLevels == null)
			{
				Debug.LogError(type.ToString() + "damageLevels are not defined");
			}
			return damageLevels[level];
		}
	}

	public float cooldown
	{
		get
		{
			if (cooldownLevels == null)
			{
				Debug.LogError(type.ToString() + "cooldownLevels are not defined");
			}
			return cooldownLevels[level] * ShotFactory.globaleCooldownModifier;
		}
	}

	public int cost
	{
		get
		{
			if (costLevels == null)
			{
				Debug.LogError(type.ToString() + "costLevels are not defined");
			}
			return costLevels[level];
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
