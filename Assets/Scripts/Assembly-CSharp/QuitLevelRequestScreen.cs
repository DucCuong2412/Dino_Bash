using UnityEngine;

public class QuitLevelRequestScreen : BaseScreen
{
	private tk2dUIItem okButton;

	private tk2dUIItem cancelButton;

	private float time_scale = 1f;

	protected void Start()
	{
		okButton = base.transform.Search("btn_ok").GetComponent<tk2dUIItem>();
		cancelButton = base.transform.Search("btn_cancel").GetComponent<tk2dUIItem>();
		okButton.OnClick += delegate
		{
			HideTo(base.left, delegate
			{
				Time.timeScale = 1f;
				Level.Instance.stateAbort();
			}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		};
		cancelButton.OnClick += delegate
		{
			Hide();
		};
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public void Show(float time_scale)
	{
		if (!base.isVisible)
		{
			base.gameObject.SetActive(true);
			base.Show();
			this.time_scale = time_scale;
			OnEscapeUp = Hide;
			ShowFrom(base.right, null, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		}
	}

	public override void Show()
	{
		Show(time_scale);
	}

	public override void Hide()
	{
		if (base.isVisible)
		{
			base.Hide();
			base.isVisible = true;
			Time.timeScale = time_scale;
			ScreenManager.GetScreen<CoverScreen>(this).Hide();
			HideTo(base.left, delegate
			{
				base.gameObject.SetActive(false);
				base.isVisible = false;
			}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		}
	}
}
