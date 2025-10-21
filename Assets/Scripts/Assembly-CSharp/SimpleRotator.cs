using UnityEngine;

public class SimpleRotator : MonoBase
{
	public Vector3 minAngle;

	public Vector3 maxAngle;

	public float speed = 1f;

	public bool randomize = true;

	private float random;

	public bool sine_wave = true;

	public bool ignore_timescale;

	private float time;

	private void Start()
	{
		random = Random.value;
		if (!randomize)
		{
			random = 0f;
		}
		time = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float num = Time.realtimeSinceStartup - time;
		time = Time.realtimeSinceStartup;
		if (ignore_timescale || Time.timeScale != 0f)
		{
			if (sine_wave)
			{
				float t = Mathf.Sin(Time.realtimeSinceStartup * speed + random) * 0.5f + 0.5f;
				base.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(minAngle), Quaternion.Euler(maxAngle), t);
			}
			else
			{
				base.transform.localRotation = Quaternion.Euler(base.transform.localRotation.eulerAngles.x, base.transform.localRotation.eulerAngles.y, (base.transform.localRotation.eulerAngles.z + speed * num) % 360f);
			}
		}
	}
}
