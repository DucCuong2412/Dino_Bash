using System;
using System.Collections;
using UnityEngine;

public class FX_DisableTimer : FX_Controller
{
	public float delay;

	public bool destroy;

	public bool autoStart;

	public bool disable;

	private Action callback;

	private void OnEnable()
	{
		if (autoStart)
		{
			Run(null);
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void Run(Action callback)
	{
		base.gameObject.SetActive(true);
		this.callback = callback;
		StopAllCoroutines();
		StartCoroutine(CountDown());
	}

	private IEnumerator CountDown()
	{
		yield return new WaitForSeconds(delay);
		if (callback != null)
		{
			callback();
		}
		base.isDone = true;
		if (disable)
		{
			base.gameObject.SetActive(false);
		}
		if (destroy)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
