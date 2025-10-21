using System;
using UnityEngine;
using dinobash;

public class LevelupScreen : BaseScreen
{
	private StandardButton btn_ok;

	private tk2dTextMesh xp_level;

	private SpriteRenderer icon_diamond;

	private FlyElementTo fly_diamond;

	private Animator level_up_star;

	private ParticleSystem fx;

	private SpriteRenderer glow;

	protected void Start()
	{
		btn_ok = base.transform.Search("btn_ok").GetComponent<StandardButton>();
		btn_ok.clickSound = Sounds.main_continue_button;
		btn_ok.uiItem.OnClick += Hide;
		xp_level = base.transform.Search("xplevel_label").GetComponent<tk2dTextMesh>();
		level_up_star = base.transform.Search("levelup_star").GetComponent<Animator>();
		fx = base.transform.Search("FX_Stars").GetComponent<ParticleSystem>();
		glow = base.transform.Search("unlock_glow_big").GetComponent<SpriteRenderer>();
		icon_diamond = base.transform.Search("icon_diamond").GetComponent<SpriteRenderer>();
		SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate(icon_diamond) as SpriteRenderer;
		SpriteTools.SetSortingLayerID(spriteRenderer, 46);
		spriteRenderer.transform.parent = icon_diamond.transform.parent;
		spriteRenderer.transform.position = icon_diamond.transform.position;
		fly_diamond = spriteRenderer.gameObject.AddComponent<FlyElementTo>();
		fly_diamond.gameObject.SetActive(false);
		if (xp_level == null || btn_ok == null)
		{
			throw new Exception("Component not found!");
		}
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public override void Show()
	{
		base.gameObject.SetActive(true);
		base.Show();
		xp_level.text = (Player.XPLevel + 1).ToString();
		xp_level.color = Colors.Invisible;
		xp_level.Commit();
		glow.transform.localScale = Vector3.zero;
		WaitThen(0.3f, delegate
		{
			Go.to(glow.transform, 0.3f, new GoTweenConfig().scale(Vector3.one));
			fx.Play();
			level_up_star.Play("star raise");
			Go.to(xp_level, 0.3f, new GoTweenConfig().colorProp("color", Colors.Visible).setDelay(0.2f));
			btn_ok.isFocused = true;
			ResourceBarScreen screen = ScreenManager.GetScreen<ResourceBarScreen>();
			float delay = 0.5f;
			fly_diamond.gameObject.SetActive(true);
			fly_diamond.Play(screen.icon_diamonds.transform.position, delay);
			Go.to(fly_diamond.transform, fly_diamond.duration, new GoTweenConfig().scale(screen.icon_diamonds.transform.localScale).setDelay(delay));
		});
		ShowFrom(base.right);
	}

	public override void Hide()
	{
		base.Interactive = false;
		WaitThen(fly_diamond.normalized_time * fly_diamond.duration, delegate
		{
			Go.to(glow.transform, 0.3f, new GoTweenConfig().scale(Vector3.zero));
			base.Hide();
			HideTo(base.left, delegate
			{
				base.gameObject.SetActive(false);
			});
		});
	}
}
