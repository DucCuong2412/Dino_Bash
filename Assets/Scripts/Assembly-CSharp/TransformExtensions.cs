using UnityEngine;

public static class TransformExtensions
{
	public static Vector3 PosX(this Transform transform, float x)
	{
		transform.position = new Vector3(x, transform.position.y, transform.position.z);
		return transform.position;
	}

	public static Vector3 PosY(this Transform transform, float y)
	{
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
		return transform.position;
	}

	public static Vector3 PosZ(this Transform transform, float z)
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, z);
		return transform.position;
	}

	public static Vector3 LocalPosX(this Transform transform, float x)
	{
		transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
		return transform.localPosition;
	}

	public static Vector3 LocalPosY(this Transform transform, float y)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
		return transform.localPosition;
	}

	public static Vector3 LocalPosZ(this Transform transform, float z)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
		return transform.localPosition;
	}

	public static Transform Search(this Transform transform, string name)
	{
		if (transform.name.Equals(name))
		{
			return transform;
		}
		foreach (Transform item in transform)
		{
			Transform transform3 = item.Search(name);
			if (transform3 != null)
			{
				return transform3;
			}
		}
		return null;
	}

	public static T Search<T>(this Transform transform, string name) where T : Component
	{
		T[] componentsInChildren = transform.GetComponentsInChildren<T>();
		if (componentsInChildren == null)
		{
			return (T)null;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].transform.name.Equals(name))
			{
				return componentsInChildren[i];
			}
		}
		return (T)null;
	}

	public static string GetPath(this Transform current)
	{
		if (current.parent == null)
		{
			return "/" + current.name;
		}
		return current.parent.GetPath() + "/" + current.name;
	}

	public static Transform RepositionAndReparent(this Transform current, Transform target, bool makeChild = false)
	{
		Transform parent = target;
		if (!makeChild)
		{
			parent = target.parent;
		}
		current.transform.parent = parent;
		current.transform.localPosition = target.localPosition;
		return current;
	}

	public static void SetLayer(this Transform current, int layer, bool changeChildren = true)
	{
		if (changeChildren)
		{
			Transform[] componentsInChildren = current.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				transform.gameObject.layer = layer;
			}
		}
		current.gameObject.layer = layer;
	}
}
