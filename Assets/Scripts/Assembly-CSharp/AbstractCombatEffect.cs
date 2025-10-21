using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractCombatEffect : MonoBase
{
	protected static ParticleSystem FX_Prefab;

	protected ParticleSystem fx;

	protected float currentDuration;

	protected float totalDuration;

	protected float updateFrequency = 0.1f;

	protected BaseEntity TargetEntitiy { get; private set; }

	public bool isRunning { get; protected set; }

	protected bool isInit { get; private set; }

	protected void Init()
	{
		base.enabled = true;
		if (!isInit)
		{
			TargetEntitiy = GetComponent<BaseEntity>();
			if (TargetEntitiy == null)
			{
				throw new NullReferenceException();
			}
			if (FX_Prefab != null)
			{
				fx = UnityEngine.Object.Instantiate(FX_Prefab) as ParticleSystem;
				fx.transform.parent = base.transform;
				fx.transform.localPosition = Vector3.zero;
				fx.transform.PosY(TargetEntitiy.RenderBounds.center.y);
				SpriteTools.SetSortingLayerID(fx, 4);
				fx.Stop();
			}
			isInit = true;
		}
	}

	private void OnDestroy()
	{
		FX_Prefab = null;
	}

	public abstract void ApplyEffect(float duration);

	protected virtual void CancelEffect()
	{
		TargetEntitiy.OnEntityDied -= CancelEffect;
	}

	protected IEnumerator Timer()
	{
		TargetEntitiy.OnEntityDied += CancelEffect;
		bool done = false;
		while (!done)
		{
			yield return new WaitForSeconds(updateFrequency);
			currentDuration -= updateFrequency;
			if (currentDuration <= 0f)
			{
				done = true;
			}
		}
		CancelEffect();
	}

	public static void Apply(CombatEffects effect, BaseEntity target, float duration, int damage = 0)
	{
		if ((target.combat_effects_mask & effect) != 0)
		{
			switch (effect)
			{
			case CombatEffects.slow:
			{
				SlowCombatEffect slowCombatEffect = target.AddOrGetComponent<SlowCombatEffect>();
				slowCombatEffect.ApplyEffect(duration);
				break;
			}
			case CombatEffects.poison:
			{
				PoisonCombatEffect poisonCombatEffect = target.AddOrGetComponent<PoisonCombatEffect>();
				poisonCombatEffect.effectDamage = damage;
				poisonCombatEffect.ApplyEffect(duration);
				break;
			}
			case CombatEffects.none | CombatEffects.slow:
				break;
			}
		}
	}
}
