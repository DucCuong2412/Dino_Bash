using System;
using System.Security.Cryptography;
using System.Text;

namespace LeanplumSDK.WebSocketSharp
{
	internal class AuthenticationResponse
	{
		private string _algorithm;

		private string _cnonce;

		private string _method;

		private string _nc;

		private string _nonce;

		private string _opaque;

		private string _password;

		private string _qop;

		private string _realm;

		private string _response;

		private string _scheme;

		private string _uri;

		private string _userName;

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

		public string Cnonce
		{
			get
			{
				return _cnonce ?? string.Empty;
			}
			private set
			{
				_cnonce = value;
			}
		}

		public string Nc
		{
			get
			{
				return _nc ?? string.Empty;
			}
			private set
			{
				_nc = value;
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

		public string Response
		{
			get
			{
				return _response ?? string.Empty;
			}
			private set
			{
				_response = value;
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

		public string Uri
		{
			get
			{
				return _uri ?? string.Empty;
			}
			private set
			{
				_uri = value;
			}
		}

		public string UserName
		{
			get
			{
				return _userName ?? string.Empty;
			}
			private set
			{
				_userName = value;
			}
		}

		private AuthenticationResponse()
		{
		}

		public AuthenticationResponse(WsCredential credential)
		{
			_userName = credential.UserName;
			_password = credential.Password;
			_scheme = "Basic";
		}

		public AuthenticationResponse(WsCredential credential, AuthenticationChallenge challenge)
		{
			_userName = credential.UserName;
			_password = credential.Password;
			_scheme = challenge.Scheme;
			_realm = challenge.Realm;
			if (_scheme == "Digest")
			{
				initForDigest(credential, challenge);
			}
		}

		private string a1()
		{
			string text = string.Format("{0}:{1}:{2}", _userName, _realm, _password);
			return (_algorithm == null || !(_algorithm.ToLower() == "md5-sess")) ? text : string.Format("{0}:{1}:{2}", hash(text), _nonce, _cnonce);
		}

		private string a2()
		{
			return string.Format("{0}:{1}", _method, _uri);
		}

		private static string createNonceValue()
		{
			byte[] array = new byte[16];
			Random random = new Random();
			random.NextBytes(array);
			StringBuilder stringBuilder = new StringBuilder(32);
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private string createRequestDigest()
		{
			if (Qop == "auth")
			{
				string data = string.Format("{0}:{1}:{2}:{3}:{4}", _nonce, _nc, _cnonce, _qop, hash(a2()));
				return kd(hash(a1()), data);
			}
			return kd(hash(a1()), string.Format("{0}:{1}", _nonce, hash(a2())));
		}

		private static string hash(string value)
		{
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			byte[] array = mD.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder(64);
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private void initForDigest(WsCredential credential, AuthenticationChallenge challenge)
		{
			_nonce = challenge.Nonce;
			_method = "GET";
			_uri = credential.Domain;
			_algorithm = challenge.Algorithm;
			_opaque = challenge.Opaque;
			string[] array = challenge.Qop.Split(',');
			foreach (string text in array)
			{
				if (text.Trim().ToLower() == "auth")
				{
					_qop = "auth";
					_nc = "00000001";
					break;
				}
			}
			_cnonce = createNonceValue();
			_response = createRequestDigest();
		}

		private static string kd(string secret, string data)
		{
			string value = string.Format("{0}:{1}", secret, data);
			return hash(value);
		}

		private string toBasicCredentials()
		{
			string s = string.Format("{0}:{1}", _userName, _password);
			string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
			return "Basic " + text;
		}

		private string toDigestCredentials()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("username={0}", _userName.Quote());
			stringBuilder.AppendFormat(", realm={0}", _realm.Quote());
			stringBuilder.AppendFormat(", nonce={0}", _nonce.Quote());
			stringBuilder.AppendFormat(", uri={0}", _uri.Quote());
			stringBuilder.AppendFormat(", response={0}", _response.Quote());
			if (!_algorithm.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat(", algorithm={0}", _algorithm);
			}
			if (!_opaque.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat(", opaque={0}", _opaque.Quote());
			}
			if (!_qop.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat(", qop={0}", _qop);
			}
			if (!_nc.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat(", nc={0}", _nc);
			}
			if (!_qop.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat(", cnonce={0}", _cnonce.Quote());
			}
			return "Digest " + stringBuilder.ToString();
		}

		public static AuthenticationResponse Parse(string response)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return (!(_scheme == "Basic")) ? toDigestCredentials() : toBasicCredentials();
		}
	}
}
