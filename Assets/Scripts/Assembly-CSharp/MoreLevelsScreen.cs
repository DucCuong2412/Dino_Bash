public class MoreLevelsScreen : BaseScreen
{
	public static bool shownInSession { get; private set; }

	private void Start()
	{
		StandardButton component = base.transform.Search("btn_close").GetComponent<StandardButton>();
		component.clickSound = Sounds.main_close_popup;
		component.uiItem.OnClick += delegate
		{
			Hide();
		};
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public override void Show()
	{
		shownInSession = true;
		base.Show();
		base.gameObject.SetActive(true);
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		ShowFrom(base.right, delegate
		{
		});
	}

	public override void Hide()
	{
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		base.Hide();
		HideTo(base.left, delegate
		{
			base.gameObject.SetActive(false);
		});
	}
}
