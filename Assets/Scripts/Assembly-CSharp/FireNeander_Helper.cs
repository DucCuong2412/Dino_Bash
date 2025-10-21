using System;
using UnityEngine;

public class FireNeander_Helper : MonoBehaviour
{
	public ParticleSystem fire_weapon;

	private ParticleSystem[] fx;

	private Unit unit;

	private SlowCombatEffect combat_effect;

	private bool _hasfire = true;

	private bool init;

	public bool hasFire
	{
		get
		{
			return _hasfire;
		}
		private set
		{
			_hasfire = value;
			Array.ForEach(fx, delegate(ParticleSystem ps)
			{
				ps.enableEmission = _hasfire;
			});
			unit.CombatBehaviour.AttackModifier = ((!_hasfire) ? 0.001f : 1f);
			unit.GetComponent<FX_UnitAttack>().enabled = _hasfire;
		}
	}

	public void ExtinguishFire()
	{
		if (hasFire)
		{
			hasFire = false;
		}
	}

	private void Start()
	{
		if (init)
		{
			return;
		}
		init = true;
		unit = base.transform.parent.GetComponent<Unit>();
		unit.OnEntityHit += onEntityHit;
		unit.OnEntityDied += onEntitiyDied;
		fx = fire_weapon.GetComponentsInChildren<ParticleSystem>();
		if (fx != null)
		{
			Array.ForEach(fx, delegate(ParticleSystem ps)
			{
				ps.Clear();
			});
		}
		hasFire = true;
	}

	private void OnEnable()
	{
		if (!init)
		{
			return;
		}
		hasFire = true;
		if (fx != null)
		{
			Array.ForEach(fx, delegate(ParticleSystem ps)
			{
				ps.Clear();
			});
		}
	}

	private void onEntitiyDied()
	{
		ExtinguishFire();
	}

	private void onEntityHit(BaseEntity entity)
	{
		SlowCombatEffect component = unit.GetComponent<SlowCombatEffect>();
		if (component != null && component.isRunning)
		{
			ExtinguishFire();
		}
	}
}
