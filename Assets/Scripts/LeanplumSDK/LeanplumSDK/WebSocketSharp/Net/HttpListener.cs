using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace LeanplumSDK.WebSocketSharp.Net
{
	internal sealed class HttpListener : IDisposable
	{
		private AuthenticationSchemes auth_schemes;

		private AuthenticationSchemeSelector auth_selector;

		private string cert_folder_path;

		private Dictionary<HttpConnection, HttpConnection> connections;

		private List<HttpListenerContext> ctx_queue;

		private X509Certificate2 default_cert;

		private bool disposed;

		private bool ignore_write_exceptions;

		private bool listening;

		private HttpListenerPrefixCollection prefixes;

		private string realm;

		private Dictionary<HttpListenerContext, HttpListenerContext> registry;

		private bool unsafe_ntlm_auth;

		private List<ListenerAsyncResult> wait_queue;

		public AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				CheckDisposed();
				return auth_schemes;
			}
			set
			{
				CheckDisposed();
				auth_schemes = value;
			}
		}

		public AuthenticationSchemeSelector AuthenticationSchemeSelectorDelegate
		{
			get
			{
				CheckDisposed();
				return auth_selector;
			}
			set
			{
				CheckDisposed();
				auth_selector = value;
			}
		}

		public string CertificateFolderPath
		{
			get
			{
				CheckDisposed();
				if (cert_folder_path.IsNullOrEmpty())
				{
					cert_folder_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				}
				return cert_folder_path;
			}
			set
			{
				CheckDisposed();
				cert_folder_path = value;
			}
		}

		public X509Certificate2 DefaultCertificate
		{
			get
			{
				CheckDisposed();
				return default_cert;
			}
			set
			{
				CheckDisposed();
				default_cert = value;
			}
		}

		public bool IgnoreWriteExceptions
		{
			get
			{
				CheckDisposed();
				return ignore_write_exceptions;
			}
			set
			{
				CheckDisposed();
				ignore_write_exceptions = value;
			}
		}

		public bool IsListening
		{
			get
			{
				return listening;
			}
		}

		public static bool IsSupported
		{
			get
			{
				return true;
			}
		}

		public HttpListenerPrefixCollection Prefixes
		{
			get
			{
				CheckDisposed();
				return prefixes;
			}
		}

		public string Realm
		{
			get
			{
				CheckDisposed();
				return realm;
			}
			set
			{
				CheckDisposed();
				realm = value;
			}
		}

		public bool UnsafeConnectionNtlmAuthentication
		{
			get
			{
				CheckDisposed();
				return unsafe_ntlm_auth;
			}
			set
			{
				CheckDisposed();
				unsafe_ntlm_auth = value;
			}
		}

		public HttpListener()
		{
			prefixes = new HttpListenerPrefixCollection(this);
			registry = new Dictionary<HttpListenerContext, HttpListenerContext>();
			connections = new Dictionary<HttpConnection, HttpConnection>();
			ctx_queue = new List<HttpListenerContext>();
			wait_queue = new List<ListenerAsyncResult>();
			auth_schemes = AuthenticationSchemes.Anonymous;
		}

		void IDisposable.Dispose()
		{
			if (!disposed)
			{
				Close(true);
				disposed = true;
			}
		}

		private void Cleanup(bool force)
		{
			lock (((ICollection)registry).SyncRoot)
			{
				if (!force)
				{
					SendServiceUnavailable();
				}
				CleanupContextRegistry();
				CleanupConnections();
				CleanupWaitQueue();
			}
		}

		private void CleanupConnections()
		{
			lock (((ICollection)connections).SyncRoot)
			{
				if (connections.Count != 0)
				{
					ICollection keys = connections.Keys;
					HttpConnection[] array = new HttpConnection[keys.Count];
					keys.CopyTo(array, 0);
					connections.Clear();
					for (int num = array.Length - 1; num >= 0; num--)
					{
						array[num].Close(true);
					}
				}
			}
		}

		private void CleanupContextRegistry()
		{
			lock (((ICollection)registry).SyncRoot)
			{
				if (registry.Count != 0)
				{
					ICollection keys = registry.Keys;
					HttpListenerContext[] array = new HttpListenerContext[keys.Count];
					keys.CopyTo(array, 0);
					registry.Clear();
					for (int num = array.Length - 1; num >= 0; num--)
					{
						array[num].Connection.Close(true);
					}
				}
			}
		}

		private void CleanupWaitQueue()
		{
			lock (((ICollection)wait_queue).SyncRoot)
			{
				if (wait_queue.Count == 0)
				{
					return;
				}
				ObjectDisposedException exc = new ObjectDisposedException(GetType().ToString());
				foreach (ListenerAsyncResult item in wait_queue)
				{
					item.Complete(exc);
				}
				wait_queue.Clear();
			}
		}

		private void Close(bool force)
		{
			EndPointManager.RemoveListener(this);
			Cleanup(force);
		}

		private HttpListenerContext GetContextFromQueue()
		{
			if (ctx_queue.Count == 0)
			{
				return null;
			}
			HttpListenerContext result = ctx_queue[0];
			ctx_queue.RemoveAt(0);
			return result;
		}

		private void SendServiceUnavailable()
		{
			lock (((ICollection)ctx_queue).SyncRoot)
			{
				if (ctx_queue.Count != 0)
				{
					HttpListenerContext[] array = ctx_queue.ToArray();
					ctx_queue.Clear();
					HttpListenerContext[] array2 = array;
					foreach (HttpListenerContext httpListenerContext in array2)
					{
						HttpListenerResponse response = httpListenerContext.Response;
						response.StatusCode = 503;
						response.Close();
					}
				}
			}
		}

		internal void AddConnection(HttpConnection cnc)
		{
			connections[cnc] = cnc;
		}

		internal void CheckDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}

		internal void RegisterContext(HttpListenerContext context)
		{
			lock (((ICollection)registry).SyncRoot)
			{
				registry[context] = context;
			}
			ListenerAsyncResult listenerAsyncResult = null;
			lock (((ICollection)wait_queue).SyncRoot)
			{
				if (wait_queue.Count == 0)
				{
					lock (((ICollection)ctx_queue).SyncRoot)
					{
						ctx_queue.Add(context);
					}
				}
				else
				{
					listenerAsyncResult = wait_queue[0];
					wait_queue.RemoveAt(0);
				}
			}
			if (listenerAsyncResult != null)
			{
				listenerAsyncResult.Complete(context);
			}
		}

		internal void RemoveConnection(HttpConnection cnc)
		{
			connections.Remove(cnc);
		}

		internal AuthenticationSchemes SelectAuthenticationScheme(HttpListenerContext context)
		{
			if (AuthenticationSchemeSelectorDelegate != null)
			{
				return AuthenticationSchemeSelectorDelegate(context.Request);
			}
			return auth_schemes;
		}

		internal void UnregisterContext(HttpListenerContext context)
		{
			lock (((ICollection)registry).SyncRoot)
			{
				registry.Remove(context);
			}
			lock (((ICollection)ctx_queue).SyncRoot)
			{
				int num = ctx_queue.IndexOf(context);
				if (num >= 0)
				{
					ctx_queue.RemoveAt(num);
				}
			}
		}

		public void Abort()
		{
			if (!disposed)
			{
				Close(true);
				disposed = true;
			}
		}

		public IAsyncResult BeginGetContext(AsyncCallback callback, object state)
		{
			CheckDisposed();
			if (!listening)
			{
				throw new InvalidOperationException("Please, call Start before using this method.");
			}
			ListenerAsyncResult listenerAsyncResult = new ListenerAsyncResult(callback, state);
			lock (((ICollection)wait_queue).SyncRoot)
			{
				lock (((ICollection)ctx_queue).SyncRoot)
				{
					HttpListenerContext contextFromQueue = GetContextFromQueue();
					if (contextFromQueue != null)
					{
						listenerAsyncResult.Complete(contextFromQueue, true);
						return listenerAsyncResult;
					}
				}
				wait_queue.Add(listenerAsyncResult);
				return listenerAsyncResult;
			}
		}

		public void Close()
		{
			if (!disposed)
			{
				Close(false);
				disposed = true;
			}
		}

		public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
		{
			CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			ListenerAsyncResult listenerAsyncResult = asyncResult as ListenerAsyncResult;
			if (listenerAsyncResult == null)
			{
				throw new ArgumentException("Wrong IAsyncResult.", "asyncResult");
			}
			if (listenerAsyncResult.EndCalled)
			{
				throw new InvalidOperationException("Cannot reuse this IAsyncResult.");
			}
			listenerAsyncResult.EndCalled = true;
			if (!listenerAsyncResult.IsCompleted)
			{
				listenerAsyncResult.AsyncWaitHandle.WaitOne();
			}
			lock (((ICollection)wait_queue).SyncRoot)
			{
				int num = wait_queue.IndexOf(listenerAsyncResult);
				if (num >= 0)
				{
					wait_queue.RemoveAt(num);
				}
			}
			HttpListenerContext context = listenerAsyncResult.GetContext();
			context.ParseAuthentication(SelectAuthenticationScheme(context));
			return context;
		}

		public HttpListenerContext GetContext()
		{
			if (prefixes.Count == 0)
			{
				throw new InvalidOperationException("Please, call AddPrefix before using this method.");
			}
			ListenerAsyncResult listenerAsyncResult = (ListenerAsyncResult)BeginGetContext(null, null);
			listenerAsyncResult.InGet = true;
			return EndGetContext(listenerAsyncResult);
		}

		public void Start()
		{
			CheckDisposed();
			if (!listening)
			{
				EndPointManager.AddListener(this);
				listening = true;
			}
		}

		public void Stop()
		{
			CheckDisposed();
			if (listening)
			{
				listening = false;
				EndPointManager.RemoveListener(this);
				SendServiceUnavailable();
			}
		}
	}
}
