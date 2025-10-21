using UnityEngine;

public class RewardedFBLoginScreen : BaseScreen
{
	public static bool shown_in_session;

	private StandardButton connectButton;

	private SpriteRenderer loader_icon;

	private tk2dTextMesh sign_into_facebook;

	private bool logged_in;

	private int login_reward
	{
		get
		{
			return Konfiguration.GameConfig.Facebook_connect_reward;
		}
	}

	protected void Start()
	{
		StandardButton component = base.transform.Search("btn_close").GetComponent<StandardButton>();
		component.uiItem.OnClick += Hide;
		component.clickSound = Sounds.main_close_popup;
		connectButton = base.transform.Search("btn_login").GetComponent<StandardButton>();
		connectButton.uiItem.OnClick += OnConnectClick;
		tk2dTextMesh component2 = base.transform.Search("label_benefits1").GetComponent<tk2dTextMesh>();
		component2.text = "facebook_connect_benefits1".Localize();
		component2 = base.transform.Search("label_benefits2").GetComponent<tk2dTextMesh>();
		component2.text = "facebook_connect_benefits2".Localize();
		component2 = base.transform.Search("label_benefits3").GetComponent<tk2dTextMesh>();
		component2.text = "facebook_connect_benefits3".Localize();
		component2 = base.transform.Search("label_benefits4").GetComponent<tk2dTextMesh>();
		component2.text = "facebook_connect_benefits4".Localize();
		component2 = base.transform.Search("label_benefits5").GetComponent<tk2dTextMesh>();
		component2.text = string.Format("facebook_connect_benefits5".Localize(), login_reward);
		sign_into_facebook = base.transform.Search("sign_into_facebook").GetComponent<tk2dTextMesh>();
		sign_into_facebook.text = string.Format("facebook_connect_benefits_login".Localize(), login_reward);
		loader_icon = base.transform.Search("icon_connecting").GetComponent<SpriteRenderer>();
		loader_icon.color = Colors.Invisible;
		base.transform.localPosition += base.left;
		base.gameObject.SetActive(false);
	}

	private void OnConnectClick()
	{
		if (App.Instance.facebookManager.isLoggedIn)
		{
			Hide();
		}
		else if (App.Instance.InternetConnectivity)
		{
			connectButton.Enabled = false;
			sign_into_facebook.color *= 0.75f;
			Go.to(loader_icon, 0.5f, new GoTweenConfig().colorProp("color", Colors.Visible));
			App.Instance.facebookManager.Login(delegate(bool success)
			{
				if (base.isVisible)
				{
					Go.to(loader_icon, 0.5f, new GoTweenConfig().colorProp("color", Colors.Invisible));
					if (success)
					{
						logged_in = true;
						Wallet.GiveCoins(login_reward);
						Hide();
					}
					else
					{
						connectButton.Enabled = true;
						sign_into_facebook.color *= 1.3333334f;
						Debug.Log("User canceled login");
					}
				}
			});
		}
		else
		{
			ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
		}
	}

	public override void Show()
	{
		base.gameObject.SetActive(true);
		base.Show();
		shown_in_session = true;
		Tracking.rewarded_facebook_login("show");
		OnEscapeUp = Hide;
		ShowFrom(base.right);
		base.isVisible = true;
		CoverScreen screen = ScreenManager.GetScreen<CoverScreen>(this);
		if (!screen.isVisible)
		{
			screen.Show();
		}
	}

	public override void Hide()
	{
		base.Hide();
		if (logged_in)
		{
			Tracking.rewarded_facebook_login("login");
		}
		else
		{
			Tracking.rewarded_facebook_login("cancel");
		}
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		HideTo(base.left, delegate
		{
			base.gameObject.SetActive(false);
		});
	}
}
