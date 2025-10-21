using System;
using UnityEngine;

public class GameInput : MonoBase
{
	public static event Action OnEscapeKeyUp;

	public static void ResetEvents()
	{
		GameInput.OnEscapeKeyUp = null;
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Escape) && !Loader.instance.isVisible && GameInput.OnEscapeKeyUp != null)
		{
			GameInput.OnEscapeKeyUp();
		}
	}
}
