namespace LeanplumSDK.WebSocketSharp
{
	internal class WsCredential
	{
		private string _domain;

		private string _password;

		private string _userName;

		public string Domain
		{
			get
			{
				return _domain ?? string.Empty;
			}
			internal set
			{
				_domain = value;
			}
		}

		public string Password
		{
			get
			{
				return _password ?? string.Empty;
			}
			internal set
			{
				_password = value;
			}
		}

		public string UserName
		{
			get
			{
				return _userName ?? string.Empty;
			}
			internal set
			{
				_userName = value;
			}
		}

		internal WsCredential()
		{
		}

		internal WsCredential(string userName, string password)
			: this(userName, password, null)
		{
		}

		internal WsCredential(string userName, string password, string domain)
		{
			_userName = userName;
			_password = password;
			_domain = domain;
		}
	}
}
