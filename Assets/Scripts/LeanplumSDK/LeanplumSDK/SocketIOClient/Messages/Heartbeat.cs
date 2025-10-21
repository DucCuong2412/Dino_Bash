namespace LeanplumSDK.SocketIOClient.Messages
{
	internal class Heartbeat : Message
	{
		public static string HEARTBEAT = "2::";

		public override string Encoded
		{
			get
			{
				return HEARTBEAT;
			}
		}

		public Heartbeat()
		{
			base.MessageType = SocketIOMessageTypes.Heartbeat;
		}
	}
}
