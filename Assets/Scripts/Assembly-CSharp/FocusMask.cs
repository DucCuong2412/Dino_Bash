using UnityEngine;

public class FocusMask : MonoBase
{
	private Vector3 offsize = Vector3.one * 60f;

	private Renderer meshrenderer;

	public GoUpdateType updateType;

	public Transform center { get; private set; }

	private void Start()
	{
		base.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		center = base.transform.Search("Hole");
		center.transform.localScale = offsize;
		meshrenderer = GetComponentInChildren<Renderer>();
		meshrenderer.material.SetColor("_Color", new Color(0f, 0f, 0f, 0.4f));
		meshrenderer.enabled = false;
	}

	public void Show(Vector3 pos, float size)
	{
		Show(pos, Vector3.one * size);
	}

	public void Show(Vector3 pos, Vector3 size)
	{
		center.position = pos;
		meshrenderer.enabled = true;
		Go.to(center, 1f, new GoTweenConfig().scale(size).setEaseType(GoEaseType.CubicOut).setUpdateType(updateType));
	}

	public void Hide()
	{
		Go.to(center, 0.5f, new GoTweenConfig().scale(offsize).setEaseType(GoEaseType.CubicIn).setUpdateType(updateType)
			.onComplete(delegate
			{
				meshrenderer.enabled = false;
			}));
	}
}
