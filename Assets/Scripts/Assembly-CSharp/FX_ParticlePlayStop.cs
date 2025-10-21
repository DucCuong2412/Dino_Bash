using UnityEngine;

public class FX_ParticlePlayStop : FX_Controller
{
	private ParticleSystem ps;

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	private void OnEnable()
	{
		if (ps == null)
		{
			Start();
		}
		base.isDone = false;
		ps.Play();
	}

	private void OnDisable()
	{
		ps.Stop();
		ps.Clear();
		base.isDone = true;
	}
}
