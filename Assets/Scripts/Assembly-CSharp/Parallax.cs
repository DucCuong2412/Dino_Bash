using UnityEngine;

public class Parallax : MonoBase
{
	private Transform cameraTransform;

	[SerializeField]
	private float speedX;

	private Vector3 startPos;

	private void Start()
	{
		cameraTransform = Camera.main.transform;
		startPos = base.transform.position;
	}

	private void LateUpdate()
	{
		if (Time.timeScale != 0f)
		{
			base.transform.PosX(startPos.x + Mathf.Round(cameraTransform.position.x * speedX));
		}
	}
}
