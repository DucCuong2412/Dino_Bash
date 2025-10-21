using System;

namespace LeanplumSDK
{
	public class LeanplumException : Exception
	{
		public LeanplumException(string message)
			: base(message)
		{
		}
	}
}
