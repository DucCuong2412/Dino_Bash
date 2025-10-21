using UnityEngine;

public class UnityAdsConfig
{
	private static UnityAdsConfig mInstance;

	private static AndroidJavaObject _androidBridge;

	private static readonly string AndroidBridge = "com.supersonic.unity.androidbridge.AndroidBridge";

	public static UnityAdsConfig Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new UnityAdsConfig();
			}
			return mInstance;
		}
	}

	public UnityAdsConfig()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(AndroidBridge))
		{
			_androidBridge = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		}
	}

	public void setGameId(string id)
	{
		_androidBridge.Call("setUnityAdsGameId", id);
	}

	public void setZoneId(string zoneId)
	{
		_androidBridge.Call("setUnityAdsZoneId", zoneId);
	}
}
