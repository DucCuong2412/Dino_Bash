using System;

namespace LeanplumSDK.WebSocketSharp
{
	internal class AuthenticationChallenge
	{
		private string _algorithm;

		private string _domain;

		private string _nonce;

		private string _opaque;

		private string _qop;

		private string _realm;

		private string _scheme;

		private string _stale;

		public string Algorithm
		{
			get
			{
				return _algorithm ?? string.Empty;
			}
			private set
			{
				_algorithm = value;
			}
		}

		public string Domain
		{
			get
			{
				return _domain ?? string.Empty;
			}
			private set
			{
				_domain = value;
			}
		}

		public string Nonce
		{
			get
			{
				return _nonce ?? string.Empty;
			}
			private set
			{
				_nonce = value;
			}
		}

		public string Opaque
		{
			get
			{
				return _opaque ?? string.Empty;
			}
			private set
			{
				_opaque = value;
			}
		}

		public string Qop
		{
			get
			{
				return _qop ?? string.Empty;
			}
			private set
			{
				_qop = value;
			}
		}

		public string Realm
		{
			get
			{
				return _realm ?? string.Empty;
			}
			private set
			{
				_realm = value;
			}
		}

		public string Scheme
		{
			get
			{
				return _scheme ?? string.Empty;
			}
			private set
			{
				_scheme = value;
			}
		}

		public string Stale
		{
			get
			{
				return _stale ?? string.Empty;
			}
			private set
			{
				_stale = value;
			}
		}

		private AuthenticationChallenge()
		{
		}

		public static AuthenticationChallenge Parse(string challenge)
		{
			AuthenticationChallenge authenticationChallenge = new AuthenticationChallenge();
			if (challenge.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
			{
				authenticationChallenge.Scheme = "Basic";
				authenticationChallenge.Realm = challenge.Substring(6).GetValueInternal("=").Trim('"');
				return authenticationChallenge;
			}
			foreach (string item in challenge.SplitHeaderValue(','))
			{
				string text = item.Trim();
				if (text.StartsWith("digest", StringComparison.OrdinalIgnoreCase))
				{
					authenticationChallenge.Scheme = "Digest";
					authenticationChallenge.Realm = text.Substring(7).GetValueInternal("=").Trim('"');
					continue;
				}
				string text2 = text.GetValueInternal("=").Trim('"');
				if (text.StartsWith("domain", StringComparison.OrdinalIgnoreCase))
				{
					authenticationChallenge.Domain = text2;
				}
				else if (text.StartsWith("nonce", StringComparison.OrdinalIgnoreCase))
				{
					authenticationChallenge.Nonce = text2;
				}
				else if (text.StartsWith("opaque", StringComparison.OrdinalIgnoreCase))
				{
					authenticationChallenge.Opaque = text2;
				}
				else if (text.StartsWith("stale", StringComparison.OrdinalIgnoreCase))
				{
					authenticationChallenge.Stale = text2;
				}
				else if (text.StartsWith("algorithm", StringComparison.OrdinalIgnoreCase))
				{
					authenticationChallenge.Algorithm = text2;
				}
				else if (text.StartsWith("qop", StringComparison.OrdinalIgnoreCase))
				{
					authenticationChallenge.Qop = text2;
				}
			}
			return authenticationChallenge;
		}
	}
}
