using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace LeanplumSDK
{
	public class LeanplumUnityHelper : MonoBehaviour
	{
		private enum ModalType
		{
			Message = 0,
			MessageWithText = 1
		}

		private class Modal
		{
			public ModalType Type;

			public Action<string> Callback;

			public string Title;

			public string Message;

			public string TextResponse;
		}

		private static LeanplumUnityHelper instance;

		internal static List<Action> delayed = new List<Action>();

		private Modal activeModal;

		private bool developerModeEnabled;

		public static LeanplumUnityHelper Instance
		{
			get
			{
				if (instance != null)
				{
					return instance;
				}
				instance = UnityEngine.Object.FindObjectOfType(typeof(LeanplumUnityHelper)) as LeanplumUnityHelper;
				GameObject gameObject = new GameObject("LeanplumUnityHelper", typeof(LeanplumUnityHelper));
				instance = gameObject.GetComponent<LeanplumUnityHelper>();
				if (instance == null)
				{
					LeanplumNative.CompatibilityLayer.LogError("Problem during the creation of LeanplumUnityHelper.");
				}
				return instance;
			}
			private set
			{
				instance = value;
			}
		}

		public void NativeCallback(string message)
		{
			LeanplumFactory.SDK.NativeCallback(message);
		}

		private void Start()
		{
			developerModeEnabled = Leanplum.IsDeveloperModeEnabled;
			activeModal = null;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		private void OnGUI()
		{
			if (activeModal != null)
			{
				Rect clientRect = MakeRectAtCenter(350, 150);
				GUI.ModalWindow(0, clientRect, DrawModal, activeModal.Title);
			}
		}

		private void OnApplicationQuit()
		{
			LeanplumNative.CompatibilityLayer.FlushSavedSettings();
			if (LeanplumNative.calledStart)
			{
				LeanplumNative.Stop();
			}
			LeanplumNative.isStopped = true;
		}

		private void OnApplicationPause(bool isPaused)
		{
			if (LeanplumNative.calledStart)
			{
				if (isPaused)
				{
					LeanplumNative.Pause();
				}
				else
				{
					LeanplumNative.Resume();
				}
			}
		}

		private void Update()
		{
			if (VarCache.VarsNeedUpdate && developerModeEnabled && Leanplum.HasStarted)
			{
				VarCache.CheckVarsUpdate();
			}
			List<Action> list;
			lock (delayed)
			{
				list = new List<Action>(delayed);
				delayed.Clear();
			}
			foreach (Action item in list)
			{
				item();
			}
		}

		internal void StartRequest(string url, WWWForm wwwForm, Action<WebResponse> responseHandler, int timeout, bool isAsset = false)
		{
			StartCoroutine(RunRequest(url, wwwForm, responseHandler, timeout, isAsset));
		}

		private static IEnumerator RunRequest(string url, WWWForm wwwForm, Action<WebResponse> responseHandler, int timeout, bool isAsset)
		{
			WWW www = ((!isAsset) ? ((wwwForm != null) ? new WWW(url, wwwForm) : new WWW(url)) : WWW.LoadFromCacheOrDownload(url, 1));
			Timer timeoutTimer = new Timer(timeout * 1000);
			Action<WebResponse> responseHandler2 = default(Action<WebResponse>);
			timeoutTimer.Elapsed += delegate
			{
				timeoutTimer.Stop();
				www.Dispose();
				QueueOnMainThread(delegate
				{
					responseHandler2(new UnityWebResponse("Request timed out.", string.Empty, null));
				});
			};
			timeoutTimer.Start();
			yield return www;
			if (timeoutTimer.Enabled)
			{
				timeoutTimer.Stop();
				responseHandler(new UnityWebResponse(www.error, (!string.IsNullOrEmpty(www.error) || isAsset) ? null : www.text, (!string.IsNullOrEmpty(www.error)) ? null : www.assetBundle));
				www.Dispose();
			}
		}

		internal void DisplayMessageModal(string title, string message)
		{
			activeModal = new Modal
			{
				Title = title,
				Message = message
			};
		}

		internal void DisplayTextModal(string title, string message, Action<string> callback)
		{
			activeModal = new Modal
			{
				Title = title,
				Message = message,
				Callback = callback,
				TextResponse = string.Empty,
				Type = ModalType.MessageWithText
			};
		}

		private Rect MakeRectAtCenter(int width, int height)
		{
			return new Rect((Screen.width - width) / 2, (Screen.height - height) / 3, width, height);
		}

		private void DrawModal(int windowID)
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label(activeModal.Message);
			if (activeModal.Type == ModalType.MessageWithText)
			{
				GUILayout.FlexibleSpace();
				activeModal.TextResponse = GUILayout.TextField(activeModal.TextResponse);
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Close"))
			{
				activeModal = null;
			}
			if (activeModal != null && activeModal.Type == ModalType.MessageWithText && GUILayout.Button("Submit") && activeModal.TextResponse != string.Empty)
			{
				if (activeModal.Callback != null)
				{
					activeModal.Callback(activeModal.TextResponse);
				}
				activeModal = null;
			}
			GUILayout.EndHorizontal();
		}

		internal static void QueueOnMainThread(Action method)
		{
			lock (delayed)
			{
				delayed.Add(method);
			}
		}
	}
}
