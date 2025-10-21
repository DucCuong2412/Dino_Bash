using System.Collections;
using UnityEngine;

public class KamikazeCombatBehaviour : AbstractUnitCombatBehaviour
{
	protected int damagePool;

	public bool spawn_at_screen_border;

	public bool stop_at_level_end = true;

	protected override void SyncToState(Unit unit, Unit.State state)
	{
		switch (state)
		{
		case Unit.State.disabled:
			damagePool = base.Entity.Config.attackStrengh;
			StopAllCoroutines();
			break;
		case Unit.State.move:
			if (base.Entity.lastState == Unit.State.disabled)
			{
				base.Entity.StopAtLevelEnd = stop_at_level_end;
				if (!stop_at_level_end)
				{
					StartCoroutine(Remove());
				}
				if (spawn_at_screen_border)
				{
					float x = Camera.main.transform.position.x - base.Entity.RenderBounds.extents.x;
					base.Entity.transform.PosX(x);
				}
			}
			break;
		case Unit.State.stand:
			if (base.Entity.lastState != Unit.State.disabled)
			{
				StopAllCoroutines();
				base.Entity.stateDie();
			}
			break;
		default:
			StopAllCoroutines();
			base.CombatTargets.Clear();
			break;
		}
	}

	private IEnumerator Remove()
	{
		while (!(base.Entity.transform.position.x > (float)Level.Instance.Config.levelwidth + base.Entity.RenderBounds.size.x))
		{
			yield return new WaitForSeconds(0.1f);
		}
		base.Entity.stateDie();
	}

	private void OnTriggerEnter(Collider pOther)
	{
		if (!isInRayCastMask(pOther.gameObject.layer) || pOther.attachedRigidbody == null)
		{
			return;
		}
		BaseEntity component = pOther.attachedRigidbody.GetComponent<BaseEntity>();
		if (component == null)
		{
			Debug.LogError(pOther.attachedRigidbody.name + "does not have an entitiy?");
		}
		else if (canHitUnit(component.unitType))
		{
			base.CombatTargets.Add(component);
			int num = component.Health;
			if (Konfiguration.isConsumable(base.Entity.unitType))
			{
				num = Konfiguration.scaleConsumableWithPlayerProgress(num);
			}
			if (damagePool <= num)
			{
				component.ChangeHealth(-damagePool, base.Entity);
				damagePool = 0;
				base.Entity.stateDie();
			}
			else
			{
				damagePool -= num;
				component.ChangeHealth(-num, base.Entity);
				base.Entity.PlaySound(EntitySound.ATTACK);
				base.Entity.stateMove();
			}
		}
	}
}
