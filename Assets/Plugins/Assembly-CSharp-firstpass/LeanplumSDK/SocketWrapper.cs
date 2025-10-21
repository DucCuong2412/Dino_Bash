using System.Net;
using System.Net.Sockets;

namespace LeanplumSDK
{
	public class SocketWrapper : ISocket
	{
		private Socket socket;

		public Socket WrappedSocket
		{
			get
			{
				return socket;
			}
		}

		public EndPoint LocalEndPoint
		{
			get
			{
				return socket.LocalEndPoint;
			}
		}

		public EndPoint RemoteEndPoint
		{
			get
			{
				return socket.RemoteEndPoint;
			}
		}

		public SocketWrapper(AddressFamily addressFamily)
		{
			socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
		}

		public SocketWrapper(Socket socket)
		{
			this.socket = socket;
		}

		public void Bind(IPEndPoint endpoint)
		{
			socket.Bind(endpoint);
		}

		public void Listen(int backlog)
		{
			socket.Listen(backlog);
		}

		public void AcceptAsync(ISocketAsyncEventArgs args)
		{
			socket.AcceptAsync(((SocketAsyncEventArgsWrapper)args).WrappedArgs);
		}

		public void Close()
		{
			socket.Close();
		}

		public void Shutdown()
		{
			socket.Shutdown(SocketShutdown.Both);
		}
	}
}
