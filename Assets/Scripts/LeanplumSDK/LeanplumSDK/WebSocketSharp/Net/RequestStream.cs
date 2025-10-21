using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal class RequestStream : Stream
	{
		private byte[] buffer;

		private bool disposed;

		private int length;

		private int offset;

		private long remaining_body;

		private Stream stream;

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal RequestStream(Stream stream, byte[] buffer, int offset, int length)
			: this(stream, buffer, offset, length, -1L)
		{
		}

		internal RequestStream(Stream stream, byte[] buffer, int offset, int length, long contentlength)
		{
			this.stream = stream;
			this.buffer = buffer;
			this.offset = offset;
			this.length = length;
			remaining_body = contentlength;
		}

		private int FillFromBuffer(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			int num = buffer.Length;
			if (offset > num)
			{
				throw new ArgumentException("Destination offset is beyond array size.");
			}
			if (offset > num - count)
			{
				throw new ArgumentException("Reading would overrun buffer.");
			}
			if (remaining_body == 0L)
			{
				return -1;
			}
			if (length == 0)
			{
				return 0;
			}
			int num2 = Math.Min(length, count);
			if (remaining_body > 0)
			{
				num2 = (int)Math.Min(num2, remaining_body);
			}
			if (this.offset > this.buffer.Length - num2)
			{
				num2 = Math.Min(num2, this.buffer.Length - this.offset);
			}
			if (num2 == 0)
			{
				return 0;
			}
			Buffer.BlockCopy(this.buffer, this.offset, buffer, offset, num2);
			this.offset += num2;
			length -= num2;
			if (remaining_body > 0)
			{
				remaining_body -= num2;
			}
			return num2;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			int num = FillFromBuffer(buffer, offset, count);
			if (num > 0 || num == -1)
			{
				HttpStreamAsyncResult httpStreamAsyncResult = new HttpStreamAsyncResult();
				httpStreamAsyncResult.Buffer = buffer;
				httpStreamAsyncResult.Offset = offset;
				httpStreamAsyncResult.Count = count;
				httpStreamAsyncResult.Callback = cback;
				httpStreamAsyncResult.State = state;
				httpStreamAsyncResult.SyncRead = num;
				httpStreamAsyncResult.Complete();
				return httpStreamAsyncResult;
			}
			if (remaining_body >= 0 && count > remaining_body)
			{
				count = (int)Math.Min(2147483647L, remaining_body);
			}
			return stream.BeginRead(buffer, offset, count, cback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			throw new NotSupportedException();
		}

		public override void Close()
		{
			disposed = true;
		}

		public override int EndRead(IAsyncResult ares)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (ares == null)
			{
				throw new ArgumentNullException("ares");
			}
			if (ares is HttpStreamAsyncResult)
			{
				HttpStreamAsyncResult httpStreamAsyncResult = (HttpStreamAsyncResult)ares;
				if (!ares.IsCompleted)
				{
					ares.AsyncWaitHandle.WaitOne();
				}
				return httpStreamAsyncResult.SyncRead;
			}
			int num = stream.EndRead(ares);
			if (remaining_body > 0 && num > 0)
			{
				remaining_body -= num;
			}
			return num;
		}

		public override void EndWrite(IAsyncResult async_result)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
		}

		public override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			int num = FillFromBuffer(buffer, offset, count);
			if (num == -1)
			{
				return 0;
			}
			if (num > 0)
			{
				return num;
			}
			num = stream.Read(buffer, offset, count);
			if (num > 0 && remaining_body > 0)
			{
				remaining_body -= num;
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}
