using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LeanplumSDK.WebSocketSharp
{
	internal class WsFrame : IEnumerable, IEnumerable<byte>
	{
		internal static readonly byte[] EmptyUnmaskPingData;

		internal bool IsBinary
		{
			get
			{
				return Opcode == Opcode.BINARY;
			}
		}

		internal bool IsClose
		{
			get
			{
				return Opcode == Opcode.CLOSE;
			}
		}

		internal bool IsCompressed
		{
			get
			{
				return Rsv1 == Rsv.ON;
			}
		}

		internal bool IsContinuation
		{
			get
			{
				return Opcode == Opcode.CONT;
			}
		}

		internal bool IsControl
		{
			get
			{
				Opcode opcode = Opcode;
				return opcode == Opcode.CLOSE || opcode == Opcode.PING || opcode == Opcode.PONG;
			}
		}

		internal bool IsData
		{
			get
			{
				Opcode opcode = Opcode;
				return opcode == Opcode.BINARY || opcode == Opcode.TEXT;
			}
		}

		internal bool IsFinal
		{
			get
			{
				return Fin == Fin.FINAL;
			}
		}

		internal bool IsFragmented
		{
			get
			{
				return Fin == Fin.MORE || Opcode == Opcode.CONT;
			}
		}

		internal bool IsMasked
		{
			get
			{
				return Mask == Mask.MASK;
			}
		}

		internal bool IsPerMessageCompressed
		{
			get
			{
				Opcode opcode = Opcode;
				return (opcode == Opcode.BINARY || opcode == Opcode.TEXT) && Rsv1 == Rsv.ON;
			}
		}

		internal bool IsPing
		{
			get
			{
				return Opcode == Opcode.PING;
			}
		}

		internal bool IsPong
		{
			get
			{
				return Opcode == Opcode.PONG;
			}
		}

		internal bool IsText
		{
			get
			{
				return Opcode == Opcode.TEXT;
			}
		}

		internal ulong Length
		{
			get
			{
				return (ulong)(2L + (long)(ExtPayloadLen.Length + MaskingKey.Length)) + PayloadData.Length;
			}
		}

		public Fin Fin { get; private set; }

		public Rsv Rsv1 { get; private set; }

		public Rsv Rsv2 { get; private set; }

		public Rsv Rsv3 { get; private set; }

		public Opcode Opcode { get; private set; }

		public Mask Mask { get; private set; }

		public byte PayloadLen { get; private set; }

		public byte[] ExtPayloadLen { get; private set; }

		public byte[] MaskingKey { get; private set; }

		public PayloadData PayloadData { get; private set; }

		private WsFrame()
		{
		}

		public WsFrame(Opcode opcode, PayloadData payload)
			: this(opcode, Mask.MASK, payload)
		{
		}

		public WsFrame(Opcode opcode, Mask mask, PayloadData payload)
			: this(Fin.FINAL, opcode, mask, payload)
		{
		}

		public WsFrame(Fin fin, Opcode opcode, Mask mask, PayloadData payload)
			: this(fin, opcode, mask, payload, false)
		{
		}

		public WsFrame(Fin fin, Opcode opcode, Mask mask, PayloadData payload, bool compressed)
		{
			Fin = fin;
			Rsv1 = ((isData(opcode) && compressed) ? Rsv.ON : Rsv.OFF);
			Rsv2 = Rsv.OFF;
			Rsv3 = Rsv.OFF;
			Opcode = opcode;
			Mask = mask;
			ulong length = payload.Length;
			byte b2 = (PayloadLen = (byte)((length < 126) ? ((byte)length) : ((length >= 65536) ? 127 : 126)));
			ExtPayloadLen = ((b2 < 126) ? new byte[0] : ((b2 != 126) ? length.ToByteArrayInternally(ByteOrder.BIG) : ((ushort)length).ToByteArrayInternally(ByteOrder.BIG)));
			bool flag = mask == Mask.MASK;
			byte[] maskingKey = (MaskingKey = ((!flag) ? new byte[0] : createMaskingKey()));
			if (flag)
			{
				payload.Mask(maskingKey);
			}
			PayloadData = payload;
		}

		static WsFrame()
		{
			EmptyUnmaskPingData = CreatePingFrame(Mask.UNMASK).ToByteArray();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static byte[] createMaskingKey()
		{
			byte[] array = new byte[4];
			Random random = new Random();
			random.NextBytes(array);
			return array;
		}

		private static string dump(WsFrame frame)
		{
			ulong length = frame.Length;
			long num = (long)(length / 4uL);
			int num2 = (int)(length % 4uL);
			int num3;
			string countFmt;
			if (num < 10000)
			{
				num3 = 4;
				countFmt = "{0,4}";
			}
			else if (num < 65536)
			{
				num3 = 4;
				countFmt = "{0,4:X}";
			}
			else if (num < 4294967296L)
			{
				num3 = 8;
				countFmt = "{0,8:X}";
			}
			else
			{
				num3 = 16;
				countFmt = "{0,16:X}";
			}
			string arg5 = string.Format("{{0,{0}}}", num3);
			string format = string.Format("{0} 01234567 89ABCDEF 01234567 89ABCDEF\n{0}+--------+--------+--------+--------+\\n", arg5);
			string format2 = string.Format("{0}+--------+--------+--------+--------+", arg5);
			StringBuilder buffer = new StringBuilder(64);
			Func<Action<string, string, string, string>> func = delegate
			{
				long lineCount = 0L;
				string lineFmt = string.Format("{0}|{{1,8}} {{2,8}} {{3,8}} {{4,8}}|\n", countFmt);
				return delegate(string arg1, string arg2, string arg3, string arg4)
				{
					buffer.AppendFormat(lineFmt, ++lineCount, arg1, arg2, arg3, arg4);
				};
			};
			Action<string, string, string, string> action = func();
			buffer.AppendFormat(format, string.Empty);
			byte[] array = frame.ToByteArray();
			for (int i = 0; i <= num; i++)
			{
				int num4 = i * 4;
				if (i < num)
				{
					action(Convert.ToString(array[num4], 2).PadLeft(8, '0'), Convert.ToString(array[num4 + 1], 2).PadLeft(8, '0'), Convert.ToString(array[num4 + 2], 2).PadLeft(8, '0'), Convert.ToString(array[num4 + 3], 2).PadLeft(8, '0'));
				}
				else if (num2 > 0)
				{
					action(Convert.ToString(array[num4], 2).PadLeft(8, '0'), (num2 < 2) ? string.Empty : Convert.ToString(array[num4 + 1], 2).PadLeft(8, '0'), (num2 != 3) ? string.Empty : Convert.ToString(array[num4 + 2], 2).PadLeft(8, '0'), string.Empty);
				}
			}
			buffer.AppendFormat(format2, string.Empty);
			return buffer.ToString();
		}

		private static bool isBinary(Opcode opcode)
		{
			return opcode == Opcode.BINARY;
		}

		private static bool isClose(Opcode opcode)
		{
			return opcode == Opcode.CLOSE;
		}

		private static bool isContinuation(Opcode opcode)
		{
			return opcode == Opcode.CONT;
		}

		private static bool isControl(Opcode opcode)
		{
			return opcode == Opcode.CLOSE || opcode == Opcode.PING || opcode == Opcode.PONG;
		}

		private static bool isData(Opcode opcode)
		{
			return opcode == Opcode.TEXT || opcode == Opcode.BINARY;
		}

		private static bool isFinal(Fin fin)
		{
			return fin == Fin.FINAL;
		}

		private static bool isMasked(Mask mask)
		{
			return mask == Mask.MASK;
		}

		private static bool isPing(Opcode opcode)
		{
			return opcode == Opcode.PING;
		}

		private static bool isPong(Opcode opcode)
		{
			return opcode == Opcode.PONG;
		}

		private static bool isText(Opcode opcode)
		{
			return opcode == Opcode.TEXT;
		}

		private static WsFrame parse(byte[] header, Stream stream, bool unmask)
		{
			Fin fin = (((header[0] & 0x80) == 128) ? Fin.FINAL : Fin.MORE);
			Rsv rsv = (((header[0] & 0x40) == 64) ? Rsv.ON : Rsv.OFF);
			Rsv rsv2 = (((header[0] & 0x20) == 32) ? Rsv.ON : Rsv.OFF);
			Rsv rsv3 = (((header[0] & 0x10) == 16) ? Rsv.ON : Rsv.OFF);
			Opcode opcode = (Opcode)(header[0] & 0xFu);
			Mask mask = (((header[1] & 0x80) == 128) ? Mask.MASK : Mask.UNMASK);
			byte b = (byte)(header[1] & 0x7Fu);
			string text = ((isControl(opcode) && fin == Fin.MORE) ? "A control frame is fragmented." : ((isData(opcode) || rsv != Rsv.ON) ? null : "A non data frame is compressed."));
			if (text != null)
			{
				throw new WebSocketException(CloseStatusCode.INCORRECT_DATA, text);
			}
			if (isControl(opcode) && b > 125)
			{
				throw new WebSocketException(CloseStatusCode.INCONSISTENT_DATA, "The payload data length of a control frame is greater than 125 bytes.");
			}
			WsFrame wsFrame = new WsFrame();
			wsFrame.Fin = fin;
			wsFrame.Rsv1 = rsv;
			wsFrame.Rsv2 = rsv2;
			wsFrame.Rsv3 = rsv3;
			wsFrame.Opcode = opcode;
			wsFrame.Mask = mask;
			wsFrame.PayloadLen = b;
			WsFrame wsFrame2 = wsFrame;
			int num = ((b >= 126) ? ((b != 126) ? 8 : 2) : 0);
			byte[] array = ((num <= 0) ? new byte[0] : stream.ReadBytes(num));
			if (num > 0 && array.Length != num)
			{
				throw new WebSocketException("The 'Extended Payload Length' of a frame cannot be read from the data source.");
			}
			wsFrame2.ExtPayloadLen = array;
			bool flag = mask == Mask.MASK;
			byte[] array2 = ((!flag) ? new byte[0] : stream.ReadBytes(4));
			if (flag && array2.Length != 4)
			{
				throw new WebSocketException("The 'Masking Key' of a frame cannot be read from the data source.");
			}
			wsFrame2.MaskingKey = array2;
			ulong num2 = ((b < 126) ? b : ((b != 126) ? array.ToUInt64(ByteOrder.BIG) : array.ToUInt16(ByteOrder.BIG)));
			byte[] array3 = null;
			if (num2 != 0)
			{
				if (b > 126 && num2 > long.MaxValue)
				{
					throw new WebSocketException(CloseStatusCode.TOO_BIG, "The 'Payload Data' length is greater than the allowable length.");
				}
				array3 = ((b <= 126) ? stream.ReadBytes((int)num2) : stream.ReadBytes((long)num2, 1024));
				if (array3.LongLength != (long)num2)
				{
					throw new WebSocketException("The 'Payload Data' of a frame cannot be read from the data source.");
				}
			}
			else
			{
				array3 = new byte[0];
			}
			PayloadData payloadData = new PayloadData(array3, flag);
			if (flag && unmask)
			{
				payloadData.Mask(array2);
				wsFrame2.Mask = Mask.UNMASK;
				wsFrame2.MaskingKey = new byte[0];
			}
			wsFrame2.PayloadData = payloadData;
			return wsFrame2;
		}

		private static string print(WsFrame frame)
		{
			string text = frame.Opcode.ToString();
			byte payloadLen = frame.PayloadLen;
			byte[] extPayloadLen = frame.ExtPayloadLen;
			int num = extPayloadLen.Length;
			string text2;
			switch (num)
			{
			case 2:
				text2 = extPayloadLen.ToUInt16(ByteOrder.BIG).ToString();
				break;
			case 8:
				text2 = extPayloadLen.ToUInt64(ByteOrder.BIG).ToString();
				break;
			default:
				text2 = string.Empty;
				break;
			}
			string text3 = text2;
			bool flag = frame.IsMasked;
			string text4 = ((!flag) ? string.Empty : BitConverter.ToString(frame.MaskingKey));
			string text5 = ((payloadLen == 0) ? string.Empty : ((num > 0) ? string.Format("A {0} data with {1} bytes.", text.ToLower(), text3) : ((!flag && !frame.IsFragmented && !frame.IsBinary && !frame.IsClose) ? Encoding.UTF8.GetString(frame.PayloadData.ApplicationData) : BitConverter.ToString(frame.PayloadData.ToByteArray()))));
			string format = "                 FIN: {0}\n                RSV1: {1}\n                RSV2: {2}\n                RSV3: {3}\n              Opcode: {4}\n                MASK: {5}\n         Payload Len: {6}\nExtended Payload Len: {7}\n         Masking Key: {8}\n        Payload Data: {9}";
			return string.Format(format, frame.Fin, frame.Rsv1, frame.Rsv2, frame.Rsv3, text, frame.Mask, payloadLen, text3, text4, text5);
		}

		internal static WsFrame CreateCloseFrame(Mask mask, PayloadData payload)
		{
			return new WsFrame(Opcode.CLOSE, mask, payload);
		}

		internal static WsFrame CreatePongFrame(Mask mask, PayloadData payload)
		{
			return new WsFrame(Opcode.PONG, mask, payload);
		}

		public static WsFrame CreateCloseFrame(Mask mask, byte[] data)
		{
			return new WsFrame(Opcode.CLOSE, mask, new PayloadData(data));
		}

		public static WsFrame CreateCloseFrame(Mask mask, CloseStatusCode code, string reason)
		{
			return new WsFrame(Opcode.CLOSE, mask, new PayloadData(((ushort)code).Append(reason)));
		}

		public static WsFrame CreateFrame(Fin fin, Opcode opcode, Mask mask, byte[] data, bool compressed)
		{
			return new WsFrame(fin, opcode, mask, new PayloadData(data), compressed);
		}

		public static WsFrame CreatePingFrame(Mask mask)
		{
			return new WsFrame(Opcode.PING, mask, new PayloadData());
		}

		public static WsFrame CreatePingFrame(Mask mask, byte[] data)
		{
			return new WsFrame(Opcode.PING, mask, new PayloadData(data));
		}

		public IEnumerator<byte> GetEnumerator()
		{
			byte[] array = ToByteArray();
			for (int i = 0; i < array.Length; i++)
			{
				yield return array[i];
			}
		}

		public static WsFrame Parse(byte[] src)
		{
			return Parse(src, true);
		}

		public static WsFrame Parse(Stream stream)
		{
			return Parse(stream, true);
		}

		public static WsFrame Parse(byte[] src, bool unmask)
		{
			//Discarded unreachable code: IL_0014
			using (MemoryStream stream = new MemoryStream(src))
			{
				return Parse(stream, unmask);
			}
		}

		public static WsFrame Parse(Stream stream, bool unmask)
		{
			byte[] array = stream.ReadBytes(2);
			if (array.Length != 2)
			{
				throw new WebSocketException("The header part of a frame cannot be read from the data source.");
			}
			return parse(array, stream, unmask);
		}

		public static void ParseAsync(Stream stream, Action<WsFrame> completed)
		{
			ParseAsync(stream, true, completed, null);
		}

		public static void ParseAsync(Stream stream, Action<WsFrame> completed, Action<Exception> error)
		{
			ParseAsync(stream, true, completed, error);
		}

		public static void ParseAsync(Stream stream, bool unmask, Action<WsFrame> completed, Action<Exception> error)
		{
			stream.ReadBytesAsync(2, delegate(byte[] header)
			{
				if (header.Length != 2)
				{
					throw new WebSocketException("The header part of a frame cannot be read from the data source.");
				}
				WsFrame obj = parse(header, stream, unmask);
				if (completed != null)
				{
					completed(obj);
				}
			}, error);
		}

		public void Print(bool dumped)
		{
			Console.WriteLine((!dumped) ? print(this) : dump(this));
		}

		public string PrintToString(bool dumped)
		{
			return (!dumped) ? print(this) : dump(this);
		}

		public byte[] ToByteArray()
		{
			//Discarded unreachable code: IL_00f0
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int fin = (int)Fin;
				fin = (fin << 1) + (int)Rsv1;
				fin = (fin << 1) + (int)Rsv2;
				fin = (fin << 1) + (int)Rsv3;
				fin = (fin << 4) + (int)Opcode;
				fin = (fin << 1) + (int)Mask;
				fin = (fin << 7) + PayloadLen;
				memoryStream.Write(((ushort)fin).ToByteArrayInternally(ByteOrder.BIG), 0, 2);
				if (PayloadLen > 125)
				{
					memoryStream.Write(ExtPayloadLen, 0, ExtPayloadLen.Length);
				}
				if (Mask == Mask.MASK)
				{
					memoryStream.Write(MaskingKey, 0, MaskingKey.Length);
				}
				if (PayloadLen > 0)
				{
					byte[] array = PayloadData.ToByteArray();
					if (PayloadLen < 127)
					{
						memoryStream.Write(array, 0, array.Length);
					}
					else
					{
						memoryStream.WriteBytes(array);
					}
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		public override string ToString()
		{
			return BitConverter.ToString(ToByteArray());
		}
	}
}
