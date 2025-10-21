using System;
using LeanplumSDK;
using UnityEngine;
using dinobash;

public class BundleOffer : MonoBehaviour
{
	private static Var<string> var_bundle_id;

	private static Var<int> var_bundle_hours;

	private static Var<int> var_blizzard_amount;

	private static Var<int> var_megaball_amount;

	private static Var<float> var_percentOff;

	private DateTime time;

	public static int blizzard_amount
	{
		get
		{
			return var_blizzard_amount.Value;
		}
	}

	public static int megaball_amount
	{
		get
		{
			return var_megaball_amount.Value;
		}
	}

	public static float PercentOff
	{
		get
		{
			return var_percentOff.Value;
		}
	}

	public static bool is_offered
	{
		get
		{
			return Player.Instance.PlayerData.offer_bundle && userUnlockedConsumables;
		}
		set
		{
			Player.Instance.PlayerData.offer_bundle = value;
		}
	}

	private static bool userUnlockedConsumables
	{
		get
		{
			return Player.hasUnlockedConsumable(UnitType.Blizzard) && Player.hasUnlockedConsumable(UnitType.MegaBall);
		}
	}

	public static TimeSpan time_remaining
	{
		get
		{
			if (is_offered)
			{
				TimeSpan result = offer_end - DateTime.UtcNow;
				if (result.Seconds > 0)
				{
					return result;
				}
			}
			return new TimeSpan(0L);
		}
	}

	private static DateTime offer_end
	{
		get
		{
			return Player.Instance.PlayerData.bundleOfferEnd;
		}
		set
		{
			Player.Instance.PlayerData.bundleOfferEnd = value;
		}
	}

	private string bundle_id
	{
		get
		{
			return Player.Instance.PlayerData.bundle_id;
		}
		set
		{
			Player.Instance.PlayerData.bundle_id = value;
		}
	}

	public static event Action offerEnded;

	public static void EndOffer()
	{
		if (is_offered)
		{
			offer_end = DateTime.UtcNow;
			is_offered = false;
			if (BundleOffer.offerEnded != null)
			{
				BundleOffer.offerEnded();
			}
			Debug.Log("bundle_promotion: ended!");
		}
	}

	private void Start()
	{
		App.OnStateChange += HandleOnStateChange;
		var_bundle_id = Var.Define("store.bundle.id", "default");
		var_bundle_hours = Var.Define("store.bundle.duration_in_hours", 0);
		var_blizzard_amount = Var.Define("store.bundle.reward_blizzard_amount", 5);
		var_megaball_amount = Var.Define("store.bundle.reward_megaball_amount", 3);
		var_percentOff = Var.Define("store.bundle.label_percent_off", 0.8f);
		time = DateTime.UtcNow;
	}

	private void HandleOnStateChange(App.States state)
	{
		if (state != App.States.Map)
		{
			return;
		}
		try
		{
			if (bundle_id != var_bundle_id.Value)
			{
				bundle_id = var_bundle_id.Value;
				if (bundle_id != "default")
				{
					is_offered = true;
					offer_end = DateTime.UtcNow.AddHours(var_bundle_hours.Value);
				}
				else
				{
					is_offered = false;
					offer_end = DateTime.UtcNow;
				}
				Debug.Log(string.Format("bundle_promotion: id:{3} offered:{0} ends:{1} item:{2}", is_offered, offer_end, ShopItems.bundle_entry.Value, bundle_id));
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("bundle_promotion: had an error - probably due to faulty input: " + ex.Message + ex.StackTrace);
			is_offered = false;
			offer_end = DateTime.UtcNow;
		}
	}

	private void Update()
	{
		DateTime utcNow = DateTime.UtcNow;
		if (!(time > utcNow))
		{
			time = utcNow.AddSeconds(1.0);
			if (time_remaining.Seconds < 0 && is_offered)
			{
				EndOffer();
			}
		}
	}
}
