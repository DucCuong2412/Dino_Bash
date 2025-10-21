using System.IO;
using System.Net;
using System.Net.Sockets;

namespace LeanplumSDK
{
	public class TcpClientWrapper : ITcpClient
	{
		private TcpClient tcpClient;

		public TcpClient GetClient
		{
			get
			{
				return tcpClient;
			}
		}

		public TcpClientWrapper()
		{
			tcpClient = new TcpClient();
		}

		public TcpClientWrapper(string host, int port)
		{
			tcpClient = new TcpClient(host, port);
		}

		public EndPoint GetLocalEndpoint()
		{
			return tcpClient.Client.LocalEndPoint;
		}

		public EndPoint GetRemoteEndpoint()
		{
			return tcpClient.Client.RemoteEndPoint;
		}

		public Stream GetStream()
		{
			return tcpClient.GetStream();
		}

		public void Close()
		{
			tcpClient.Close();
		}
	}
}
