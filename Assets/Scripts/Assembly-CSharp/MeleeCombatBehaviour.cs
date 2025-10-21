using System.Collections.Generic;
using UnityEngine;

public class MeleeCombatBehaviour : AbstractUnitCombatBehaviour
{
	public bool canMultiAttack = true;

	protected override void SyncToState(Unit unit, Unit.State state)
	{
		StopAllCoroutines();
		switch (state)
		{
		case Unit.State.stand:
		case Unit.State.move:
			base.CombatTargets.Clear();
			StartCoroutine(SearchForTarget());
			break;
		case Unit.State.combat:
			StartCoroutine(MonitorCombat());
			break;
		default:
			base.CombatTargets.Clear();
			break;
		}
	}

	public override void CombatAnimationEvent()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position + viewDirectionAndRange * 0.5f, viewDirectionAndRange.magnitude * 0.5f, base.RayCastMask);
		Debug.DrawLine(base.transform.position, base.transform.position + viewDirectionAndRange, Color.red, 0.5f);
		List<BaseEntity> list = AbstractCombatBehaviour.EntitiesForCollider(array);
		list.RemoveAll((BaseEntity entity) => !canHitUnit(entity.unitType));
		for (int i = 0; i < list.Count; i++)
		{
			BaseEntity baseEntity = list[i];
			if (baseEntity.unitType != UnitType.DinoEgg || list.Count <= 1 || base.Entity.unitType == UnitType.Neander_Disguise)
			{
				Attack(baseEntity);
				if (!canMultiAttack)
				{
					break;
				}
			}
		}
	}
}
