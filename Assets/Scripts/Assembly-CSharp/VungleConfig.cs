using UnityEngine;

public class VungleConfig
{
	private static VungleConfig mInstance;

	private static AndroidJavaObject _androidBridge;

	private static readonly string AndroidBridge = "com.supersonic.unity.androidbridge.AndroidBridge";

	public static VungleConfig Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new VungleConfig();
			}
			return mInstance;
		}
	}

	public VungleConfig()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(AndroidBridge))
		{
			_androidBridge = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		}
	}

	public void setAppId(string id)
	{
		_androidBridge.Call("setVungleAppId", id);
	}
}
