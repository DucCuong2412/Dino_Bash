namespace LeanplumSDK.SocketIOClient.Messages
{
	internal class TextMessage : Message
	{
		private string eventName = "message";

		public override string Event
		{
			get
			{
				return eventName;
			}
		}

		public TextMessage()
		{
			base.MessageType = SocketIOMessageTypes.Message;
		}

		public TextMessage(string textMessage)
			: this()
		{
			base.MessageText = textMessage;
		}

		public static TextMessage Deserialize(string rawMessage)
		{
			TextMessage textMessage = new TextMessage();
			textMessage.RawMessage = rawMessage;
			string[] array = rawMessage.Split(Message.SPLITCHARS, 4);
			if (array.Length == 4)
			{
				int result;
				if (int.TryParse(array[1], out result))
				{
					textMessage.AckId = result;
				}
				textMessage.Endpoint = array[2];
				textMessage.MessageText = array[3];
			}
			else
			{
				textMessage.MessageText = rawMessage;
			}
			return textMessage;
		}
	}
}
