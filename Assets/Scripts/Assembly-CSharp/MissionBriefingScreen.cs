using System;
using UnityEngine;
using dinobash;

public class MissionBriefingScreen : BaseScreen
{
	private int targetLevelID;

	private tk2dUIItem btn_play;

	private tk2dUIItem btn_close;

	private tk2dUIItem btn_ok;

	private Transform needMoreLifesPopup;

	private Vector3 flat = new Vector3(1f, 0f, 1f);

	private CoverScreen coverScreen;

	protected void Start()
	{
		base.transform.localPosition += base.left;
		coverScreen = ScreenManager.GetScreen<CoverScreen>(this);
		needMoreLifesPopup = base.transform.Search("NeedMoreLifes");
		btn_play = base.transform.Search("btn_play").GetComponent<tk2dUIItem>();
		btn_close = base.transform.Search("btn_close").GetComponent<tk2dUIItem>();
		btn_ok = base.transform.Search("btn_ok").GetComponent<tk2dUIItem>();
		btn_close.OnClick += delegate
		{
			coverScreen.Hide();
			Hide(base.left, delegate
			{
				ScreenManager.GetScreen<MapScreen>().Show();
			});
		};
	}

	public void Show(int levelID)
	{
		base.Show();
		coverScreen.Show();
		targetLevelID = levelID;
		needMoreLifesPopup.transform.localScale = flat;
		btn_play.OnClick += PlayLevel;
		ShowFrom(base.left);
	}

	private void PlayLevel()
	{
		if (Player.Lives > 0)
		{
			ScreenManager.GetScreen<ResourceBarScreen>().Hide();
			Hide(base.right, delegate
			{
				ScreenManager.GetScreen<SelectDinoScreen>().Show(targetLevelID, base.left);
			});
		}
		else
		{
			btn_play.OnClick -= PlayLevel;
			NotEnoughtLifesIn();
		}
	}

	private void NotEnoughtLifesIn()
	{
		btn_ok.OnClick += NotEnoughtLifesOut;
		Go.to(needMoreLifesPopup, 0.5f, new GoTweenConfig().scale(Vector3.one).setEaseType(GoEaseType.BounceOut));
	}

	private void NotEnoughtLifesOut()
	{
		btn_ok.OnClick -= NotEnoughtLifesOut;
		btn_play.OnClick += PlayLevel;
		Go.to(needMoreLifesPopup, 0.35f, new GoTweenConfig().scale(flat).setEaseType(GoEaseType.BounceIn));
	}

	public void Hide(Vector3 hideTo, Action callback = null)
	{
		base.Hide();
		btn_ok.OnClick -= NotEnoughtLifesOut;
		HideTo(hideTo, callback);
	}
}
