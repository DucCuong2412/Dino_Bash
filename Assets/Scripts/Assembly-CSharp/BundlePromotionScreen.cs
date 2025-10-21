using System;
using System.Collections.Generic;
using UnityEngine;

public class BundlePromotionScreen : BaseScreen
{
	private tk2dTextMesh label_title;

	private tk2dTextMesh label_timer;

	private StandardButton btn_buy;

	private Transform loader;

	private bool is_buying;

	private List<FlyElementTo> diamond_fly_elements = new List<FlyElementTo>();

	private List<FlyElementTo> consumable_fly_elements = new List<FlyElementTo>();

	private ParticleSystem buyFx;

	private List<GoTween> loader_tweens = new List<GoTween>();

	private ShopItems.Pack pack
	{
		get
		{
			if (ShopItems.bundle_entry != null)
			{
				ShopItems.Pack packById = ShopItems.getPackById(ShopItems.bundle_entry.Value);
				if (packById != null)
				{
					return packById;
				}
				Debug.LogError("bundle_promotion: Item setup is faulty: " + ShopItems.bundle_entry.Value);
			}
			else
			{
				Debug.LogError("bundle_promotion: Item is null!");
			}
			return ShopItems.getPackById("com.pokokostudio.dinobash.starter_pack");
		}
	}

	private void Buy(ShopItems.Pack pack)
	{
		App.Instance.paymentManager.onPackPurchased += handleOnPurchase;
		App.Instance.paymentManager.onPurchaseAborted += handlePurchaseAborted;
		App.Instance.paymentManager.onPurchaseFailed += handleonPurchaseFailed;
		is_buying = true;
		App.Instance.paymentManager.PurchasePack(pack);
	}

	private void handleonPurchaseFailed()
	{
		Debug.Log("bundle_promotion: handleonPurchaseFailed");
		ErrorMessageScreen.ShowError(ErrorMessages.IAP_PURCHASE_FAILED);
		OnEscapeUp = null;
		ErrorMessageScreen screen = ScreenManager.GetScreen<ErrorMessageScreen>();
		screen.OnScreenHide += EnableBackButton;
		end_IAP_Process();
	}

	private void handlePurchaseAborted()
	{
		Debug.Log("bundle_promotion: handlePurchaseAborted");
		end_IAP_Process();
	}

	private void handleOnPurchase(ShopItems.Pack pack)
	{
		Debug.Log("bundle_promotion: handleOnPurchase");
		BundleOffer.offerEnded -= HandleofferEnded;
		BundleOffer.EndOffer();
		if (App.State == App.States.Map)
		{
			buyFx.gameObject.SetActive(true);
			playFlyElements();
		}
		App.Instance.cloudSaveGameManager.checkAndPrompt(false);
		end_IAP_Process();
		WaitThenRealtime(4f, Hide);
	}

	private void end_IAP_Process()
	{
		if (is_buying)
		{
			is_buying = false;
			App.Instance.paymentManager.onPackPurchased -= handleOnPurchase;
			App.Instance.paymentManager.onPurchaseAborted -= end_IAP_Process;
			App.Instance.paymentManager.onPurchaseFailed -= handleonPurchaseFailed;
			if (btn_buy != null)
			{
				btn_buy.Enabled = BundleOffer.is_offered;
			}
			ForceHideLoader();
		}
	}

	public void OnAppEnter()
	{
		Debug.Log(string.Format("process_buying:{0} gameObject.activeSelf:{1} isVisible:{2} Interactive:{3}", is_buying, base.gameObject.activeSelf, base.isVisible, base.Interactive));
		if (is_buying && base.isVisible && base.Interactive)
		{
			end_IAP_Process();
			Debug.Log("bundle_promotion: end_iap_process");
		}
	}

	private void ForceHideLoader()
	{
		if (!(loader == null))
		{
			Debug.Log("bundle_promotion: hide loader");
			loader_tweens.ForEach(delegate(GoTween tween)
			{
				tween.rewind();
			});
			loader.gameObject.SetActive(false);
		}
	}

	protected void Start()
	{
		StandardButton component = base.transform.Search("btn_close").GetComponent<StandardButton>();
		component.uiItem.OnClick += Hide;
		component.clickSound = Sounds.main_close_popup;
		label_timer = base.transform.Search("label_timer").GetComponent<tk2dTextMesh>();
		label_title = base.transform.Search("title_label").GetComponent<tk2dTextMesh>();
		loader = base.transform.FindChild("MiddleCenter/loader");
		SetLoaderTween();
		loader.gameObject.SetActive(false);
		tk2dTextMesh component2 = base.transform.Search("label_percent_off").GetComponent<tk2dTextMesh>();
		component2.text = string.Format("{0:P0} OFF".Localize(), BundleOffer.PercentOff);
		btn_buy = base.transform.Search("btn_buy").GetComponent<StandardButton>();
		btn_buy.uiItem.OnClick += OnBuyClicked;
		tk2dTextMesh component3 = base.transform.Search("label_diamond").GetComponent<tk2dTextMesh>();
		component3.text = pack.amount.ToString();
		tk2dTextMesh component4 = base.transform.Search("label_blizzard").GetComponent<tk2dTextMesh>();
		component4.text = BundleOffer.blizzard_amount.ToString();
		tk2dTextMesh component5 = base.transform.Search("label_megaball").GetComponent<tk2dTextMesh>();
		component5.text = BundleOffer.megaball_amount.ToString();
		diamond_fly_elements.AddRange(createFlyToElement(base.transform.Search("fly_icon_diamond"), pack.amount));
		consumable_fly_elements.AddRange(createFlyToElement(base.transform.Search("fly_icon_megaball"), BundleOffer.megaball_amount));
		consumable_fly_elements.AddRange(createFlyToElement(base.transform.Search("fly_icon_blizzard"), BundleOffer.blizzard_amount));
		consumable_fly_elements.Shuffle();
		buyFx = FindChildComponent<ParticleSystem>("MiddleCenter/FX_Stars");
		buyFx.gameObject.SetActive(false);
		base.transform.localPosition += base.left;
		base.gameObject.SetActive(false);
	}

	private List<FlyElementTo> createFlyToElement(Transform original, int amount)
	{
		List<FlyElementTo> list = new List<FlyElementTo>();
		for (int i = 0; i < amount; i++)
		{
			Transform transform = UnityEngine.Object.Instantiate(original) as Transform;
			transform.transform.parent = original.transform.parent;
			transform.transform.position = original.transform.position;
			list.Add(transform.AddOrGetComponent<FlyElementTo>());
			transform.GetComponent<SpriteRenderer>().sortingOrder += 5000;
			transform.gameObject.SetActive(false);
		}
		return list;
	}

	private void playFlyElements()
	{
		ResourceBarScreen screen = ScreenManager.GetScreen<ResourceBarScreen>();
		for (int i = 0; i < diamond_fly_elements.Count; i++)
		{
			diamond_fly_elements[i].gameObject.SetActive(true);
			diamond_fly_elements[i].Play(screen.icon_diamonds.transform.position, 0.5f + (float)i * (2f / (float)diamond_fly_elements.Count));
		}
		Transform transform = base.transform.Search("FlyToTarget");
		for (int j = 0; j < consumable_fly_elements.Count; j++)
		{
			float delay = 0.5f + (float)j * (2f / (float)consumable_fly_elements.Count);
			consumable_fly_elements[j].gameObject.SetActive(true);
			consumable_fly_elements[j].Play(transform.position, delay);
			Go.to(consumable_fly_elements[j].transform, consumable_fly_elements[j].duration, new GoTweenConfig().scale(Vector3.zero).setDelay(delay));
		}
	}

	private void OnBuyClicked()
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
				btn_buy.Enabled = false;
				WaitThenRealtime(0.3f, delegate
				{
					Buy(pack);
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

	public static string getTitle()
	{
		if (ShopItems.bundle_entry.Value.Contains("starter"))
		{
			return "bundle_promotion_title_starter".Localize();
		}
		return "bundle_promotion_title_regular".Localize();
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
		ForceHideLoader();
		is_buying = false;
		label_title.text = getTitle();
		TextMesh componentInChildren = btn_buy.GetComponentInChildren<TextMesh>();
		componentInChildren.text = pack.localizedPriceString;
		int num = 8;
		if (componentInChildren.text.Length > num)
		{
			componentInChildren.fontSize = ShopScreen.getPriceFontSize(componentInChildren.text.Length, num);
		}
		OnEscapeUp = Hide;
		BundleOffer.offerEnded += HandleofferEnded;
		ShowFrom(base.right);
		base.isVisible = true;
		CoverScreen screen = ScreenManager.GetScreen<CoverScreen>(this);
		if (!screen.isVisible)
		{
			screen.Show();
		}
	}

	private void HandleofferEnded()
	{
		if (!is_buying && base.isVisible)
		{
			Hide();
		}
	}

	private void Update()
	{
		TimeSpan time_remaining = BundleOffer.time_remaining;
		if (time_remaining.Seconds >= 0)
		{
			label_timer.text = string.Format("bundle_time_left".Localize(), time_remaining.Humanize());
		}
		else
		{
			label_timer.text = string.Empty;
		}
	}

	public override void Hide()
	{
		if (base.isVisible)
		{
			base.Hide();
			BundleOffer.offerEnded -= HandleofferEnded;
			ScreenManager.GetScreen<CoverScreen>(this).Hide();
			HideTo(base.left, delegate
			{
				base.gameObject.SetActive(false);
			});
		}
	}
}
