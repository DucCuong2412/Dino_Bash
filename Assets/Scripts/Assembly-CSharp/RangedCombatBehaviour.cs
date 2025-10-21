using System;
using UnityEngine;

public class RangedCombatBehaviour : AbstractUnitCombatBehaviour
{
	[SerializeField]
	private GameObject projectile;

	[SerializeField]
	private string projectileSpawnPointName;

	private Transform projectileSpawnPoint;

	public override void Init()
	{
		if (!isInititalized)
		{
			base.Init();
			if (projectile == null)
			{
				throw new Exception("no projectile found!");
			}
			projectileSpawnPoint = base.transform.Search(projectileSpawnPointName);
			if (projectileSpawnPoint == null)
			{
				throw new Exception("spawnpoint not found!");
			}
			ObjectPool.CreatePool(projectile.transform);
		}
	}

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
			StartCoroutine(MonitorCombat());
			break;
		default:
			base.CombatTargets.Clear();
			break;
		}
	}

	public override void CombatAnimationEvent()
	{
		GameObject gameObject = ObjectPool.Spawn(projectile.transform).gameObject;
		gameObject.SetActive(true);
		gameObject.transform.parent = base.transform;
		float targetVelocity = ((!base.CombatTargets[0].Config.isFriendly) ? (0f - base.CombatTargets[0].Config.walkspeed) : base.CombatTargets[0].Config.walkspeed);
		gameObject.GetComponent<AbstractProjectile>().Fire(projectileSpawnPoint.position, targetVelocity);
		if (base.CombatTargets[0].unitType == UnitType.DinoEgg)
		{
			WaitThen(0.3f, base.Entity.stateStand);
		}
	}
}
