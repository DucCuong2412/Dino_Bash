using UnityEngine;
using dinobash;

public class RateAppScreen : BaseScreen
{
	private void Start()
	{
		StandardButton component = base.transform.Search("btn_close").GetComponent<StandardButton>();
		component.clickSound = Sounds.main_close_popup;
		component.uiItem.OnClick += delegate
		{
			Hide();
			Player.Instance.shouldShowRatePrompt = true;
		};
		base.transform.Search("btn_yes").GetComponent<tk2dUIItem>().OnClick += delegate
		{
			Hide();
			OpenStoreRatingSite();
		};
		base.transform.Search("btn_later").GetComponent<tk2dUIItem>().OnClick += delegate
		{
			Hide();
			Player.Instance.shouldShowRatePrompt = true;
		};
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public override void Hide()
	{
		base.Hide();
		HideTo(base.left, delegate
		{
			base.gameObject.SetActive(false);
		});
	}

	public override void Show()
	{
		base.Show();
		base.gameObject.SetActive(true);
		ShowFrom(base.right, delegate
		{
			base.transform.Find("MiddleCenter/stars").GetComponent<Animator>().Play("rate app_stars in");
		});
	}

	public static void OpenStoreRatingSite()
	{
		Tracking.rate_button();
		Debug.Log("Opening app store rating page at " + Konfiguration.RateURL);
		Application.OpenURL(Konfiguration.RateURL);
		Player.Instance.shouldShowRatePrompt = false;
	}
}
