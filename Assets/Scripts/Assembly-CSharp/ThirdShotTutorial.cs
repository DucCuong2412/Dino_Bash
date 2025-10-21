using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class ThirdShotTutorial : AbstractTutorialScreen
{
	private TutorialArrow arrow;

	private HudScreen hud;

	private ShotCarrier shot_carrier;

	private bool useDragShots;

	private ShootButton third_shot_button;

	protected override void Start()
	{
		base.Start();
		useDragShots = Konfiguration.GameConfig.Use_dragshot_feature;
		hud = ScreenManager.GetScreen<HudScreen>();
		hud.ConsumableButtons.ForEach(delegate(BuyUnitButton x)
		{
			x.disable = true;
		});
		hud.ShotButtons.ForEach(delegate(ShootButton x)
		{
			x.Enabled = false;
		});
		arrow = getArrow();
		arrow.transform.RepositionAndReparent(hud.ShotButtons[0].transform);
		arrow.transform.localScale = Vector3.one;
		ShotType unlockShot = Konfiguration.levels.FindAll((LevelData x) => x.unlockShot != ShotType.None)[1].unlockShot;
		shot_carrier = ShotFactory.GetCarrier(unlockShot);
		shot_carrier.tutorialMode = true;
		SetDialogTween(base.top, base.upperDialogPosition);
		GameCamera.Instance.EnterState_Disabled();
		StartCoroutine(TutorialPart1());
	}

	private void HideArrow()
	{
		third_shot_button.uiItem.OnUp -= HideArrow;
		arrow.Out();
	}

	private IEnumerator TutorialPart1()
	{
		third_shot_button = hud.ShotButtons[0];
		List<LevelEnemy> enemies = Level.Instance.Config.enemies;
		Level.Instance.SpawnEnemy(enemies[0]);
		BaseEntity enemy = EntityFactory.GetEntities(false)[0];
		if (useDragShots)
		{
			third_shot_button.uiItem.OnUp += HideArrow;
			GameCamera.Instance.PlayIntroPan(Level.Instance.Config.levelwidth);
			yield return new WaitForSeconds(2f);
		}
		yield return new WaitForSeconds(1f);
		if (useDragShots)
		{
			FollowTarget(enemy.transform);
		}
		StartAppleCap(Player.Instance.Apples);
		ShowDialog("Oh very well, you unlocked the third shot!");
		while (base.dialog_open)
		{
			yield return null;
		}
		StopAppleCap();
		yield return new WaitForSeconds(1f);
		ShowDialog("Fire your new shot!", true);
		arrow.In();
		ShootButton shot_button = hud.ShotButtons[0];
		shot_button.Enabled = true;
		while (shot_button.Enabled)
		{
			yield return null;
		}
		follow = false;
		if (!useDragShots)
		{
			arrow.Out();
		}
		HideDialog();
		yield return new WaitForSeconds(0.3f);
		SetDialogTween(base.bottom, base.lowerDialogPosition);
		if (!useDragShots)
		{
			BaseEntity neander = EntityFactory.GetEntities(false)[0];
			while (shot_carrier.transform.position.x < neander.transform.position.x - 500f)
			{
				yield return null;
			}
			ShowDialog("Tap anywhere to fire", true);
			Time.timeScale = 0f;
			while (base.game_is_paused || (!Input.GetMouseButton(0) && Input.touchCount <= 0))
			{
				yield return null;
			}
			shot_carrier.TutorialShoot();
			HideDialog();
			Time.timeScale = 1f;
			while (hud.state != HudScreen.State.In)
			{
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(1f);
		}
		yield return new WaitForSeconds(1f);
		StartAppleCap(Player.Instance.Apples);
		SetDialogTween(base.top, base.upperDialogPosition);
		ShowDialog("Now have fun giving them hell!");
		while (base.dialog_open)
		{
			yield return null;
		}
		StopAppleCap();
		GameCamera.Instance.EnterState_Playing();
		Level.Instance.EndTutorial(Level.State.playing);
		hud.ShotButtons.ForEach(delegate(ShootButton x)
		{
			if (x != shot_button)
			{
				x.Enabled = true;
			}
		});
		hud.ConsumableButtons.ForEach(delegate(BuyUnitButton x)
		{
			x.disable = false;
		});
	}
}
