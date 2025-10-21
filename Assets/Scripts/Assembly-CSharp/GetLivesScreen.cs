using System;
using System.Collections;
using UnityEngine;
using dinobash;

public class GetLivesScreen : BaseScreen
{
	private tk2dUIItem btn_askfriends;

	private tk2dUIItem btn_buy;

	private tk2dTextMesh diamondPrice;

	private tk2dTextMesh timer;

	public StandardButton btn_close;

	private bool hideCoverScreen = true;

	private Action onHide;

	private void Start()
	{
		btn_askfriends = FindChildComponent<tk2dUIItem>("MiddleCenter/inlay_askfriends/btn_askfriends");
		btn_askfriends.OnClick += OnAskFriendsClicked;
		btn_buy = FindChildComponent<tk2dUIItem>("MiddleCenter/inlay_buylives/btn_buy");
		btn_buy.OnClick += OnBuyClicked;
		diamondPrice = btn_buy.GetComponentInChildren<tk2dTextMesh>();
		diamondPrice.text = Konfiguration.GameConfig.RefillLivesDiamondCost.ToString();
		timer = FindChildComponent<tk2dTextMesh>("MiddleCenter/inlay_livetimer/label_time");
		btn_close = FindChildComponent<StandardButton>("MiddleCenter/btn_close");
		btn_close.clickSound = Sounds.main_close_popup;
		btn_close.uiItem.OnClick += delegate
		{
			Hide();
		};
		base.transform.localPosition += base.left;
		base.gameObject.SetActive(false);
	}

	private void OnBuyClicked()
	{
		if (Wallet.Diamonds >= Konfiguration.GameConfig.RefillLivesDiamondCost)
		{
			Wallet.TakeDiamonds(Konfiguration.GameConfig.RefillLivesDiamondCost);
			Player.Instance.PlayerData.timeManipulationPenalty = false;
			Player.Lives = 5;
			string origin = ((App.State != App.States.Map) ? "lives_store_level" : "lives_store_map");
			int refillLivesDiamondCost = Konfiguration.GameConfig.RefillLivesDiamondCost;
			Tracking.buy_item(UnitType.None, ShotType.None, "lives", refillLivesDiamondCost, origin);
		}
		else
		{
			hideCoverScreen = false;
			onHide = delegate
			{
				Tracking.store_open(Wallet.Currency.Diamonds, "getlives_screen", "life");
				ShopScreenDiamonds screen = ScreenManager.GetScreen<ShopScreenDiamonds>();
				screen.Show();
				screen.HideCoverScreen = false;
				if (App.State == App.States.Map)
				{
					screen.OnCloseClick = Show_Delayed;
				}
			};
		}
		Hide();
	}

	private void OnAskFriendsClicked()
	{
		hideCoverScreen = false;
		if (!App.Instance.facebookManager.isLoggedIn)
		{
			hideCoverScreen = true;
			onHide = delegate
			{
				FacebookLoginRequestScreen screen2 = ScreenManager.GetScreen<FacebookLoginRequestScreen>();
				screen2.Show(FacebookManager.AppRequestType.ASK_FOR_LIVES);
			};
		}
		else
		{
			if (!App.Instance.InternetConnectivity)
			{
				hideCoverScreen = true;
				ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
				return;
			}
			hideCoverScreen = true;
			onHide = delegate
			{
				SelectFriendsScreen screen = ScreenManager.GetScreen<SelectFriendsScreen>();
				screen.Show(FacebookManager.AppRequestType.ASK_FOR_LIVES);
			};
		}
		Hide();
	}

	private IEnumerator UpdateTimerLabel()
	{
		while (base.isVisible)
		{
			TimeSpan ts = TimeSpan.FromSeconds(Player.GetSecondsToNextLive());
			string hours = string.Empty;
			if (ts.Hours > 0)
			{
				hours = ts.Hours.ToString("00") + ":";
			}
			timer.text = hours + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
			if (Player.Lives == 5)
			{
				Hide();
				break;
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public void Show_Delayed()
	{
		base.gameObject.SetActive(true);
		ResourceBarScreen resource_bar = ScreenManager.GetScreen<ResourceBarScreen>();
		resource_bar.switch_in_progress = true;
		WaitThen(0.3f, delegate
		{
			resource_bar.switch_in_progress = false;
			Show();
		});
	}

	public override void Show()
	{
		base.gameObject.SetActive(true);
		base.Show();
		OnEscapeUp = Hide;
		hideCoverScreen = true;
		onHide = null;
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		AudioPlayer.PlayGuiSFX(Sounds.main_get_lifes, 0f);
		StartCoroutine(UpdateTimerLabel());
		ShowFrom(base.right);
	}

	public override void Hide()
	{
		if (hideCoverScreen)
		{
			ScreenManager.GetScreen<CoverScreen>(this).Hide();
		}
		base.Interactive = false;
		ResourceBarScreen resource_bar = ScreenManager.GetScreen<ResourceBarScreen>();
		resource_bar.switch_in_progress = true;
		HideTo(base.left, delegate
		{
			resource_bar.switch_in_progress = false;
			if (onHide != null)
			{
				onHide();
			}
			base.Hide();
			base.gameObject.SetActive(false);
		});
	}
}
