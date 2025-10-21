using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanplumSDK.SocketIOClient
{
	internal class SocketIOHandshake
	{
		public List<string> Transports = new List<string>();

		public string SID { get; set; }

		public int HeartbeatTimeout { get; set; }

		public string ErrorMessage { get; set; }

		public bool HadError
		{
			get
			{
				return !string.IsNullOrEmpty(ErrorMessage);
			}
		}

		public TimeSpan HeartbeatInterval
		{
			get
			{
				return new TimeSpan(0, 0, HeartbeatTimeout);
			}
		}

		public int ConnectionTimeout { get; set; }

		public static SocketIOHandshake LoadFromString(string value)
		{
			SocketIOHandshake socketIOHandshake = new SocketIOHandshake();
			if (!string.IsNullOrEmpty(value))
			{
				string[] array = value.Split(':');
				if (array.Count() == 4)
				{
					int result = 0;
					int result2 = 0;
					socketIOHandshake.SID = array[0];
					if (int.TryParse(array[1], out result))
					{
						int heartbeatTimeout = (int)((double)result * 0.75);
						socketIOHandshake.HeartbeatTimeout = heartbeatTimeout;
					}
					if (int.TryParse(array[2], out result2))
					{
						socketIOHandshake.ConnectionTimeout = result2;
					}
					socketIOHandshake.Transports.AddRange(array[3].Split(','));
					return socketIOHandshake;
				}
			}
			return null;
		}
	}
}
