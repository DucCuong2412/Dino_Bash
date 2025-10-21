using UnityEngine;

public class UpdateAppScreen : BaseScreen
{
	private bool init;

	private StandardButton closebutton;

	private LocalizedText message;

	private void Start()
	{
		if (init)
		{
			return;
		}
		init = true;
		message = base.transform.Search("message_label").GetComponent<LocalizedText>();
		closebutton = base.transform.Search("btn_close").GetComponent<StandardButton>();
		closebutton.clickSound = Sounds.main_close_popup;
		closebutton.uiItem.OnClick += delegate
		{
			Hide();
		};
		OnEscapeUp = delegate
		{
			if (!Konfiguration.GameConfig.Force_App_Update)
			{
				Hide();
			}
		};
		base.transform.Search("btn_yes").GetComponent<tk2dUIItem>().OnClick += delegate
		{
			OpenStoreSite();
		};
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public override void Show()
	{
		if (Konfiguration.GameConfig.Show_update_prompt)
		{
			Start();
			closebutton.gameObject.SetActive(!Konfiguration.GameConfig.Force_App_Update);
			message.Key = ((!Konfiguration.GameConfig.Force_App_Update) ? "update_app_message" : "update_app_message_forced");
			base.Show();
			base.gameObject.SetActive(true);
			ScreenManager.GetScreen<CoverScreen>(this).Show();
			ShowFrom(base.right, delegate
			{
			});
		}
	}

	public override void Hide()
	{
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		base.Hide();
		base.isVisible = true;
		HideTo(base.left, delegate
		{
			base.isVisible = false;
			base.gameObject.SetActive(false);
		});
	}

	private void OpenStoreSite()
	{
		Debug.Log("Opening app store to update app at " + Konfiguration.RateURL);
		Application.OpenURL(Konfiguration.RateURL);
	}
}
