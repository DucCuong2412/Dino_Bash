public class ErrorMessageScreen : BaseScreen
{
	private LocalizedText error_label;

	public static void ShowError(ErrorMessages error)
	{
		ErrorMessageScreen screen = ScreenManager.GetScreen<ErrorMessageScreen>();
		if (screen != null)
		{
			screen.Show(error.ToString());
		}
	}

	private void Start()
	{
		error_label = GetComponentInChildren<LocalizedText>();
		StandardButton standardButton = FindChildComponent<StandardButton>("MiddleCenter/btn_close");
		standardButton.uiItem.OnClick += Hide;
		standardButton.clickSound = Sounds.main_close_popup;
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public void Show(string error_i18n_key)
	{
		Go.tweensWithTarget(base.transform).ForEach(delegate(GoTween t)
		{
			t.complete();
		});
		error_label.Key = error_i18n_key;
		base.gameObject.SetActive(true);
		base.Show();
		OnEscapeUp = Hide;
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		ShowFrom(base.right, null, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
	}

	public override void Hide()
	{
		base.Interactive = false;
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		HideTo(base.left, delegate
		{
			base.Hide();
			base.gameObject.SetActive(false);
		}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
	}
}
