using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FX_DisableParticleSystem : FX_Controller
{
	public bool destroy;

	public bool autoStart;

	private ParticleSystem target;

	private void OnEnable()
	{
		base.isDone = false;
		target = GetComponent<ParticleSystem>();
		if (autoStart)
		{
			Run();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void Run()
	{
		ResetOnFxDoneEvent();
		StopAllCoroutines();
		StartCoroutine(CountDown());
	}

	private IEnumerator CountDown()
	{
		bool emitStarted = false;
		while (true)
		{
			if (!emitStarted && target.particleCount > 0)
			{
				emitStarted = true;
			}
			if (emitStarted && target.particleCount == 0)
			{
				break;
			}
			yield return null;
		}
		target.Stop();
		base.isDone = true;
		if (destroy)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
