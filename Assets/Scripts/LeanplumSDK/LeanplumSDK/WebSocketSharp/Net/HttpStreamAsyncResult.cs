using System;
using System.Threading;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal class HttpStreamAsyncResult : IAsyncResult
	{
		private bool completed;

		private ManualResetEvent handle;

		private object locker = new object();

		internal AsyncCallback Callback;

		internal int Count;

		internal byte[] Buffer;

		internal Exception Error;

		internal int Offset;

		internal object State;

		internal int SyncRead;

		public object AsyncState
		{
			get
			{
				return State;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				lock (locker)
				{
					if (handle == null)
					{
						handle = new ManualResetEvent(completed);
					}
				}
				return handle;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return SyncRead == Count;
			}
		}

		public bool IsCompleted
		{
			get
			{
				//Discarded unreachable code: IL_0019
				lock (locker)
				{
					return completed;
				}
			}
		}

		public void Complete()
		{
			lock (locker)
			{
				if (!completed)
				{
					completed = true;
					if (handle != null)
					{
						handle.Set();
					}
					if (Callback != null)
					{
						Callback.BeginInvoke(this, null, null);
					}
				}
			}
		}

		public void Complete(Exception e)
		{
			Error = e;
			Complete();
		}
	}
}
