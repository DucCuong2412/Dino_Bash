using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
//using LeanplumSDK;
using UnityEngine;
using dinobash;
using mixpanel;

public static class ShopItems
{
	public class Pack
	{
		[XmlAttribute("id")]
		public readonly string id = "id";

		[XmlAttribute("preview_price")]
		public readonly string preview_price = "1.99";

		[XmlAttribute("currency")]
		public readonly Wallet.Currency currency;

		[XmlAttribute("base_amount")]
		public readonly int base_amount = 1;


		public int var_amount;

		public string var_id;

		public int amount
		{
			get
			{
				int value = var_amount;
				return (currency != 0) ? value : ((int)((float)value * Konfiguration.GetChapterData(Player.MaxLevelID).shop_amount_multiplier));
			}
		}

		public int baseAmountWithMultiplier
		{
			get
			{
				int num = base_amount;
				return (currency != 0) ? num : ((int)((float)num * Konfiguration.GetChapterData(Player.MaxLevelID).shop_amount_multiplier));
			}
		}

		public string label
		{
			get
			{
				Pack pack = ((currency != 0) ? getPackById(diamond_shop_entries[0]) : getPackById(coin_shop_entries[0]));
				if (this == pack || pack == null)
				{
					return string.Empty;
				}
				float num = pack.priceInLocalCurrency / (float)pack.amount;
				float num2 = num * (float)amount;
				float num3 = (num2 - priceInLocalCurrency) / num2;
				string value = string.Format("Save {0:P0}".Localize(), num3);
				return fixSpaceBeforePercentage(value);
			}
		}

		public string localizedPriceString
		{
			get
			{
				if (item.localizedPriceString != null)
				{
					return item.localizedPriceString;
				}
				return preview_price;
			}
		}

		public float priceInLocalCurrency
		{
			get
			{
				return (float)item.priceInLocalCurrency;
			}
		}

		public string isoCurrencyCode
		{
			get
			{
				return item.isoCurrencySymbol;
			}
		}

		public double priceInUSD
		{
			get
			{
				return Mixpanel.GetExchangeRate(isoCurrencyCode) * (double)priceInLocalCurrency;
			}
		}

		private PurchasableItem item
		{
			get
			{
				return null;// Unibiller.GetPurchasableItemById(id);
			}
		}

		private Pack()
		{
		}

		public void init(string prefix)
		{
			string[] array = id.Split('.');
			string name = "store." + prefix + "." + array[array.Length - 1];
			//var_amount = Var.Define(name, base_amount);
			var_amount = base_amount;

		}


		public float getQuantitySavingsAsFloat()
		{
			float num = amount - baseAmountWithMultiplier;
			return num / (float)baseAmountWithMultiplier;
		}

		public bool hasQuantitySavings()
		{
			return getQuantitySavingsAsFloat() != 0f;
		}

		public string getQuantitySavings()
		{
			float quantitySavingsAsFloat = getQuantitySavingsAsFloat();
			string value = string.Format("{0:P0} MORE".Localize(), quantitySavingsAsFloat);
			return fixSpaceBeforePercentage(value);
		}

		public float getPriceSavingsAsFloat(Pack reference_pack)
		{
			if (reference_pack == null)
			{
				return 0f;
			}
			float num = reference_pack.priceInLocalCurrency - priceInLocalCurrency;
			return num / reference_pack.priceInLocalCurrency;
		}

		public bool hasPriceSavings(Pack reference_pack)
		{
			return getPriceSavingsAsFloat(reference_pack) != 0f;
		}

		public string getPriceSavings(Pack reference_pack)
		{
			float priceSavingsAsFloat = getPriceSavingsAsFloat(reference_pack);
			string value = string.Format("{0:P0} OFF".Localize(), priceSavingsAsFloat);
			return fixSpaceBeforePercentage(value);
		}

		public string getPriceAndQuantitySavings(Pack reference_pack)
		{
			if (reference_pack == null)
			{
				return string.Empty;
			}
			float num = reference_pack.priceInLocalCurrency / (float)reference_pack.baseAmountWithMultiplier;
			float num2 = priceInLocalCurrency / (float)amount;
			float num3 = num - num2;
			float num4 = num3 / num;
			string value = string.Format("{0:P0} OFF".Localize(), num4);
			return fixSpaceBeforePercentage(value);
		}

		private string fixSpaceBeforePercentage(string value)
		{
			StringBuilder stringBuilder = new StringBuilder(value);
			int num = value.IndexOf(' ');
			if (num > 0)
			{
				stringBuilder[num] = Convert.ToChar(160);
			}
			return stringBuilder.ToString();
		}
	}

	public class Packs
	{
		public SerializableList<Pack> CoinPacks = new SerializableList<Pack>();

		public SerializableList<Pack> DiamondPacks = new SerializableList<Pack>();

		public SerializableList<Pack> BundlePacks = new SerializableList<Pack>();
	}

	public const string bundle_entry_default = "com.pokokostudio.dinobash.starter_pack";

	public static string  bundle_entry;

	public static List<string> coin_shop_entries = new List<string>();

	private static List<string> coin_shop_defaults = new List<string>(new string[5] { "com.pokokostudio.dinobash.coin_pack_1", "com.pokokostudio.dinobash.coin_pack_2", "com.pokokostudio.dinobash.coin_pack_3", "com.pokokostudio.dinobash.coin_pack_4", "com.pokokostudio.dinobash.coin_pack_5" });

	public static List<string> diamond_shop_entries = new List<string>();

	private static List<string> diamond_shop_defaults = new List<string>(new string[5] { "com.pokokostudio.dinobash.diamond_pack_2_1", "com.pokokostudio.dinobash.diamond_pack_2_2", "com.pokokostudio.dinobash.diamond_pack_2_3", "com.pokokostudio.dinobash.diamond_pack_2_4", "com.pokokostudio.dinobash.diamond_pack_2_5" });

	public static Packs packs { get; private set; }

    //private static void RegisterShopLists(string lp_group_name, List<string> ids, List<string> list)
    //{
    //	for (int i = 0; i != ids.Count; i++)
    //	{
    //		list.Add(Var.Define(lp_group_name + ".slot_" + (i + 1), ids[i]));
    //	}
    //}
    private static void RegisterShopLists(string lp_group_name, List<string> ids, List<string> list)
    {
        for (int i = 0; i < ids.Count; i++)
        {
            list.Add(ids[i]); // Copy trực tiếp giá trị từ ids
        }
    }
    public static void Init()
	{
		packs = Serializer.DeserializeFileOrTextAsset<Packs>("XML/shop_items");
		packs.CoinPacks.ForEach(delegate(Pack p)
		{
			p.init("IAP_CoinProducts");
		});
		packs.DiamondPacks.ForEach(delegate(Pack p)
		{
			p.init("IAP_DiamondProducts");
		});
		packs.BundlePacks.ForEach(delegate(Pack p)
		{
			p.init("IAP_BundleProducts");
		});
		RegisterShopLists("store.Coin_shoplist", coin_shop_defaults, coin_shop_entries);
		RegisterShopLists("store.Diamond_shoplist", diamond_shop_defaults, diamond_shop_entries);
        bundle_entry = bundle_entry_default;
    }

	public static Pack getPackById(string id)
	{
		if (id == null)
		{
			Debug.LogError("pack id is null! fix in lp?");
			return null;
		}
		foreach (Pack coinPack in packs.CoinPacks)
		{
			if (coinPack.id == id)
			{
				return coinPack;
			}
		}
		foreach (Pack diamondPack in packs.DiamondPacks)
		{
			if (diamondPack.id == id)
			{
				return diamondPack;
			}
		}
		foreach (Pack bundlePack in packs.BundlePacks)
		{
			if (bundlePack.id == id)
			{
				return bundlePack;
			}
		}
		return null;
	}
}
