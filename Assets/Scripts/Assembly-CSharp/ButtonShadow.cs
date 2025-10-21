using UnityEngine;

public class ButtonShadow : MonoBehaviour
{
	private Camera ui_camera;

	private SpriteRenderer sprite;

	public float distance_from_center;

	private void Start()
	{
		ui_camera = ScreenManager.Camera;
		sprite = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		distance_from_center = ui_camera.WorldToViewportPoint(base.transform.parent.position + sprite.bounds.extents).x - 0.5f;
		base.transform.LocalPosX(40f * distance_from_center - 10f);
	}
}
