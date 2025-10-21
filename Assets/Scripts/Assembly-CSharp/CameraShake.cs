using UnityEngine;

public class CameraShake : MonoBase
{
	public enum Intensity
	{
		none = 0,
		gentle = 1,
		medium = 2,
		strong = 3,
		single = 4
	}

	public static CameraShake instance;

	private static float ShakeDecay;

	private static float ShakeIntensity;

	private static Vector3 OriginalPos;

	private void Start()
	{
		instance = this;
	}

	private void Update()
	{
		if (ShakeIntensity > 0f && Time.timeScale > 0f)
		{
			base.transform.localPosition = Random.insideUnitSphere * ShakeIntensity;
			ShakeIntensity -= ShakeDecay * Time.deltaTime;
			if (ShakeIntensity <= 0f)
			{
				base.transform.localPosition = Vector3.zero;
			}
		}
	}

	public static void Shake(Intensity intensity)
	{
		switch (intensity)
		{
		case Intensity.none:
			ShakeIntensity = 0f;
			ShakeDecay = 0f;
			break;
		case Intensity.gentle:
			ShakeIntensity = 16f;
			ShakeDecay = 40f;
			break;
		case Intensity.medium:
			ShakeIntensity = 64f;
			ShakeDecay = 92f;
			break;
		case Intensity.strong:
			ShakeIntensity = 96f;
			ShakeDecay = 128f;
			break;
		case Intensity.single:
			ShakeIntensity = 20f;
			ShakeDecay = 128f;
			break;
		}
	}
}
