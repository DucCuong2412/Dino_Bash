namespace LeanplumSDK.WebSocketSharp.Net
{
	internal class HttpHeaderInfo
	{
		public bool IsMultiValueInRequest
		{
			get
			{
				return (Type & HttpHeaderType.MultiValueInRequest) == HttpHeaderType.MultiValueInRequest;
			}
		}

		public bool IsMultiValueInResponse
		{
			get
			{
				return (Type & HttpHeaderType.MultiValueInResponse) == HttpHeaderType.MultiValueInResponse;
			}
		}

		public bool IsRequest
		{
			get
			{
				return (Type & HttpHeaderType.Request) == HttpHeaderType.Request;
			}
		}

		public bool IsResponse
		{
			get
			{
				return (Type & HttpHeaderType.Response) == HttpHeaderType.Response;
			}
		}

		public string Name { get; set; }

		public HttpHeaderType Type { get; set; }

		public bool IsMultiValue(bool response)
		{
			return ((Type & HttpHeaderType.MultiValue) != HttpHeaderType.MultiValue) ? ((!response) ? IsMultiValueInRequest : IsMultiValueInResponse) : ((!response) ? IsRequest : IsResponse);
		}

		public bool IsRestricted(bool response)
		{
			return (Type & HttpHeaderType.Restricted) == HttpHeaderType.Restricted && ((!response) ? IsRequest : IsResponse);
		}
	}
}
