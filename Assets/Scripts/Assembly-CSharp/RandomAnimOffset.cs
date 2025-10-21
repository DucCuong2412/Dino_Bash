using System;
using System.Collections;
using UnityEngine;

public class RandomAnimOffset : MonoBehaviour
{
	private void Start()
	{
		IEnumerator enumerator = base.animation.GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				AnimationState animationState = (AnimationState)enumerator.Current;
				animationState.normalizedTime = UnityEngine.Random.Range(0f, 1f);
				animationState.speed = UnityEngine.Random.Range(0.8f, 1.5f);
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}
}
