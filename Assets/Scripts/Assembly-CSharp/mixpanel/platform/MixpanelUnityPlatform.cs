using UnityEngine;

namespace mixpanel.platform
{
	public class MixpanelUnityPlatform
	{
		public static string get_android_advertising_id()
		{
			//Discarded unreachable code: IL_005f, IL_006e
			try
			{
				string empty = string.Empty;
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
				AndroidJavaObject androidJavaObject = androidJavaClass2.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", new object[1] { @static });
				return androidJavaObject.Call<string>("getId", new object[0]).ToString();
			}
			catch (AndroidJavaException)
			{
				return null;
			}
		}

		public static string get_android_id()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]);
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.provider.Settings$Secure");
			return androidJavaClass2.CallStatic<string>("getString", new object[2] { androidJavaObject, "android_id" });
		}

		public static string get_android_version_name()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getPackageManager", new object[0]);
			string text = @static.Call<string>("getPackageName", new object[0]);
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageInfo", new object[2] { text, 0 });
			return androidJavaObject2.Get<string>("versionName");
		}

		public static int get_android_version_code()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getPackageManager", new object[0]);
			string text = @static.Call<string>("getPackageName", new object[0]);
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageInfo", new object[2] { text, 0 });
			return androidJavaObject2.Get<int>("versionCode");
		}

		public static string get_distinct_id()
		{
			string android_advertising_id = get_android_advertising_id();
			if (string.IsNullOrEmpty(android_advertising_id))
			{
				Debug.Log("Android Advertising ID not available, using ANDROID_ID");
				return get_android_id();
			}
			return android_advertising_id;
		}

		public static string get_storage_directory()
		{
			return Application.persistentDataPath;
		}
	}
}
