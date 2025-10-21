using System;
using System.IO;
using System.Net;

namespace LeanplumSDK
{
	public class DisabledSocketUtils : ISocketUtils
	{
		public bool AreSocketsAvailable
		{
			get
			{
				return false;
			}
		}

		public bool IsDataAvailable(Stream stream)
		{
			return false;
		}

		public Stream CreateNetworkStream(ISocket socket, bool ownsSocket)
		{
			return null;
		}

		public ISocket CreateSocket(IPAddress address)
		{
			return null;
		}

		public ISocketAsyncEventArgs CreateSocketAsyncEventArgs()
		{
			return null;
		}

		public ISocketAsyncEventArgs CreateSocketAsyncEventArgs(EventArgs e)
		{
			return null;
		}

		public ITcpClient CreateTcpClient(string host, int port)
		{
			return null;
		}
	}
}
