using System.Collections;
using UnityEngine;

public class PoisonCombatEffect : AbstractCombatEffect
{
	public int effectDamage = 1;

	private int damagePool;

	public override void ApplyEffect(float duration)
	{
		if (AbstractCombatEffect.FX_Prefab == null)
		{
			AbstractCombatEffect.FX_Prefab = Resources.Load<ParticleSystem>("Fx/FX_Poison");
		}
		Init();
		currentDuration += duration;
		totalDuration += duration;
		damagePool += effectDamage;
		base.TargetEntitiy.colorTinter.ChangeBaseColor(EntityColorTinter.poisonColor, currentDuration);
		fx.Play();
		if (!base.isRunning)
		{
			base.isRunning = true;
			StartCoroutine(Timer());
			StartCoroutine(DamageOverTime());
		}
	}

	private IEnumerator DamageOverTime()
	{
		while (base.isRunning)
		{
			yield return new WaitForSeconds(0.5f);
			int tick = Mathf.RoundToInt((float)damagePool / (totalDuration / 0.5f));
			base.TargetEntitiy.ChangeHealth(-tick, null, false);
		}
	}

	protected override void CancelEffect()
	{
		base.CancelEffect();
		StopAllCoroutines();
		currentDuration = 0f;
		totalDuration = 0f;
		base.isRunning = false;
		base.enabled = false;
		fx.AddOrGetComponent<FX_ParticleSystemEmitStopper>().enabled = true;
	}
}
