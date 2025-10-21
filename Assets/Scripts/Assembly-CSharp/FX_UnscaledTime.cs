using UnityEngine;

public class FX_UnscaledTime : MonoBehaviour
{
	private ParticleSystem ps;

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (Time.timeScale < 0.01f)
		{
			ps.Simulate(Time.unscaledDeltaTime, true, false);
		}
	}
}
