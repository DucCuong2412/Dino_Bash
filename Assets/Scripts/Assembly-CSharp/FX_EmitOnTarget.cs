using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FX_EmitOnTarget : MonoBase
{
	private int emitCount = 12;

	private ParticleSystem ps;

	private void OnEnable()
	{
		ps = GetComponent<ParticleSystem>();
	}

	public void Emit(Component[] targets)
	{
		foreach (Component target in targets)
		{
			Emit(target);
		}
	}

	public void Emit(Component target)
	{
		Transform[] componentsInChildren = target.GetComponentsInChildren<Transform>();
		ps.Emit(emitCount);
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles(array);
		for (int i = ps.particleCount - emitCount; i < ps.particleCount; i++)
		{
			array[i].position = componentsInChildren[Random.Range(0, componentsInChildren.Length)].transform.position;
		}
		ps.SetParticles(array, array.Length);
	}
}
