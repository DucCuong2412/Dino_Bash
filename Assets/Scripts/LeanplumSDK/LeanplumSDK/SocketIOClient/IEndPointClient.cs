using System;
using LeanplumSDK.SocketIOClient.Messages;

namespace LeanplumSDK.SocketIOClient
{
	internal interface IEndPointClient
	{
		void On(string eventName, Action<IMessage> action);

		void Emit(string eventName, object payload, Action<object> callBack);

		void Send(IMessage msg);
	}
}
