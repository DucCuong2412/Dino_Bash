using ChartboostSDK;
using UnityEngine;

public static class ChartboostWrapper
{
	public static bool isShowingInterstital { get; private set; }

	public static void Init()
	{
		App.Instance.gameObject.AddComponent<Chartboost>();
		Chartboost.setShouldRequestInterstitialsInFirstSession(false);
		isShowingInterstital = false;
	}

	public static void ShowOnStartInterstital()
	{
		if (Chartboost.hasInterstitial(CBLocation.Startup))
		{
			ShowInterstitial(CBLocation.Startup);
		}
	}

	public static void ShowLevelCompleteInterstital()
	{
		if (Chartboost.hasInterstitial(CBLocation.LevelComplete))
		{
			ShowInterstitial(CBLocation.LevelComplete);
		}
	}

	private static void ShowInterstitial(CBLocation location)
	{
		isShowingInterstital = true;
		Chartboost.didDismissInterstitial += HandledidDismissInterstitial;
		Chartboost.didFailToLoadInterstitial += HandledidFailToLoadInterstitial;
		Chartboost.showInterstitial(location);
	}

	private static void HandledidDismissInterstitial(CBLocation location)
	{
		Chartboost.didDismissInterstitial -= HandledidDismissInterstitial;
		isShowingInterstital = false;
	}

	private static void HandledidFailToLoadInterstitial(CBLocation location, CBImpressionError error)
	{
		Debug.Log("Chartboost Error: " + error);
		Chartboost.didFailToLoadInterstitial -= HandledidFailToLoadInterstitial;
		isShowingInterstital = false;
	}
}
