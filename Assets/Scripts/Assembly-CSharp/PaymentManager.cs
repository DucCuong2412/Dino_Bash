using System;
using Unibill;
using UnityEngine;
using dinobash;

public class PaymentManager : ScriptableObject
{
	public bool PaymentAvailable { get; private set; }

	public event Action onPurchaseAborted;

	public event Action onPurchaseFailed;

	public event Action<ShopItems.Pack> onPackPurchased;

	public void Init()
	{
		Unibiller.onBillerReady += onInitialised;
		Unibiller.onPurchaseCompleteEvent += onPurchased;
		Unibiller.onPurchaseFailed += onFailed;
		Unibiller.Initialise();
	}

	private void onInitialised(UnibillState result)
	{
		PaymentAvailable = result == UnibillState.SUCCESS || result == UnibillState.SUCCESS_WITH_ERRORS;
		switch (result)
		{
		}
	}

	private void onFailed(PurchaseFailedEvent obj)
	{
		Debug.Log(string.Concat("############### Payment: onFailed, Reason=", obj.Reason, ", message=", obj.Message));
		if (obj.Reason == PurchaseFailureReason.USER_CANCELLED)
		{
			if (this.onPurchaseAborted != null)
			{
				this.onPurchaseAborted();
			}
			else if (this.onPurchaseFailed != null)
			{
				this.onPurchaseFailed();
			}
		}
	}

	private void onPurchased(PurchaseEvent evt)
	{
		PurchasableItem purchasedItem = evt.PurchasedItem;
		Debug.Log("Purchase OK: " + purchasedItem.Id);
		Debug.Log(string.Format("{0} has now been purchased {1} times.", purchasedItem.name, Unibiller.GetPurchaseCount(purchasedItem)));
		ShopItems.Pack packById = ShopItems.getPackById(purchasedItem.Id);
		if (packById != null)
		{
			Wallet.Total_spent += packById.priceInLocalCurrency;
			switch (packById.currency)
			{
			case Wallet.Currency.Coins:
				Wallet.GiveCoins(packById.amount);
				break;
			case Wallet.Currency.Diamonds:
				Wallet.GiveDiamonds(packById.amount);
				break;
			case Wallet.Currency.StarterPack:
			case Wallet.Currency.RegularPack:
				Wallet.GiveDiamonds(packById.amount);
				Player.changeConsumableCount(UnitType.MegaBall, BundleOffer.megaball_amount);
				Player.changeConsumableCount(UnitType.Blizzard, BundleOffer.blizzard_amount);
				break;
			}
		}
		else
		{
			Debug.LogError("Unable to lookup ShopItem with id '" + purchasedItem.Id + "'!");
		}
		if (this.onPackPurchased != null)
		{
			this.onPackPurchased(packById);
		}
		Tracking.OnBuyPack(packById, purchasedItem, evt.Receipt);
	}

	public void PurchasePack(ShopItems.Pack pack)
	{
		Unibiller.initiatePurchase(Unibiller.GetPurchasableItemById(pack.id), string.Empty);
	}

	public void Reset()
	{
		Unibiller.clearTransactions();
	}
}
