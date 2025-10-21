using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal sealed class EndPointListener
	{
		private List<ListenerPrefix> _all;

		private X509Certificate2 _cert;

		private IPEndPoint _endpoint;

		private Dictionary<ListenerPrefix, HttpListener> _prefixes;

		private bool _secure;

		private ISocket _socket;

		private List<ListenerPrefix> _unhandled;

		private Dictionary<HttpConnection, HttpConnection> _unregistered;

		public EndPointListener(IPAddress address, int port, bool secure, string certFolderPath, X509Certificate2 defaultCert)
		{
			if (secure)
			{
				_secure = secure;
				_cert = getCertificate(port, certFolderPath, defaultCert);
				if (_cert == null)
				{
					throw new ArgumentException("Server certificate not found.");
				}
			}
			_endpoint = new IPEndPoint(address, port);
			_socket = SocketUtilsFactory.Utils.CreateSocket(address);
			_socket.Bind(_endpoint);
			_socket.Listen(500);
			ISocketAsyncEventArgs socketAsyncEventArgs = SocketUtilsFactory.Utils.CreateSocketAsyncEventArgs();
			socketAsyncEventArgs.UserToken = this;
			socketAsyncEventArgs.Completed += onAccept;
			_socket.AcceptAsync(socketAsyncEventArgs);
			_prefixes = new Dictionary<ListenerPrefix, HttpListener>();
			_unregistered = new Dictionary<HttpConnection, HttpConnection>();
		}

		private static void addSpecial(List<ListenerPrefix> prefixes, ListenerPrefix prefix)
		{
			if (prefixes == null)
			{
				return;
			}
			foreach (ListenerPrefix prefix2 in prefixes)
			{
				if (prefix2.Path == prefix.Path)
				{
					throw new HttpListenerException(400, "Prefix already in use.");
				}
			}
			prefixes.Add(prefix);
		}

		private void checkIfRemove()
		{
			if (_prefixes.Count <= 0 && (_unhandled == null || _unhandled.Count <= 0) && (_all == null || _all.Count <= 0))
			{
				EndPointManager.RemoveEndPoint(this, _endpoint);
			}
		}

		private static RSACryptoServiceProvider createRSAFromFile(string filename)
		{
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			byte[] array = null;
			using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
			}
			rSACryptoServiceProvider.ImportCspBlob(array);
			return rSACryptoServiceProvider;
		}

		private static X509Certificate2 getCertificate(int port, string certFolderPath, X509Certificate2 defaultCert)
		{
			try
			{
				string text = Path.Combine(certFolderPath, string.Format("{0}.cer", port));
				string text2 = Path.Combine(certFolderPath, string.Format("{0}.key", port));
				if (File.Exists(text) && File.Exists(text2))
				{
					X509Certificate2 x509Certificate = new X509Certificate2(text);
					x509Certificate.PrivateKey = createRSAFromFile(text2);
					return x509Certificate;
				}
			}
			catch
			{
			}
			return defaultCert;
		}

		private static HttpListener matchFromList(string host, string path, List<ListenerPrefix> list, out ListenerPrefix prefix)
		{
			prefix = null;
			if (list == null)
			{
				return null;
			}
			HttpListener result = null;
			int num = -1;
			foreach (ListenerPrefix item in list)
			{
				string path2 = item.Path;
				if (path2.Length >= num && path.StartsWith(path2))
				{
					num = path2.Length;
					result = item.Listener;
					prefix = item;
				}
			}
			return result;
		}

		private static void onAccept(object sender, EventArgs e)
		{
			//Discarded unreachable code: IL_0056
			ISocketAsyncEventArgs socketAsyncEventArgs = SocketUtilsFactory.Utils.CreateSocketAsyncEventArgs(e);
			EndPointListener endPointListener = (EndPointListener)socketAsyncEventArgs.UserToken;
			ISocket socket = null;
			if (socketAsyncEventArgs.IsSuccess())
			{
				socket = socketAsyncEventArgs.AcceptSocket;
				socketAsyncEventArgs.AcceptSocket = null;
			}
			try
			{
				endPointListener._socket.AcceptAsync(socketAsyncEventArgs);
			}
			catch
			{
				if (socket != null)
				{
					socket.Close();
				}
				return;
			}
			if (socket == null)
			{
				return;
			}
			HttpConnection httpConnection = null;
			try
			{
				httpConnection = new HttpConnection(socket, endPointListener, endPointListener._secure, endPointListener._cert);
				lock (((ICollection)endPointListener._unregistered).SyncRoot)
				{
					endPointListener._unregistered[httpConnection] = httpConnection;
				}
				httpConnection.BeginReadRequest();
			}
			catch
			{
				if (httpConnection != null)
				{
					httpConnection.Close(true);
				}
				else
				{
					socket.Close();
				}
			}
		}

		private static bool removeSpecial(List<ListenerPrefix> prefixes, ListenerPrefix prefix)
		{
			if (prefixes == null)
			{
				return false;
			}
			int count = prefixes.Count;
			for (int i = 0; i < count; i++)
			{
				if (prefixes[i].Path == prefix.Path)
				{
					prefixes.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		private HttpListener searchListener(Uri uri, out ListenerPrefix prefix)
		{
			prefix = null;
			if (uri == null)
			{
				return null;
			}
			string host = uri.Host;
			int port = uri.Port;
			string text = HttpUtility.UrlDecode(uri.AbsolutePath);
			string text2 = ((text[text.Length - 1] != '/') ? (text + "/") : text);
			HttpListener result = null;
			int num = -1;
			if (host != null && host.Length > 0)
			{
				foreach (ListenerPrefix key in _prefixes.Keys)
				{
					string path = key.Path;
					if (path.Length >= num && !(key.Host != host) && key.Port == port && (text.StartsWith(path) || text2.StartsWith(path)))
					{
						num = path.Length;
						result = _prefixes[key];
						prefix = key;
					}
				}
				if (num != -1)
				{
					return result;
				}
			}
			List<ListenerPrefix> unhandled = _unhandled;
			result = matchFromList(host, text, unhandled, out prefix);
			if (text != text2 && result == null)
			{
				result = matchFromList(host, text2, unhandled, out prefix);
			}
			if (result != null)
			{
				return result;
			}
			unhandled = _all;
			result = matchFromList(host, text, unhandled, out prefix);
			if (text != text2 && result == null)
			{
				result = matchFromList(host, text2, unhandled, out prefix);
			}
			if (result != null)
			{
				return result;
			}
			return null;
		}

		internal static bool CertificateExists(int port, string certFolderPath)
		{
			string path = Path.Combine(certFolderPath, string.Format("{0}.cer", port));
			string path2 = Path.Combine(certFolderPath, string.Format("{0}.key", port));
			return File.Exists(path) && File.Exists(path2);
		}

		internal void RemoveConnection(HttpConnection connection)
		{
			lock (((ICollection)_unregistered).SyncRoot)
			{
				_unregistered.Remove(connection);
			}
		}

		public void AddPrefix(ListenerPrefix prefix, HttpListener listener)
		{
			if (prefix.Host == "*")
			{
				List<ListenerPrefix> unhandled;
				List<ListenerPrefix> list;
				do
				{
					unhandled = _unhandled;
					list = ((unhandled == null) ? new List<ListenerPrefix>() : new List<ListenerPrefix>(unhandled));
					prefix.Listener = listener;
					addSpecial(list, prefix);
				}
				while (Interlocked.CompareExchange(ref _unhandled, list, unhandled) != unhandled);
				return;
			}
			if (prefix.Host == "+")
			{
				List<ListenerPrefix> unhandled;
				List<ListenerPrefix> list;
				do
				{
					unhandled = _all;
					list = ((unhandled == null) ? new List<ListenerPrefix>() : new List<ListenerPrefix>(unhandled));
					prefix.Listener = listener;
					addSpecial(list, prefix);
				}
				while (Interlocked.CompareExchange(ref _all, list, unhandled) != unhandled);
				return;
			}
			Dictionary<ListenerPrefix, HttpListener> prefixes;
			Dictionary<ListenerPrefix, HttpListener> dictionary;
			do
			{
				prefixes = _prefixes;
				if (prefixes.ContainsKey(prefix))
				{
					HttpListener httpListener = prefixes[prefix];
					if (httpListener != listener)
					{
						throw new HttpListenerException(400, "There's another listener for " + prefix);
					}
					break;
				}
				dictionary = new Dictionary<ListenerPrefix, HttpListener>(prefixes);
				dictionary[prefix] = listener;
			}
			while (Interlocked.CompareExchange(ref _prefixes, dictionary, prefixes) != prefixes);
		}

		public bool BindContext(HttpListenerContext context)
		{
			HttpListenerRequest request = context.Request;
			ListenerPrefix prefix;
			HttpListener httpListener = searchListener(request.Url, out prefix);
			if (httpListener == null)
			{
				return false;
			}
			context.Listener = httpListener;
			context.Connection.Prefix = prefix;
			return true;
		}

		public void Close()
		{
			_socket.Close();
			lock (((ICollection)_unregistered).SyncRoot)
			{
				Dictionary<HttpConnection, HttpConnection> dictionary = new Dictionary<HttpConnection, HttpConnection>(_unregistered);
				foreach (HttpConnection key in dictionary.Keys)
				{
					key.Close(true);
				}
				dictionary.Clear();
				_unregistered.Clear();
			}
		}

		public void RemovePrefix(ListenerPrefix prefix, HttpListener listener)
		{
			if (prefix.Host == "*")
			{
				List<ListenerPrefix> unhandled;
				List<ListenerPrefix> list;
				do
				{
					unhandled = _unhandled;
					list = ((unhandled == null) ? new List<ListenerPrefix>() : new List<ListenerPrefix>(unhandled));
				}
				while (removeSpecial(list, prefix) && Interlocked.CompareExchange(ref _unhandled, list, unhandled) != unhandled);
				checkIfRemove();
				return;
			}
			if (prefix.Host == "+")
			{
				List<ListenerPrefix> unhandled;
				List<ListenerPrefix> list;
				do
				{
					unhandled = _all;
					list = ((unhandled == null) ? new List<ListenerPrefix>() : new List<ListenerPrefix>(unhandled));
				}
				while (removeSpecial(list, prefix) && Interlocked.CompareExchange(ref _all, list, unhandled) != unhandled);
				checkIfRemove();
				return;
			}
			Dictionary<ListenerPrefix, HttpListener> prefixes;
			Dictionary<ListenerPrefix, HttpListener> dictionary;
			do
			{
				prefixes = _prefixes;
				if (!prefixes.ContainsKey(prefix))
				{
					break;
				}
				dictionary = new Dictionary<ListenerPrefix, HttpListener>(prefixes);
				dictionary.Remove(prefix);
			}
			while (Interlocked.CompareExchange(ref _prefixes, dictionary, prefixes) != prefixes);
			checkIfRemove();
		}

		public void UnbindContext(HttpListenerContext context)
		{
			if (context != null && context.Request != null)
			{
				context.Listener.UnregisterContext(context);
			}
		}
	}
}
