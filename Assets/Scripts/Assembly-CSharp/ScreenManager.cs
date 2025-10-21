using System;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenManager
{
	public const int maxSortingLayers = 46;

	private static LinkedList<BaseScreen> screens = new LinkedList<BaseScreen>();

	private static Transform ui_root;

	public static Camera Camera { get; private set; }

	public static Transform UI_Root
	{
		get
		{
			if (ui_root == null)
			{
				UI_Root = new GameObject("UI_Root").transform;
			}
			return ui_root;
		}
		private set
		{
			ui_root = value;
		}
	}

	public static void Push(BaseScreen screen, BaseScreen after_this_screen = null)
	{
		int num = ((!(after_this_screen != null) || !screens.Contains(after_this_screen)) ? screens.Count : (after_this_screen.SortingLayerID + 1));
		if (num > 46)
		{
			num = 0;
			Debug.LogError(screen.name + " pushing to default layer - maybe out of sortinglayers");
		}
		if (after_this_screen == null)
		{
			screen.internal_SetSortingLayerID(num);
			screens.AddLast(screen);
		}
		else
		{
			LinkedListNode<BaseScreen> node = screens.Find(after_this_screen);
			screens.AddAfter(node, screen);
			UpdateScreenOrder(num);
		}
	}

	public static void Remove(BaseScreen screen)
	{
		if (screens.Contains(screen))
		{
			screens.Remove(screen);
		}
	}

	private static void UpdateScreenOrder(int reorderFrom)
	{
		LinkedListNode<BaseScreen> linkedListNode = screens.First;
		for (int i = 0; i < screens.Count; i++)
		{
			if (i >= reorderFrom)
			{
				linkedListNode.Value.internal_SetSortingLayerID(i);
			}
			linkedListNode.Value.transform.SetAsLastSibling();
			linkedListNode = linkedListNode.Next;
		}
	}

	public static T GetScreen<T>(BaseScreen above_this_screen = null) where T : BaseScreen
	{
		LinkedListNode<BaseScreen> linkedListNode = null;
		linkedListNode = ((!(above_this_screen == null)) ? screens.Find(above_this_screen) : screens.Last);
		while (linkedListNode != null)
		{
			LinkedListNode<BaseScreen> previous = linkedListNode.Previous;
			if (linkedListNode.Value is T && linkedListNode.Value != above_this_screen)
			{
				return linkedListNode.Value as T;
			}
			linkedListNode = previous;
		}
		return (T)null;
	}

	public static List<T> GetScreensOfType<T>(BaseScreen above_this_screen = null) where T : BaseScreen
	{
		LinkedListNode<BaseScreen> linkedListNode = null;
		List<T> list = new List<T>();
		linkedListNode = ((!(above_this_screen == null)) ? screens.Find(above_this_screen) : screens.Last);
		while (linkedListNode != null)
		{
			LinkedListNode<BaseScreen> previous = linkedListNode.Previous;
			if (linkedListNode.Value is T)
			{
				list.Add(linkedListNode.Value as T);
			}
			linkedListNode = previous;
		}
		return list;
	}

	public static bool isTopmostScreen(BaseScreen screenInstance)
	{
		for (LinkedListNode<BaseScreen> linkedListNode = screens.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
		{
			if (linkedListNode.Value.isVisible)
			{
				if (linkedListNode.Value == screenInstance)
				{
					return true;
				}
				return false;
			}
		}
		throw new NullReferenceException();
	}

	public static void BringToFront(BaseScreen screen)
	{
		int sortingLayerID = screen.SortingLayerID;
		screens.Remove(screen);
		screens.AddLast(screen);
		UpdateScreenOrder(sortingLayerID);
	}

	public static void Initialize()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("GUI/UICamera", typeof(GameObject))) as GameObject;
		gameObject.camera.farClipPlane = 24064f;
		gameObject.name = "UI_Camera";
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		Camera = gameObject.camera;
	}

	public static T LoadAndPush<T>(string prefabName = null, BaseScreen after_this_screen = null) where T : BaseScreen
	{
		T val = Load<T>(prefabName);
		Push(val, after_this_screen);
		return val;
	}

	public static T Load<T>(string prefabName = null) where T : BaseScreen
	{
		if (prefabName == null)
		{
			prefabName = typeof(T).Name;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("GUI/" + prefabName)) as GameObject;
		if (gameObject == null)
		{
			throw new NullReferenceException();
		}
		gameObject.name = typeof(T).Name;
		T val = gameObject.GetComponent<T>();
		if ((UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			val = gameObject.AddComponent<T>();
		}
		val.transform.parent = UI_Root;
		return val;
	}
}
