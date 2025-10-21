namespace LeanplumSDK.SocketIOClient
{
	internal enum SocketIOMessageTypes
	{
		Disconnect = 0,
		Connect = 1,
		Heartbeat = 2,
		Message = 3,
		JSONMessage = 4,
		Event = 5,
		ACK = 6,
		Error = 7,
		Noop = 8
	}
}
