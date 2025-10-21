namespace LeanplumSDK
{
	public class LeanplumFactory
	{
		private static LeanplumSDKObject _sdk;

		public static LeanplumSDKObject SDK
		{
			get
			{
				return _sdk;
			}
			set
			{
				_sdk = value;
			}
		}
	}
}
