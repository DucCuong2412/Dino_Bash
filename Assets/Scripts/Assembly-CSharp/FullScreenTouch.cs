using System;

public class FullScreenTouch : MonoBase
{
	public static event Action onClick;

	private void Start()
	{
		GetComponent<tk2dUIItem>().OnClick += delegate
		{
			if (FullScreenTouch.onClick != null)
			{
				FullScreenTouch.onClick();
			}
		};
	}

	private void OnDestroy()
	{
		FullScreenTouch.onClick = null;
	}
}
