using UnityEngine;

namespace mixpanel
{
	public class CurrencyHelper
	{
		public static string getCurrencyCode()
		{
			//Discarded unreachable code: IL_005a, IL_006e, IL_0080, IL_0092
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.util.Locale"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getDefault", new object[0]))
				{
					using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("java.util.Currency"))
					{
						object[] args = new object[1] { androidJavaObject };
						using (AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", args))
						{
							return androidJavaObject2.Call<string>("getCurrencyCode", new object[0]);
						}
					}
				}
			}
		}
	}
}
