using System;
using System.Text;

namespace LeanplumSDK.WebSocketSharp
{
	internal class CloseEventArgs : EventArgs
	{
		private bool _clean;

		private ushort _code;

		private string _reason;

		public ushort Code
		{
			get
			{
				return _code;
			}
		}

		public string Reason
		{
			get
			{
				return _reason;
			}
		}

		public bool WasClean
		{
			get
			{
				return _clean;
			}
			internal set
			{
				_clean = value;
			}
		}

		internal CloseEventArgs(PayloadData payload)
		{
			byte[] applicationData = payload.ApplicationData;
			_code = getCodeFrom(applicationData);
			_reason = getReasonFrom(applicationData);
			_clean = false;
		}

		private static ushort getCodeFrom(byte[] data)
		{
			return (ushort)((data.Length <= 1) ? 1005 : data.SubArray(0, 2).ToUInt16(ByteOrder.BIG));
		}

		private static string getReasonFrom(byte[] data)
		{
			int num = data.Length;
			return (num <= 2) ? string.Empty : Encoding.UTF8.GetString(data.SubArray(2, num - 2));
		}
	}
}
