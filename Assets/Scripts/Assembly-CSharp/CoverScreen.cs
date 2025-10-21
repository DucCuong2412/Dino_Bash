using UnityEngine;

public class CoverScreen : BaseScreen
{
	private SpriteRenderer sprite;

	private Collider clickBlocker;

	private bool is_init;

	private GoTween blendTween;

	protected virtual void Start()
	{
		if (!is_init)
		{
			is_init = true;
			sprite = GetComponentInChildren<SpriteRenderer>();
			sprite.color = Color.clear;
			sprite.enabled = false;
			clickBlocker = GetComponentInChildren<Collider>();
			clickBlocker.enabled = false;
			sprite.AddOrGetComponent<SpriteStretcher>();
			Hide();
		}
	}

	public void Show(bool skip_animation = false)
	{
		Start();
		base.gameObject.SetActive(true);
		clickBlocker.enabled = true;
		if (blendTween != null)
		{
			blendTween.destroy();
		}
		base.Show();
		sprite.enabled = true;
		Color color = new Color(0.6f, 0.6f, 0.6f, 0.75f);
		if (!skip_animation)
		{
			blendTween = Go.to(sprite, 0.3f, new GoTweenConfig().colorProp("color", color).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
		}
		else
		{
			sprite.color = color;
		}
	}

	public override void Hide()
	{
		clickBlocker.enabled = false;
		if (blendTween != null)
		{
			blendTween.destroy();
		}
		base.Hide();
		blendTween = Go.to(sprite, 0.3f, new GoTweenConfig().colorProp("color", Color.clear).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate).onComplete(delegate
		{
			base.gameObject.SetActive(false);
		}));
	}
}
