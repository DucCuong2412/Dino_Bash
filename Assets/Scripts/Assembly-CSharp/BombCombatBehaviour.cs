using UnityEngine;

public class BombCombatBehaviour : AbstractUnitCombatBehaviour
{
	protected override void SyncToState(Unit unit, Unit.State pState)
	{
		StopAllCoroutines();
		switch (pState)
		{
		case Unit.State.stand:
		case Unit.State.move:
			base.CombatTargets.Clear();
			StartCoroutine(SearchForTarget());
			break;
		case Unit.State.combat:
			break;
		default:
			base.CombatTargets.Clear();
			break;
		}
	}

	public override void CombatAnimationEvent()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, base.Entity.Config.attackRange, base.RayCastMask);
		Debug.DrawLine(base.transform.position - viewDirectionAndRange, base.transform.position + viewDirectionAndRange, Color.red, 0.5f);
		for (int i = 0; i < array.Length; i++)
		{
			BaseEntity component = array[i].gameObject.GetComponent<BaseEntity>();
			Attack(component);
		}
		base.Entity.stateDie();
	}
}
