using System;
using System.Collections.Generic;
//using LeanplumSDK;
using UnityEngine;
using dinobash;
using mixpanel.platform;

public class LeanplumWrapper : MonoBehaviour
{
	public string AppID;

	public string ProductionKey;

	public string DevelopmentKey;

	public string AppVersion;

	public static LeanplumWrapper instance { get; private set; }

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		instance = this;
		//if (Application.isEditor)
		//{
		//	LeanplumFactory.SDK = new LeanplumNative();
		//}
		//else
		//{
		//	LeanplumFactory.SDK = new LeanplumAndroid();
		//	Leanplum.SetGcmSenderId(Leanplum.LeanplumGcmSenderId);
		//}
		//Leanplum.SetGcmSenderId(Leanplum.LeanplumGcmSenderId);
	}

	private Dictionary<string, object> getUserAttributes()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("user_id", MixpanelUnityPlatform.get_distinct_id());
		dictionary.Add("client_version", App.VERSION_CODE);
		dictionary.Add("install_date", Player.Instance.PlayerData.time_of_install.ToShortDateString());
		dictionary.Add("test_id", Player.Instance.splitTestingRandomId);
		dictionary.Add("test_group", Player.Instance.splitTestingGroupName);
		dictionary.Add("highest_level_reached", Player.MaxLevelID);
		return dictionary;
	}

	public static T get<T>(T default_value, params string[] list)
	{
		//Discarded unreachable code: IL_002c
		try
		{
			T result = default_value;
			//object obj = Leanplum.ObjectForKeyPath(list);
			//if (obj != null)
			//{
			//	result = (T)Convert.ChangeType(obj, typeof(T));
			//}
			return result;
		}
		catch
		{
			Debug.LogError("Leanplum - failed to cast type");
			return default_value;
		}
	}

	public static void Init()
	{
		if (!(instance == null))
		{
			//SocketUtilsFactory.Utils = new SocketUtils();
			if (!string.IsNullOrEmpty(App.VERSION_CODE))
			{
				//Leanplum.SetAppVersion(App.VERSION_CODE);
			}
			if (string.IsNullOrEmpty(instance.AppID) || string.IsNullOrEmpty(instance.ProductionKey) || string.IsNullOrEmpty(instance.DevelopmentKey))
			{
				Debug.LogError("Please make sure to enter your AppID, Production Key, and Development Key in the Leanplum GameObject inspector before starting.");
			}
			if (Debug.isDebugBuild)
			{
				//Leanplum.SetAppIdForDevelopmentMode(instance.AppID, instance.DevelopmentKey);
			}
			else
			{
				//Leanplum.SetAppIdForProductionMode(instance.AppID, instance.ProductionKey);
			}
			//Leanplum.Start(instance.getUserAttributes());
		}
	}
}
