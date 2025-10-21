using UnityEngine;

public class UIAnchorScaler : MonoBehaviour
{
	private float ScaleValue = 0.9f;

	private void Awake()
	{
		if (ScreenManager.Camera.aspect <= 1.35f)
		{
			base.transform.localScale = Vector3.one * ScaleValue;
		}
		Object.Destroy(this);
	}
}
