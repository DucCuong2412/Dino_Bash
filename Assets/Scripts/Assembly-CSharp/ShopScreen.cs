using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShopScreen : BaseScreen
{
	private bool is_buying;

	protected List<Transform> shopItems = new List<Transform>();

	protected LocalizedText title;

	protected Sounds onDialogOpenSound;

	private List<ShopItems.Pack> packs = new List<ShopItems.Pack>();

	private Transform loader;

	private tk2dTextMesh promotion_timer;

	private List<GoTween> loader_tweens = new List<GoTween>();

	private int label_chars_space = 5;

	public Action OnCloseClick { get; set; }

	public bool HideCoverScreen { get; set; }

	protected void Buy(ShopItems.Pack pack)
	{
		App.Instance.paymentManager.onPackPurchased += handleOnPurchase;
		App.Instance.paymentManager.onPurchaseAborted += handlePurchaseAborted;
		App.Instance.paymentManager.onPurchaseFailed += handleonPurchaseFailed;
		is_buying = true;
		//App.Instance.paymentManager.PurchasePack(pack);
	}

	private void handleonPurchaseFailed()
	{
		Debug.Log("shopscreen: handleonPurchaseFailed");
		ErrorMessageScreen.ShowError(ErrorMessages.IAP_PURCHASE_FAILED);
		OnEscapeUp = null;
		ErrorMessageScreen screen = ScreenManager.GetScreen<ErrorMessageScreen>();
		screen.OnScreenHide += EnableBackButton;
		Wallet.Currency currency = ((!(this is ShopScreenCoins)) ? Wallet.Currency.Diamonds : Wallet.Currency.Coins);
		Tracking.store_purchase_failed(currency);
		end_IAP_Process();
	}

	private void handlePurchaseAborted()
	{
		Debug.Log("shopscreen: handlePurchaseAborted");
		Wallet.Currency currency = ((!(this is ShopScreenCoins)) ? Wallet.Currency.Diamonds : Wallet.Currency.Coins);
		Tracking.store_purchase_failed(currency);
		end_IAP_Process();
	}

	private void handleOnPurchase(ShopItems.Pack pack)
	{
		Sounds sound = Sounds.main_get_coins;
		switch (pack.currency)
		{
		case Wallet.Currency.Coins:
			sound = Sounds.main_buy_coins;
			break;
		case Wallet.Currency.Diamonds:
			sound = Sounds.main_buy_diamods;
			break;
		}
		AudioPlayer.PlayGuiSFX(sound, 0f);
		App.Instance.cloudSaveGameManager.checkAndPrompt(false);
		end_IAP_Process();
	}

	private void end_IAP_Process()
	{
		if (!is_buying)
		{
			return;
		}
		is_buying = false;
		App.Instance.paymentManager.onPackPurchased -= handleOnPurchase;
		App.Instance.paymentManager.onPurchaseAborted -= end_IAP_Process;
		App.Instance.paymentManager.onPurchaseFailed -= handleonPurchaseFailed;
		shopItems.ForEach(delegate(Transform item)
		{
			if (item != null)
			{
				item.GetComponentInChildren<StandardButton>().uiItem.enabled = true;
			}
		});
		ForceHideLoader();
	}

	public void OnAppEnter()
	{
		Debug.Log(string.Format("process_buying:{0} gameObject.activeSelf:{1} isVisible:{2} Interactive:{3}", is_buying, base.gameObject.activeSelf, base.isVisible, base.Interactive));
		if (is_buying && base.isVisible && base.Interactive)
		{
			end_IAP_Process();
			Debug.Log("shopscreen: end_iap_process");
		}
	}

	private void ForceHideLoader()
	{
		if (!(loader == null))
		{
			loader_tweens.ForEach(delegate(GoTween tween)
			{
				tween.rewind();
			});
			loader.gameObject.SetActive(false);
		}
	}

	private void Start()
	{
		shopItems.Add(base.transform.Find("MiddleCenter/pack_0"));
		shopItems.Add(base.transform.Find("MiddleCenter/pack_1"));
		shopItems.Add(base.transform.Find("MiddleCenter/pack_2"));
		shopItems.Add(base.transform.Find("MiddleCenter/pack_3"));
		shopItems.Add(base.transform.Find("MiddleCenter/pack_4"));
		loader = base.transform.Find("MiddleCenter/loader");
		SetLoaderTween();
		loader.gameObject.SetActive(false);
		foreach (Transform shopItem in shopItems)
		{
			tk2dUIItem componentInChildren = shopItem.GetComponentInChildren<tk2dUIItem>();
			componentInChildren.OnClickUIItem += OnBuyClicked;
		}
		title = FindChildComponent<LocalizedText>("MiddleCenter/title/title_label");
		StandardButton standardButton = FindChildComponent<StandardButton>("MiddleCenter/btn_close");
		standardButton.uiItem.OnClick += OnClose;
		standardButton.clickSound = Sounds.main_close_popup;
		base.transform.localPosition += base.left;
		base.gameObject.SetActive(false);
	}

	protected virtual void SetupShop()
	{
		promotion_timer = base.transform.Find("MiddleCenter/offer_end/offer_time_label").GetComponent<tk2dTextMesh>();
		promotion_timer.transform.parent.gameObject.SetActive(ShopPromotions.is_sale);
		Transform transform = base.transform.Find("MiddleCenter/special_offer");
		transform.gameObject.SetActive(!ShopPromotions.is_sale);
		Transform transform2 = base.transform.Find("MiddleCenter/best_price");
		transform2.gameObject.SetActive(!ShopPromotions.is_sale);
	}

	private void OnClose()
	{
		if (OnCloseClick != null)
		{
			OnCloseClick();
		}
		OnCloseClick = null;
		if (App.State == App.States.Game && ScreenManager.GetScreen<ResultScreen>().show_GetLivesScreen)
		{
			ScreenManager.GetScreen<GetLivesScreen>().Show_Delayed();
		}
		Hide();
	}

	public override void Show()
	{
		if (!App.Instance.InternetConnectivity)
		{
			ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
			return;
		}
		base.gameObject.SetActive(true);
		base.Show();
		HideCoverScreen = true;
		OnEscapeUp = OnClose;
		is_buying = false;
		ForceHideLoader();
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		ResourceBarScreen resource_bar = ScreenManager.GetScreen<ResourceBarScreen>();
		resource_bar.switch_in_progress = true;
		SetupShop();
		if (ShopPromotions.is_sale)
		{
			StartCoroutine(WatchPromotionTime());
		}
		shopItems.ForEach(delegate(Transform item)
		{
			item.GetComponentInChildren<StandardButton>().Enabled = true;
		});
		AudioPlayer.PlayGuiSFX(onDialogOpenSound, 0f);
		StopAllCoroutines();
		ShowFrom(base.right, delegate
		{
			resource_bar.switch_in_progress = false;
			WobbleEntries();
		}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
	}

	private IEnumerator WatchPromotionTime()
	{
		bool running = true;
		while (running)
		{
			TimeSpan remaining_time = ShopPromotions.remainingTime;
			running = remaining_time.Seconds > 0;
			promotion_timer.text = "Ending in: ".Localize() + " " + remaining_time.Humanize();
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		SetupShop();
	}

	public override void Hide()
	{
		App.Instance.paymentManager.onPackPurchased -= handleOnPurchase;
		base.Hide();
		base.isVisible = true;
		if (HideCoverScreen)
		{
			ScreenManager.GetScreen<CoverScreen>(this).Hide();
		}
		ResourceBarScreen resource_bar = ScreenManager.GetScreen<ResourceBarScreen>();
		resource_bar.switch_in_progress = true;
		OnCloseClick = null;
		MapScreen screen = ScreenManager.GetScreen<MapScreen>();
		if (screen != null)
		{
			screen.UpdateGiftCoinsButtonState();
		}
		HideTo(base.left, delegate
		{
			base.isVisible = false;
			base.gameObject.SetActive(false);
			resource_bar.switch_in_progress = false;
		}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
	}

	private void WobbleEntries()
	{
		GoTweenConfig goTweenConfig = new GoTweenConfig();
		goTweenConfig.scale(Vector3.one * 1.05f);
		goTweenConfig.loopType = GoLoopType.PingPong;
		goTweenConfig.iterations = 2;
		goTweenConfig.setUpdateType(GoUpdateType.TimeScaleIndependentUpdate);
		float num = 0f;
		foreach (Transform shopItem in shopItems)
		{
			goTweenConfig.delay = num;
			Go.to(shopItem, 0.2f, goTweenConfig);
			num += 0.1f;
		}
	}

	private void OnBuyClicked(tk2dUIItem button)
	{
		if (App.Instance.InternetConnectivity)
		{
			if (App.Instance.paymentManager.PaymentAvailable)
			{
				loader.gameObject.SetActive(true);
				loader_tweens.ForEach(delegate(GoTween t)
				{
					t.restart();
				});
				shopItems.ForEach(delegate(Transform item)
				{
					item.GetComponentInChildren<StandardButton>().uiItem.enabled = false;
				});
				int index = shopItems.FindIndex((Transform x) => x == button.transform.parent);
				Debug.Log("bought: " + packs[index].id);
				WaitThenRealtime(0.3f, delegate
				{
					Buy(packs[index]);
				});
			}
			else
			{
				ErrorMessageScreen.ShowError(ErrorMessages.IAP_NOT_AVAILABLE);
				OnEscapeUp = null;
				ErrorMessageScreen screen = ScreenManager.GetScreen<ErrorMessageScreen>();
				screen.OnScreenHide += EnableBackButton;
			}
		}
		else
		{
			ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
			OnEscapeUp = null;
			ErrorMessageScreen screen2 = ScreenManager.GetScreen<ErrorMessageScreen>();
			screen2.OnScreenHide += EnableBackButton;
		}
	}

	private void EnableBackButton()
	{
		ErrorMessageScreen screen = ScreenManager.GetScreen<ErrorMessageScreen>();
		screen.OnScreenHide -= EnableBackButton;
		OnEscapeUp = Hide;
	}

	private void SetLoaderTween()
	{
		GoTweenConfig goTweenConfig = new GoTweenConfig();
		goTweenConfig.colorProp("color", Colors.Invisible);
		goTweenConfig.setUpdateType(GoUpdateType.TimeScaleIndependentUpdate);
		loader_tweens.Add(Go.from(loader.GetComponentInChildren<SpriteRenderer>(), 0.3f, goTweenConfig));
		loader_tweens.Add(Go.from(loader.GetComponentInChildren<tk2dTextMesh>(), 0.3f, goTweenConfig));
		loader_tweens.Add(Go.from(loader.GetComponentInChildren<tk2dBaseSprite>(), 0.3f, goTweenConfig));
		loader_tweens.ForEach(delegate(GoTween t)
		{
			t.pause();
			t.autoRemoveOnComplete = false;
		});
	}

	public static int getPriceFontSize(int text_length, int max_char_count)
	{
		return Mathf.CeilToInt(60f * (float)max_char_count / (float)text_length);
	}

	protected void SetShopItem(Transform parent, ShopItems.Pack pack, string reference_pack_id)
	{
		ShopItems.Pack packById = ShopItems.getPackById(reference_pack_id);
		if (packById == null)
		{
			Debug.LogError("Reference-Pack is null: " + reference_pack_id + " this needs to be fixed in LP ASAP!");
		}
		tk2dTextMesh component = parent.Find("inlay/label_count").GetComponent<tk2dTextMesh>();
		bool flag = ShopPromotions.is_quantity_promotion && pack.hasQuantitySavings();
		bool flag2 = ShopPromotions.is_price_promotion && pack.hasPriceSavings(packById);
		bool flag3 = flag || flag2;
		if (flag && packById != null)
		{
			component.text = packById.baseAmountWithMultiplier.ToString().GetGroupedNumberString();
		}
		else
		{
			component.text = pack.amount.ToString().GetGroupedNumberString();
		}
		SpriteRenderer component2 = parent.Find("inlay/OffStroke").GetComponent<SpriteRenderer>();
		component2.enabled = flag;
		TextMesh component3 = parent.Find("label_old_price").GetComponent<TextMesh>();
		component3.gameObject.SetActive(flag2);
		parent.Find("offer_amount").gameObject.SetActive(flag);
		tk2dTextMesh component4 = parent.Find("special_offer/special_offer_label").GetComponent<tk2dTextMesh>();
		component4.transform.parent.gameObject.SetActive(flag3);
		TextMesh component5 = parent.Find("btn_buy/label_price").GetComponent<TextMesh>();
		component5.text = pack.localizedPriceString;
		if (component5.text.Length > label_chars_space)
		{
			component5.fontSize = getPriceFontSize(component5.text.Length, label_chars_space);
		}
		if (flag3)
		{
			parent.Find("sale").gameObject.SetActive(false);
		}
		else
		{
			tk2dTextMesh component6 = parent.Find("sale/label_saverate").GetComponent<tk2dTextMesh>();
			component6.text = pack.label;
			parent.Find("sale").gameObject.SetActive(true);
		}
		if (flag && flag2)
		{
			tk2dTextMesh component7 = parent.Find("offer_amount/label_count").GetComponent<tk2dTextMesh>();
			component7.text = pack.amount.ToString().GetGroupedNumberString();
			component7.ForceBuild();
			component3.text = packById.localizedPriceString;
			component4.text = pack.getPriceAndQuantitySavings(packById);
		}
		else if (flag2)
		{
			component3.text = packById.localizedPriceString;
			component4.text = pack.getPriceSavings(packById);
			parent.Find("inlay").LocalPosY(-315f);
		}
		else if (flag)
		{
			tk2dTextMesh component8 = parent.Find("offer_amount/label_count").GetComponent<tk2dTextMesh>();
			component8.text = pack.amount.ToString().GetGroupedNumberString();
			component8.ForceBuild();
			component4.text = pack.getQuantitySavings();
		}
		switch (pack.currency)
		{
		case Wallet.Currency.Coins:
			parent.Find("inlay/diamonds_currency").GetComponent<SpriteRenderer>().enabled = false;
			parent.Find("inlay/diamonds_icon").GetComponent<SpriteRenderer>().enabled = false;
			parent.Find("offer_amount/diamonds_currency").GetComponent<SpriteRenderer>().enabled = false;
			break;
		case Wallet.Currency.Diamonds:
			parent.Find("inlay/coins_currency").GetComponent<SpriteRenderer>().enabled = false;
			parent.Find("inlay/coins_icon").GetComponent<SpriteRenderer>().enabled = false;
			parent.Find("offer_amount/coins_currency").GetComponent<SpriteRenderer>().enabled = false;
			break;
		}
		packs.Add(pack);
	}
}
