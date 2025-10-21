using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_ParticleSystemEmitStopper : FX_Controller
{
	private List<ParticleSystem> particle_systems;

	private void OnEnable()
	{
		particle_systems = GetComponentsInChildrenList<ParticleSystem>();
		particle_systems.ForEach(delegate(ParticleSystem ps)
		{
			ps.enableEmission = false;
		});
		StartCoroutine(StopParticleSystems());
	}

	private void OnDisable()
	{
		particle_systems[0].Stop();
		particle_systems.ForEach(delegate(ParticleSystem ps)
		{
			ps.Clear();
			ps.enableEmission = true;
		});
	}

	private IEnumerator StopParticleSystems()
	{
		yield return new WaitForSeconds(particle_systems[0].startLifetime);
		base.enabled = false;
	}
}
