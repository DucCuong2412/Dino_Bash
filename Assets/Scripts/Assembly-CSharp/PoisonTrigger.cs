using UnityEngine;

public class PoisonTrigger : GenericCombatBehaviour<BaseEntity>
{
	private void OnTriggerEnter(Collider pOther)
	{
		if (base.gameObject.layer != pOther.gameObject.layer)
		{
			BaseEntity component = pOther.GetComponent<BaseEntity>();
			if (component != null && isValidAttackTarget(component.unitType))
			{
				PoisonCombatEffect poisonCombatEffect = component.AddOrGetComponent<PoisonCombatEffect>();
				poisonCombatEffect.effectDamage = base.Entity.Config.attackStrengh;
				poisonCombatEffect.ApplyEffect(5f);
				base.Entity.PlaySound(EntitySound.ATTACK);
				base.Entity.animator.Play("attack");
			}
		}
	}
}
