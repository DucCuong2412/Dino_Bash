using System;

public class CounterAttackMeelee : GenericCombatBehaviour<BaseEntity>
{
	public BaseEntity mTarget;

	public override void Init()
	{
		if (!isInititalized)
		{
			base.Init();
			if (mTarget == null)
			{
				throw new Exception("mTarget must be set before init!");
			}
			mTarget.OnEntityHit += HandleOnEntityHit;
		}
	}

	private void HandleOnEntityHit(BaseEntity pEntity)
	{
		if (pEntity.GetComponent<MeleeCombatBehaviour>() != null)
		{
			Attack(pEntity);
			base.Entity.animator.Play("hit", 0, 0f);
		}
	}
}
