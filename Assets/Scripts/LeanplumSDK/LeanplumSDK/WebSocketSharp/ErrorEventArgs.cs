using System;

namespace LeanplumSDK.WebSocketSharp
{
	internal class ErrorEventArgs : EventArgs
	{
		public string Message { get; private set; }

		internal ErrorEventArgs(string message)
		{
			Message = message;
		}
	}
}
