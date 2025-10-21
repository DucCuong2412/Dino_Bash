using System;
using System.Text;
using LeanplumSDK.Prefs;
using UnityEngine;

namespace LeanplumSDK
{
	internal class UnityCompatibilityLayer : ICompatibilityLayer
	{
		public bool LeanplumDeveloperMode { get; set; }

		public string VersionName { get; set; }

		public void Init()
		{
			if (LeanplumUnityHelper.Instance == null)
			{
				LogWarning("Unable to listen for app lifecycle callbacks");
			}
		}

		public void LogDebug(string message)
		{
			if (LeanplumDeveloperMode)
			{
				Debug.Log(message);
			}
		}

		public void Log(string message)
		{
			Debug.Log("Leanplum: " + message);
		}

		public void LogWarning(string message)
		{
			if (!LeanplumNative.isStopped)
			{
				Debug.LogWarning("Leanplum Warning: " + message);
			}
		}

		public void LogError(string message)
		{
			if (!LeanplumNative.isStopped)
			{
				Debug.LogError("Leanplum Error: " + message);
			}
		}

		public void LogError(Exception error)
		{
			LogError(error.ToString());
			LogError(error.StackTrace);
		}

		public void LogError(string message, Exception error)
		{
			LogError(message);
			LogError(error);
		}

		public void DisplayModal(string title, string message)
		{
			LeanplumUnityHelper.Instance.DisplayMessageModal(title, message);
		}

		public void DisplayTextInputModal(string title, string message, Action<string> callback)
		{
			LeanplumUnityHelper.Instance.DisplayTextModal(title, message, callback);
		}

		public string GetSavedString(string key, string defaultValue = null)
		{
			return LeanplumSDK.Prefs.PlayerPrefs.GetString(key, defaultValue);
		}

		public int GetSavedInt(string key, int defaultValue = 0)
		{
			return LeanplumSDK.Prefs.PlayerPrefs.GetInt(key, defaultValue);
		}

		public void StoreSavedString(string key, string val)
		{
			LeanplumSDK.Prefs.PlayerPrefs.SetString(key, val);
		}

		public void StoreSavedInt(string key, int val)
		{
			LeanplumSDK.Prefs.PlayerPrefs.SetInt(key, val);
		}

		public void DeleteSavedSetting(string key)
		{
			LeanplumSDK.Prefs.PlayerPrefs.DeleteKey(key);
		}

		public void FlushSavedSettings()
		{
			LeanplumSDK.Prefs.PlayerPrefs.Flush();
		}

		public WebRequest CreateWebRequest(string url, int timeout)
		{
			return new UnityWebRequest(url, timeout);
		}

		public string URLEncode(string str)
		{
			return WWW.EscapeURL(str, Encoding.UTF8);
		}

		public string GetDeviceName()
		{
			if (GetPlatformName() == "iOS" || GetPlatformName() == "Android")
			{
				return GetDeviceModel();
			}
			return SystemInfo.deviceName;
		}

		public string GetDeviceId()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public string GetDeviceModel()
		{
			return Util.Capitalize(SystemInfo.deviceModel);
		}

		public string GetSystemName()
		{
			if (IsSimulator())
			{
				return "Unity Editor";
			}
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				return "Android OS";
			case RuntimePlatform.IPhonePlayer:
				return "iPhone OS";
			case RuntimePlatform.OSXPlayer:
				return "Unity Standalone Mac OS";
			case RuntimePlatform.WindowsPlayer:
				return "Unity Standalone Windows";
			case RuntimePlatform.LinuxPlayer:
				return "Unity Standalone Linux";
			default:
				return "Unknown";
			}
		}

		public string GetPlatformName()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.Android:
				return "Android";
			default:
				return "Standalone";
			}
		}

		public string GetSystemVersion()
		{
			string[] array = SystemInfo.operatingSystem.Split(' ');
			if (GetPlatformName() == "Android")
			{
				return array[(array.Length <= 2) ? (array.Length - 1) : 2];
			}
			return array[array.Length - 1];
		}

		public bool IsSimulator()
		{
			return Application.isEditor;
		}

		public bool IsConnected()
		{
			return Application.internetReachability != NetworkReachability.NotReachable;
		}
	}
}
