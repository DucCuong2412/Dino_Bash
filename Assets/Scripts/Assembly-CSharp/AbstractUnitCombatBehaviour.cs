using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractUnitCombatBehaviour : GenericCombatBehaviour<Unit>
{
	protected Vector3 viewDirectionAndRange;

	public List<BaseEntity> CombatTargets { get; protected set; }

	protected float spawnpointXPosition
	{
		get
		{
			float num = 0f;
			if (base.Entity.Config.isFriendly)
			{
				float num2 = ((!(this is MeleeCombatBehaviour)) ? (-500) : 0);
				return EntityFactory.dinoSpawnRoot.position.x + num2;
			}
			num = (float)Level.Instance.Config.levelwidth - 160f;
			return num - base.Entity.RenderBounds.extents.x * 0.5f;
		}
	}

	protected abstract void SyncToState(Unit unit, Unit.State state);

	protected virtual IEnumerator SearchForTarget()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.2f);
			while (!hasLeftSpawnPoint())
			{
				yield return new WaitForSeconds(0.1f);
			}
			Debug.DrawRay(base.transform.position, viewDirectionAndRange, Color.red, 0.2f);
			Collider[] hitColliders = Physics.OverlapSphere(base.transform.position + viewDirectionAndRange * 0.5f, viewDirectionAndRange.magnitude * 0.5f, base.RayCastMask);
			if (hitColliders.Length > 0)
			{
				List<BaseEntity> targets = AbstractCombatBehaviour.EntitiesForCollider(hitColliders);
				targets.Find((BaseEntity target) => ChangedToAttack(target));
			}
		}
	}

	private bool ChangedToAttack(BaseEntity entity)
	{
		if (isValidAttackTarget(entity.unitType))
		{
			CombatTargets.Add(entity);
			base.Entity.stateCombat();
			return true;
		}
		return false;
	}

	protected bool hasLeftSpawnPoint()
	{
		float f = base.transform.position.x - spawnpointXPosition;
		float num = ((!base.Entity.Config.isFriendly) ? (-1f) : 1f);
		if (Mathf.Sign(f) != num)
		{
			Debug.DrawLine(new Vector3(spawnpointXPosition, 2048f), new Vector3(spawnpointXPosition, -1024f), Color.blue, 1f);
			if (base.Entity.state != Unit.State.move)
			{
				base.Entity.stateMove();
			}
			return false;
		}
		return true;
	}

	protected virtual IEnumerator MonitorCombat()
	{
		while (CombatTargets[0].Health > 0 && !PassedTarget() && CombatTargets[0].gameObject.activeInHierarchy && isInAttackRange() && isValidAttackTarget(CombatTargets[0].unitType))
		{
			yield return null;
		}
		CombatTargets.Clear();
		base.Entity.stateStand();
	}

	protected bool isInAttackRange()
	{
		if (CombatTargets.Count == 0)
		{
			return false;
		}
		float num = base.transform.position.x + viewDirectionAndRange.x;
		float num2 = CombatTargets[0].transform.position.x + (float)((!base.Entity.Config.isFriendly) ? 1 : (-1)) * CombatTargets[0].RenderBounds.extents.x;
		if (base.Entity.Config.isFriendly)
		{
			return num > num2;
		}
		return num < num2;
	}

	protected bool PassedTarget()
	{
		if (CombatTargets.Count == 0)
		{
			return false;
		}
		if (base.Entity.Config.isFriendly)
		{
			return CombatTargets[0].transform.position.x < base.Entity.transform.position.x;
		}
		return CombatTargets[0].transform.position.x > base.Entity.transform.position.x;
	}

	public override void Init()
	{
		if (isInititalized)
		{
			return;
		}
		base.Init();
		CombatTargets = new List<BaseEntity>();
		RuntimeAnimatorController runtimeAnimatorController = base.Entity.animator.runtimeAnimatorController;
		if (runtimeAnimatorController is AnimatorOverrideController)
		{
			AnimationClipPair[] clips = (runtimeAnimatorController as AnimatorOverrideController).clips;
			AnimationClipPair[] array = clips;
			foreach (AnimationClipPair animationClipPair in array)
			{
				if (animationClipPair.originalClip.name == "dummy_attack")
				{
					if (animationClipPair.overrideClip != null)
					{
						AttackActionsPerSecond = animationClipPair.overrideClip.length;
					}
					break;
				}
			}
		}
		base.Entity.OnStateChanged += SyncToState;
		base.Entity.OnAnimationEvent += OnAnimationEvent;
		Action value = delegate
		{
			if (base.Entity.gameObject.activeSelf)
			{
				base.Entity.AllowStandToWalk = false;
				base.Entity.stateStand();
			}
		};
		Level.Instance.OnLevelWon += value;
		Level.Instance.OnLevelLost += value;
		float num = (float)base.Entity.Config.attackRange + base.Entity.bodyCollider.bounds.extents.x + base.Entity.bodyCollider.center.x;
		viewDirectionAndRange = ((!base.Entity.Config.isFriendly) ? new Vector3(0f - num, 0f, 0f) : new Vector3(num, 0f, 0f));
	}

	protected virtual void OnDestroy()
	{
		if (base.Entity != null)
		{
			base.Entity.OnStateChanged -= SyncToState;
			base.Entity.OnAnimationEvent -= OnAnimationEvent;
		}
	}

	public virtual void CombatAnimationEvent()
	{
	}

	private void OnAnimationEvent(UnitAnimationEvents anim_event)
	{
		if (anim_event == UnitAnimationEvents.UnitAttack && base.Entity.state == Unit.State.combat && CombatTargets != null)
		{
			base.Entity.PlaySound(EntitySound.ATTACK);
			CombatAnimationEvent();
		}
	}
}
