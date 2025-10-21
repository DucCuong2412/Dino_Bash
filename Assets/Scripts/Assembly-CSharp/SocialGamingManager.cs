using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using GooglePlayGames;
using UnityEngine;
using dinobash;

public class SocialGamingManager
{
	[Serializable]
	public class AchievementState
	{
		[XmlAttribute]
		public string achievement_id;

		[XmlAttribute]
		public bool reporting_succeeded = true;

		[XmlAttribute]
		public bool completion_reported;

		[XmlAttribute]
		public bool popup_shown;

		[XmlAttribute]
		public int events_done;

		[XmlAttribute]
		public int total_events;

		public double Progress
		{
			get
			{
				return Math.Min(100.0, 100.0 * (double)events_done / (double)total_events);
			}
		}

		public bool Complete
		{
			get
			{
				return Progress == 100.0;
			}
		}

		public override string ToString()
		{
			object[] args = new object[7]
			{
				achievement_id,
				reporting_succeeded,
				completion_reported,
				popup_shown,
				events_done,
				total_events,
				Progress / 100.0
			};
			return string.Format("<AchievementState achievement_id={0} reporting_succeeded={1} completion_reported={2} popup_shown={3} events_done={4} total_events={5} progress={6:P2}>", args);
		}
	}

	private static readonly SocialGamingManager _instance = new SocialGamingManager();

	private bool _initialized;

	public static SocialGamingManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public bool isAuthenticated
	{
		get
		{
			if (Social.localUser == null)
			{
				return false;
			}
			return Social.localUser.authenticated;
		}
	}

	public void Initialize(Action onDone = null)
	{
		if (!_initialized)
		{
			PlayGamesPlatform.Activate();
			_initialized = true;
		}
		if (onDone != null)
		{
			onDone();
		}
	}

	public void Authenticate(Action<bool> onDone = null)
	{
		Initialize(delegate
		{
			if (!isAuthenticated)
			{
				Social.localUser.Authenticate(delegate(bool success)
				{
					if (Player.Instance != null)
					{
						Tracking.social_gaming_login(Social.localUser.id);
						Player.Instance.wasLoggedIntoSocialGamingProvider = success;
						Player.SavePlayer();
					}
					if (success)
					{
						ReReportAchievements();
					}
					if (onDone != null)
					{
						onDone(success);
					}
				});
			}
			else if (onDone != null)
			{
				onDone(isAuthenticated);
			}
		});
	}

	private void ReportProgress(string achievement_id, double progress, Action<bool> onDone = null)
	{
		if (isAuthenticated)
		{
			if ((PlayGamesPlatform)Social.Active != null)
			{
				progress /= 100.0;
			}
			Social.ReportProgress(achievement_id, progress, onDone);
		}
		else if (onDone != null)
		{
			onDone(false);
		}
	}

	private void printAchievements()
	{
		if (Player.Instance == null || Player.Instance.achievementStates == null)
		{
			Debug.LogError("ReportProgress aborted because something is null");
			return;
		}
		string text = string.Empty;
		foreach (AchievementState achievementState in Player.Instance.achievementStates)
		{
			text = string.Concat(text, achievementState, "\n");
		}
		Debug.Log(text);
	}

	private void RecursiveReport(List<AchievementState> to_report)
	{
		if (to_report.Count == 0)
		{
			Debug.Log("**** done re-reporting achievements");
			printAchievements();
			Player.SavePlayer();
			return;
		}
		AchievementState entry = to_report[0];
		to_report.RemoveAt(0);
		Social.ReportProgress(entry.achievement_id, entry.Progress, delegate(bool success)
		{
			Debug.Log("**** recursive report success=" + success);
			entry.reporting_succeeded = isAuthenticated;
			RecursiveReport(to_report);
		});
	}

	private void ReReportAchievements()
	{
		Debug.Log("**** re-reporting achievements");
		printAchievements();
		List<AchievementState> list = new List<AchievementState>();
		foreach (AchievementState achievementState in Player.Instance.achievementStates)
		{
			if (isAuthenticated && !achievementState.completion_reported && !achievementState.reporting_succeeded)
			{
				list.Add(achievementState);
			}
		}
		RecursiveReport(list);
	}

	public void ReportProgress(string achievement_id, int num_events, int num_total_events)
	{
		if (Player.Instance == null || Player.Instance.achievementStates == null)
		{
			Debug.LogError("ReportProgress aborted because something is null");
			return;
		}
		AchievementState state = Player.Instance.achievementStates.Find((AchievementState x) => x.achievement_id == achievement_id);
		if (state == null)
		{
			state = new AchievementState();
			state.achievement_id = achievement_id;
			Player.Instance.achievementStates.Add(state);
		}
		state.events_done += num_events;
		state.total_events = num_total_events;
		state.reporting_succeeded = false;
		if (!isAuthenticated || state.completion_reported)
		{
			return;
		}
		ReportProgress(achievement_id, state.Progress, delegate(bool success)
		{
			state.completion_reported = success && state.Complete;
			state.reporting_succeeded = isAuthenticated;
			if (!state.popup_shown && state.completion_reported)
			{
				ShowAchievementPopup(achievement_id);
				state.popup_shown = true;
			}
			Player.SavePlayer();
		});
	}

	public void ReportProgress(string achievement_id, int num_total_events)
	{
		ReportProgress(achievement_id, 1, num_total_events);
	}

	public void ReportProgress(string achievement_id)
	{
		ReportProgress(achievement_id, 1);
	}

	public void ReportScore(string leaderboard_id, long score, Action<bool> onDone = null)
	{
		if (isAuthenticated)
		{
			Social.ReportScore(score, leaderboard_id, onDone);
		}
		else if (onDone != null)
		{
			onDone(false);
		}
	}

	public void ShowAchievementsUI(Action<bool> onDone = null)
	{
		Authenticate(delegate(bool success)
		{
			if (success && App.State == App.States.StartScreen)
			{
				Social.ShowAchievementsUI();
			}
			if (onDone != null)
			{
				onDone(success);
			}
		});
	}

	public void ShowLeaderboardUI(string leaderboard_id = null, Action<bool> onDone = null)
	{
		Authenticate(delegate(bool success)
		{
			if (success && App.State == App.States.StartScreen)
			{
				if (leaderboard_id == null || leaderboard_id.Length == 0 || (PlayGamesPlatform)Social.Active == null)
				{
					Social.ShowLeaderboardUI();
				}
				else
				{
					((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(leaderboard_id);
				}
			}
			if (onDone != null && App.State == App.States.StartScreen)
			{
				onDone(success);
			}
		});
	}

	public void ShowAchievementPopup(string achievement_id)
	{
		Debug.Log("ShowAchievementPopup: " + achievement_id);
	}
}
