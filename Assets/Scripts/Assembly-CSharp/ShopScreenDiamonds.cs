using UnityEngine;

public class ShopScreenDiamonds : ShopScreen
{
	protected override void SetupShop()
	{
		base.SetupShop();
		onDialogOpenSound = Sounds.main_get_diamonds;
		title.Key = "Get Diamonds";
		for (int i = 0; i != shopItems.Count; i++)
		{
			ShopItems.Pack packById = ShopItems.getPackById(ShopItems.diamond_shop_entries[i]);
			if (packById == null)
			{
				packById = ShopItems.getPackById("com.pokokostudio.dinobash.diamond_pack_2_" + (i + 1));
				Debug.LogError("Store Diamond List is faulty - please fix in LP: " + ShopItems.coin_shop_entries[i]);
			}
			SetShopItem(shopItems[i], packById, "com.pokokostudio.dinobash.diamond_pack_2_" + (i + 1));
		}
	}
}
