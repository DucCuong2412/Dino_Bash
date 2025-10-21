using System;
using System.Collections.Generic;

namespace LeanplumSDK
{
	public abstract class WebRequest
	{
		protected string url;

		protected int timeout;

		internal WebRequest(string url, int timeout)
		{
			this.url = url;
			this.timeout = timeout;
		}

		internal void AttachGetParameters(IDictionary<string, string> parameters)
		{
			string text = string.Empty;
			if (parameters == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				if (parameter.Value == null)
				{
					LeanplumNative.CompatibilityLayer.LogWarning("Request param " + parameter.Key + " is null");
					continue;
				}
				text += ((text.Length != 0) ? '&' : '?');
				text = text + parameter.Key + "=" + LeanplumNative.CompatibilityLayer.URLEncode(parameter.Value);
			}
			url += text;
		}

		internal abstract void AttachPostParameters(IDictionary<string, string> parameters);

		internal abstract void AttachBinaryField(string key, byte[] data);

		internal abstract void GetResponseAsync(Action<WebResponse> responseHandler);

		internal abstract void GetAssetBundle(Action<WebResponse> responseHandler);
	}
}
