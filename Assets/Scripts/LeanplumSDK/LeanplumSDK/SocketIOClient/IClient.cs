using System;
using LeanplumSDK.SocketIOClient.Messages;
using LeanplumSDK.WebSocketSharp;

namespace LeanplumSDK.SocketIOClient
{
	internal interface IClient
	{
		SocketIOHandshake HandShake { get; }

		bool IsConnected { get; }

		WebSocketState ReadyState { get; }

		event EventHandler Opened;

		event EventHandler<MessageEventArgs> Message;

		event EventHandler SocketConnectionClosed;

		event EventHandler<ErrorEventArgs> Error;

		void Connect();

		IEndPointClient Connect(string endPoint);

		void Close();

		void Dispose();

		void On(string eventName, Action<IMessage> action);

		void On(string eventName, string endPoint, Action<IMessage> action);

		void Emit(string eventName, object payload);

		void Emit(string eventName, object payload, string endPoint, Action<object> callBack);

		void Send(IMessage msg);
	}
}
