using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Apsalar : MonoBehaviour
{
	private enum NSType
	{
		STRING = 0,
		INT = 1,
		LONG = 2,
		FLOAT = 3,
		DOUBLE = 4,
		NULL = 5,
		ARRAY = 6,
		DICTIONARY = 7
	}

	private static AndroidJavaClass jniApsalar;

	private static AndroidJavaClass jniApsalarUnityAuxiliaryFunctions;

	private static AndroidJavaObject jniCurrentActivity;

	private static Apsalar instance;

	private static bool initialized;

	public string APIkey;

	public string secret;

	public bool autoIAPComplete = true;

	public bool batchEvents;

	public bool endSessionOnGoingToBackground;

	public bool restartSessionOnReturningToForeground;

	public static bool StartApsalarSession(string key, string secret)
	{
		if (!Application.isEditor)
		{
			jniApsalar.CallStatic("startSession", jniCurrentActivity, key, secret);
			return true;
		}
		return false;
	}

	public static void RestartApsalarSession(string key, string secret)
	{
		if (!Application.isEditor)
		{
			jniApsalar.CallStatic("restartSession", jniCurrentActivity, key, secret);
		}
	}

	public static void EndApsalarSession()
	{
		if (!Application.isEditor)
		{
			jniApsalar.CallStatic("endSession");
		}
	}

	public static void SendEvent(string name)
	{
		if (!Application.isEditor)
		{
			jniApsalar.CallStatic("event", name);
		}
	}

	public static void SendEvent(Dictionary<string, object> args, string name)
	{
		if (!Application.isEditor)
		{
			jniApsalarUnityAuxiliaryFunctions.CallStatic("eventJSON", name, ConvertToJSON(args));
		}
	}

	public static void SendEvent(string name, params object[] args)
	{
		if (!Application.isEditor && args.Length % 2 == 0)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			for (int i = 0; i < args.Length; i += 2)
			{
				dictionary.Add(args[i].ToString(), args[i + 1]);
			}
			SendEvent(dictionary, name);
		}
	}

	public static void SetAllowBatchesEvents(bool allowed)
	{
		if (Application.isEditor)
		{
			Debug.Log("Batches are not supported for Android");
		}
	}

	public static void SetBatchInterval(int interval)
	{
		if (Application.isEditor)
		{
			Debug.Log("Batches are not supported for Android");
		}
	}

	public static void SendAllBatches()
	{
	}

	public static void SetAge(int age)
	{
		if (Mathf.Clamp(age, 0, 100) != age)
		{
			Debug.Log("Age " + age + "is not between 0 and 100");
		}
		else if (!Application.isEditor)
		{
			jniApsalar.CallStatic("setAge", age);
		}
	}

	public static void SetGender(string gender)
	{
		if (gender != "m" && gender != "f")
		{
			Debug.Log("gender " + gender + "is not m or f");
		}
		else if (!Application.isEditor)
		{
			jniApsalar.CallStatic("setGender", gender);
		}
	}

	public static void SetAllowAutoIAPComplete(bool allowed)
	{
		if (Application.isEditor)
		{
			Debug.Log("SetAllowAutoIAPComplete is not supported on Android");
		}
	}

	private void Awake()
	{
		if (!initialized)
		{
			instance = this;
			initialized = true;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (!Application.isEditor)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				jniCurrentActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				jniApsalar = new AndroidJavaClass("com.apsalar.sdk.Apsalar");
				jniApsalarUnityAuxiliaryFunctions = new AndroidJavaClass("com.apsalar.sdk.unity3d.ApsalarUnityAuxiliaryFunctions");
			}
			if (!Application.isEditor)
			{
				StartApsalarSession(APIkey, secret);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			if (!Application.isEditor && endSessionOnGoingToBackground)
			{
				EndApsalarSession();
			}
		}
		else if (!Application.isEditor && restartSessionOnReturningToForeground)
		{
			RestartApsalarSession(APIkey, secret);
		}
	}

	private void OnApplicationQuit()
	{
		EndApsalarSession();
	}

	private void Update()
	{
	}

	private static string ConvertToJSON(ArrayList args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (object arg in args)
		{
			if (flag)
			{
				flag = false;
				stringBuilder.Append("[");
			}
			else
			{
				stringBuilder.Append(",");
			}
			if (arg == null)
			{
				stringBuilder.Append("null");
				continue;
			}
			Type type = arg.GetType();
			if (type == typeof(int) || type == typeof(long))
			{
				stringBuilder.Append(arg.ToString());
			}
			else if (type == typeof(float) || type == typeof(double))
			{
				stringBuilder.Append(string.Format("{0:F8}", arg));
			}
			else if (type == typeof(Dictionary<string, object>))
			{
				stringBuilder.Append(ConvertToJSON((Dictionary<string, object>)arg));
			}
			else if (type == typeof(ArrayList))
			{
				stringBuilder.Append(ConvertToJSON((ArrayList)arg));
			}
			else if (type == typeof(string))
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(arg);
				stringBuilder.Append("\"");
			}
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	private static string ConvertToJSON(Dictionary<string, object> args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		Dictionary<string, object>.Enumerator enumerator = args.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (flag)
			{
				flag = false;
				stringBuilder.Append("{");
			}
			else
			{
				stringBuilder.Append(",");
			}
			if (enumerator.Current.Value == null)
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(enumerator.Current.Key);
				stringBuilder.Append("\":null");
				continue;
			}
			Type type = enumerator.Current.Value.GetType();
			stringBuilder.Append("\"");
			stringBuilder.Append(enumerator.Current.Key);
			stringBuilder.Append("\":");
			if (type == typeof(int) || type == typeof(long))
			{
				stringBuilder.Append(enumerator.Current.Value.ToString());
			}
			else if (type == typeof(float) || type == typeof(double))
			{
				stringBuilder.Append(string.Format("{0:F8}", enumerator.Current.Value));
			}
			else if (type == typeof(Dictionary<string, object>))
			{
				stringBuilder.Append(ConvertToJSON((Dictionary<string, object>)enumerator.Current.Value));
			}
			else if (type == typeof(ArrayList))
			{
				stringBuilder.Append(ConvertToJSON((ArrayList)enumerator.Current.Value));
			}
			else if (type == typeof(string))
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(enumerator.Current.Value);
				stringBuilder.Append("\"");
			}
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public static void SetFBAppId(string s)
	{
		if (!Application.isEditor)
		{
			jniApsalar.CallStatic("setFBAppId", s);
		}
	}

	public static string GetAPID()
	{
		return null;
	}
}
