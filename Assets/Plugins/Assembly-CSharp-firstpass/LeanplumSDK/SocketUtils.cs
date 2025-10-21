using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace LeanplumSDK
{
	public class SocketUtils : ISocketUtils
	{
		public bool AreSocketsAvailable
		{
			get
			{
				return true;
			}
		}

		public bool IsDataAvailable(Stream stream)
		{
			return ((NetworkStream)stream).DataAvailable;
		}

		public Stream CreateNetworkStream(ISocket socket, bool ownsSocket)
		{
			return new NetworkStream(((SocketWrapper)socket).WrappedSocket, ownsSocket);
		}

		public ISocket CreateSocket(IPAddress address)
		{
			return new SocketWrapper(address.AddressFamily);
		}

		public ISocketAsyncEventArgs CreateSocketAsyncEventArgs()
		{
			return new SocketAsyncEventArgsWrapper();
		}

		public ISocketAsyncEventArgs CreateSocketAsyncEventArgs(EventArgs e)
		{
			return new SocketAsyncEventArgsWrapper(e);
		}

		public ITcpClient CreateTcpClient(string host, int port)
		{
			return new TcpClientWrapper(host, port);
		}
	}
}
