using UnityEngine;

public class LockRotation : MonoBase
{
	public Vector3 eulerRotation;

	private Quaternion rotation;

	private void Start()
	{
		rotation = Quaternion.Euler(eulerRotation);
	}

	private void Update()
	{
		base.transform.rotation = rotation;
	}
}
