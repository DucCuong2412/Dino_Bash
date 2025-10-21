namespace LeanplumSDK
{
	public static class SocketUtilsFactory
	{
		private static ISocketUtils utils = new DisabledSocketUtils();

		public static ISocketUtils Utils
		{
			get
			{
				return utils;
			}
			set
			{
				utils = value;
			}
		}
	}
}
