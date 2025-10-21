using UnityEngine;

public class NewCloudSaveAvailableScreen : BaseScreen
{
	private tk2dTextMesh label_desc;

	private tk2dTextMesh label_level;

	private tk2dTextMesh label_device;

	private tk2dTextMesh label_current;

	private CloudSaveGames.CloudSaveGameProgress progress;

	private SpriteRenderer bg_fill;

	private SpriteRenderer throbber;

	public static void ShowPrompt(CloudSaveGames.CloudSaveGameProgress progress)
	{
		NewCloudSaveAvailableScreen screen = ScreenManager.GetScreen<NewCloudSaveAvailableScreen>();
		if (screen != null)
		{
			screen.Show(progress);
		}
	}

	private void Start()
	{
		label_desc = base.transform.Search("label_description").GetComponent<tk2dTextMesh>();
		label_level = base.transform.Search("label_level").GetComponent<tk2dTextMesh>();
		label_device = base.transform.Search("label_device").GetComponent<tk2dTextMesh>();
		label_current = base.transform.Search("label_current").GetComponent<tk2dTextMesh>();
		bg_fill = base.transform.Search("bg_fill").GetComponent<SpriteRenderer>();
		bg_fill.color = Colors.Invisible;
		throbber = base.transform.Search("throbber").GetComponent<SpriteRenderer>();
		StandardButton load_btn = base.transform.Search("Load_Button").GetComponent<StandardButton>();
		StandardButton continue_btn = base.transform.Search("Continue_Button").GetComponent<StandardButton>();
		load_btn.disabledColor = (continue_btn.disabledColor = Colors.Visible * 0.85f);
		base.transform.Search("Load_Button").GetComponent<tk2dUIItem>().OnClick += delegate
		{
			load_btn.Enabled = false;
			continue_btn.Enabled = false;
			Go.to(throbber, 0.3f, new GoTweenConfig().colorProp("color", Colors.Visible));
			App.Instance.cloudSaveGameManager.loadFromCloudAndApply(progress.timestamp, delegate
			{
				Hide();
			});
		};
		base.transform.Search("Continue_Button").GetComponent<tk2dUIItem>().OnClick += delegate
		{
			load_btn.Enabled = false;
			continue_btn.Enabled = false;
			Go.to(throbber, 0.3f, new GoTweenConfig().colorProp("color", Colors.Visible));
			App.Instance.cloudSaveGameManager.saveToCloud(delegate
			{
				Hide();
			});
		};
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	private void SetMaxChars(tk2dTextMesh tm)
	{
		tm.maxChars = tm.text.Length;
		tm.Commit();
	}

	public void Show(CloudSaveGames.CloudSaveGameProgress progress)
	{
		Go.tweensWithTarget(base.transform).ForEach(delegate(GoTween t)
		{
			t.complete();
		});
		label_desc.text = "NEW_CLOUD_SAVEGAME_PROMPT".Localize();
		label_level.text = string.Format("NEW_CLOUD_SAVEGAME_LEVEL".Localize(), progress.max_level);
		label_device.text = string.Format("NEW_CLOUD_SAVEGAME_DEVICE".Localize(), progress.device_name);
		label_current.text = "NEW_CLOUD_SAVEGAME_CURRENT".Localize();
		SetMaxChars(label_desc);
		SetMaxChars(label_level);
		SetMaxChars(label_device);
		SetMaxChars(label_current);
		base.gameObject.SetActive(true);
		base.Show();
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		ShowFrom(base.right, delegate
		{
			Go.to(bg_fill, 0.3f, new GoTweenConfig().colorProp("Color", new Color(1f, 1f, 1f, 0.5f)));
		}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		base.transform.Search("Load_Button").GetComponent<StandardButton>().Enabled = true;
		base.transform.Search("Continue_Button").GetComponent<StandardButton>().Enabled = true;
		throbber.color = Colors.Invisible;
		this.progress = progress;
	}

	public override void Hide()
	{
		base.Interactive = false;
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		Go.to(bg_fill, 0.3f, new GoTweenConfig().colorProp("Color", Colors.Invisible).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate).onComplete(delegate
		{
			HideTo(base.left, delegate
			{
				base.Hide();
				base.gameObject.SetActive(false);
			}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		}));
	}
}
