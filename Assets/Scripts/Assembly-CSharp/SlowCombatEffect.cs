using UnityEngine;

public class SlowCombatEffect : AbstractCombatEffect
{
	private float slowRate = 0.35f;

	private Vector3 originalSpeed = Vector3.zero;

	public override void ApplyEffect(float duration)
	{
		Init();
		if (base.TargetEntitiy is Unit)
		{
			currentDuration += duration;
			base.TargetEntitiy.colorTinter.ChangeBaseColor(EntityColorTinter.slowColor, currentDuration);
			Unit unit = base.TargetEntitiy as Unit;
			if (!base.isRunning)
			{
				originalSpeed = unit.Speed;
				unit.Speed = originalSpeed * slowRate;
				unit.animator.speed = slowRate;
				base.isRunning = true;
				StartCoroutine(Timer());
			}
		}
	}

	protected override void CancelEffect()
	{
		base.CancelEffect();
		if (!(originalSpeed == Vector3.zero))
		{
			(base.TargetEntitiy as Unit).Speed = originalSpeed;
			base.TargetEntitiy.animator.speed = 1f;
			currentDuration = 0f;
			base.isRunning = false;
			originalSpeed = Vector3.zero;
			base.enabled = false;
		}
	}
}
