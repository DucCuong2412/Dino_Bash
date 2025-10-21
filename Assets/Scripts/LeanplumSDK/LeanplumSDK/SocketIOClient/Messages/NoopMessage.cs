namespace LeanplumSDK.SocketIOClient.Messages
{
	internal class NoopMessage : Message
	{
		public NoopMessage()
		{
			base.MessageType = SocketIOMessageTypes.Noop;
		}

		public static NoopMessage Deserialize(string rawMessage)
		{
			return new NoopMessage();
		}
	}
}
