using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanplumSDK.WebSocketSharp
{
	internal class PayloadData : IEnumerable, IEnumerable<byte>
	{
		public const ulong MaxLength = 9223372036854775807uL;

		internal bool ContainsReservedCloseStatusCode
		{
			get
			{
				return ApplicationData.Length > 1 && ApplicationData.SubArray(0, 2).ToUInt16(ByteOrder.BIG).IsReserved();
			}
		}

		internal bool IsMasked { get; private set; }

		public byte[] ExtensionData { get; private set; }

		public byte[] ApplicationData { get; private set; }

		public ulong Length
		{
			get
			{
				return (ulong)(ExtensionData.LongLength + ApplicationData.LongLength);
			}
		}

		public PayloadData()
			: this(new byte[0])
		{
		}

		public PayloadData(byte[] appData)
			: this(new byte[0], appData)
		{
		}

		public PayloadData(string appData)
			: this(Encoding.UTF8.GetBytes(appData))
		{
		}

		public PayloadData(byte[] appData, bool masked)
			: this(new byte[0], appData, masked)
		{
		}

		public PayloadData(byte[] extData, byte[] appData)
			: this(extData, appData, false)
		{
		}

		public PayloadData(byte[] extData, byte[] appData, bool masked)
		{
			if ((ulong)(extData.LongLength + appData.LongLength) > 9223372036854775807uL)
			{
				throw new ArgumentOutOfRangeException("The length of 'extData' plus 'appData' must be less than MaxLength.");
			}
			ExtensionData = extData;
			ApplicationData = appData;
			IsMasked = masked;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static void mask(byte[] src, byte[] key)
		{
			for (long num = 0L; num < src.LongLength; num++)
			{
				src[num] = (byte)(src[num] ^ key[num % 4]);
			}
		}

		public IEnumerator<byte> GetEnumerator()
		{
			byte[] extensionData = ExtensionData;
			for (int i = 0; i < extensionData.Length; i++)
			{
				yield return extensionData[i];
			}
			byte[] applicationData = ApplicationData;
			for (int j = 0; j < applicationData.Length; j++)
			{
				yield return applicationData[j];
			}
		}

		public void Mask(byte[] maskingKey)
		{
			if (ExtensionData.LongLength > 0)
			{
				mask(ExtensionData, maskingKey);
			}
			if (ApplicationData.LongLength > 0)
			{
				mask(ApplicationData, maskingKey);
			}
			IsMasked = !IsMasked;
		}

		public byte[] ToByteArray()
		{
			return (ExtensionData.LongLength <= 0) ? ApplicationData : this.ToArray();
		}

		public override string ToString()
		{
			return BitConverter.ToString(ToByteArray());
		}
	}
}
