using UnityEngine;

public class SocialGamingIds
{
	public static readonly AchievementIds achievements;

	public static readonly LeaderboardIds leaderboards;

	public static string getI18NKeyForLeaderboardId(string LeaderboardId)
	{
		switch (LeaderboardId)
		{
		case "CgkIsrfD6ZodEAIQGA":
			return "TOTAL_COINS";
		case "CgkIsrfD6ZodEAIQCA":
			return "_1ST_SURVIVAL_LEVEL";
		case "CgkIsrfD6ZodEAIQCQ":
			return "_2ND_SURVIVAL_LEVEL";
		case "CgkIsrfD6ZodEAIQCg":
			return "_3RD_SURVIVAL_LEVEL";
		case "CgkIsrfD6ZodEAIQCw":
			return "_4TH_SURVIVAL_LEVEL";
		case "CgkIsrfD6ZodEAIQGw":
			return "_5TH_SURVIVAL_LEVEL";
		default:
			Debug.LogError("Unknown Leaderboard Id" + LeaderboardId);
			return "Unknown Leaderboard Id '" + LeaderboardId + "'";
		}
	}

	public static string getI18NKeyForAchievementId(string AchievementId)
	{
		switch (AchievementId)
		{
		case "CgkIsrfD6ZodEAIQAQ":
			return "COMPLETED_FIRST_ISLAND";
		case "CgkIsrfD6ZodEAIQAg":
			return "COMPLETED_THE_SECOND_ISLAND";
		case "CgkIsrfD6ZodEAIQBA":
			return "COMPLETED_THE_THIRD_ISLAND";
		case "CgkIsrfD6ZodEAIQBQ":
			return "COMPLETED_THE_FOURTH_ISLAND";
		case "CgkIsrfD6ZodEAIQGg":
			return "COMPLETED_THE_FIFTH_ISLAND";
		case "CgkIsrfD6ZodEAIQBg":
			return "COLLECTED_10000_COINS";
		case "CgkIsrfD6ZodEAIQDA":
			return "COLLECTED_100000_COINS";
		case "CgkIsrfD6ZodEAIQDQ":
			return "USE_FIVE_MEGABALLS";
		case "CgkIsrfD6ZodEAIQDg":
			return "USE_20_MEGABALLS";
		case "CgkIsrfD6ZodEAIQDw":
			return "KILL_100_CAVEMEN";
		case "CgkIsrfD6ZodEAIQEA":
			return "DEPLOY_500_DINOSAURS";
		case "CgkIsrfD6ZodEAIQEQ":
			return "DEPLOY_50_RAPTORS";
		case "CgkIsrfD6ZodEAIQEg":
			return "DEPLOY_50_BRACHIOS";
		case "CgkIsrfD6ZodEAIQEw":
			return "COMPLETELY_UPGRADE_YOUR_APPLE_COUNTER";
		case "CgkIsrfD6ZodEAIQFA":
			return "COMPLETELY_UPGRADE_EVERYTHING_IN_THE_UPGRADE_MENU";
		case "CgkIsrfD6ZodEAIQFQ":
			return "RATE_DINO_BASH";
		case "CgkIsrfD6ZodEAIQFg":
			return "HEAL_A_DINOSAUR";
		case "CgkIsrfD6ZodEAIQFw":
			return "GIVE_A_LIVE_TO_A_FRIEND";
		default:
			Debug.LogError("Unknown Achievement Id" + AchievementId);
			return "Unknown Achievement Id '" + AchievementId + "'";
		}
	}
}
