using System;
using UnityEngine;

[Serializable]
public class TimerItem
{
	public UnitType entity;

	public ShotType shot = ShotType.None;

	public DateTime completion_time;

	public bool Done
	{
		get
		{
			return DateTime.UtcNow >= completion_time;
		}
	}

	public TimerItem()
	{
	}

	public TimerItem(UnitType unittype, ShotType shottype, int seconds_to_completion)
	{
		if (unittype != 0)
		{
			entity = unittype;
		}
		else
		{
			if (shottype == ShotType.None)
			{
				throw new Exception("invalid construction of UpgradeQueueItem");
			}
			shot = shottype;
		}
		completion_time = DateTime.UtcNow.AddSeconds(seconds_to_completion);
		Debug.Log(string.Format("UpgradeJob: {0}, {1}, {2}", entity, shot, completion_time));
	}
}
