using System;
using System.IO;
using System.Net;

namespace LeanplumSDK
{
	public interface ISocketUtils
	{
		bool AreSocketsAvailable { get; }

		bool IsDataAvailable(Stream stream);

		Stream CreateNetworkStream(ISocket socket, bool ownsSocket);

		ISocket CreateSocket(IPAddress address);

		ISocketAsyncEventArgs CreateSocketAsyncEventArgs();

		ISocketAsyncEventArgs CreateSocketAsyncEventArgs(EventArgs e);

		ITcpClient CreateTcpClient(string host, int port);
	}
}
