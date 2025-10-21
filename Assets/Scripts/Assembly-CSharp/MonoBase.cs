using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonoBase : MonoBehaviour
{
	private Transform _transform;

	private Rigidbody _rigidbody;

	public new Transform transform
	{
		get
		{
			if (_transform == null)
			{
				_transform = GetComponent<Transform>();
			}
			return _transform;
		}
	}

	public new Rigidbody rigidbody
	{
		get
		{
			if (_rigidbody == null)
			{
				_rigidbody = GetComponent<Rigidbody>();
			}
			return _rigidbody;
		}
	}

	protected void WaitThen(float delay, Action callback)
	{
		if (delay == 0f)
		{
			callback();
		}
		else
		{
			StartCoroutine(Wait(delay, callback));
		}
	}

	private IEnumerator Wait(float delay, Action callback)
	{
		yield return new WaitForSeconds(delay);
		callback();
	}

	protected void WaitThenRealtime(float delay, Action callback)
	{
		if (delay == 0f)
		{
			callback();
		}
		else
		{
			StartCoroutine(WaitRealtime(delay, callback));
		}
	}

	public static IEnumerator WaitForSecondsRealtime(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}

	private IEnumerator WaitRealtime(float delay, Action callback)
	{
		yield return StartCoroutine(WaitForSecondsRealtime(delay));
		callback();
	}

	protected T FindChildComponent<T>(string path) where T : Component
	{
		Transform transform = this.transform.FindChild(path);
		if (transform != null)
		{
			return transform.GetComponent<T>();
		}
		return (T)null;
	}

	protected List<T> GetComponentsInChildrenList<T>(Transform root = null) where T : Component
	{
		if (root != null)
		{
			return root.GetComponentsInChildren<T>().ToList();
		}
		return GetComponentsInChildren<T>().ToList();
	}
}
