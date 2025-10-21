using UnityEngine;

public class SpriteInitialSettings : MonoBase
{
	private Vector3 position;

	private Quaternion rotation;

	private Vector3 scale;

	private Sprite sprite;

	private Color color;

	private void Awake()
	{
		Bind();
	}

	public void Bind()
	{
		position = base.transform.localPosition;
		rotation = base.transform.localRotation;
		scale = base.transform.localScale;
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if (component != null)
		{
			sprite = component.sprite;
			color = component.color;
		}
	}

	public void Revert()
	{
		base.transform.localPosition = position;
		base.transform.localRotation = rotation;
		base.transform.localScale = scale;
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if (component != null)
		{
			component.sprite = sprite;
			component.color = color;
		}
	}
}
