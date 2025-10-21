using System;
//using LeanplumSDK;
using UnityEngine;
using dinobash;

public class ShopPromotions : MonoBehaviour
{
	private bool var_use_quantity_promotion;

	private bool var_use_price_promotion;

	private string var_promotion_id;

	private int var_promotion_duration;

	public static bool is_price_promotion
	{
		get
		{
			return Player.Instance.PlayerData.price_promo && DateTime.UtcNow < promotion_end_date;
		}
	}

	public static bool is_quantity_promotion
	{
		get
		{
			return Player.Instance.PlayerData.quantity_promo && DateTime.UtcNow < promotion_end_date;
		}
	}

	public static bool is_sale
	{
		get
		{
			return is_price_promotion || is_quantity_promotion;
		}
	}

	public static TimeSpan remainingTime
	{
		get
		{
			if (is_sale)
			{
				TimeSpan result = promotion_end_date - DateTime.UtcNow;
				if (result.Seconds > 0)
				{
					return result;
				}
			}
			return new TimeSpan(0L);
		}
	}

	private static DateTime promotion_end_date
	{
		get
		{
			return Player.Instance.PlayerData.promotionEnd;
		}
		set
		{
			Player.Instance.PlayerData.promotionEnd = value;
		}
	}

	public static string promotion_id
	{
		get
		{
			return Player.Instance.PlayerData.promotion_id;
		}
		private set
		{
			Player.Instance.PlayerData.promotion_id = value;
		}
	}

	private DateTime get_end_date()
	{
		//Discarded unreachable code: IL_001f, IL_0045
		try
		{
			return DateTime.UtcNow.AddHours(var_promotion_duration);
		}
		catch (Exception ex)
		{
			Debug.LogError("Something went wrong with Promotion End date!\n" + ex.Message);
			return DateTime.UtcNow;
		}
	}

	private void Start()
	{
		App.OnStateChange += HandleOnStateChange;
		var_promotion_id = "default";

        var_use_quantity_promotion = false;
		var_use_price_promotion = false;
		var_promotion_duration = 0;
	}

	private void HandleOnStateChange(App.States state)
	{
		if (state != App.States.Map && state != App.States.Game)
		{
			return;
		}
		try
		{
			if (promotion_id != var_promotion_id)
			{
				promotion_id = var_promotion_id;
				if (promotion_id != "default")
				{
					promotion_end_date = get_end_date();
					Player.Instance.PlayerData.price_promo = var_use_price_promotion;
					Player.Instance.PlayerData.quantity_promo = var_use_quantity_promotion;
				}
				else
				{
					Debug.Log("SHOP PROMOTION: setting back to default - disabling all sales");
					TurnOffPromotions();
				}
				Debug.Log(string.Format("SHOP PROMOTION: id:{3} price:{0} quantity:{1} Ends:{2}", Player.Instance.PlayerData.price_promo, Player.Instance.PlayerData.quantity_promo, promotion_end_date, promotion_id));
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Shop promotion had an error - probably due to faulty input: " + ex.Message + ex.StackTrace);
			TurnOffPromotions();
		}
	}

	private void TurnOffPromotions()
	{
		promotion_end_date = DateTime.UtcNow;
		Player.Instance.PlayerData.price_promo = false;
		Player.Instance.PlayerData.quantity_promo = false;
	}
}
