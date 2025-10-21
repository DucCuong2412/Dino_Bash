using System;
using UnityEngine;

public class FireProjectileHelper : MonoBase
{
	public ParticleSystem fire_weapon;

	private ParticleSystem[] fx;

	private AbstractProjectile projectile;

	private bool initialized;

	private void Start()
	{
		if (!initialized)
		{
			initialized = true;
			projectile = GetComponent<AbstractProjectile>();
			projectile.onProjectileFired += HandleOnProjectileFired;
			fx = fire_weapon.GetComponentsInChildren<ParticleSystem>();
			HandleOnProjectileFired();
		}
	}

	private void OnEnable()
	{
		if (initialized)
		{
			Array.ForEach(fx, delegate(ParticleSystem ps)
			{
				ps.Clear();
			});
			HandleOnProjectileFired();
		}
	}

	private void ToggleFX(bool state)
	{
		for (int i = 0; i < fx.Length; i++)
		{
			fx[i].renderer.enabled = state;
		}
	}

	private void HandleOnProjectileFired()
	{
		FireNeander_Helper fireNeander_Helper = projectile.unit.GetComponentsInChildren<FireNeander_Helper>(true)[0];
		ToggleFX(fireNeander_Helper.hasFire);
	}
}
