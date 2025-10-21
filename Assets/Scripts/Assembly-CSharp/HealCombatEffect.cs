using System.Collections;
using UnityEngine;

public class HealCombatEffect : AbstractCombatEffect
{
	public int effectHeal;

	public override void ApplyEffect(float duration)
	{
		Init();
		currentDuration += duration;
		base.TargetEntitiy.colorTinter.ChangeBaseColor(EntityColorTinter.poisonColor, currentDuration);
		if (!base.isRunning)
		{
			base.isRunning = true;
			StartCoroutine(Timer());
			StartCoroutine(HealOverTime());
		}
	}

	private IEnumerator HealOverTime()
	{
		while (base.isRunning)
		{
			yield return new WaitForSeconds(0.5f);
			base.TargetEntitiy.ChangeHealth(effectHeal, null);
		}
	}

	protected override void CancelEffect()
	{
		base.CancelEffect();
		StopAllCoroutines();
		currentDuration = 0f;
		base.isRunning = false;
		base.enabled = false;
	}
}
