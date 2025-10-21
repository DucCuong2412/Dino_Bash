using System;
using UnityEngine;

public class WebViewObject : MonoBehaviour
{
	private Action<string> callback;

	private AndroidJavaObject webView;

	private bool mIsKeyboardVisible;

	public bool IsKeyboardVisible
	{
		get
		{
			return mIsKeyboardVisible;
		}
	}

	public void SetKeyboardVisible(string pIsVisible)
	{
		mIsKeyboardVisible = pIsVisible == "true";
	}

	public void Init(Action<string> cb = null)
	{
		callback = cb;
		webView = new AndroidJavaObject("net.gree.unitywebview.WebViewPlugin");
		webView.Call("Init", base.name);
	}

	private void OnDestroy()
	{
		if (webView != null)
		{
			webView.Call("Destroy");
			webView = null;
		}
	}

	public void SetCenterPositionWithScale(Vector2 center, Vector2 scale)
	{
	}

	public void SetMargins(int left, int top, int right, int bottom)
	{
		if (webView != null)
		{
			webView.Call("SetMargins", left, top, right, bottom);
		}
	}

	public void SetVisibility(bool v)
	{
		if (Application.platform != RuntimePlatform.WindowsEditor && webView != null)
		{
			webView.Call("SetVisibility", v);
		}
	}

	public void LoadURL(string url)
	{
		if (webView != null)
		{
			webView.Call("LoadURL", url);
		}
	}

	public void EvaluateJS(string js)
	{
		if (webView != null)
		{
			webView.Call("LoadURL", "javascript:" + js);
		}
	}

	public void CallFromJS(string message)
	{
		if (callback != null)
		{
			callback(message);
		}
	}
}
