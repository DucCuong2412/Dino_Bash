using System;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;

public class MixpanelExample : MonoBehaviour
{
	public GUISkin skin;

	private void OnGUI()
	{
		GUI.skin = skin;
		GUILayout.BeginArea(new Rect((float)Screen.width * 0.3f, (float)Screen.height * 0.45f, (float)Screen.width * 0.4f, (float)Screen.height * 0.5f));
		if (GUILayout.Button("Track"))
		{
			Mixpanel.Track("The Button Was Clicked");
		}
		if (GUILayout.Button("Engage"))
		{
			Mixpanel.people.Increment("clicks", 1);
		}
		if (GUILayout.Button("Test Exception"))
		{
			throw new AccessViolationException("This is an example exception");
		}
		GUILayout.EndArea();
	}

	private void Start()
	{
		Mixpanel.people.TrackCharge(0.42);
		Mixpanel.people.TrackChargeConverting(0.8, CurrencyHelper.getCurrencyCode());
		Dictionary<string, double> dictionary = new Dictionary<string, double>();
		dictionary.Add("EUR", 1.14);
		dictionary.Add("MXN", 0.061);
		Mixpanel.UpdateExchangeRates(dictionary);
		Mixpanel.Track("Hello From Unity");
		Value value = new Value();
		value["level"] = 84;
		value["coins"] = 99;
		value["health"] = 83.2f;
		value["bar"]["nested"]["value"] = 20.0;
		value["unicode"] = "€öäüß✓✓✓✓";
		Mixpanel.Track("event with parameters", value);
		Mixpanel.people.Set("gender", "male");
		Mixpanel.StartTimedEvent("time_it");
		Mixpanel.Track("time_it");
		Mixpanel.people.Name = "Tilo Tester";
		Mixpanel.people.Email = "tilo.tester@example.com";
		Mixpanel.FlushQueue();
	}
}
