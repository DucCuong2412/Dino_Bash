using System;
using UnityEngine;

public class UnitAnimationEvent : MonoBase
{
	private float kMinDelayBetweenEvents = 0.5f;

	private float lastAnimEventCall;

	public event Action<UnitAnimationEvents> OnAnimationEvent;

	private void AnimEvent(UnitAnimationEvents pEvent)
	{
		if (!(Time.time < lastAnimEventCall + kMinDelayBetweenEvents))
		{
			if (this.OnAnimationEvent != null)
			{
				this.OnAnimationEvent(pEvent);
			}
			lastAnimEventCall = Time.time;
		}
	}

	private void SetColor(Color pColor)
	{
		SpriteTools.SetColor(this, pColor);
	}

	private void FadeOut(float pTime)
	{
		SpriteTools.FadeOut(this, pTime);
	}

	private void CamShake(CameraShake.Intensity intensity)
	{
		CameraShake.Shake(intensity);
	}

	private void PlayParticleSystem(string path)
	{
		Transform transform = base.transform.Find(path);
		if (transform != null)
		{
			ParticleSystem component = transform.GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Play();
			}
		}
	}
}
