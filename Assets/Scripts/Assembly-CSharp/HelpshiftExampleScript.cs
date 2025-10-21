using System;
using System.Collections.Generic;
using HSMiniJSON;
using Helpshift;
using UnityEngine;
using UnityEngine.UI;

public class HelpshiftExampleScript : MonoBehaviour
{
	private HelpshiftSdk _support;

	public void updateMetaData(string nothing)
	{
		Debug.Log("Update metadata ************************************************************");
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("user-level", "21");
		dictionary.Add("hs-tags", new string[1] { "Tag-1" });
		_support.updateMetaData(dictionary);
	}

	public void helpshiftSessionBegan(string message)
	{
		Debug.Log("Session Began ************************************************************");
	}

	public void helpshiftSessionEnded(string message)
	{
		Debug.Log("Session ended ************************************************************");
	}

	public void alertToRateAppAction(string result)
	{
		Debug.Log("User action on alert :" + result);
	}

	public void didReceiveNotificationCount(string count)
	{
		Debug.Log("Notification async count : " + count);
	}

	public void didReceiveInAppNotificationCount(string count)
	{
		Debug.Log("In-app Notification count : " + count);
	}

	public void newConversationStarted(string message)
	{
	}

	public void userRepliedToConversation(string newMessage)
	{
	}

	public void userCompletedCustomerSatisfactionSurvey(string json)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
		Debug.Log("Customer satisfaction information : " + dictionary);
	}

	private void Start()
	{
		_support = HelpshiftSdk.getInstance();
		_support.install();
		_support.login("identifier", "name", "email");
	}

	public void onShowFAQsClick()
	{
		Debug.Log("Show FAQs clicked !!");
		_support.showFAQs();
	}

	public void onShowDynamicClick()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("type", "conversationFlow");
		dictionary.Add("title", "Converse");
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2.Add("conversationPrefillText", "This is from dynamic");
		Dictionary<string, object> value = dictionary2;
		dictionary.Add("config", value);
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		dictionary3.Add("type", "faqsFlow");
		dictionary3.Add("title", "FAQs");
		Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
		dictionary4.Add("type", "faqSectionFlow");
		dictionary4.Add("title", "FAQ section");
		dictionary4.Add("data", "1509");
		Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
		dictionary5.Add("type", "singleFaqFlow");
		dictionary5.Add("title", "FAQ");
		dictionary5.Add("data", "2998");
		Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
		dictionary6.Add("type", "dynamicFormFlow");
		dictionary6.Add("title", "Next form");
		dictionary6.Add("data", new Dictionary<string, object>[4] { dictionary, dictionary3, dictionary4, dictionary5 });
		_support.showDynamicForm("This is a dynamic form", new Dictionary<string, object>[5] { dictionary, dictionary3, dictionary4, dictionary5, dictionary6 });
	}

	public void onShowConversationClick()
	{
		Debug.Log("Show Conversation clicked !!");
		_support.showConversation();
	}

	public void onShowFAQSectionClick()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("faq_section_id");
		InputField component = gameObject.GetComponent<InputField>();
		try
		{
			Convert.ToInt16(component.text);
			_support.showFAQSection(component.text);
		}
		catch (FormatException ex)
		{
			Debug.Log("Input string is not a sequence of digits : " + ex);
		}
	}

	public void onShowFAQClick()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("faq_id");
		InputField component = gameObject.GetComponent<InputField>();
		try
		{
			Convert.ToInt16(component.text);
			_support.showSingleFAQ(component.text);
		}
		catch (FormatException ex)
		{
			Debug.Log("Input string is not a sequence of digits : " + ex);
		}
	}

	public void onShowReviewReminderClick()
	{
		_support.showAlertToRateAppWithURL("market://details?id=com.RunnerGames.game.YooNinja_Lite");
	}

	public void onCampaignsTabClick()
	{
		Application.LoadLevel(1);
	}
}
