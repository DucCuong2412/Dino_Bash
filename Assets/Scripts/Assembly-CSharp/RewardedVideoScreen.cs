using UnityEngine;
using dinobash;

public class RewardedVideoScreen : BaseScreen
{
	private int amount;

	private StandardButton play_button;

	private StandardButton close_button;

	private Transform glow;

	private LocalizedText title;

	private LocalizedText reward_description;

	private SpriteRenderer reward_sprite;

	private SpriteRenderer reward_coins;

	private bool init;

	private Vector3 heart_position;

	private Sprite heart_sprite;

	public RewardedVideoItems item { get; private set; }

	private void Start()
	{
		if (!init)
		{
			init = true;
			close_button = base.transform.Search("btn_close").GetComponent<StandardButton>();
			close_button.clickSound = Sounds.main_close_popup;
			close_button.uiItem.OnClick += delegate
			{
				Tracking.rewarded_video(Player.CurrentLevelID, Tracking.RewardedVideoAction.closed, item, 0);
				Hide();
			};
			title = FindChildComponent<LocalizedText>("MiddleCenter/title/title_label");
			reward_description = FindChildComponent<LocalizedText>("MiddleCenter/inlay/reward_description");
			reward_sprite = FindChildComponent<SpriteRenderer>("MiddleCenter/inlay/reward");
			heart_sprite = reward_sprite.sprite;
			heart_position = reward_sprite.transform.localPosition;
			reward_coins = FindChildComponent<SpriteRenderer>("MiddleCenter/inlay/reward_coins");
			reward_coins.gameObject.SetActive(false);
			play_button = FindChildComponent<StandardButton>("MiddleCenter/btn_play_video");
			play_button.uiItem.OnClick += StartRewardedVideo;
			base.transform.localPosition += base.right;
			base.gameObject.SetActive(false);
		}
	}

	private void StartRewardedVideo()
	{
		AudioPlayer.IsAdPlaying = true;
		if (RewardedVideosWrapper.hasVideo)
		{
			if (RewardedVideosWrapper.ShowVideo(item, Hide))
			{
				play_button.uiItem.enabled = false;
				close_button.uiItem.enabled = false;
				OnEscapeUp = null;
			}
		}
		else
		{
			Hide();
		}
	}

	private string getTitle(RewardedVideoItems item)
	{
		switch (item)
		{
		case RewardedVideoItems.MegaBall:
		case RewardedVideoItems.Blizzard:
		case RewardedVideoItems.MeteorStorm:
			return "rewarded_video_title_consumable";
		case RewardedVideoItems.Lives:
			return "rewarded_video_title_lives";
		case RewardedVideoItems.Coins:
			return "rewarded_video_title_coins";
		default:
			return "item not found!";
		}
	}

	public void Show(RewardedVideoItems item)
	{
		this.item = item;
		Show();
	}

	public override void Show()
	{
		Start();
		base.gameObject.SetActive(true);
		base.Show();
		play_button.uiItem.enabled = true;
		close_button.uiItem.enabled = true;
		OnEscapeUp = Hide;
		int rewardAmount = RewardedVideosWrapper.getRewardAmount(item);
		title.Key = getTitle(item);
		string text = "rewarded_video_description".Localize();
		if (rewardAmount == 1)
		{
			text = text + " " + (item.ToString() + ".reward.singular").Localize();
		}
		else
		{
			string text2 = text;
			text = text2 + " " + rewardAmount + " " + (item.ToString() + ".reward.plural").Localize();
		}
		reward_description.textMesh.text = text;
		reward_sprite.transform.localPosition = heart_position;
		reward_sprite.transform.localScale = Vector3.one;
		if (item > (RewardedVideoItems)0)
		{
			Sprite consumableSprite = SpriteRessources.getConsumableSprite((UnitType)item);
			reward_sprite.sprite = consumableSprite;
			reward_sprite.transform.localScale = 1.2f * Vector3.one;
		}
		else if (item == RewardedVideoItems.Lives)
		{
			reward_sprite.sprite = heart_sprite;
		}
		else if (item == RewardedVideoItems.Coins)
		{
			reward_sprite.sprite = reward_coins.sprite;
			reward_sprite.transform.localPosition = reward_coins.transform.localPosition;
			reward_sprite.transform.localScale = reward_coins.transform.localScale;
		}
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		Tracking.rewarded_video(Player.CurrentLevelID, Tracking.RewardedVideoAction.offer, item, 0);
		ShowFrom(base.right, delegate
		{
		});
	}

	public override void Hide()
	{
		base.Hide();
		base.isVisible = true;
		AudioPlayer.IsAdPlaying = false;
		if (App.State == App.States.Map)
		{
			MapScreen screen = ScreenManager.GetScreen<MapScreen>();
			screen.UpdateGiftCoinsButtonState();
		}
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		HideTo(base.left, delegate
		{
			base.gameObject.SetActive(false);
			base.isVisible = false;
		});
	}
}
