public class CollectableUnit : BaseEntity
{
	protected override void OnEnable()
	{
		base.OnEnable();
		SpriteTools.SetSortingLayerID(this, 20);
		base.colorTinter.SetSpriteColor(Colors.TintNeutral);
		base.animator.Play("in");
		base.OnAnimationEvent += Free;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		base.OnAnimationEvent -= Free;
	}

	public override void stateDie()
	{
		base.stateDie();
		base.collider.enabled = false;
		base.animator.CrossFade("destroyed", 0.2f);
	}

	private void Free(UnitAnimationEvents pEvent)
	{
		if (pEvent == UnitAnimationEvents.UnitDied)
		{
			EntityFactory.Free(this);
		}
	}
}
