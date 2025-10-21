using UnityEngine;

public class DiamondWidget : MonoBehaviour
{
	private tk2dTextMesh diamondsLabel;

	public int tweenDiamonds { get; set; }

	private void Start()
	{
		diamondsLabel = base.transform.Search("diamondsLabel").GetComponent<tk2dTextMesh>();
		tweenDiamonds = Wallet.Diamonds;
		diamondsLabel.text = Wallet.Diamonds.ToString();
		StandardButton component = base.transform.Search("btn_BuyDiamonds").GetComponent<StandardButton>();
		component.uiItem.OnClick += delegate
		{
			ShopScreenDiamonds screen = ScreenManager.GetScreen<ShopScreenDiamonds>();
			if (!screen.isVisible)
			{
				track_shop_calls();
				screen.Show();
			}
		};
	}

	private void OnEnable()
	{
		Wallet.OnBalanceChanged += HandleOnBalanceChanged;
		if (diamondsLabel != null)
		{
			tweenDiamonds = Wallet.Diamonds;
			diamondsLabel.text = Wallet.Diamonds.ToString();
		}
	}

	private void track_shop_calls()
	{
		RefillConsumablesScreen screen = ScreenManager.GetScreen<RefillConsumablesScreen>();
		if (screen != null && screen.isVisible)
		{
			Tracking.store_open(Wallet.Currency.Diamonds, "refill_consumables_screen", "refill_consumables_screen");
		}
		DinoRageScreen screen2 = ScreenManager.GetScreen<DinoRageScreen>();
		if (screen2 != null && screen2.isVisible)
		{
			Tracking.store_open(Wallet.Currency.Diamonds, "dinorage_screen", "dinorage_screen");
		}
		UpgradeInfoDinoScreen screen3 = ScreenManager.GetScreen<UpgradeInfoDinoScreen>();
		if (screen3 != null && screen3.isVisible)
		{
			Tracking.store_open(Wallet.Currency.Diamonds, "specialoffer_screen", screen3.item.ToString());
		}
		UpgradeInfoShotScreen screen4 = ScreenManager.GetScreen<UpgradeInfoShotScreen>();
		if (screen4 != null && screen4.isVisible)
		{
			Tracking.store_open(Wallet.Currency.Diamonds, "specialoffer_screen", screen4.item.ToString());
		}
		UpgradeInfoUpgradesScreen screen5 = ScreenManager.GetScreen<UpgradeInfoUpgradesScreen>();
		if (screen5 != null && screen5.isVisible)
		{
			Tracking.store_open(Wallet.Currency.Diamonds, "specialoffer_screen", screen5.item.ToString());
		}
	}

	private void OnDisable()
	{
		Wallet.OnBalanceChanged -= HandleOnBalanceChanged;
	}

	private void HandleOnBalanceChanged()
	{
		Go.to(this, 1f, new GoTweenConfig().intProp("tweenDiamonds", Wallet.Diamonds).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate).onUpdate(delegate
		{
			diamondsLabel.text = tweenDiamonds.ToString();
		}));
	}
}
