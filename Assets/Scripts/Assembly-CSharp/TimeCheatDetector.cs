using System;
using System.Threading;
using UnityEngine;

public class TimeCheatDetector : MonoBehaviour
{
	public static volatile bool time_cheat_detected;

	private DateTime last_time;

	private volatile bool should_exit;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		last_time = DateTime.UtcNow;
	}

	private void OnDestroy()
	{
		should_exit = true;
	}

	private void Check()
	{
		while (!should_exit)
		{
			Thread.Sleep(1000);
			DateTime utcNow = DateTime.UtcNow;
			if (Mathf.Abs((float)(utcNow - last_time).TotalSeconds) > 10f)
			{
				time_cheat_detected = true;
			}
			last_time = utcNow;
		}
	}
}
