using System;
using System.Text;

namespace LeanplumSDK.WebSocketSharp
{
	internal class MessageEventArgs : EventArgs
	{
		private string _data;

		private Opcode _opcode;

		private byte[] _rawData;

		public string Data
		{
			get
			{
				return _data;
			}
		}

		public byte[] RawData
		{
			get
			{
				return _rawData;
			}
		}

		public Opcode Type
		{
			get
			{
				return _opcode;
			}
		}

		internal MessageEventArgs(Opcode opcode, byte[] data)
		{
			if ((ulong)data.LongLength > 9223372036854775807uL)
			{
				throw new WebSocketException(CloseStatusCode.TOO_BIG);
			}
			_opcode = opcode;
			_rawData = data;
			_data = convertToString(opcode, data);
		}

		internal MessageEventArgs(Opcode opcode, PayloadData payload)
		{
			_opcode = opcode;
			_rawData = payload.ApplicationData;
			_data = convertToString(opcode, _rawData);
		}

		private static string convertToString(Opcode opcode, byte[] data)
		{
			return (data.LongLength == 0L) ? string.Empty : ((opcode != Opcode.TEXT) ? opcode.ToString() : Encoding.UTF8.GetString(data));
		}
	}
}
