using System;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class EntityTimers : MonoBehaviour
{
	private DateTime time;

	private static SerializableList<TimerItem> temp_unlocks
	{
		get
		{
			return Player.Instance.PlayerData.temp_unlocks;
		}
	}

	private static SerializableList<TimerItem> upgrade_timers
	{
		get
		{
			return Player.Instance.PlayerData.upgrade_queue;
		}
	}

	private static SerializableList<UnitType> newly_trained_dinos
	{
		get
		{
			return Player.Instance.PlayerData.newly_trained_dinos;
		}
	}

	public static event Action<UnitType> onUpgradeStarted;

	public static event Action<UnitType> onUpgradeComplete;

	public static event Action<UnitType> onTemporaryUpgradeOver;

	private void Start()
	{
		time = DateTime.UtcNow.AddSeconds(1.0);
	}

	private void Update()
	{
		DateTime utcNow = DateTime.UtcNow;
		if (!(time > utcNow))
		{
			time = utcNow.AddSeconds(1.0);
			UpdateItems();
		}
	}

	private void UpdateItems()
	{
		List<TimerItem> list = upgrade_timers.FindAll((TimerItem item) => item.Done);
		list.ForEach(delegate(TimerItem item)
		{
			Debug.Log("upgrade complete: " + item.entity.ToString() + " - " + item.shot);
			if (item.entity != 0 && Konfiguration.isDinoUnit(item.entity))
			{
				Player.UpgradeUnit(item.entity, false);
				if (EntityTimers.onUpgradeComplete != null)
				{
					EntityTimers.onUpgradeComplete(item.entity);
				}
				UpdateNewlyTrainedDinos(item.entity);
			}
			else if (item.shot != ShotType.None)
			{
				Player.UpgradeShot(item.shot, false);
			}
		});
		foreach (TimerItem item in list)
		{
			upgrade_timers.Remove(item);
		}
		list = temp_unlocks.FindAll((TimerItem item) => item.Done);
		list.ForEach(delegate(TimerItem item)
		{
			RemoveTempUnlock(item);
		});
		temp_unlocks.RemoveAll((TimerItem item) => item.Done);
	}

	private void UpdateNewlyTrainedDinos(UnitType entity)
	{
		UpgradeInfoDinoScreen screen = ScreenManager.GetScreen<UpgradeInfoDinoScreen>();
		UpgradeScreen screen2 = ScreenManager.GetScreen<UpgradeScreen>();
		if ((screen == null || !screen.isVisible) && (screen2 == null || !screen2.isVisible || screen2.CurrentOpenTab != 0))
		{
			newly_trained_dinos.Add(entity);
		}
	}

	public static UnitType GetNewlyTrainedDino()
	{
		if (newly_trained_dinos.Count > 0)
		{
			return newly_trained_dinos[newly_trained_dinos.Count - 1];
		}
		return UnitType.None;
	}

	public static void RemoveFromNewlyTrainedList(UnitType entity)
	{
		if (newly_trained_dinos.Contains(entity))
		{
			newly_trained_dinos.Remove(entity);
		}
	}

	public void Add(UnitType unit)
	{
		if (upgrade_timers.Find((TimerItem item) => item.entity == unit) == null)
		{
			Add(unit, ShotType.None);
		}
		else
		{
			Debug.LogError("Upgrade already in queue");
		}
	}

	public void Add(ShotType shot)
	{
		if (upgrade_timers.Find((TimerItem item) => item.shot == shot) == null)
		{
			Add(UnitType.None, shot);
		}
		else
		{
			Debug.LogError("Upgrade already in queue");
		}
	}

	private void Add(UnitType unit, ShotType shot)
	{
		int upgradeTime = Konfiguration.GetUpgradeTime(unit, shot);
		upgrade_timers.Add(new TimerItem(unit, shot, upgradeTime));
		if (EntityTimers.onUpgradeStarted != null)
		{
			EntityTimers.onUpgradeStarted(unit);
		}
		UpdateItems();
	}

	public void Remove(UnitType unit)
	{
		TimerItem timerItem = upgrade_timers.Find((TimerItem x) => x.entity == unit);
		if (timerItem != null)
		{
			upgrade_timers.Remove(timerItem);
		}
	}

	public void Remove(ShotType shot)
	{
		TimerItem timerItem = upgrade_timers.Find((TimerItem x) => x.shot == shot);
		if (timerItem != null)
		{
			upgrade_timers.Remove(timerItem);
		}
	}

	public void AddTempUnlock(UnitType unit)
	{
		if (Konfiguration.UnitData[unit].Minutes_available != 0 && temp_unlocks.Find((TimerItem x) => x.entity == unit) == null)
		{
			int seconds_to_completion = Konfiguration.UnitData[unit].Minutes_available * 60;
			TimerItem item = new TimerItem(unit, ShotType.None, seconds_to_completion);
			temp_unlocks.Add(item);
		}
	}

	private void RemoveTempUnlock(TimerItem item)
	{
		if (item != null)
		{
			temp_unlocks.Remove(item);
			if (Konfiguration.isUpgrade(item.entity))
			{
				Player.DisableUpgrade(item.entity);
			}
			if (EntityTimers.onTemporaryUpgradeOver != null)
			{
				EntityTimers.onTemporaryUpgradeOver(item.entity);
			}
		}
	}

	public static bool is_temporarily_unlocked(UnitType unit)
	{
		return temp_unlocks.Find((TimerItem item) => item.entity == unit) != null;
	}

	public static bool is_upgrading(UnitType entity)
	{
		return upgrade_timers.Find((TimerItem item) => item.entity == entity) != null;
	}

	public static bool is_upgrading(ShotType shot)
	{
		return upgrade_timers.Find((TimerItem item) => item.shot == shot) != null;
	}

	public static TimeSpan getTempUnlockTime(UnitType entity)
	{
		if (is_temporarily_unlocked(entity))
		{
			return temp_unlocks.Find((TimerItem item) => item.entity == entity).completion_time - DateTime.UtcNow;
		}
		return new TimeSpan(0, 0, 0);
	}

	public static TimeSpan getUpgradeTime(UnitType entity)
	{
		if (is_upgrading(entity))
		{
			return upgrade_timers.Find((TimerItem item) => item.entity == entity).completion_time - DateTime.UtcNow;
		}
		return new TimeSpan(0, 0, 0);
	}

	public static TimeSpan getUpgradeTime(ShotType shot)
	{
		return upgrade_timers.Find((TimerItem item) => item.shot == shot).completion_time - DateTime.UtcNow;
	}
}
