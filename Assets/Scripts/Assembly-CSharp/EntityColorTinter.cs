using System.Collections.Generic;
using UnityEngine;

public class EntityColorTinter : MonoBase
{
	private class ColorProp
	{
		private float elapsed;

		private float duration;

		public Color color;

		public float blend_speed = 1f;

		public bool done
		{
			get
			{
				return (double)(elapsed / duration) >= 1.0;
			}
		}

		public ColorProp(float duration, Color color, float blend_speed)
		{
			this.duration = duration;
			this.color = color;
			this.blend_speed = blend_speed;
			elapsed = 0f;
		}

		public void Update(float dt)
		{
			elapsed += dt;
		}
	}

	private const float blend_speed = 8f;

	private const float fast_blendspeed = 16f;

	public static readonly Color baseColor = Colors.TintNeutral;

	public static readonly Color hitColor = new Color(0.75f, 0.75f, 0.75f, 1f);

	public static readonly Color slowColor = new Color(0.4f, 0.85f, 1f, 1f);

	public static readonly Color poisonColor = new Color(0.18f, 0.57f, 0.15f, 1f);

	public static readonly Color healColor = new Color(0.1f, 0.6f, 0.5f, 1f);

	private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

	private EntityShadow shadow;

	private static Material spriteTintMaterial;

	[SerializeField]
	private bool ignore;

	private List<ColorProp> blends = new List<ColorProp>();

	public Color CurrentColor { get; private set; }

	private void Awake()
	{
		if (ignore)
		{
			return;
		}
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>(true);
		if (spriteTintMaterial == null)
		{
			spriteTintMaterial = Resources.Load<Material>("Units/UnitTintMaterial");
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			EntityShadow component = componentsInChildren[i].GetComponent<EntityShadow>();
			if ((bool)component)
			{
				shadow = component;
				continue;
			}
			componentsInChildren[i].material = spriteTintMaterial;
			componentsInChildren[i].color = baseColor;
			sprites.Add(componentsInChildren[i]);
		}
		CurrentColor = baseColor;
	}

	public void Revert()
	{
		if (!ignore)
		{
			CurrentColor = baseColor;
			SetSpriteColor(CurrentColor);
			blends.Clear();
			if (shadow != null)
			{
				shadow.Revert();
			}
		}
	}

	private void Update()
	{
		if (ignore || blends.Count == 0)
		{
			return;
		}
		blends.ForEach(delegate(ColorProp b)
		{
			b.Update(Time.deltaTime);
		});
		blends.RemoveAll((ColorProp b) => b.done);
		if (blends.Count == 0 && CurrentColor != baseColor)
		{
			blends.Add(new ColorProp(0.5f, baseColor, 8f));
		}
		if (blends.Count > 0)
		{
			ColorProp colorProp = blends[blends.Count - 1];
			CurrentColor = Color.Lerp(CurrentColor, colorProp.color, Time.deltaTime * colorProp.blend_speed);
			if (CurrentColor != colorProp.color)
			{
				SetSpriteColor(CurrentColor);
			}
		}
	}

	public void FlashColor(Color flash_color, float duration)
	{
		if (!ignore)
		{
			blends.Add(new ColorProp(duration, flash_color, 16f));
		}
	}

	public void ChangeBaseColor(Color color, float duration)
	{
		if (!ignore)
		{
			blends.Add(new ColorProp(duration, color, 8f));
		}
	}

	public void SetSpriteColor(Color color)
	{
		if (!ignore)
		{
			for (int i = 0; i < sprites.Count; i++)
			{
				sprites[i].color = color;
			}
		}
	}
}
