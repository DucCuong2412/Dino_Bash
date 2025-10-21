namespace LeanplumSDK.SocketIOClient.Messages
{
	internal class DisconnectMessage : Message
	{
		public override string Event
		{
			get
			{
				return "disconnect";
			}
		}

		public override string Encoded
		{
			get
			{
				return string.Format("0::{0}", Endpoint);
			}
		}

		public DisconnectMessage()
		{
			base.MessageType = SocketIOMessageTypes.Disconnect;
		}

		public DisconnectMessage(string endPoint)
			: this()
		{
			Endpoint = endPoint;
		}

		public static DisconnectMessage Deserialize(string rawMessage)
		{
			DisconnectMessage disconnectMessage = new DisconnectMessage();
			disconnectMessage.RawMessage = rawMessage;
			string[] array = rawMessage.Split(Message.SPLITCHARS, 3);
			if (array.Length == 3 && !string.IsNullOrEmpty(array[2]))
			{
				disconnectMessage.Endpoint = array[2];
			}
			return disconnectMessage;
		}
	}
}
