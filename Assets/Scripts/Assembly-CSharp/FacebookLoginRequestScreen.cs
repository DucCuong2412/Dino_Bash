using System;
using UnityEngine;

public class FacebookLoginRequestScreen : BaseScreen
{
	private StandardButton closeButton;

	private tk2dUIItem connectButton;

	private SpriteRenderer icon_friendgate;

	private SpriteRenderer icon_lives;

	private LocalizedText sign_label;

	private FacebookManager.AppRequestType request_type;

	private Action onHide;

	public bool hideCoverScreen { get; set; }

	protected void Start()
	{
		closeButton = base.transform.Search("btn_close").GetComponent<StandardButton>();
		closeButton.uiItem.OnClick += OnCloseClick;
		closeButton.clickSound = Sounds.main_close_popup;
		connectButton = base.transform.Search("btn_login").GetComponent<tk2dUIItem>();
		connectButton.OnClick += OnConnectClick;
		icon_friendgate = base.transform.Search("icon_friendgate").GetComponent<SpriteRenderer>();
		icon_lives = base.transform.Search("icon_lives").GetComponent<SpriteRenderer>();
		sign_label = base.transform.Search("sign_label").GetComponent<LocalizedText>();
		base.transform.localPosition += base.left;
		base.gameObject.SetActive(false);
	}

	private void OnCloseClick()
	{
		Hide();
		if (App.State == App.States.Map)
		{
			ScreenManager.GetScreen<MapScreen>().Show();
		}
		else if (App.State == App.States.Game)
		{
			ScreenManager.GetScreen<GetLivesScreen>().Show_Delayed();
		}
	}

	private void OnConnectClick()
	{
		if (App.Instance.InternetConnectivity)
		{
			App.Instance.facebookManager.Login(delegate(bool success)
			{
				if (success)
				{
					onHide = delegate
					{
						ScreenManager.GetScreen<SelectFriendsScreen>().Show(request_type);
					};
					Hide();
				}
				else
				{
					Debug.Log("User canceled login");
				}
			});
		}
		else
		{
			ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
		}
	}

	public void Show(FacebookManager.AppRequestType request_type)
	{
		base.gameObject.SetActive(true);
		this.request_type = request_type;
		hideCoverScreen = true;
		onHide = null;
		OnEscapeUp = Hide;
		ShowFrom(base.right, base.Show);
		base.isVisible = true;
		if (request_type != FacebookManager.AppRequestType.ASK_FOR_FRIENDGATE && request_type != FacebookManager.AppRequestType.ASK_FOR_LIVES)
		{
			Debug.LogError("request_type must be ASK_FOR_FRIENDGATE or ASK_FOR_LIVES");
		}
		bool flag = request_type == FacebookManager.AppRequestType.ASK_FOR_FRIENDGATE;
		sign_label.Key = ((!flag) ? "login_request_get_lives" : "login_request_unlock_levels");
		icon_lives.enabled = !flag;
		icon_friendgate.enabled = flag;
		CoverScreen screen = ScreenManager.GetScreen<CoverScreen>(this);
		if (!screen.isVisible)
		{
			screen.Show();
		}
	}

	public override void Hide()
	{
		base.Hide();
		if (hideCoverScreen)
		{
			ScreenManager.GetScreen<CoverScreen>(this).Hide();
		}
		HideTo(base.left, delegate
		{
			if (onHide != null)
			{
				onHide();
			}
			base.gameObject.SetActive(false);
		});
	}
}
