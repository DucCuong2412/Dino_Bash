using System;
using UnityEngine;
using dinobash;

public class UnlockScreen : BaseScreen
{
	private class UnlockContainer
	{
		public UnitType dino;

		public ShotType shot = ShotType.None;

		public UnitType upgrade;

		public UnlockContainer(ShotType shot)
		{
			if (shot != ShotType.None)
			{
				this.shot = shot;
				return;
			}
			throw new Exception("not a valid shottype");
		}

		public UnlockContainer(UnitType entitiy)
		{
			if (Konfiguration.isDinoUnit(entitiy) || Konfiguration.isConsumable(entitiy))
			{
				dino = entitiy;
				return;
			}
			if (Konfiguration.isUpgrade(entitiy))
			{
				upgrade = entitiy;
				return;
			}
			throw new Exception("not a valid unittype");
		}
	}

	private StandardButton btn_shop;

	private StandardButton btn_ok;

	private Transform unlock_image;

	private LocalizedText description;

	private LocalizedText itemTitle;

	private LocalizedText screenTitle;

	private SpriteRenderer glow;

	private ParticleSystem fx;

	public bool openUpUpgradeScreen;

	private UnlockContainer unlocked;

	private bool init;

	private Transform unlock_portrait;

	public void Show(UnitType unitType)
	{
		if (unitType != 0)
		{
			unlocked = new UnlockContainer(unitType);
			Show();
		}
	}

	public void Show(ShotType shot)
	{
		if (shot != ShotType.None)
		{
			unlocked = new UnlockContainer(shot);
			Show();
		}
	}

	protected void Start()
	{
		if (init)
		{
			return;
		}
		init = true;
		btn_ok = base.transform.Search("btn_ok").GetComponent<StandardButton>();
		btn_ok.clickSound = Sounds.main_continue_button;
		btn_ok.uiItem.OnClick += Hide;
		btn_shop = base.transform.Search("btn_shop").GetComponent<StandardButton>();
		btn_shop.clickSound = Sounds.main_continue_button;
		btn_shop.uiItem.OnClick += delegate
		{
			if (Player.WatchedUpgradeTutorial && App.State == App.States.Game)
			{
				openUpUpgradeScreen = true;
			}
			Hide();
		};
		unlock_image = base.transform.Search("unlock_picture");
		screenTitle = base.transform.Search("title_label").GetComponent<LocalizedText>();
		itemTitle = base.transform.Search("unlock_name").GetComponent<LocalizedText>();
		description = base.transform.Search("unlock_description").GetComponent<LocalizedText>();
		if (itemTitle == null || description == null || btn_ok == null || unlock_image == null)
		{
			throw new Exception("Component not found!");
		}
		glow = base.transform.Search("unlock_glow_big").GetComponent<SpriteRenderer>();
		fx = base.transform.Search("FX_Stars").GetComponent<ParticleSystem>();
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	public override void Show()
	{
		Start();
		base.gameObject.SetActive(true);
		if (Player.CurrentLevelID < Tutorials.LevelID("UpgradeTutorial") || (unlocked.dino != 0 && Konfiguration.isConsumable(unlocked.dino)))
		{
			btn_shop.gameObject.SetActive(false);
			btn_ok.transform.LocalPosX(0f);
		}
		Sounds unlock_sound = Sounds.unlock_dino;
		if (unlock_portrait != null)
		{
			UnityEngine.Object.Destroy(unlock_portrait.gameObject);
		}
		if (unlocked.dino != 0)
		{
			screenTitle.Key = ((!Konfiguration.isConsumable(unlocked.dino)) ? "you unlocked a new dino!" : "you unlocked a powerup");
			itemTitle.Key = unlocked.dino.ToString();
			description.Key = unlocked.dino.ToString() + ".description";
			unlock_portrait = UnityEngine.Object.Instantiate(Resources.Load<Transform>("GUI/Unlocks/Dino_" + unlocked.dino)) as Transform;
			unlock_portrait.RepositionAndReparent(unlock_image);
			EntityShadow componentInChildren = unlock_portrait.GetComponentInChildren<EntityShadow>();
			if (componentInChildren != null)
			{
				componentInChildren.sprite.color = GameColors.UnlockScreenShadow;
			}
			Animator componentInChildren2 = unlock_portrait.GetComponentInChildren<Animator>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.Play("stand");
			}
			unlock_sound = Sounds.unlock_dino;
		}
		else if (unlocked.shot != ShotType.None)
		{
			screenTitle.Key = "you unlocked a new shot!";
			itemTitle.Key = unlocked.shot.ToString();
			description.Key = unlocked.shot.ToString() + ".description";
			unlock_portrait = UnityEngine.Object.Instantiate(Resources.Load<Transform>("GUI/Unlocks/Shot_" + unlocked.shot)) as Transform;
			unlock_portrait.RepositionAndReparent(unlock_image);
			unlock_sound = Sounds.unlock_shot;
		}
		else
		{
			if (unlocked.upgrade == UnitType.None)
			{
				Debug.LogError("Not a valid call on UnlockScreen?");
				return;
			}
			screenTitle.Key = "you unlocked a new upgrade!";
			itemTitle.Key = unlocked.upgrade.ToString();
			string text = unlocked.upgrade.ToString() + ".description";
			if (unlocked.upgrade == UnitType.AppleStartBonus)
			{
				description.textMesh.text = string.Format(text.Localize(), Konfiguration.GameConfig.AppleBoostAmount);
			}
			else
			{
				description.Key = text;
			}
			unlock_portrait = UnityEngine.Object.Instantiate(Resources.Load<Transform>("GUI/Unlocks/Upgrade_" + unlocked.upgrade)) as Transform;
			unlock_portrait.RepositionAndReparent(unlock_image);
			unlock_sound = Sounds.unlock_dino;
		}
		SpriteTools.SetSortingLayerID(unlock_portrait, base.SortingLayerID);
		SpriteTools.OffsetSortingOrder(unlock_portrait, 1000);
		glow.transform.localScale = Vector3.zero;
		base.Show();
		WaitThen(0.3f, delegate
		{
			btn_shop.isFocused = true;
			Go.to(glow.transform, 0.3f, new GoTweenConfig().scale(Vector3.one));
			fx.Play();
			if (App.State == App.States.Game)
			{
				AudioPlayer.PlayGuiSFX(unlock_sound, 0f);
			}
		});
		ShowFrom(base.right);
	}

	public override void Hide()
	{
		base.Interactive = false;
		Go.to(glow.transform, 0.3f, new GoTweenConfig().scale(Vector3.zero));
		Tracking.unlock_snappy();
		Tracking.unlocked_volcano_bomb();
		Tracking.unlocked_raptor();
		HideTo(base.left, delegate
		{
			base.Hide();
			base.gameObject.SetActive(false);
		});
	}
}
