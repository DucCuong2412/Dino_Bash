using System.IO;
using System.Net;

namespace LeanplumSDK
{
	public interface ITcpClient
	{
		EndPoint GetLocalEndpoint();

		EndPoint GetRemoteEndpoint();

		Stream GetStream();

		void Close();
	}
}
