using System;
using dinobash;

public class UpgradeReminderScreen : BaseScreen
{
	private StandardButton upgrade_button;

	private StandardButton close_button;

	private Action hide_callback;

	private void Start()
	{
		close_button = base.transform.Search("btn_close").GetComponent<StandardButton>();
		close_button.uiItem.OnClick += Hide;
		close_button.clickSound = Sounds.main_close_popup;
		if (Player.MaxLevelID < 10)
		{
			close_button.gameObject.SetActive(false);
		}
		upgrade_button = FindChildComponent<StandardButton>("MiddleCenter/Upgrade_Button");
		upgrade_button.uiItem.OnClick += delegate
		{
			hide_callback = delegate
			{
				ScreenManager.GetScreen<UpgradeScreen>().Show(UpgradeScreen.Tab.dinos);
			};
			Hide();
		};
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public override void Show()
	{
		UpgradeScreen screen = ScreenManager.GetScreen<UpgradeScreen>();
		SelectDinoScreen screen2 = ScreenManager.GetScreen<SelectDinoScreen>();
		SelectShotScreen screen3 = ScreenManager.GetScreen<SelectShotScreen>();
		if (!screen.isVisible && !screen2.isVisible && !screen3.isVisible)
		{
			base.gameObject.SetActive(true);
			base.Show();
			ScreenManager.GetScreen<CoverScreen>(this).Show();
			ShowFrom(base.right);
		}
	}

	public override void Hide()
	{
		base.Hide();
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		HideTo(base.left, delegate
		{
			if (hide_callback != null)
			{
				hide_callback();
			}
			base.gameObject.SetActive(false);
		});
	}
}
