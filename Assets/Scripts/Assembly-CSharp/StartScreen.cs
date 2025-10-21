using System;
using UnityEngine;
using dinobash;

public class StartScreen : BaseScreen
{
	private Transform logo;

	private tk2dUIItem start_button;

	private tk2dUIItem rate_button;

	private StandardButton info_button;

	private StandardButton fb_login_button;

	private StandardButton googleplay_button;

	private StandardButton leaderboards_button;

	private StandardButton achievements_button;

	private StandardButton moregames_button;

	private LocalizedText label_login_successfull;

	private bool started;

	private Color login_message_color;

	private LocalizedText fb_login_label;

	private Vector3 fb_login_label_size;

	private GoTween fadeout_tween;

	private bool authentication_tried;

	private StartInfoScreen info_screen
	{
		get
		{
			return ScreenManager.GetScreen<StartInfoScreen>();
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			fb_login_button.Enabled = true;
		}
	}

	protected void Start()
	{
		if (started)
		{
			return;
		}
		started = true;
		OnEscapeUp = delegate
		{
			bool flag5 = ScreenManager.GetScreen<QuitGameScreen>().isVisible;
			bool flag6 = ScreenManager.GetScreen<MoreGamesScreen>().isVisible;
			bool flag7 = ScreenManager.GetScreen<UpdateAppScreen>().isVisible;
			if (!info_screen.isVisible && !flag5 && !flag6 && !flag7)
			{
				ScreenManager.GetScreen<QuitGameScreen>().Show();
			}
		};
		logo = base.transform.Search("DinoBash_Logo");
		tk2dTextMesh component = base.transform.Search("version_label").GetComponent<tk2dTextMesh>();
		component.text = App.VERSION_CODE;
		component.ForceBuild();
		label_login_successfull = base.transform.Search("login_success_label").GetComponent<LocalizedText>();
		label_login_successfull.GetComponent<Renderer>().enabled = false;
		login_message_color = label_login_successfull.textMesh.color;
		info_button = base.transform.Search("button_settings").GetComponent<StandardButton>();
		info_button.uiItem.OnClick += delegate
		{
			if (!info_screen.isVisible)
			{
				info_screen.Show();
				info_button.Enabled = false;
				info_screen.OnScreenHide += EnableInfoButton;
			}
		};
		start_button = base.transform.Search("start_button").GetComponent<tk2dUIItem>();
		start_button.OnClick += delegate
		{
			if (Player.MaxLevelID <= Tutorials.LevelID("BasicShooting_Tutorial"))
			{
				Tracking.play_button_pressed_tutorial();
				Hide();
				App.stateGame(Player.MaxLevelID);
			}
			else
			{
				Hide();
				App.stateMap();
			}
		};
		fb_login_button = base.transform.Search("fb_login_button").GetComponent<StandardButton>();
		fb_login_button.uiItem.OnClick += delegate
		{
			if (!App.Instance.facebookManager.isLoggedIn)
			{
				if (App.Instance.InternetConnectivity)
				{
					fb_login_button.Enabled = false;
					App.Instance.facebookManager.Login(delegate(bool success)
					{
						if (this != null && base.isVisible)
						{
							OnLoggedIn(success);
						}
						fb_login_button.Enabled = true;
					});
				}
				else
				{
					ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
				}
			}
			else if (App.Instance.InternetConnectivity)
			{
				App.Instance.facebookManager.OpenInvitePopup();
			}
			else
			{
				ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
			}
		};
		fb_login_label = fb_login_button.GetComponentInChildren<LocalizedText>();
		fb_login_label_size = fb_login_label.textMesh.scale;
		base.gameObject.SetActive(false);
		rate_button = base.transform.Find("UpperCenter/rate_button").GetComponent<StandardButton>().uiItem;
		rate_button.OnClick += delegate
		{
			RateAppScreen.OpenStoreRatingSite();
		};
		moregames_button = base.transform.Search("button_moregames").GetComponent<StandardButton>();
		moregames_button.uiItem.OnClick += delegate
		{
			if (App.Instance.InternetConnectivity)
			{
				Tracking.moregames_button();
				ScreenManager.GetScreen<MoreGamesScreen>().Show();
			}
			else
			{
				ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
			}
		};
		leaderboards_button = base.transform.Search("leaderboards_button").GetComponent<StandardButton>();
		achievements_button = base.transform.Search("achievements_button").GetComponent<StandardButton>();
		Action action = delegate
		{
			Debug.Log("show_leaderboards");
			if (App.Instance.InternetConnectivity)
			{
				StandardButton standardButton3 = leaderboards_button;
				bool flag3 = false;
				achievements_button.Enabled = flag3;
				standardButton3.Enabled = flag3;
				SocialGamingManager instance = SocialGamingManager.Instance;
				Action<bool> onDone = delegate
				{
					StandardButton standardButton4 = leaderboards_button;
					bool flag4 = true;
					achievements_button.Enabled = flag4;
					standardButton4.Enabled = flag4;
				};
				instance.ShowLeaderboardUI(null, onDone);
			}
			else
			{
				ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
			}
		};
		Action action2 = delegate
		{
			Debug.Log("show_achievements");
			if (App.Instance.InternetConnectivity)
			{
				StandardButton standardButton = leaderboards_button;
				bool flag = false;
				achievements_button.Enabled = flag;
				standardButton.Enabled = flag;
				SocialGamingManager.Instance.ShowAchievementsUI(delegate
				{
					StandardButton standardButton2 = leaderboards_button;
					bool flag2 = true;
					achievements_button.Enabled = flag2;
					standardButton2.Enabled = flag2;
				});
			}
			else
			{
				ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
			}
		};
		Action value = action;
		Action value2 = action2;
		Transform transform = base.transform.Search("achievements_button").Search("icon_apple");
		if ((bool)transform)
		{
			transform.gameObject.SetActive(false);
		}
		transform = base.transform.Search("leaderboards_button").Search("icon_apple");
		if ((bool)transform)
		{
			transform.gameObject.SetActive(false);
		}
		leaderboards_button.uiItem.OnClick += value;
		achievements_button.uiItem.OnClick += value2;
		googleplay_button = base.transform.Search("google_play_button").GetComponent<StandardButton>();
		googleplay_button.gameObject.SetActive(false);
		googleplay_button.uiItem.OnClick += delegate
		{
			if (App.Instance.InternetConnectivity)
			{
				SocialGamingLogin();
			}
			else
			{
				ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
			}
		};
		if (Player.Instance != null && !Player.Instance.wasLoggedIntoSocialGamingProvider && !SocialGamingManager.Instance.isAuthenticated)
		{
			showGooglePlayLoginButton(true);
		}
		else
		{
			showGooglePlayLoginButton(false);
		}
		googleplay_button.disabledColor = (moregames_button.disabledColor = (leaderboards_button.disabledColor = (achievements_button.disabledColor = Colors.Visible * 0.85f)));
	}

	private void EnableInfoButton()
	{
		info_screen.OnScreenHide -= EnableInfoButton;
		info_button.Enabled = true;
	}

	private void OnLoggedIn(bool success)
	{
		if (success)
		{
			label_login_successfull.Key = "login_success";
			App.Instance.cloudSaveGameManager.checkAndPrompt();
			App.Instance.facebookManager.OpenInvitePopup();
		}
		else
		{
			label_login_successfull.Key = "login_failed";
		}
		label_login_successfull.GetComponent<Renderer>().enabled = true;
		label_login_successfull.textMesh.color = login_message_color;
		label_login_successfull.transform.localScale = Vector3.zero;
		Go.to(label_login_successfull.transform, 0.5f, new GoTweenConfig().scale(Vector3.one).setEaseType(GoEaseType.BounceOut));
		if (fadeout_tween != null)
		{
			fadeout_tween.destroy();
		}
		fadeout_tween = Go.to(label_login_successfull.textMesh, 1f, new GoTweenConfig().colorProp("color", Colors.Invisible).setDelay(5f));
		fadeout_tween.setOnCompleteHandler(delegate
		{
			label_login_successfull.GetComponent<Renderer>().enabled = false;
		});
	}

	public void setFBLoginButton()
	{
		if (App.Instance.facebookManager.isLoggedIn)
		{
			fb_login_label.textMesh.scale = fb_login_label_size;
			fb_login_label.Key = "INVITE_FRIENDS";
		}
		else
		{
			fb_login_label.textMesh.scale = fb_login_label_size;
			fb_login_label.Key = "Connect";
		}
	}

	private void showGooglePlayLoginButton(bool state)
	{
		if (App.State == App.States.Menu && state != googleplay_button.isActiveAndEnabled)
		{
			googleplay_button.gameObject.SetActive(state);
			leaderboards_button.gameObject.SetActive(!state);
			achievements_button.gameObject.SetActive(!state);
		}
	}

	private void Update()
	{
		setFBLoginButton();
		if (App.Instance.InternetConnectivity && Player.Instance != null && Player.Instance.wasLoggedIntoSocialGamingProvider && !SocialGamingManager.Instance.isAuthenticated && !authentication_tried)
		{
			authentication_tried = true;
			SocialGamingLogin();
		}
	}

	private void SocialGamingLogin()
	{
		Debug.Log("Trying To log into social gaming");
		SocialGamingManager.Instance.Authenticate(delegate(bool success)
		{
			Debug.Log("Trying To log into social gaming: " + success);
			showGooglePlayLoginButton(!success);
		});
	}

	public override void Show()
	{
		if (App.Instance.facebookManager.isLoggedIn)
		{
			App.Instance.cloudSaveGameManager.checkAndPrompt();
		}
		Start();
		base.gameObject.SetActive(true);
		base.Show();
		setFBLoginButton();
		float y = logo.position.y;
		logo.position = logo.position.SetY(1136f);
		logo.localScale = new Vector3(0.6f, 1f, 1f);
		Go.to(logo, 0.6f, new GoTweenConfig().setDelay(0.2f).position(logo.position.SetY(y)).setEaseType(GoEaseType.CubicOut));
		Go.to(logo, 0.6f, new GoTweenConfig().setDelay(0.1f).scale(Vector3.one).setEaseType(GoEaseType.BounceOut));
		Vector3 position = rate_button.transform.position;
		rate_button.transform.position = rate_button.transform.position.SetY(-2000f);
		Go.to(rate_button.transform, 0.75f, new GoTweenConfig().setDelay(0.8f).position(position).setEaseType(GoEaseType.CubicOut));
		Vector3 position2 = fb_login_button.transform.position;
		fb_login_button.transform.position = fb_login_button.transform.position.SetY(-1800f);
		Go.to(fb_login_button.transform, 0.75f, new GoTweenConfig().setDelay(0.6f).position(position2).setEaseType(GoEaseType.CubicOut));
		Vector3 position3 = start_button.transform.position;
		start_button.transform.position = start_button.transform.position.SetY(-1600f);
		Go.to(start_button.transform, 0.75f, new GoTweenConfig().setDelay(0.4f).position(position3).setEaseType(GoEaseType.CubicOut)).setOnCompleteHandler(delegate
		{
			start_button.GetComponent<StandardButton>().isFocused = true;
			ChartboostWrapper.ShowOnStartInterstital();
		});
		ScreenManager.GetScreen<UpdateAppScreen>().Show();
		info_screen.OnScreenShow += delegate
		{
			moregames_button.Enabled = false;
			StandardButton standardButton2 = leaderboards_button;
			bool flag2 = false;
			achievements_button.Enabled = flag2;
			standardButton2.Enabled = flag2;
		};
		info_screen.OnScreenHide += delegate
		{
			moregames_button.Enabled = true;
			StandardButton standardButton = leaderboards_button;
			bool flag = true;
			achievements_button.Enabled = flag;
			standardButton.Enabled = flag;
		};
		Transform transform = base.transform.Search("btn_root");
		transform.localPosition += new Vector3(-2048f, 0f, 0f);
		Go.to(transform, 1f, new GoTweenConfig().localPosition(Vector3.zero).setDelay(0.5f).setEaseType(GoEaseType.CubicOut));
		Tracking.welcomescreen_visible_tutorial();
		Tracking.welcomescreen_visible();
	}
}
