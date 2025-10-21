using UnityEngine;

public class ImmersiveModeEnabler : MonoBehaviour
{
	private AndroidJavaObject unityActivity;

	private AndroidJavaObject javaObj;

	private AndroidJavaClass javaClass;

	private bool paused;

	private static bool created;

	private void Awake()
	{
		if (!Application.isEditor)
		{
			HideNavigationBar();
		}
		if (!created)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			created = true;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void HideNavigationBar()
	{
		lock (this)
		{
			using (javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				unityActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			if (unityActivity == null)
			{
				return;
			}
			using (javaClass = new AndroidJavaClass("com.rak24.androidimmersivemode.Main"))
			{
				if (javaClass == null)
				{
					return;
				}
				javaObj = javaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
				if (javaObj != null)
				{
					unityActivity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
					{
						javaObj.Call("EnableImmersiveMode", unityActivity);
					});
				}
			}
		}
	}

	private void OnApplicationPause(bool pausedState)
	{
		paused = pausedState;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus && javaObj != null && !paused)
		{
			unityActivity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				javaObj.CallStatic("ImmersiveModeFromCache", unityActivity);
			});
		}
	}

	public void PinThisApp()
	{
		if (javaObj != null)
		{
			javaObj.CallStatic("EnableAppPin", unityActivity);
		}
	}

	public void UnPinThisApp()
	{
		if (javaObj != null)
		{
			javaObj.CallStatic("DisableAppPin", unityActivity);
		}
	}
}
