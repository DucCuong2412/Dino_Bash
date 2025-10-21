using System;
using System.Net.Sockets;

namespace LeanplumSDK
{
	public class SocketAsyncEventArgsWrapper : ISocketAsyncEventArgs
	{
		private SocketAsyncEventArgs args;

		public SocketAsyncEventArgs WrappedArgs
		{
			get
			{
				return args;
			}
		}

		public object UserToken
		{
			get
			{
				return args.UserToken;
			}
			set
			{
				args.UserToken = value;
			}
		}

		public ISocket AcceptSocket
		{
			get
			{
				return new SocketWrapper(args.AcceptSocket);
			}
			set
			{
				if (value == null)
				{
					args.AcceptSocket = null;
				}
				else
				{
					args.AcceptSocket = ((SocketWrapper)value).WrappedSocket;
				}
			}
		}

		public event EventHandler<EventArgs> Completed;

		public SocketAsyncEventArgsWrapper()
		{
			args = new SocketAsyncEventArgs();
			args.Completed += onAccept;
		}

		public SocketAsyncEventArgsWrapper(EventArgs e)
		{
			args = (SocketAsyncEventArgs)e;
		}

		public bool IsSuccess()
		{
			return args.SocketError == SocketError.Success;
		}

		private void onAccept(object sender, EventArgs e)
		{
			this.Completed(sender, e);
		}
	}
}
