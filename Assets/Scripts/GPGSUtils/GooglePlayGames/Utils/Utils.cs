namespace GooglePlayGames.Utils
{
	public class Utils
	{
		public static bool BuffersAreIdentical(byte[] a, byte[] b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
