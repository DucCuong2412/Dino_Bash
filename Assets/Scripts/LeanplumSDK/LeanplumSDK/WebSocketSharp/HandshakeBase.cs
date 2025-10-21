using System;
using System.Collections.Specialized;
using System.Text;
using LeanplumSDK.WebSocketSharp.Net;

namespace LeanplumSDK.WebSocketSharp
{
	internal abstract class HandshakeBase
	{
		protected const string CrLf = "\r\n";

		public NameValueCollection Headers { get; protected set; }

		public Version ProtocolVersion { get; protected set; }

		protected HandshakeBase()
		{
			ProtocolVersion = HttpVersion.Version11;
			Headers = new NameValueCollection();
		}

		public void AddHeader(string name, string value)
		{
			Headers.Add(name, value);
		}

		public bool ContainsHeader(string name)
		{
			return Headers.Contains(name);
		}

		public bool ContainsHeader(string name, string value)
		{
			return Headers.Contains(name, value);
		}

		public string[] GetHeaderValues(string name)
		{
			return Headers.GetValues(name);
		}

		public byte[] ToByteArray()
		{
			return Encoding.UTF8.GetBytes(ToString());
		}
	}
}
