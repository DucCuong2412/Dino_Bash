using System.Collections;
using UnityEngine;

public class BarrierUnit : BaseEntity
{
	public override int Health
	{
		get
		{
			return base.Health;
		}
		protected set
		{
			base.Health = value;
			StartCoroutine(SetAnimation());
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		base.collider.enabled = true;
		base.OnAnimationEvent += AnimationEventHandler;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		base.OnAnimationEvent -= AnimationEventHandler;
	}

	public override void stateDie()
	{
		base.collider.enabled = false;
		base.stateDie();
		base.animator.SetBool("isDead", true);
	}

	private void AnimationEventHandler(UnitAnimationEvents e)
	{
		if (e == UnitAnimationEvents.UnitDied)
		{
			StartCoroutine(Deactivate());
		}
	}

	private IEnumerator Deactivate()
	{
		yield return new WaitForEndOfFrame();
		base.gameObject.SetActive(false);
	}

	private IEnumerator SetAnimation()
	{
		if (Health > 0)
		{
			base.animator.SetBool("isHit", true);
			yield return new WaitForSeconds(0.1f);
			base.animator.SetBool("isHit", false);
		}
	}
}
