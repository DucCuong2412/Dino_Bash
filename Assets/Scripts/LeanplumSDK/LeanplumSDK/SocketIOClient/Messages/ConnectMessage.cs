namespace LeanplumSDK.SocketIOClient.Messages
{
	internal class ConnectMessage : Message
	{
		public string Query { get; private set; }

		public override string Event
		{
			get
			{
				return "connect";
			}
		}

		public override string Encoded
		{
			get
			{
				return string.Format("1::{0}{1}", Endpoint, (!string.IsNullOrEmpty(Query)) ? string.Format("?{0}", Query) : string.Empty);
			}
		}

		public ConnectMessage()
		{
			base.MessageType = SocketIOMessageTypes.Connect;
		}

		public ConnectMessage(string endPoint)
			: this()
		{
			Endpoint = endPoint;
		}

		public static ConnectMessage Deserialize(string rawMessage)
		{
			ConnectMessage connectMessage = new ConnectMessage();
			connectMessage.RawMessage = rawMessage;
			string[] array = rawMessage.Split(Message.SPLITCHARS, 3);
			if (array.Length == 3)
			{
				string[] array2 = array[2].Split('?');
				if (array2.Length > 0)
				{
					connectMessage.Endpoint = array2[0];
				}
				if (array2.Length > 1)
				{
					connectMessage.Query = array2[1];
				}
			}
			return connectMessage;
		}
	}
}
