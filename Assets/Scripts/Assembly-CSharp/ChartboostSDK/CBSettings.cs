using UnityEngine;

namespace ChartboostSDK
{
	public class CBSettings : ScriptableObject
	{
		private const string cbSettingsAssetName = "ChartboostSettings";

		private const string cbSettingsPath = "Chartboost/Resources";

		private const string cbSettingsAssetExtension = ".asset";

		private static CBSettings instance;

		[SerializeField]
		public string iOSAppId = "0";

		[SerializeField]
		public string iOSAppSecret = "0";

		[SerializeField]
		public string androidAppId = "0";

		[SerializeField]
		public string androidAppSecret = "0";

		[SerializeField]
		public string amazonAppId = "0";

		[SerializeField]
		public string amazonAppSecret = "0";

		[SerializeField]
		public bool isLoggingEnabled;

		[SerializeField]
		public string[] androidPlatformLabels = new string[2] { "Google Play", "Amazon" };

		[SerializeField]
		public int selectedAndroidPlatformIndex;

		private static CBSettings Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load("ChartboostSettings") as CBSettings;
					if (instance == null)
					{
						instance = ScriptableObject.CreateInstance<CBSettings>();
					}
				}
				return instance;
			}
		}

		public int SelectedAndroidPlatformIndex
		{
			get
			{
				return selectedAndroidPlatformIndex;
			}
		}

		public string[] AndroidPlatformLabels
		{
			get
			{
				return androidPlatformLabels;
			}
			set
			{
				if (androidPlatformLabels != value)
				{
					androidPlatformLabels = value;
					DirtyEditor();
				}
			}
		}

		public void SetAndroidPlatformIndex(int index)
		{
			if (selectedAndroidPlatformIndex != index)
			{
				selectedAndroidPlatformIndex = index;
				DirtyEditor();
			}
		}

		public void SetIOSAppId(string id)
		{
			if (Instance.iOSAppId != id)
			{
				Instance.iOSAppId = id;
				DirtyEditor();
			}
		}

		public static string getIOSAppId()
		{
			return Instance.iOSAppId;
		}

		public void SetIOSAppSecret(string secret)
		{
			if (Instance.iOSAppSecret != secret)
			{
				Instance.iOSAppSecret = secret;
				DirtyEditor();
			}
		}

		public static string getIOSAppSecret()
		{
			return Instance.iOSAppSecret;
		}

		public void SetAndroidAppId(string id)
		{
			if (Instance.androidAppId != id)
			{
				Instance.androidAppId = id;
				DirtyEditor();
			}
		}

		public static string getAndroidAppId()
		{
			return Instance.androidAppId;
		}

		public void SetAndroidAppSecret(string secret)
		{
			if (Instance.androidAppSecret != secret)
			{
				Instance.androidAppSecret = secret;
				DirtyEditor();
			}
		}

		public static string getAndroidAppSecret()
		{
			return Instance.androidAppSecret;
		}

		public void SetAmazonAppId(string id)
		{
			if (Instance.amazonAppId != id)
			{
				Instance.amazonAppId = id;
				DirtyEditor();
			}
		}

		public static string getAmazonAppId()
		{
			return Instance.amazonAppId;
		}

		public void SetAmazonAppSecret(string secret)
		{
			if (Instance.amazonAppSecret != secret)
			{
				Instance.amazonAppSecret = secret;
				DirtyEditor();
			}
		}

		public static string getAmazonAppSecret()
		{
			return Instance.amazonAppSecret;
		}

		public static string getSelectAndroidAppId()
		{
			if (Instance.selectedAndroidPlatformIndex == 0)
			{
				return Instance.androidAppId;
			}
			return Instance.amazonAppId;
		}

		public static string getSelectAndroidAppSecret()
		{
			if (Instance.selectedAndroidPlatformIndex == 0)
			{
				return Instance.androidAppSecret;
			}
			return Instance.amazonAppSecret;
		}

		public static void enableLogging(bool enabled)
		{
			Instance.isLoggingEnabled = enabled;
		}

		public static bool isLogging()
		{
			return Instance.isLoggingEnabled;
		}

		private static void DirtyEditor()
		{
		}
	}
}
