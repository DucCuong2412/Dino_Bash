using System;
using UnityEngine;
using dinobash;

public class RefillConsumablesScreen : BaseScreen
{
	private int refill_cost;

	private tk2dCameraAnchor upper_center;

	private tk2dCameraAnchor middle_center;

	private float screen_width;

	private UnitType consumable_item;

	private float time_scale = 1f;

	private void Start()
	{
		upper_center = FindChildComponent<tk2dCameraAnchor>("UpperCenter");
		middle_center = FindChildComponent<tk2dCameraAnchor>("MiddleCenter");
		screen_width = ScreenManager.Camera.GetComponent<tk2dCamera>().ScreenExtents.width;
		StandardButton standardButton = FindChildComponent<StandardButton>("MiddleCenter/btn_close");
		standardButton.clickSound = Sounds.main_close_popup;
		standardButton.uiItem.OnClick += Hide;
		StandardButton standardButton2 = FindChildComponent<StandardButton>("MiddleCenter/btn_buy");
		standardButton2.uiItem.OnClick += delegate
		{
			if (Wallet.Diamonds >= refill_cost)
			{
				Wallet.TakeDiamonds(refill_cost);
				Player.changeConsumableCount(consumable_item, Player.getConsumableRefillAmount(consumable_item));
				int spent = refill_cost;
				Tracking.buy_item(consumable_item, ShotType.None, string.Empty, spent, "refill_consumables");
				Hide();
			}
			else
			{
				Tracking.store_open(Wallet.Currency.Diamonds, "refill_consumables_screen", consumable_item.ToString());
				ShopScreenDiamonds screen = ScreenManager.GetScreen<ShopScreenDiamonds>();
				screen.Show();
			}
		};
		middle_center.AnchorOffsetPixels = new Vector2(screen_width, 0f);
		upper_center.AnchorOffsetPixels = new Vector2(0f, 768f);
		base.gameObject.SetActive(false);
	}

	private void ShowCenterDialog()
	{
		middle_center.AnchorOffsetPixels = new Vector2(screen_width, 0f);
		Go.to(middle_center, 0.3f, new GoTweenConfig().vector2Prop("AnchorOffsetPixels", new Vector2(0f, 0f)).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
	}

	private void HideCenterDialog(Action callback)
	{
		Go.to(middle_center, 0.3f, new GoTweenConfig().vector2Prop("AnchorOffsetPixels", new Vector2(0f - screen_width, 0f)).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate).onComplete(delegate
		{
			callback();
		}));
	}

	public void Show(UnitType consumable)
	{
		if (Konfiguration.isConsumable(consumable))
		{
			consumable_item = consumable;
			refill_cost = Konfiguration.UnitData[consumable_item].premiumCost;
			base.transform.Search("label_count").GetComponent<tk2dTextMesh>().text = Player.getConsumableRefillAmount(consumable_item).ToString();
			base.transform.Search("label_price").GetComponent<tk2dTextMesh>().text = refill_cost.ToString();
			base.transform.Search("label_teaser").GetComponent<tk2dTextMesh>().text = (consumable_item.ToString() + ".refill").Localize();
			SpriteRenderer component = base.transform.Search("item_icon").GetComponent<SpriteRenderer>();
			component.sprite = SpriteRessources.getSpiteForUnitType(consumable_item);
			base.gameObject.SetActive(true);
			base.Show();
			time_scale = Time.timeScale;
			Time.timeScale = 0f;
			OnEscapeUp = Hide;
			ScreenManager.GetScreen<CoverScreen>(this).Show();
			ShowCenterDialog();
			upper_center.AnchorOffsetPixels = new Vector2(0f, 768f);
			Go.to(upper_center, 0.3f, new GoTweenConfig().vector2Prop("AnchorOffsetPixels", Vector2.zero).setEaseType(GoEaseType.CircOut).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
		}
	}

	public override void Hide()
	{
		base.Hide();
		Time.timeScale = time_scale;
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		ScreenManager.GetScreen<HudScreen>().Show();
		HideCenterDialog(delegate
		{
			base.gameObject.SetActive(false);
		});
		Go.to(upper_center, 0.3f, new GoTweenConfig().vector2Prop("AnchorOffsetPixels", new Vector2(0f, 768f)).setEaseType(GoEaseType.CircIn).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
	}
}
