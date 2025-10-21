namespace LeanplumSDK.WebSocketSharp
{
	internal enum WebSocketState : ushort
	{
		CONNECTING = 0,
		OPEN = 1,
		CLOSING = 2,
		CLOSED = 3
	}
}
