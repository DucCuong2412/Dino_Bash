using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUICamera")]
public class tk2dUICamera : MonoBehaviour
{
	public enum tk2dRaycastType
	{
		Physics3D = 0,
		Physics2D = 1
	}

	[SerializeField]
	private LayerMask raycastLayerMask = -1;

	[SerializeField]
	private tk2dRaycastType raycastType;

	public tk2dRaycastType RaycastType
	{
		get
		{
			return raycastType;
		}
	}

	public LayerMask FilteredMask
	{
		get
		{
			return (int)raycastLayerMask & base.GetComponent<Camera>().cullingMask;
		}
	}

	public Camera HostCamera
	{
		get
		{
			return base.GetComponent<Camera>();
		}
	}

	public void AssignRaycastLayerMask(LayerMask mask)
	{
		raycastLayerMask = mask;
	}

	private void OnEnable()
	{
		if (base.GetComponent<Camera>() == null)
		{
			Debug.LogError("tk2dUICamera should only be attached to a camera.");
			base.enabled = false;
		}
		else if (!base.GetComponent<Camera>().orthographic && raycastType == tk2dRaycastType.Physics2D)
		{
			Debug.LogError("tk2dUICamera - Physics2D raycast only works with orthographic cameras.");
			base.enabled = false;
		}
		else
		{
			tk2dUIManager.RegisterCamera(this);
		}
	}

	private void OnDisable()
	{
		tk2dUIManager.UnregisterCamera(this);
	}
}
