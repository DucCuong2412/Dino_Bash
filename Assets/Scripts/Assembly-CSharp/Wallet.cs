using System;
using UnityEngine;
using dinobash;

public class Wallet : MonoBase
{
	public enum Currency
	{
		Coins = 0,
		Diamonds = 1,
		StarterPack = 2,
		RegularPack = 3
	}

	private const string key0 = ")UK^sg\nF=&\r]_T>r";

	private const string key1 = "j{Un\r)azU\n$yQg¶";

	private const string key2 = "Rx\tM4_562e1Hp\txy";

	private const string key3 = "H#YHwify.&W]qF\fJ";

	private const string key4 = "?)-\ni90Jp\niO(]be";

	private const string key5 = "LPQ5J!T@%sHmo3w.";

	private const string key6 = "ZczanelMIZgjGmtC";

	private const string key7 = "u(6.{4om\\g\\Spb2p";

	private static int coins;

	private static int diamonds;

	private static float total_spent;

	public static bool IsPayingUser { get; set; }

	public static int Coins
	{
		get
		{
			return coins;
		}
	}

	public static int Diamonds
	{
		get
		{
			return diamonds;
		}
	}

	public static float Total_spent
	{
		get
		{
			return total_spent;
		}
		set
		{
			total_spent = value;
		}
	}

	public static event Action OnBalanceChanged;

	public static void Load()
	{
		int result = -1;
		coins = (diamonds = 0);
		IsPayingUser = false;
		int.TryParse(PlayerPrefs.GetString("coins"), out coins);
		int.TryParse(PlayerPrefs.GetString("diamonds"), out diamonds);
		int.TryParse(PlayerPrefs.GetString("check_sum".Encrypt("?)-\ni90Jp\niO(]be")).Decrypt("LPQ5J!T@%sHmo3w."), out result);
		bool result2 = false;
		bool.TryParse(PlayerPrefs.GetString("isPayingUser"), out result2);
		IsPayingUser = result2;
		total_spent = 0f;
		float.TryParse(PlayerPrefs.GetString("total_spent"), out total_spent);
	}

	private static void Save()
	{
		PlayerPrefs.SetString("coins", coins.ToString());
		PlayerPrefs.SetString("diamonds", diamonds.ToString());
		PlayerPrefs.SetString("check_sum".Encrypt("?)-\ni90Jp\niO(]be"), (coins + diamonds).ToString().Encrypt("LPQ5J!T@%sHmo3w."));
		PlayerPrefs.SetString("isPayingUser", IsPayingUser.ToString());
		PlayerPrefs.SetString("total_spent", total_spent.ToString());
		PlayerPrefs.Save();
	}

	public static void ApplyCloudSaveGame(CloudSaveGames.CloudSaveGame csg)
	{
		coins = csg.wallet.coins;
		diamonds = csg.wallet.diamonds;
		IsPayingUser = csg.wallet.isPayingUser;
		total_spent = csg.wallet.total_spent;
	}

	private static void onBalanceChange()
	{
		if (Wallet.OnBalanceChanged != null)
		{
			Wallet.OnBalanceChanged();
		}
	}

	public static void Reset()
	{
		TakeCoins(Coins);
		TakeDiamonds(Diamonds);
		//App.Instance.paymentManager.Reset();
	}

	public static void GiveCoins(int amount)
	{
		SocialGamingManager.Instance.ReportProgress(AchievementIds.COLLECTED_10000_COINS, amount, 10000);
		SocialGamingManager.Instance.ReportProgress(AchievementIds.COLLECTED_100000_COINS, amount, 100000);
		coins += amount;
		Tracking.total_coins_earned(amount);
		Player.Instance.PlayerData.total_coins_collected += amount;
		SocialGamingManager.Instance.ReportScore(LeaderboardIds.TOTAL_COINS, Player.Instance.PlayerData.total_coins_collected);
		Save();
		onBalanceChange();
	}

	public static void GiveDiamonds(int amount)
	{
		Debug.Log(string.Format("=== Give {0} Diamonds ===", amount));
		diamonds += amount;
		Tracking.total_diamonds_earned(amount);
		Save();
		onBalanceChange();
	}

	public static void TakeCoins(int amount)
	{
		coins -= amount;
		Tracking.total_coins_spent(amount);
		Save();
		onBalanceChange();
	}

	public static void TakeDiamonds(int amount)
	{
		Debug.Log(string.Format("=== Taking {0} Diamonds ===", amount));
		diamonds -= amount;
		Tracking.total_diamonds_spent(amount);
		Save();
		onBalanceChange();
	}
}
