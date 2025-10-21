using System.Collections.Generic;
using UnityEngine;

public class ExplodeCombatBehaviour : GenericCombatBehaviour<BaseEntity>
{
	public override void Init()
	{
		base.Init();
		base.RayCastMask = 2048;
		base.Entity.OnEntityDied += OnEntityDied;
	}

	private void OnEntityDied()
	{
		base.Entity.OnEntityDied -= OnEntityDied;
		base.Entity.FireAnimationEvent(UnitAnimationEvents.UnitAttack);
		Vector3 vector = new Vector3(base.Entity.Config.attackRange, 0f, 0f);
		Collider[] array = Physics.OverlapSphere(base.transform.position, base.Entity.Config.attackRange, base.RayCastMask);
		Debug.DrawLine(base.transform.position - vector, base.transform.position + vector, Color.red, 0.5f);
		List<BaseEntity> list = AbstractCombatBehaviour.EntitiesForCollider(array);
		list.FindAll((BaseEntity x) => x != base.Entity).ForEach(delegate(BaseEntity x)
		{
			Attack(x);
		});
		base.GetComponent<Collider>().enabled = false;
		foreach (Transform item in base.transform)
		{
			if (item.tag == "EntityGraphics")
			{
				item.gameObject.SetActive(false);
			}
		}
	}
}
