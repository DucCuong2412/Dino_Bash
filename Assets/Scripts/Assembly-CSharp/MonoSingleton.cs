using System;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBase where T : MonoSingleton<T>
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
				if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
				{
					_instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
					if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
					{
						throw new Exception("Problem during the creation of " + typeof(T).ToString());
					}
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
		{
			_instance = this as T;
		}
	}

	private void Start()
	{
	}

	private void OnApplicationQuit()
	{
		_instance = (T)null;
	}
}
