using System;
using dinobash;

public static class NotificationManager
{
	private static bool isInitialized;

	public static bool debug_mode;

	public static void OnApplicationPause(bool isPaused)
	{
		if (isPaused)
		{
			OnAppExit();
		}
		else
		{
			OnAppEnter();
		}
	}

	public static void Init()
	{
		if (!isInitialized)
		{
			isInitialized = true;
		}
	}

	private static void OnAppEnter()
	{
	}

	private static void ScheduleLocalNotification(string i18n_key, DateTime when)
	{
		SetNotification(i18n_key.Localize(), when);
	}

	private static void SetNotification(string message, DateTime when)
	{
	}

	private static void OnAppExit()
	{
		debug_mode = false;
	}

	private static void SetTimerNotifications()
	{
		if (!Konfiguration.GameConfig.use_upgrade_timers)
		{
			return;
		}
		foreach (TimerItem item in Player.Instance.PlayerData.upgrade_queue)
		{
			string message = string.Format("NOTIFICATION_TRAINING_COMPLETE".Localize(), item.entity.ToString().Localize());
			SetNotification(message, item.completion_time);
		}
	}
}
