namespace LeanplumSDK.SocketIOClient.Messages
{
	internal class ErrorMessage : Message
	{
		public string Reason { get; set; }

		public string Advice { get; set; }

		public override string Event
		{
			get
			{
				return "error";
			}
		}

		public ErrorMessage()
		{
			base.MessageType = SocketIOMessageTypes.Error;
		}

		public static ErrorMessage Deserialize(string rawMessage)
		{
			ErrorMessage errorMessage = new ErrorMessage();
			string[] array = rawMessage.Split(':');
			if (array.Length == 4)
			{
				errorMessage.Endpoint = array[2];
				errorMessage.MessageText = array[3];
				string[] array2 = array[3].Split('+');
				if (array2.Length > 1)
				{
					errorMessage.Advice = array2[1];
					errorMessage.Reason = array2[0];
				}
			}
			return errorMessage;
		}
	}
}
