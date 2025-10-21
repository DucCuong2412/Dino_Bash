using System.Collections.Generic;
using UnityEngine;

public class Blizzard : BaseEntity
{
	protected override void OnEnable()
	{
		base.OnEnable();
		if (Level.Instance.state == Level.State.invalid)
		{
			return;
		}
		base.OnAnimationEvent -= AnimationEventHandler;
		base.OnAnimationEvent += AnimationEventHandler;
		SpriteTools.SetSortingLayerID(this, 47);
		base.transform.parent = GameCamera.Instance.transform;
		base.transform.localPosition = Vector3.zero;
		base.animator.Play("blizzard");
		base.animator.Update(Time.deltaTime);
		List<BaseEntity> list = EntityFactory.GetEntities(false).FindAll((BaseEntity neander) => GameCamera.Instance.isInView(neander.transform));
		QuestManager.instance.ReportProgress(QuestObjective.IceAge, list.Count);
		list.ForEach(delegate(BaseEntity neander)
		{
			SlowCombatEffect slowCombatEffect = neander.AddOrGetComponent<SlowCombatEffect>();
			slowCombatEffect.ApplyEffect(base.Config.walkspeed);
			int attackStrengh = base.Config.attackStrengh;
			attackStrengh = Konfiguration.scaleConsumableWithPlayerProgress(attackStrengh);
			neander.ChangeHealth(-attackStrengh, this);
			if (neander is Unit)
			{
				Unit unit = (Unit)neander;
				if (unit.state == Unit.State.fly)
				{
					unit.stateFall();
				}
			}
		});
	}

	private void AnimationEventHandler(UnitAnimationEvents e)
	{
		if (e == UnitAnimationEvents.UnitDied)
		{
			EntityFactory.Free(this);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		base.OnAnimationEvent -= AnimationEventHandler;
	}
}
