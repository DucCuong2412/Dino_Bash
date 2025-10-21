using System;
using UnityEngine;
using dinobash;

public class PauseScreen : BaseScreen
{
	private tk2dUIItem continueButton;

	private tk2dUIItem endButton;

	private tk2dUIItem targetButton;

	private LocalizedText targetText;

	public float time_scale = 1f;

	public bool isTransitioning { get; private set; }

	protected void Start()
	{
		continueButton = base.transform.Search("Continue_Button").GetComponent<tk2dUIItem>();
		endButton = base.transform.Search("End_Button").GetComponent<tk2dUIItem>();
		targetButton = base.transform.Search("FocusVisibleNeanders_Button").GetComponent<tk2dUIItem>();
		targetText = targetButton.GetComponentInChildren<LocalizedText>();
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
		StandardButton standardButton = FindChildComponent<StandardButton>("MiddleCenter/FAQ_Button");
		if (HelpShiftWapper.instance != null)
		{
			standardButton.uiItem.OnClick += HelpShiftWapper.instance.ShowFAQ;
		}
		else
		{
			standardButton.uiItem.OnClick += delegate
			{
				//Application.OpenURL(Konfiguration.GameConfig.faq_url);
			};
		}
		targetText.Key = ((!Player.Instance.bird_focus_on_visible_neanders) ? "bird_target_first" : "bird_target_visible");
		targetButton.OnClick += delegate
		{
			Player.Instance.bird_focus_on_visible_neanders = !Player.Instance.bird_focus_on_visible_neanders;
			targetText.Key = ((!Player.Instance.bird_focus_on_visible_neanders) ? "bird_target_first" : "bird_target_visible");
		};
		continueButton.OnClick += Hide;
		endButton.OnClick += delegate
		{
			base.Hide();
			HideTo(base.left, ScreenManager.GetScreen<QuitLevelRequestScreen>().Show, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		};
		//if (Konfiguration.GameConfig.Use_dragshot_feature)
		//{
		//	targetButton.gameObject.SetActive(false);
		//	endButton.transform.position = targetButton.transform.position;
		//}
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public void Show(bool skip_animation = false)
	{
		if (isTransitioning)
		{
			return;
		}
		isTransitioning = true;
		base.gameObject.SetActive(true);
		base.Show();
		OnEscapeUp = Hide;
		time_scale = Time.timeScale;
		Time.timeScale = 0f;
		if (!skip_animation)
		{
			ScreenManager.GetScreen<CoverScreen>(this).Show();
			ShowFrom(base.right, delegate
			{
				isTransitioning = false;
			}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		}
		else
		{
			isTransitioning = false;
			ScreenManager.GetScreen<CoverScreen>(this).Show(skip_animation);
			base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		}
	}

	public override void Hide()
	{
		if (!isTransitioning)
		{
			isTransitioning = true;
			base.Hide();
			ScreenManager.GetScreen<CoverScreen>(this).Hide();
			Time.timeScale = time_scale;
			HideTo(base.left, delegate
			{
				isTransitioning = false;
				base.gameObject.SetActive(false);
			}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		}
	}
}
