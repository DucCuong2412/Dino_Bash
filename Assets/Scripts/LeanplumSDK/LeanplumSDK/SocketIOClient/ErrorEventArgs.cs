using System;

namespace LeanplumSDK.SocketIOClient
{
	internal class ErrorEventArgs : EventArgs
	{
		public string Message { get; set; }

		public string Exception { get; set; }

		public ErrorEventArgs(string message)
		{
			Message = message;
		}

		public ErrorEventArgs(string message, string exception)
		{
			Message = message;
			Exception = exception;
		}
	}
}
