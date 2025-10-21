using System.Text.RegularExpressions;

namespace LeanplumSDK.SocketIOClient.Messages
{
	internal sealed class AckMessage : Message
	{
		private static Regex reAckComplex = new Regex("^\\[(?<payload>.*)\\]$");

		private static object ackLock = new object();

		private static int _akid = 0;

		public static int NextAckID
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
					object[] obj = new object[4] { messageType, null, null, null };
					int? ackId = AckId;
					obj[1] = ((!ackId.HasValue) ? (-1) : ackId.Value);
					obj[2] = Endpoint;
					obj[3] = MessageText;
					return string.Format("{0}:{1}+:{2}:{3}", obj);
				}
				return string.Format("{0}::{1}:{2}", messageType, Endpoint, MessageText);
			}
		}

		public AckMessage()
		{
			base.MessageType = SocketIOMessageTypes.ACK;
		}

		public static AckMessage Deserialize(string rawMessage)
		{
			AckMessage ackMessage = new AckMessage();
			ackMessage.RawMessage = rawMessage;
			string[] array = rawMessage.Split(Message.SPLITCHARS, 4);
			if (array.Length == 4)
			{
				ackMessage.Endpoint = array[2];
				string[] array2 = array[3].Split('+');
				int result;
				if (array2.Length > 1 && int.TryParse(array2[0], out result))
				{
					ackMessage.AckId = result;
					ackMessage.MessageText = array2[1];
					Match match = reAckComplex.Match(ackMessage.MessageText);
					if (match.Success)
					{
						ackMessage.Json = new JsonEncodedEventMessage();
						ackMessage.Json.args = new string[1] { match.Groups["payload"].Value };
					}
				}
			}
			return ackMessage;
		}
	}
}
