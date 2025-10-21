using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal class ChunkedInputStream : RequestStream
	{
		private class ReadBufferState
		{
			public HttpStreamAsyncResult Ares;

			public byte[] Buffer;

			public int Count;

			public int InitialCount;

			public int Offset;

			public ReadBufferState(byte[] buffer, int offset, int count, HttpStreamAsyncResult ares)
			{
				Buffer = buffer;
				Offset = offset;
				Count = count;
				InitialCount = count;
				Ares = ares;
			}
		}

		private HttpListenerContext context;

		private ChunkStream decoder;

		private bool disposed;

		private bool no_more_data;

		public ChunkStream Decoder
		{
			get
			{
				return decoder;
			}
			set
			{
				decoder = value;
			}
		}

		public ChunkedInputStream(HttpListenerContext context, Stream stream, byte[] buffer, int offset, int length)
			: base(stream, buffer, offset, length)
		{
			this.context = context;
			WebHeaderCollection headers = (WebHeaderCollection)context.Request.Headers;
			decoder = new ChunkStream(headers);
		}

		private void OnRead(IAsyncResult base_ares)
		{
			ReadBufferState readBufferState = (ReadBufferState)base_ares.AsyncState;
			HttpStreamAsyncResult ares = readBufferState.Ares;
			try
			{
				int size = base.EndRead(base_ares);
				decoder.Write(ares.Buffer, ares.Offset, size);
				size = decoder.Read(readBufferState.Buffer, readBufferState.Offset, readBufferState.Count);
				readBufferState.Offset += size;
				readBufferState.Count -= size;
				if (readBufferState.Count == 0 || !decoder.WantMore || size == 0)
				{
					no_more_data = !decoder.WantMore && size == 0;
					ares.Count = readBufferState.InitialCount - readBufferState.Count;
					ares.Complete();
				}
				else
				{
					ares.Offset = 0;
					ares.Count = Math.Min(8192, decoder.ChunkLeft + 6);
					base.BeginRead(ares.Buffer, ares.Offset, ares.Count, (AsyncCallback)OnRead, (object)readBufferState);
				}
			}
			catch (Exception ex)
			{
				context.Connection.SendError(ex.Message, 400);
				ares.Complete(ex);
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = buffer.Length;
			if (offset < 0 || offset > num)
			{
				throw new ArgumentOutOfRangeException("'offset' exceeds the size of buffer.");
			}
			if (count < 0 || offset > num - count)
			{
				throw new ArgumentOutOfRangeException("'offset' + 'count' exceeds the size of buffer.");
			}
			HttpStreamAsyncResult httpStreamAsyncResult = new HttpStreamAsyncResult();
			httpStreamAsyncResult.Callback = cback;
			httpStreamAsyncResult.State = state;
			if (no_more_data)
			{
				httpStreamAsyncResult.Complete();
				return httpStreamAsyncResult;
			}
			int num2 = decoder.Read(buffer, offset, count);
			offset += num2;
			count -= num2;
			if (count == 0)
			{
				httpStreamAsyncResult.Count = num2;
				httpStreamAsyncResult.Complete();
				return httpStreamAsyncResult;
			}
			if (!decoder.WantMore)
			{
				no_more_data = num2 == 0;
				httpStreamAsyncResult.Count = num2;
				httpStreamAsyncResult.Complete();
				return httpStreamAsyncResult;
			}
			httpStreamAsyncResult.Buffer = new byte[8192];
			httpStreamAsyncResult.Offset = 0;
			httpStreamAsyncResult.Count = 8192;
			ReadBufferState readBufferState = new ReadBufferState(buffer, offset, count, httpStreamAsyncResult);
			readBufferState.InitialCount += num2;
			base.BeginRead(httpStreamAsyncResult.Buffer, httpStreamAsyncResult.Offset, httpStreamAsyncResult.Count, (AsyncCallback)OnRead, (object)readBufferState);
			return httpStreamAsyncResult;
		}

		public override void Close()
		{
			if (!disposed)
			{
				disposed = true;
				base.Close();
			}
		}

		public override int EndRead(IAsyncResult ares)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (ares == null)
			{
				throw new ArgumentException("Invalid IAsyncResult.", "ares");
			}
			if (!ares.IsCompleted)
			{
				ares.AsyncWaitHandle.WaitOne();
			}
			HttpStreamAsyncResult httpStreamAsyncResult = ares as HttpStreamAsyncResult;
			if (httpStreamAsyncResult.Error != null)
			{
				throw new HttpListenerException(400, "I/O operation aborted.");
			}
			return httpStreamAsyncResult.Count;
		}

		public override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			IAsyncResult ares = BeginRead(buffer, offset, count, null, null);
			return EndRead(ares);
		}
	}
}
