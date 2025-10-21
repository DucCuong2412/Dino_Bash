using System;
using LeanplumSDK.SocketIOClient.Messages;

namespace LeanplumSDK.SocketIOClient
{
	internal class MessageEventArgs : EventArgs
	{
		public IMessage Message { get; private set; }

		public MessageEventArgs(IMessage msg)
		{
			Message = msg;
		}
	}
}
