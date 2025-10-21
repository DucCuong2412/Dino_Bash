using System.Linq;
using System.Text.RegularExpressions;

namespace LeanplumSDK.SocketIOClient.Messages
{
	internal abstract class Message : IMessage
	{
		public static char[] SPLITCHARS = new char[1] { ':' };

		private JsonEncodedEventMessage _json;

		public static Regex reMessageType = new Regex("^[0-8]{1}:", RegexOptions.IgnoreCase);

		public string RawMessage { get; protected set; }

		public SocketIOMessageTypes MessageType { get; protected set; }

		public int? AckId { get; set; }

		public string Endpoint { get; set; }

		public string MessageText { get; set; }

		public JsonEncodedEventMessage JsonEncodedMessage
		{
			get
			{
				return Json;
			}
			set
			{
				_json = value;
			}
		}

		public JsonEncodedEventMessage Json
		{
			get
			{
				if (_json == null)
				{
					if (!string.IsNullOrEmpty(MessageText) && MessageText.Contains("name") && MessageText.Contains("args"))
					{
						_json = JsonEncodedEventMessage.Deserialize(MessageText);
					}
					else
					{
						_json = new JsonEncodedEventMessage();
					}
				}
				return _json;
			}
			set
			{
				_json = value;
			}
		}

		public virtual string Event { get; set; }

		public virtual string Encoded
		{
			get
			{
				int messageType = (int)MessageType;
				if (AckId.HasValue)
				{
					object[] obj = new object[4] { messageType, null, null, null };
					int? ackId = AckId;
					obj[1] = ((!ackId.HasValue) ? (-1) : ackId.Value);
					obj[2] = Endpoint;
					obj[3] = MessageText;
					return string.Format("{0}:{1}:{2}:{3}", obj);
				}
				return string.Format("{0}::{1}:{2}", messageType, Endpoint, MessageText);
			}
		}

		public Message()
		{
			MessageType = SocketIOMessageTypes.Message;
		}

		public Message(string rawMessage)
			: this()
		{
			RawMessage = rawMessage;
			string[] array = rawMessage.Split(SPLITCHARS, 4);
			if (array.Length == 4)
			{
				int result;
				if (int.TryParse(array[1], out result))
				{
					AckId = result;
				}
				Endpoint = array[2];
				MessageText = array[3];
			}
		}

		public static IMessage Factory(string rawMessage)
		{
			if (reMessageType.IsMatch(rawMessage))
			{
				switch (rawMessage.First())
				{
				case '0':
					return DisconnectMessage.Deserialize(rawMessage);
				case '1':
					return ConnectMessage.Deserialize(rawMessage);
				case '2':
					return new Heartbeat();
				case '3':
					return TextMessage.Deserialize(rawMessage);
				case '4':
					return JSONMessage.Deserialize(rawMessage);
				case '5':
					return EventMessage.Deserialize(rawMessage);
				case '6':
					return AckMessage.Deserialize(rawMessage);
				case '7':
					return ErrorMessage.Deserialize(rawMessage);
				case '8':
					return new NoopMessage();
				default:
					return new TextMessage();
				}
			}
			return new NoopMessage();
		}
	}
}
