using System;

namespace LeanplumSDK.WebSocketSharp
{
	internal class WebSocketException : Exception
	{
		public CloseStatusCode Code { get; private set; }

		internal WebSocketException()
			: this(CloseStatusCode.ABNORMAL)
		{
		}

		internal WebSocketException(CloseStatusCode code)
			: this(code, null)
		{
		}

		internal WebSocketException(string reason)
			: this(CloseStatusCode.ABNORMAL, reason)
		{
		}

		internal WebSocketException(CloseStatusCode code, string reason)
			: base(reason ?? code.GetMessage())
		{
			Code = code;
		}
	}
}
