using UnityEngine;

public class ShopScreenCoins : ShopScreen
{
	protected override void SetupShop()
	{
		base.SetupShop();
		onDialogOpenSound = Sounds.main_get_coins;
		title.Key = "Get Coins";
		for (int i = 0; i != shopItems.Count; i++)
		{
			ShopItems.Pack packById = ShopItems.getPackById(ShopItems.coin_shop_entries[i]);
			if (packById == null)
			{
				packById = ShopItems.getPackById("com.pokokostudio.dinobash.coin_pack_" + (i + 1));
				Debug.LogError("Store Coin List is faulty - please fix in LP: " + ShopItems.coin_shop_entries[i]);
			}
			SetShopItem(shopItems[i], packById, "com.pokokostudio.dinobash.coin_pack_" + (i + 1));
		}
	}
}
