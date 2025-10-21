using UnityEngine;

public static class ComponentExtensions
{
	public static T GetComponentUpwards<T>(this Component c) where T : Component
	{
		T component = c.GetComponent<T>();
		if ((Object)component != (Object)null)
		{
			return component;
		}
		if (c.transform.parent != null)
		{
			T componentUpwards = c.transform.parent.GetComponentUpwards<T>();
			if ((Object)componentUpwards != (Object)null)
			{
				return componentUpwards;
			}
		}
		return (T)null;
	}

	public static T AddOrGetComponent<T>(this Component c) where T : Component
	{
		T val = c.GetComponent<T>();
		if ((Object)val == (Object)null)
		{
			val = c.gameObject.AddComponent<T>();
		}
		return val;
	}
}
