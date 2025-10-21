using System.Collections;
using UnityEngine;

public class FlyElementTo : MonoBase
{
	public float duration = 1f;

	public float range = 2f;

	public bool isFlying;

	private Vector3 startpos;

	private Vector3 distance;

	private float blend;

	private float time;

	public float normalized_time
	{
		get
		{
			return time / duration;
		}
	}

	public void Play(Vector3 target, float delay = 0f)
	{
		startpos = base.transform.position;
		distance = target - startpos;
		blend = Random.value;
		WaitThen(delay, delegate
		{
			StartCoroutine(Animate(duration));
		});
	}

	public void Stop()
	{
		if (!isFlying)
		{
			StopAllCoroutines();
		}
	}

	private IEnumerator Animate(float duration)
	{
		isFlying = true;
		time = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			Vector3 v = startpos + GetPosition(normalized_time);
			base.transform.position = v;
			yield return null;
		}
		base.gameObject.SetActive(false);
		isFlying = false;
	}

	private Vector3 GetPosition(float x)
	{
		x = Mathf.Clamp01(x);
		x = GoEaseCubic.EaseInOut(x, 0f, 1f, duration);
		float from = Mathf.Pow(x, 1.5f);
		float to = Mathf.Pow(x, 4f);
		float num = Mathf.Lerp(from, to, blend);
		return new Vector3(distance.x * x, distance.y * num, distance.z);
	}
}
