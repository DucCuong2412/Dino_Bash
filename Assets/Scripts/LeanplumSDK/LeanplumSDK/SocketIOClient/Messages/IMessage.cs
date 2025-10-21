using System;

namespace LeanplumSDK.SocketIOClient.Messages
{
	internal interface IMessage
	{
		SocketIOMessageTypes MessageType { get; }

		string RawMessage { get; }

		string Event { get; }

		int? AckId { get; }

		string Endpoint { get; set; }

		string MessageText { get; }

		JsonEncodedEventMessage Json { get; }

		[Obsolete(".JsonEncodedMessage has been deprecated. Please use .Json instead.")]
		JsonEncodedEventMessage JsonEncodedMessage { get; }

		string Encoded { get; }
	}
}
