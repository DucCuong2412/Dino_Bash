using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericCombatBehaviour<T> : AbstractCombatBehaviour where T : BaseEntity
{
	private BitArray ignoredUnits;

	private float[] hitModifier;

	protected bool isInititalized;

	protected float AttackActionsPerSecond = 1f;

	public float AttackModifier = 1f;

	protected T Entity { get; private set; }

	public int RayCastMask { get; protected set; }

	public override void Init()
	{
		if (!isInititalized)
		{
			isInititalized = true;
			Entity = GetComponent<T>();
			T entity = Entity;
			ignoredUnits = HitMasks.GetIgnoreMask((int)entity.unitType);
			T entity2 = Entity;
			hitModifier = HitMasks.GetHitModifier((int)entity2.unitType);
			SetRayCastLayer();
		}
	}

	public bool isValidAttackTarget(UnitType target)
	{
		return !ignoredUnits[(int)target];
	}

	public bool canHitUnit(UnitType target)
	{
		return (double)hitModifier[(int)target] < 1.0;
	}

	public bool isInRayCastMask(int raycast_layer)
	{
		return ((1 << raycast_layer) & RayCastMask) > 0;
	}

	public void SetIgnoredMaskForUnit(UnitType entity, bool state)
	{
		ignoredUnits[(int)entity] = state;
	}

	public void SetCannotHitMaskForUnit(UnitType entity, bool state)
	{
		hitModifier[(int)entity] = ((!state) ? 1f : 0f);
	}

	public void Attack(BaseEntity target)
	{
		if (canHitUnit(target.unitType))
		{
			T entity = Entity;
			target.ChangeHealth(-Mathf.RoundToInt((float)entity.Config.attackStrengh * AttackActionsPerSecond * AttackModifier * (1f - hitModifier[(int)target.unitType])), Entity);
		}
	}

	public void Attack(List<BaseEntity> targets)
	{
		for (int i = 0; i < targets.Count; i++)
		{
			Attack(targets[i]);
		}
	}

	protected void Heal(BaseEntity target)
	{
		if (canHitUnit(target.unitType))
		{
			T entity = Entity;
			target.ChangeHealth(Mathf.RoundToInt((float)entity.Config.attackStrengh * AttackActionsPerSecond * (1f - hitModifier[(int)target.unitType])), Entity);
		}
	}

	public void Heal(List<BaseEntity> targets)
	{
		for (int i = 0; i < targets.Count; i++)
		{
			Heal(targets[i]);
		}
	}

	private void SetRayCastLayer()
	{
		T entity = Entity;
		if (entity.Config.isFriendly)
		{
			base.gameObject.layer = 10;
			RayCastMask = 2048;
		}
		else
		{
			base.gameObject.layer = 11;
			RayCastMask = 1024;
		}
	}
}
