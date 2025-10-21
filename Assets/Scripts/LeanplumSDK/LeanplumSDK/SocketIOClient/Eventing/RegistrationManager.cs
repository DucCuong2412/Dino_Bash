using System;
using System.Collections.Generic;
using LeanplumSDK.SocketIOClient.Messages;

namespace LeanplumSDK.SocketIOClient.Eventing
{
	internal class RegistrationManager : IDisposable
	{
		private Dictionary<int, Action<object>> callBackRegistry;

		private Dictionary<string, Action<IMessage>> eventNameRegistry;

		public RegistrationManager()
		{
			callBackRegistry = new Dictionary<int, Action<object>>();
			eventNameRegistry = new Dictionary<string, Action<IMessage>>();
		}

		public void AddCallBack(IMessage message)
		{
			EventMessage eventMessage = message as EventMessage;
			if (eventMessage != null)
			{
				callBackRegistry.Add(eventMessage.AckId.Value, eventMessage.Callback);
			}
		}

		public void AddCallBack(int ackId, Action<object> callback)
		{
			callBackRegistry.Add(ackId, callback);
		}

		public void InvokeCallBack(int? ackId, string value)
		{
			Action<object> value2 = null;
			if (ackId.HasValue && callBackRegistry.TryGetValue(ackId.Value, out value2))
			{
				value2.BeginInvoke(value, value2.EndInvoke, null);
			}
		}

		public void InvokeCallBack(int? ackId, JsonEncodedEventMessage value)
		{
			Action<object> value2 = null;
			if (ackId.HasValue && callBackRegistry.TryGetValue(ackId.Value, out value2))
			{
				value2(value);
			}
		}

		public void AddOnEvent(string eventName, Action<IMessage> callback)
		{
			eventNameRegistry.Add(eventName, callback);
		}

		public void AddOnEvent(string eventName, string endPoint, Action<IMessage> callback)
		{
			eventNameRegistry.Add(string.Format("{0}::{1}", eventName, endPoint), callback);
		}

		public bool InvokeOnEvent(IMessage value)
		{
			bool result = false;
			try
			{
				string key = value.Event;
				if (!string.IsNullOrEmpty(value.Endpoint))
				{
					key = string.Format("{0}::{1}", value.Event, value.Endpoint);
				}
				Action<IMessage> value2;
				if (eventNameRegistry.TryGetValue(key, out value2))
				{
					result = true;
					value2(value);
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			callBackRegistry.Clear();
			eventNameRegistry.Clear();
		}
	}
}
