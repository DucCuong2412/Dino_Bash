using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerBehaviour : AbstractUnitCombatBehaviour
{
	[SerializeField]
	private FX_EmitOnTarget fx_Heal;

	private float lastWaitForFront;

	private List<BaseEntity> targets = new List<BaseEntity>();

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
		base.CombatTargets = GetDamagedUnits();
		if (base.CombatTargets.Count > 0)
		{
			Heal(base.CombatTargets);
			fx_Heal.Emit(base.CombatTargets.ToArray());
		}
	}

	protected override IEnumerator SearchForTarget()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.2f);
			while (!hasLeftSpawnPoint())
			{
				yield return new WaitForSeconds(0.1f);
			}
			targets = GetDamagedUnits();
			if (targets.Count > 0)
			{
				base.CombatTargets = targets;
				base.Entity.stateCombat();
				yield break;
			}
			Unit frontUnit = GetFrontUnit();
			if (frontUnit != null && frontUnit != base.Entity)
			{
				if (Mathf.Abs(base.transform.position.x - frontUnit.transform.position.x) > (float)base.Entity.Config.attackRange * 0.75f)
				{
					if (base.Entity.state != Unit.State.move && Time.time > lastWaitForFront)
					{
						base.Entity.stateMove();
						yield break;
					}
				}
				else if (base.Entity.state != Unit.State.stand)
				{
					lastWaitForFront = Time.time + 2f;
					base.Entity.stateStand();
					yield break;
				}
			}
			else
			{
				if (base.Entity.state != Unit.State.stand && base.Entity.hasReachedLevelEnd(0f))
				{
					base.Entity.stateStand();
					yield break;
				}
				if (base.Entity.state != Unit.State.move && !base.Entity.hasReachedLevelEnd(0f))
				{
					break;
				}
			}
		}
		base.Entity.stateMove();
	}

	protected override IEnumerator MonitorCombat()
	{
		while (true)
		{
			base.CombatTargets = base.CombatTargets.FindAll((BaseEntity x) => Mathf.Abs(x.transform.position.x - base.transform.position.x) <= (float)base.Entity.Config.attackRange && x.Health < x.max_health && x.Health > 0);
			if (base.CombatTargets.Count == 0)
			{
				break;
			}
			yield return new WaitForSeconds(1f);
		}
		base.CombatTargets.Clear();
		base.Entity.stateStand();
	}

	private List<BaseEntity> GetDamagedUnits()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, (float)base.Entity.Config.attackRange * 0.75f, base.RayCastMask);
		List<BaseEntity> list = AbstractCombatBehaviour.EntitiesForCollider(array);
		return list.FindAll((BaseEntity target) => target is Unit && 0 < target.Health && target.Health < target.max_health && target.unitType != UnitType.Neander_Catapult && target.unitType != UnitType.Neander_Healer);
	}

	private Unit GetFrontUnit()
	{
		List<BaseEntity> entities = EntityFactory.GetEntities(base.Entity.Config.isFriendly);
		entities = entities.FindAll((BaseEntity x) => x is Unit);
		float num = 0f;
		int index = 0;
		Func<int, float> func = (int x) => Mathf.Abs(entities[x].transform.position.x - base.spawnpointXPosition);
		for (int i = 0; i < entities.Count; i++)
		{
			if (func(i) > num)
			{
				num = func(i);
				index = i;
			}
		}
		return (Unit)entities[index];
	}

	public override void Init()
	{
		base.Init();
		base.Entity.AllowStandToWalk = false;
		fx_Heal = UnityEngine.Object.Instantiate(fx_Heal) as FX_EmitOnTarget;
		fx_Heal.transform.RepositionAndReparent(base.transform, true);
		SpriteTools.TargetSetSortingLayerID(fx_Heal, 10);
		base.RayCastMask = 1 << base.gameObject.layer;
	}
}
