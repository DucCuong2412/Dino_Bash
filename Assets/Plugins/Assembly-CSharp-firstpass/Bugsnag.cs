using System;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

public class Bugsnag : MonoBehaviour
{
	public class NativeBugsnag
	{
		public static AndroidJavaClass Bugsnag = new AndroidJavaClass("com.bugsnag.android.Bugsnag");

		public static Regex unityExpression = new Regex("(\\S+)\\s*\\(.*?\\)\\s*(?:(?:\\[.*\\]\\s*in\\s|\\(at\\s*\\s*)(.*):(\\d+))?", RegexOptions.IgnoreCase | RegexOptions.Multiline);

		public static void Register(string apiKey)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
			Bugsnag.CallStatic<AndroidJavaObject>("init", new object[2] { androidJavaObject, apiKey });
			Notify("errorClass", "error message", "error", new StackTrace(1, true).ToString(), true);
		}

		public static void Notify(string errorClass, string errorMessage, string severity, string stackTrace)
		{
			Notify(errorClass, errorMessage, severity, stackTrace, false);
		}

		public static void Notify(string errorClass, string errorMessage, string severity, string stackTrace, bool warmup)
		{
			ArrayList arrayList = new ArrayList();
			foreach (Match item in unityExpression.Matches(stackTrace))
			{
				string text = item.Groups[1].Value;
				string text2 = string.Empty;
				if (text == string.Empty)
				{
					text = "Unknown method";
				}
				else
				{
					int num = text.LastIndexOf(".");
					if (num >= 0)
					{
						text2 = text.Substring(0, num);
						text = text.Substring(num + 1);
					}
				}
				string text3 = item.Groups[2].Value;
				if (text3 == string.Empty || text3 == "<filename unknown>")
				{
					text3 = "unknown file";
				}
				string text4 = item.Groups[3].Value;
				if (text4 == string.Empty)
				{
					text4 = "0";
				}
				AndroidJavaObject value = new AndroidJavaObject("java.lang.StackTraceElement", text2, text, text3, Convert.ToInt32(text4));
				arrayList.Add(value);
			}
			if (arrayList.Count > 0 && !warmup)
			{
				IntPtr intPtr = AndroidJNI.NewObjectArray(arrayList.Count, ((AndroidJavaObject)arrayList[0]).GetRawClass(), ((AndroidJavaObject)arrayList[0]).GetRawObject());
				for (int i = 1; i < arrayList.Count; i++)
				{
					AndroidJNI.SetObjectArrayElement(intPtr, i, ((AndroidJavaObject)arrayList[i]).GetRawObject());
				}
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.bugsnag.android.Severity");
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("ERROR");
				if (severity == "info")
				{
					@static = androidJavaClass.GetStatic<AndroidJavaObject>("INFO");
				}
				else if (severity == "warning")
				{
					@static = androidJavaClass.GetStatic<AndroidJavaObject>("WARNING");
				}
				AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.bugsnag.android.MetaData");
				jvalue[] args = new jvalue[5]
				{
					new jvalue
					{
						l = AndroidJNI.NewStringUTF(errorClass)
					},
					new jvalue
					{
						l = AndroidJNI.NewStringUTF(errorMessage)
					},
					new jvalue
					{
						l = intPtr
					},
					new jvalue
					{
						l = @static.GetRawObject()
					},
					new jvalue
					{
						l = androidJavaObject.GetRawObject()
					}
				};
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(Bugsnag.GetRawClass(), "notify", "(Ljava/lang/String;Ljava/lang/String;[Ljava/lang/StackTraceElement;Lcom/bugsnag/android/Severity;Lcom/bugsnag/android/MetaData;)V");
				if (!warmup)
				{
					AndroidJNI.CallStaticObjectMethod(Bugsnag.GetRawClass(), staticMethodID, args);
				}
			}
		}

		public static void SetNotifyUrl(string notifyUrl)
		{
			Bugsnag.CallStatic("setEndpoint", notifyUrl);
		}

		public static void SetAutoNotify(bool autoNotify)
		{
			if (autoNotify)
			{
				Bugsnag.CallStatic("enableExceptionHandler");
			}
			else
			{
				Bugsnag.CallStatic("disableExceptionHandler");
			}
		}

		public static void SetContext(string context)
		{
			Bugsnag.CallStatic("setContext", context);
		}

		public static void SetReleaseStage(string releaseStage)
		{
			Bugsnag.CallStatic("setReleaseStage", releaseStage);
		}

		public static void SetNotifyReleaseStages(string releaseStages)
		{
			Bugsnag.CallStatic("setNotifyReleaseStages", releaseStages.Split(','));
		}

		public static void AddToTab(string tabName, string attributeName, string attributeValue)
		{
			Bugsnag.CallStatic("addToTab", tabName, attributeName, attributeValue);
		}

		public static void ClearTab(string tabName)
		{
			Bugsnag.CallStatic("clearTab", tabName);
		}
	}

	public enum LogSeverity
	{
		Log = 0,
		Warning = 1,
		Assert = 2,
		Error = 3,
		Exception = 4
	}

	public string BugsnagApiKey = string.Empty;

	public bool AutoNotify = true;

	public LogSeverity NotifyLevel = LogSeverity.Exception;

	public static string UserId
	{
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			NativeBugsnag.AddToTab("user", "id", value);
		}
	}

	public static string ReleaseStage
	{
		set
		{
			if (value == null)
			{
				value = "production";
			}
			NativeBugsnag.SetReleaseStage(value);
		}
	}

	public static string Context
	{
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			NativeBugsnag.SetContext(value);
		}
	}

	public static string NotifyUrl
	{
		set
		{
			if (value == null)
			{
				value = "https://notify.bugsnag.com/";
			}
			NativeBugsnag.SetNotifyUrl(value);
		}
	}

	public static string[] NotifyReleaseStages
	{
		set
		{
			NativeBugsnag.SetNotifyReleaseStages(string.Join(",", value));
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		NativeBugsnag.Register(BugsnagApiKey);
		if (UnityEngine.Debug.isDebugBuild)
		{
			ReleaseStage = "development";
		}
		else
		{
			ReleaseStage = "production";
		}
		Context = Application.loadedLevelName;
		NativeBugsnag.SetAutoNotify(AutoNotify);
	}

	private void OnEnable()
	{
		Application.RegisterLogCallback(HandleLog);
	}

	private void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	private void OnLevelWasLoaded(int level)
	{
		Context = Application.loadedLevelName;
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		LogSeverity logSeverity = LogSeverity.Exception;
		string severity = "error";
		switch (type)
		{
		case LogType.Assert:
			logSeverity = LogSeverity.Assert;
			break;
		case LogType.Error:
			logSeverity = LogSeverity.Error;
			break;
		case LogType.Exception:
			logSeverity = LogSeverity.Exception;
			break;
		case LogType.Log:
			logSeverity = LogSeverity.Log;
			severity = "info";
			break;
		case LogType.Warning:
			logSeverity = LogSeverity.Warning;
			severity = "warning";
			break;
		}
		if (logSeverity >= NotifyLevel && AutoNotify)
		{
			string message = string.Empty;
			Regex regex = new Regex("^(?<errorClass>\\S+):\\s*(?<message>.*)");
			Match match = regex.Match(logString);
			string errorClass;
			if (match.Success)
			{
				errorClass = match.Groups["errorClass"].Value;
				message = match.Groups["message"].Value.Trim();
			}
			else
			{
				errorClass = logString;
			}
			if (stackTrace == null || stackTrace == string.Empty)
			{
				stackTrace = new StackTrace(1, true).ToString();
			}
			NotifySafely(errorClass, message, severity, stackTrace);
		}
	}

	public static void Notify(Exception e)
	{
		if (e != null)
		{
			string text = e.StackTrace;
			if (text == null)
			{
				text = new StackTrace(1, true).ToString();
			}
			NotifySafely(e.GetType().ToString(), e.Message, "warning", text);
		}
	}

	public static void AddToTab(string tabName, string attributeName, string attributeValue)
	{
		if (tabName != null && attributeName != null)
		{
			if (attributeValue == null)
			{
				attributeValue = "null";
			}
			NativeBugsnag.AddToTab(tabName, attributeName, attributeValue);
		}
	}

	public static void ClearTab(string tabName)
	{
		if (tabName != null)
		{
			NativeBugsnag.ClearTab(tabName);
		}
	}

	private static void NotifySafely(string errorClass, string message, string severity, string stackTrace)
	{
		if (errorClass == null)
		{
			errorClass = "Error";
		}
		if (message == null)
		{
			message = string.Empty;
		}
		if (severity == null)
		{
			severity = "error";
		}
		if (stackTrace != null)
		{
			NativeBugsnag.Notify(errorClass, message, severity, stackTrace);
		}
	}
}
