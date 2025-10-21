using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractProjectile : MonoBase
{
	[SerializeField]
	private GameObject projectile;

	[SerializeField]
	private ParticleSystem hitFx;

	[SerializeField]
	private ParticleSystem trailFx;

	[SerializeField]
	protected bool canMultiAttack = true;

	[SerializeField]
	private bool doesDirectDamage = true;

	public CombatEffects combatEffect = CombatEffects.none;

	public float combatEffectDuration = 5f;

	protected bool hitGround;

	public Unit unit { get; private set; }

	protected bool isInitalized { get; private set; }

	public event Action onProjectileFired;

	public virtual void Fire(Vector3 start, float targetVelocity)
	{
		Init();
		if (this.onProjectileFired != null)
		{
			this.onProjectileFired();
		}
		ShowTrailFX();
		StartCoroutine(CheckLevelBounds());
	}

	private void Init()
	{
		unit = base.transform.parent.GetComponent<Unit>();
		projectile.gameObject.SetActive(true);
		if (!isInitalized)
		{
			isInitalized = true;
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			foreach (Renderer target in componentsInChildren)
			{
				SpriteTools.TargetSetSortingLayerID(target, 10);
			}
			hitFx.gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		projectile.SetActive(true);
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator CheckLevelBounds()
	{
		do
		{
			yield return new WaitForSeconds(0.2f);
		}
		while (Level.Instance.inLevelBounds(base.transform.position.x));
		Free();
	}

	protected virtual void OnCollisionEnter(Collision pOther)
	{
		if (!hitGround && pOther.gameObject.tag == "Ground")
		{
			hitGround = true;
			ShowHitFX();
		}
	}

	protected virtual void Attack()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, projectile.GetComponent<Collider>().bounds.extents.x * 1.2f, this.unit.CombatBehaviour.RayCastMask);
		List<BaseEntity> list = AbstractCombatBehaviour.EntitiesForCollider(array);
		for (int i = 0; i < list.Count; i++)
		{
			BaseEntity baseEntity = list[i];
			if ((baseEntity.unitType == UnitType.DinoEgg && list.Count > 1) || !this.unit.CombatBehaviour.canHitUnit(baseEntity.unitType))
			{
				continue;
			}
			AbstractCombatEffect.Apply(combatEffect, baseEntity, combatEffectDuration, this.unit.Config.attackStrengh);
			if (doesDirectDamage)
			{
				this.unit.CombatBehaviour.Attack(baseEntity);
			}
			if (baseEntity != null && baseEntity is Unit)
			{
				Unit unit = baseEntity as Unit;
				if (unit.state == Unit.State.fly)
				{
					unit.stateFall();
				}
			}
			if (!canMultiAttack)
			{
				break;
			}
		}
	}

	private IEnumerator WaitForRemoval(float delay)
	{
		yield return new WaitForSeconds(delay);
		Free();
	}

	protected void Free()
	{
		StopAllCoroutines();
		hitFx.gameObject.SetActive(false);
		hitGround = true;
		ObjectPool.Recycle(base.transform);
	}

	private void ShowTrailFX()
	{
		if (trailFx != null)
		{
			trailFx.Clear();
			trailFx.enableEmission = true;
			trailFx.Play();
		}
	}

	protected void ShowHitFX()
	{
		projectile.SetActive(false);
		if (trailFx != null)
		{
			trailFx.enableEmission = false;
		}
		if (hitFx != null)
		{
			hitFx.gameObject.SetActive(true);
			StartCoroutine(WaitForRemoval(hitFx.duration));
		}
		else
		{
			Free();
		}
	}
}
