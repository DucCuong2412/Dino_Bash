using UnityEngine;

public class RewardedVideoPopup : BaseScreen
{
	private tk2dTextMesh reward_description;

	private SpriteRenderer reward_sprite;

	private SpriteRenderer coin_reward;

	private ParticleSystem fx;

	private RewardedVideoItems reward;

	private int amount;

	private void Start()
	{
		reward_description = FindChildComponent<tk2dTextMesh>("UpperCenter/bg/label");
		reward_sprite = FindChildComponent<SpriteRenderer>("UpperCenter/bg/reward_icon");
		coin_reward = FindChildComponent<SpriteRenderer>("UpperCenter/bg/reward_icon/icon_coin");
		fx = FindChildComponent<ParticleSystem>("UpperCenter/bg/reward_icon/FX_Stars");
		fx.gameObject.SetActive(false);
		base.transform.localPosition += base.top;
		base.gameObject.SetActive(false);
		amount = 0;
	}

	public void Show(RewardedVideoItems reward, int amount)
	{
		this.reward = reward;
		this.amount = amount;
		Show();
	}

	public override void Show()
	{
		if (amount == 0)
		{
			return;
		}
		ResultScreen screen = ScreenManager.GetScreen<ResultScreen>();
		if (screen != null && reward == RewardedVideoItems.Coins)
		{
			screen.handleCoinBoostReward();
		}
		base.gameObject.SetActive(true);
		base.Show();
		Vector3 icon_scale = Vector3.one * 0.65f;
		coin_reward.enabled = false;
		if (reward == RewardedVideoItems.Coins)
		{
			coin_reward.enabled = true;
			reward_sprite.enabled = false;
		}
		else if (reward != RewardedVideoItems.Lives)
		{
			Sprite consumableSprite = SpriteRessources.getConsumableSprite((UnitType)reward);
			reward_sprite.enabled = true;
			reward_sprite.sprite = consumableSprite;
			reward_sprite.transform.localScale = 1.2f * Vector3.one;
			icon_scale = reward_sprite.transform.localScale;
		}
		reward_sprite.transform.localScale = Vector3.zero;
		reward_description.text = "rewarded_video_successfull".Localize();
		reward_description.ForceBuild();
		base.gameObject.SetActive(true);
		base.Show();
		ShowFrom(base.top, null, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		WaitThen(1f, delegate
		{
			string empty = string.Empty;
			empty = ((amount != 1) ? (amount + " " + (reward.ToString() + ".reward.plural").Localize()) : (reward.ToString() + ".reward.singular").Localize());
			reward_description.text = empty;
			reward_description.ForceBuild();
			Go.to(reward_sprite.transform, 0.5f, new GoTweenConfig().scale(icon_scale).setEaseType(GoEaseType.BounceOut));
			WaitThen(0.5f, delegate
			{
				fx.gameObject.SetActive(true);
			});
			if (App.State == App.States.Game)
			{
				WaitThen(1.5f, Hide);
			}
			else
			{
				WaitThen(3f, Hide);
			}
		});
	}

	public override void Hide()
	{
		amount = 0;
		base.Hide();
		HideTo(base.top, delegate
		{
			base.gameObject.SetActive(false);
		}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
	}
}
