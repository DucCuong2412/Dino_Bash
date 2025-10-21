using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
	private void Start()
	{
		Apsalar.SendEvent("event on start");
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height / 40, Screen.width / 2, Screen.height / 7), "Send simple event"))
		{
			Apsalar.SendEvent("Simple event");
			Debug.Log("Apsalar app id:" + Apsalar.GetAPID());
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 2 / 10 + Screen.height / 40, Screen.width / 2, Screen.height / 7), "Send event with list of arguments"))
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add("arg 3 value 1");
			arrayList.Add("arg 3 value 2");
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("arg 4 name 1", "arg 4 value 1");
			dictionary.Add("arg 4 name 2", 2.5);
			ArrayList arrayList2 = new ArrayList();
			arrayList2.Add("arg 4c value 1");
			arrayList2.Add("arg 4c value 2");
			dictionary.Add("arg 4C", arrayList2);
			Apsalar.SendEvent("Event with list of arguments", "arg 1", "arg 1 value", "arg 2", 2, "arg 3", arrayList, "arg 4", dictionary);
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 4 / 10 + Screen.height / 40, Screen.width / 2, Screen.height / 7), "Send event with dictionary of arguments"))
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("arg 1 name 1", "arg 1 value 1");
			dictionary2.Add("arg 1 name 2", 2.5);
			ArrayList arrayList3 = new ArrayList();
			arrayList3.Add("arg 1c value 1");
			arrayList3.Add("arg 1c value 2");
			dictionary2.Add("arg 1C", arrayList3);
			Apsalar.SendEvent(dictionary2, "Event with dictionary of arguments.");
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 6 / 10 + Screen.height / 40, Screen.width / 2, Screen.height / 7), "Set age to 20"))
		{
			Apsalar.SetAge(20);
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 8 / 10 + Screen.height / 40, Screen.width / 2, Screen.height / 7), "Set gender to m"))
		{
			Apsalar.SetGender("m");
		}
	}
}
