using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCombatBehaviour : MonoBase
{
	public abstract void Init();

	public static BaseEntity EntityForCollider(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			return attachedRigidbody.GetComponent<BaseEntity>();
		}
		return null;
	}

	public static List<BaseEntity> EntitiesForCollider(Collider[] collider)
	{
		List<BaseEntity> list = new List<BaseEntity>();
		for (int i = 0; i < collider.Length; i++)
		{
			BaseEntity baseEntity = EntityForCollider(collider[i]);
			if (baseEntity != null && !list.Contains(baseEntity))
			{
				list.Add(baseEntity);
			}
		}
		return list;
	}
}
