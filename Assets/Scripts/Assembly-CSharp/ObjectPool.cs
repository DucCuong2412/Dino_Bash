using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPool : MonoBehaviour
{
	private static ObjectPool _instance;

	private Dictionary<Transform, List<Transform>> objectLookup = new Dictionary<Transform, List<Transform>>();

	private Dictionary<Transform, Transform> prefabLookup = new Dictionary<Transform, Transform>();

	public static ObjectPool instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			GameObject gameObject = new GameObject("_ObjectPool");
			gameObject.transform.localPosition = Vector3.zero;
			_instance = gameObject.AddComponent<ObjectPool>();
			return _instance;
		}
	}

	public static void Clear()
	{
		instance.objectLookup.Clear();
		instance.prefabLookup.Clear();
	}

	public static void CreatePool(Transform prefab)
	{
		if (!instance.objectLookup.ContainsKey(prefab))
		{
			instance.objectLookup.Add(prefab, new List<Transform>());
		}
	}

	public static Transform Spawn(Transform prefab, Vector3 position, Quaternion rotation)
	{
		if (instance.objectLookup.ContainsKey(prefab))
		{
			Transform transform = null;
			List<Transform> list = instance.objectLookup[prefab];
			if (list.Count > 0)
			{
				while (transform == null && list.Count > 0)
				{
					transform = list[0];
					list.RemoveAt(0);
				}
				if (transform != null)
				{
					transform.transform.parent = null;
					transform.transform.localPosition = position;
					transform.transform.localRotation = rotation;
					transform.gameObject.SetActive(true);
					instance.prefabLookup.Add(transform, prefab);
					return transform;
				}
			}
			transform = (Transform)UnityEngine.Object.Instantiate(prefab, position, rotation);
			instance.prefabLookup.Add(transform, prefab);
			return transform;
		}
		throw new Exception("prefab not in pool!");
	}

	public static Transform Spawn(Transform prefab, Vector3 position)
	{
		return Spawn(prefab, position, Quaternion.identity);
	}

	public static Transform Spawn(Transform prefab)
	{
		return Spawn(prefab, Vector3.zero, Quaternion.identity);
	}

	public static void Recycle(Transform obj)
	{
		if (instance.prefabLookup.ContainsKey(obj))
		{
			try
			{
				instance.objectLookup[instance.prefabLookup[obj]].Add(obj);
				instance.prefabLookup.Remove(obj);
				obj.parent = instance.transform;
				obj.gameObject.SetActive(false);
				return;
			}
			catch (Exception ex)
			{
				Debug.LogError(obj.gameObject.name + " - " + ex.Message + "\n" + ex.StackTrace);
				return;
			}
		}
		Debug.LogError("Object not in pool - destroying");
		UnityEngine.Object.Destroy(obj.gameObject);
	}

	private IEnumerator DisableObj(Transform obj)
	{
		yield return new WaitForEndOfFrame();
		try
		{
			instance.objectLookup[instance.prefabLookup[obj]].Add(obj);
			instance.prefabLookup.Remove(obj);
			obj.parent = instance.transform;
			obj.gameObject.SetActive(false);
		}
		catch (Exception e)
		{
			Debug.LogError(obj.gameObject.name + " - " + e.Message + "\n" + e.StackTrace);
		}
	}

	public static int Count(Transform obj)
	{
		if (instance.objectLookup.ContainsKey(obj))
		{
			return instance.objectLookup[obj].Count;
		}
		return 0;
	}
}
