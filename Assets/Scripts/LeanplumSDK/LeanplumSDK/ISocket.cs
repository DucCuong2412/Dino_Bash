using System.Net;

namespace LeanplumSDK
{
	public interface ISocket
	{
		EndPoint LocalEndPoint { get; }

		EndPoint RemoteEndPoint { get; }

		void Bind(IPEndPoint endpoint);

		void Listen(int backlog);

		void AcceptAsync(ISocketAsyncEventArgs args);

		void Close();

		void Shutdown();
	}
}
