using System.IO;
using System.Net.Security;

namespace LeanplumSDK.WebSocketSharp.Net.Security
{
	internal class SslStream : System.Net.Security.SslStream
	{
		public bool DataAvailable
		{
			get
			{
				return SocketUtilsFactory.Utils.IsDataAvailable(base.InnerStream);
			}
		}

		public SslStream(Stream innerStream)
			: base(innerStream)
		{
		}

		public SslStream(Stream innerStream, bool leaveInnerStreamOpen)
			: base(innerStream, leaveInnerStreamOpen)
		{
		}

		public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback)
			: base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback)
		{
		}

		public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback)
			: base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback)
		{
		}
	}
}
