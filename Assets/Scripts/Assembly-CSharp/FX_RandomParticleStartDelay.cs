using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FX_RandomParticleStartDelay : MonoBase
{
	[SerializeField]
	private float startDelay;

	[SerializeField]
	private Vector2 randomRange = Vector2.zero;

	private void Start()
	{
		ParticleSystem component = GetComponent<ParticleSystem>();
		component.Stop();
		component.startDelay = startDelay + Random.Range(randomRange.x, randomRange.y);
		component.Play();
	}
}
