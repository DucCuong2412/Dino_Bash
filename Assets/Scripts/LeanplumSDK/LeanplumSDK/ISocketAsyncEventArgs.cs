using System;

namespace LeanplumSDK
{
	public interface ISocketAsyncEventArgs
	{
		object UserToken { get; set; }

		ISocket AcceptSocket { get; set; }

		event EventHandler<EventArgs> Completed;

		bool IsSuccess();
	}
}
