using UnityEngine;

public class FX_CollisionPlaneHelper : MonoBehaviour
{
	public float height;

	public Vector3 eulerRotation;

	private Quaternion rotation;

	private void Start()
	{
		rotation = Quaternion.Euler(eulerRotation);
	}

	private void Update()
	{
		if (base.transform.hasChanged)
		{
			base.transform.position = new Vector3(base.transform.position.x, height, base.transform.position.z);
			base.transform.rotation = rotation;
		}
	}
}
