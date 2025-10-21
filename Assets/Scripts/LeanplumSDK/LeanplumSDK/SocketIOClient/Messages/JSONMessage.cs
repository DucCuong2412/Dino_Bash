using System;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK.SocketIOClient.Messages
{
	internal class JSONMessage : Message
	{
		public JSONMessage()
		{
			base.MessageType = SocketIOMessageTypes.JSONMessage;
		}

		public JSONMessage(object jsonObject)
			: this()
		{
			base.MessageText = LeanplumSDK.MiniJSON.Json.Serialize(jsonObject);
		}

		public JSONMessage(object jsonObject, int? ackId, string endpoint)
			: this()
		{
			base.AckId = ackId;
			Endpoint = endpoint;
			base.MessageText = LeanplumSDK.MiniJSON.Json.Serialize(jsonObject);
		}

		public void SetMessage(object value)
		{
			base.MessageText = LeanplumSDK.MiniJSON.Json.Serialize(value);
		}

		public virtual T Message<T>()
		{
			//Discarded unreachable code: IL_0016, IL_001e
			try
			{
				return (T)LeanplumSDK.MiniJSON.Json.Deserialize(MessageText);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static JSONMessage Deserialize(string rawMessage)
		{
			JSONMessage jSONMessage = new JSONMessage();
			jSONMessage.RawMessage = rawMessage;
			string[] array = rawMessage.Split(LeanplumSDK.SocketIOClient.Messages.Message.SPLITCHARS, 4);
			if (array.Length == 4)
			{
				int result;
				if (int.TryParse(array[1], out result))
				{
					jSONMessage.AckId = result;
				}
				jSONMessage.Endpoint = array[2];
				jSONMessage.MessageText = array[3];
			}
			return jSONMessage;
		}
	}
}
