using UnityEngine;

public class FlyerCombatBehaviour : MeleeCombatBehaviour
{
	public Collider flyCollider;

	public float flySpeed = 400f;

	public override void Init()
	{
		base.Init();
		flyCollider.enabled = false;
		flyCollider.isTrigger = true;
		flyCollider.gameObject.layer = base.gameObject.layer;
	}

	protected override void SyncToState(Unit unit, Unit.State state)
	{
		StopAllCoroutines();
		if (base.Entity.lastState == Unit.State.fly)
		{
			base.Entity.Speed = new Vector3(Mathf.Sign(base.Entity.Speed.x) * base.Entity.Config.walkspeed, 0f, 0f);
		}
		switch (state)
		{
		case Unit.State.fly:
			base.Entity.Speed = new Vector3(Mathf.Sign(base.Entity.Speed.x) * flySpeed, 0f, 0f);
			flyCollider.enabled = true;
			base.Entity.bodyCollider.enabled = false;
			break;
		case Unit.State.die:
		case Unit.State.fall:
			flyCollider.enabled = false;
			base.Entity.bodyCollider.enabled = true;
			break;
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
}
