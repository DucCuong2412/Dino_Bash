using UnityEngine;
using mixpanel.platform;

public class MoreGamesScreen : BaseScreen
{
	private StandardButton closebutton;

	private Vector3 margin;

	private WebViewObject webViewObject;

	private string URL
	{
		get
		{
			return "";// Konfiguration.GameConfig.MoregamesURL_Android + MixpanelUnityPlatform.get_distinct_id();
		}
	}

	private void Start()
	{
		closebutton = base.transform.Search("btn_close").GetComponent<StandardButton>();
		closebutton.clickSound = Sounds.main_close_popup;
		closebutton.uiItem.OnClick += delegate
		{
			Hide();
		};
		margin = ScreenManager.Camera.WorldToScreenPoint(base.transform.Search("maringMarker").position);
		SetUpWebView();
		base.gameObject.SetActive(false);
	}

	public override void Show()
	{
		base.gameObject.SetActive(true);
		base.Show();
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		OnEscapeUp = Hide;
		loadWebView();
		SetWebViewVisibility(true);
	}

	public override void Hide()
	{
		base.Hide();
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		base.isVisible = true;
		SetWebViewVisibility(false);
		WaitThenRealtime(0.1f, delegate
		{
			base.isVisible = false;
			base.gameObject.SetActive(false);
		});
	}

	private void SetWebViewVisibility(bool v)
	{
		if (webViewObject != null)
		{
			webViewObject.SetVisibility(v);
		}
	}

	private void SetUpWebView()
	{
		int num = Mathf.RoundToInt(margin.x);
		int num2 = Mathf.RoundToInt(margin.y);
		if (Application.platform != RuntimePlatform.WindowsEditor)
		{
			webViewObject = new GameObject("WebViewObject").AddComponent<WebViewObject>();
			webViewObject.Init(delegate(string msg)
			{
				Debug.Log(string.Format("CallFromJS[{0}]", msg));
			});
			webViewObject.SetMargins(num, num2, num, num2);
			loadWebView();
		}
	}

	private void loadWebView()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.IPhonePlayer:
		case RuntimePlatform.Android:
			webViewObject.LoadURL(URL.Replace(" ", "%20"));
			if (Application.platform != RuntimePlatform.Android)
			{
				webViewObject.EvaluateJS("window.addEventListener('load', function() {\twindow.Unity = {\t\tcall:function(msg) {\t\t\tvar iframe = document.createElement('IFRAME');\t\t\tiframe.setAttribute('src', 'unity:' + msg);\t\t\tdocument.documentElement.appendChild(iframe);\t\t\tiframe.parentNode.removeChild(iframe);\t\t\tiframe = null;\t\t}\t}}, false);");
			}
			break;
		}
	}
}
