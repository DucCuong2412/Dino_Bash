using Helpshift;
using UnityEngine;
using mixpanel.platform;

public class HelpShiftWapper : MonoBehaviour
{
	private HelpshiftSdk help;

	public static HelpShiftWapper instance { get; private set; }

	public void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		instance = this;
		help = HelpshiftSdk.getInstance();
		help.install();
		help.setUserIdentifier(MixpanelUnityPlatform.get_distinct_id());
	}

	public void ShowFAQ()
	{
		Tracking.show_faq();
		Debug.Log("Helpshift:showing faq");
		help.showFAQs();
	}

	public void helpshiftSessionBegan(string message)
	{
		Debug.Log("Session Began ************************************************************");
	}

	public void helpshiftSessionEnded(string message)
	{
		Debug.Log("Session ended ************************************************************");
	}
}
