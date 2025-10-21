using UnityEngine;

public class QuitGameScreen : BaseScreen
{
	private SpriteRenderer bg_fill;

	private GoTween blend_tween;

	protected void Start()
	{
		bg_fill = base.transform.Search("bg_fill").GetComponent<SpriteRenderer>();
		bg_fill.color = Colors.Invisible;
		base.transform.Search("btn_ok").GetComponent<tk2dUIItem>().OnClick += delegate
		{
			Application.Quit();
		};
		base.transform.Search("btn_cancel").GetComponent<tk2dUIItem>().OnClick += Hide;
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public override void Show()
	{
		base.gameObject.SetActive(true);
		base.Show();
		ShowFrom(base.right, delegate
		{
			OnEscapeUp = Hide;
			if (blend_tween != null)
			{
				blend_tween.complete();
			}
			blend_tween = Go.to(bg_fill, 0.2f, new GoTweenConfig().colorProp("color", new Color(1f, 1f, 1f, 0.5f)));
		}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
	}

	public override void Hide()
	{
		base.Hide();
		base.isVisible = true;
		if (blend_tween != null)
		{
			blend_tween.complete();
		}
		blend_tween = Go.to(bg_fill, 0.1f, new GoTweenConfig().colorProp("color", Colors.Invisible).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate).onComplete(delegate
		{
			HideTo(base.left, delegate
			{
				base.gameObject.SetActive(false);
				base.isVisible = false;
			}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		}));
	}
}
