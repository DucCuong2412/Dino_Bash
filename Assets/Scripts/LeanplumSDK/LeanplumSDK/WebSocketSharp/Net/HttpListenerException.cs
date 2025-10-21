using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace LeanplumSDK.WebSocketSharp.Net
{
	[Serializable]
	internal class HttpListenerException : Win32Exception
	{
		public override int ErrorCode
		{
			get
			{
				return base.ErrorCode;
			}
		}

		public HttpListenerException()
		{
		}

		public HttpListenerException(int errorCode)
			: base(errorCode)
		{
		}

		public HttpListenerException(int errorCode, string message)
			: base(errorCode, message)
		{
		}

		protected HttpListenerException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}
