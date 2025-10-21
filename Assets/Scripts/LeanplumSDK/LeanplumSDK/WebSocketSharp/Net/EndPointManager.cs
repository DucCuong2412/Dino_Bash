using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal sealed class EndPointManager
	{
		private static Dictionary<IPAddress, Dictionary<int, EndPointListener>> _ipToEndpoints = new Dictionary<IPAddress, Dictionary<int, EndPointListener>>();

		private EndPointManager()
		{
		}

		private static void addPrefix(string uriPrefix, HttpListener httpListener)
		{
			ListenerPrefix listenerPrefix = new ListenerPrefix(uriPrefix);
			if (listenerPrefix.Path.IndexOf('%') != -1)
			{
				throw new HttpListenerException(400, "Invalid path.");
			}
			if (listenerPrefix.Path.IndexOf("//", StringComparison.Ordinal) != -1)
			{
				throw new HttpListenerException(400, "Invalid path.");
			}
			EndPointListener endPointListener = getEndPointListener(IPAddress.Any, listenerPrefix.Port, httpListener, listenerPrefix.Secure);
			endPointListener.AddPrefix(listenerPrefix, httpListener);
		}

		private static EndPointListener getEndPointListener(IPAddress address, int port, HttpListener httpListener, bool secure)
		{
			Dictionary<int, EndPointListener> dictionary = null;
			if (_ipToEndpoints.ContainsKey(address))
			{
				dictionary = _ipToEndpoints[address];
			}
			else
			{
				dictionary = new Dictionary<int, EndPointListener>();
				_ipToEndpoints[address] = dictionary;
			}
			EndPointListener endPointListener = null;
			return dictionary.ContainsKey(port) ? dictionary[port] : (dictionary[port] = new EndPointListener(address, port, secure, httpListener.CertificateFolderPath, httpListener.DefaultCertificate));
		}

		private static void removePrefix(string uriPrefix, HttpListener httpListener)
		{
			ListenerPrefix listenerPrefix = new ListenerPrefix(uriPrefix);
			if (listenerPrefix.Path.IndexOf('%') == -1 && listenerPrefix.Path.IndexOf("//", StringComparison.Ordinal) == -1)
			{
				EndPointListener endPointListener = getEndPointListener(IPAddress.Any, listenerPrefix.Port, httpListener, listenerPrefix.Secure);
				endPointListener.RemovePrefix(listenerPrefix, httpListener);
			}
		}

		public static void AddListener(HttpListener httpListener)
		{
			//Discarded unreachable code: IL_009c
			List<string> list = new List<string>();
			lock (((ICollection)_ipToEndpoints).SyncRoot)
			{
				try
				{
					foreach (string prefix in httpListener.Prefixes)
					{
						addPrefix(prefix, httpListener);
						list.Add(prefix);
					}
				}
				catch
				{
					foreach (string item in list)
					{
						removePrefix(item, httpListener);
					}
					throw;
				}
			}
		}

		public static void AddPrefix(string uriPrefix, HttpListener httpListener)
		{
			lock (((ICollection)_ipToEndpoints).SyncRoot)
			{
				addPrefix(uriPrefix, httpListener);
			}
		}

		public static void RemoveEndPoint(EndPointListener epListener, IPEndPoint endpoint)
		{
			lock (((ICollection)_ipToEndpoints).SyncRoot)
			{
				Dictionary<int, EndPointListener> dictionary = _ipToEndpoints[endpoint.Address];
				dictionary.Remove(endpoint.Port);
				if (dictionary.Count == 0)
				{
					_ipToEndpoints.Remove(endpoint.Address);
				}
				epListener.Close();
			}
		}

		public static void RemoveListener(HttpListener httpListener)
		{
			lock (((ICollection)_ipToEndpoints).SyncRoot)
			{
				foreach (string prefix in httpListener.Prefixes)
				{
					removePrefix(prefix, httpListener);
				}
			}
		}

		public static void RemovePrefix(string uriPrefix, HttpListener httpListener)
		{
			lock (((ICollection)_ipToEndpoints).SyncRoot)
			{
				removePrefix(uriPrefix, httpListener);
			}
		}
	}
}
