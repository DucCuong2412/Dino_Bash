using System;

namespace LeanplumSDK.SocketIOClient.Messages
{
	internal class EventMessage : Message
	{
		private static object ackLock = new object();

		private static int _akid = 0;

		public Action<object> Callback;

		private static int NextAckID
		{
			get
			{
				//Discarded unreachable code: IL_0034
				lock (ackLock)
				{
					_akid++;
					if (_akid < 0)
					{
						_akid = 0;
					}
					return _akid;
				}
			}
		}

		public override string Encoded
		{
			get
			{
				int messageType = (int)MessageType;
				if (AckId.HasValue)
				{
					if (Callback == null)
					{
						object[] obj = new object[4] { messageType, null, null, null };
						int? ackId = AckId;
						obj[1] = ((!ackId.HasValue) ? (-1) : ackId.Value);
						obj[2] = Endpoint;
						obj[3] = MessageText;
						return string.Format("{0}:{1}:{2}:{3}", obj);
					}
					object[] obj2 = new object[4] { messageType, null, null, null };
					int? ackId2 = AckId;
					obj2[1] = ((!ackId2.HasValue) ? (-1) : ackId2.Value);
					obj2[2] = Endpoint;
					obj2[3] = MessageText;
					return string.Format("{0}:{1}+:{2}:{3}", obj2);
				}
				return string.Format("{0}::{1}:{2}", messageType, Endpoint, MessageText);
			}
		}

		public EventMessage()
		{
			base.MessageType = SocketIOMessageTypes.Event;
		}

		public EventMessage(string eventName, object jsonObject, string endpoint, Action<object> callBack)
			: this()
		{
			Callback = callBack;
			Endpoint = endpoint;
			if (callBack != null)
			{
				base.AckId = NextAckID;
			}
			base.Json = new JsonEncodedEventMessage(eventName, jsonObject);
			base.MessageText = Json.ToJsonString();
		}

		public static EventMessage Deserialize(string rawMessage)
		{
			EventMessage eventMessage = new EventMessage();
			eventMessage.RawMessage = rawMessage;
			try
			{
				string[] array = rawMessage.Split(Message.SPLITCHARS, 4);
				if (array.Length == 4)
				{
					int result;
					if (int.TryParse(array[1], out result))
					{
						eventMessage.AckId = result;
					}
					eventMessage.Endpoint = array[2];
					eventMessage.MessageText = array[3];
					if (!string.IsNullOrEmpty(eventMessage.MessageText) && eventMessage.MessageText.Contains("name") && eventMessage.MessageText.Contains("args"))
					{
						eventMessage.Json = JsonEncodedEventMessage.Deserialize(eventMessage.MessageText);
						eventMessage.Event = eventMessage.Json.name;
					}
					else
					{
						eventMessage.Json = new JsonEncodedEventMessage();
					}
				}
			}
			catch (Exception)
			{
			}
			return eventMessage;
		}
	}
}
