using System;
using UnityEngine;

public class SpriteStretcher : MonoBehaviour
{
	private void Start()
	{
		tk2dCamera tk2dCamera2;
		if (base.transform.root == ScreenManager.UI_Root)
		{
			tk2dCamera2 = ScreenManager.Camera.GetComponent<tk2dCamera>();
		}
		else
		{
			tk2dCamera2 = UnityEngine.Object.FindObjectOfType<tk2dCamera>();
			if (tk2dCamera2 == null)
			{
				throw new NotImplementedException();
			}
		}
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		Vector3 vector = tk2dCamera2.camera.ViewportToWorldPoint(Vector3.zero);
		Vector3 vector2 = tk2dCamera2.camera.ViewportToWorldPoint(Vector3.one);
		base.transform.localScale = new Vector3(base.transform.localScale.x * 1.01f * (vector2.x - vector.x) / component.bounds.extents.x * 0.5f, base.transform.localScale.y * 1.01f * (vector2.y - vector.y) / component.bounds.extents.y * 0.5f, 1f);
	}
}
