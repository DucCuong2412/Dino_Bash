using UnityEngine;

public class AdColonyConfig
{
	private static AdColonyConfig mInstance;

	private static AndroidJavaObject _androidBridge;

	private static readonly string AndroidBridge = "com.supersonic.unity.androidbridge.AndroidBridge";

	public static AdColonyConfig Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new AdColonyConfig();
			}
			return mInstance;
		}
	}

	public AdColonyConfig()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(AndroidBridge))
		{
			_androidBridge = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		}
	}

	public void setClientOptions(string co)
	{
		_androidBridge.Call("setAdColonyClientOptions", co);
	}

	public void setAppId(string id)
	{
		_androidBridge.Call("setAdColonyAppId", id);
	}

	public void setAdColonyDeviceID(string id)
	{
		_androidBridge.Call("setAdColonyDeviceID", id);
	}

	public void setCustomID(string id)
	{
		_androidBridge.Call("setAdColonyCustomID", id);
	}

	public void setZoneID(string zoneId)
	{
		_androidBridge.Call("setAdColonyZoneID", zoneId);
	}
}
