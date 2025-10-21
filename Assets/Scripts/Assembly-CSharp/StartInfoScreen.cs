using System;
using UnityEngine;

public class StartInfoScreen : BaseScreen
{
    private StandardButton button_fb;

    private void Start()
    {
        base.transform.position += base.right;
        StandardButton standardButton = FindChildComponent<StandardButton>("MiddleCenter/btn_close");
        standardButton.uiItem.OnClick += Hide;
        standardButton.clickSound = Sounds.main_close_popup;
        ToggleButton sound_toggle = base.transform.Search("Sound_toggle").GetComponent<ToggleButton>();
        ToggleButton music_toggle = base.transform.Search("Music_toggle").GetComponent<ToggleButton>();
        Action set_sound = delegate
        {
            sound_toggle.toggle_state = AudioPlayer.SFXVolume != 0f;
        };
        Action set_music = delegate
        {
            music_toggle.toggle_state = AudioPlayer.MusicVolume != 0f;
        };
        set_sound();
        set_music();
        sound_toggle.uiItem.OnClick += delegate
        {
            AudioPlayer.ToggleSound();
            set_sound();
        };
        music_toggle.uiItem.OnClick += delegate
        {
            AudioPlayer.ToggleMusic();
            set_music();
        };
        StandardButton standardButton2 = FindChildComponent<StandardButton>("MiddleCenter/FAQ_Button");
        if (HelpShiftWapper.instance != null)
        {
            standardButton2.uiItem.OnClick += HelpShiftWapper.instance.ShowFAQ;
        }
        else
        {
            standardButton2.uiItem.OnClick += delegate
            {
                //Application.OpenURL(Konfiguration.GameConfig.faq_url);
            };
        }
        StandardButton standardButton3 = FindChildComponent<StandardButton>("MiddleCenter/TOS_Button");
        standardButton3.uiItem.OnClick += delegate
        {
            //Application.OpenURL(Konfiguration.GameConfig.terms_of_use_url);
        };
        StandardButton standardButton4 = FindChildComponent<StandardButton>("MiddleCenter/Privacy_Button");
        standardButton4.uiItem.OnClick += delegate
        {
            //Application.OpenURL(Konfiguration.GameConfig.privacy_policy_url);
        };
        button_fb = FindChildComponent<StandardButton>("MiddleCenter/FB_Button");
        button_fb.uiItem.OnClick += OnFacebookClick;
    }

    private void SetFB_Button(bool print_text = false)
    {
        button_fb.Enabled = true;
        LocalizedText componentInChildren = button_fb.GetComponentInChildren<LocalizedText>();
        //if (App.Instance.facebookManager.isLoggedIn)
        //{
        //	componentInChildren.Key = "fb_disconnect";
        //}
        //else
        //{
        //	componentInChildren.Key = "fb_connect";
        //}
        componentInChildren.textMesh.Commit();
        if (print_text)
        {
            button_fb.GetComponentInChildren<TextPrinter>().Print();
        }
    }

    private void OnFacebookClick()
    {
        if (App.Instance.InternetConnectivity)
        {
            StartScreen start_screen = ScreenManager.GetScreen<StartScreen>();
            //if (!App.Instance.facebookManager.isLoggedIn)
            //{
            //	button_fb.Enabled = false;
            //	App.Instance.facebookManager.Login(delegate
            //	{
            //		start_screen.setFBLoginButton();
            //		SetFB_Button(true);
            //	});
            //}
            //else
            //{
            //	App.Instance.facebookManager.Logout();
            //	start_screen.setFBLoginButton();
            //	SetFB_Button(true);
            //}
        }
        else
        {
            ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
        }
    }

    public override void Show()
    {
        base.Show();
        OnEscapeUp = Hide;
        SetFB_Button();
        ShowFrom(base.right);
    }

    public override void Hide()
    {
        base.Interactive = false;
        HideTo(base.left, base.Hide);
    }
}
