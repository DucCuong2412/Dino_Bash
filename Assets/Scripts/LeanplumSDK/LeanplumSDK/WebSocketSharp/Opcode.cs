namespace LeanplumSDK.WebSocketSharp
{
	internal enum Opcode : byte
	{
		CONT = 0,
		TEXT = 1,
		BINARY = 2,
		CLOSE = 8,
		PING = 9,
		PONG = 10
	}
}
