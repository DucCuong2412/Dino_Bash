using UnityEngine;

public class AchievementPopUp : BaseScreen
{
	private tk2dTextMesh label;

	private Vector3 icon_scale;

	private ParticleSystem fx;

	public SpriteRenderer icon { get; set; }

	private void Start()
	{
		label = GetComponentInChildren<tk2dTextMesh>();
		icon = GetComponentInChildren<SpriteRenderer>();
		icon_scale = icon.transform.localScale;
		fx = icon.transform.FindChild("FX_Stars").GetComponent<ParticleSystem>();
		base.transform.localPosition += base.top;
		base.gameObject.SetActive(false);
	}

	public void Show(string achievement_id)
	{
		fx.gameObject.SetActive(false);
		string text = SocialGamingIds.getI18NKeyForAchievementId(achievement_id);
		label.text = "unlocked_achievement".Localize();
		label.ForceBuild();
		icon.transform.localScale = Vector3.zero;
		Sprite sprite = Resources.Load<Sprite>("GUI/achievements/" + text);
		if (sprite != null)
		{
			icon.sprite = sprite;
		}
		base.gameObject.SetActive(true);
		base.Show();
		ShowFrom(base.top, null, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		WaitThen(1f, delegate
		{
			label.text = text.Localize();
			label.ForceBuild();
			Go.to(icon.transform, 0.5f, new GoTweenConfig().scale(icon_scale).setEaseType(GoEaseType.BounceOut));
			WaitThen(0.4f, delegate
			{
				fx.gameObject.SetActive(true);
			});
			WaitThen(3f, Hide);
		});
	}

	public override void Hide()
	{
		base.Hide();
		HideTo(base.top, delegate
		{
			base.gameObject.SetActive(false);
		}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
	}
}
