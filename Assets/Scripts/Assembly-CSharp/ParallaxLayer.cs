using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
	public float speed;

	public Bounds bounds_;

	public Vector3 startPosition;

	public bool autoRepeat;

	public float constantSpeed;

	public ParallaxLayer alignedTo;

	public Bounds bounds
	{
		get
		{
			if (bounds_.extents != Vector3.zero)
			{
				return bounds_;
			}
			startPosition = base.transform.position;
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (bounds_.extents == Vector3.zero)
				{
					bounds_ = renderer.bounds;
					continue;
				}
				bounds_.Encapsulate(renderer.bounds.min);
				bounds_.Encapsulate(renderer.bounds.max);
			}
			return bounds_;
		}
	}

	public void allignTo(ParallaxLayer other)
	{
		alignedTo = other;
		float num = bounds.extents.x / other.bounds.extents.x;
		speed = other.speed * num * 0.9f;
		float num2 = speed / other.speed;
		float num3 = base.transform.position.x - bounds.center.x;
		float num4 = other.transform.position.x - other.bounds.center.x;
		startPosition.x = (other.startPosition.x - num4) * num2 + num3;
		base.transform.LocalPosX(startPosition.x);
	}

	private void Start()
	{
		startPosition = base.transform.position;
	}
}
