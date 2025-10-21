using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class MegaBallTutorial : AbstractTutorialScreen
{
	private FocusMask focusMask;

	private TutorialArrow arrow;

	private HudScreen hud;

	private BuyUnitButton megaballButton;

	protected override void Start()
	{
		base.Start();
		hud = ScreenManager.GetScreen<HudScreen>();
		megaballButton = hud.ConsumableButtons.Find((BuyUnitButton x) => x.Unit == UnitType.MegaBall);
		megaballButton.disable = true;
		hud.ShotButtons.ForEach(delegate(ShootButton x)
		{
			x.Enabled = false;
		});
		if (Player.getConsumableCount(UnitType.MegaBall) < 3)
		{
			int consumableCount = Player.getConsumableCount(UnitType.MegaBall);
			Player.changeConsumableCount(UnitType.MegaBall, 3 - consumableCount);
		}
		focusMask = getFocusMask();
		arrow = getArrow(true);
		arrow.transform.RepositionAndReparent(megaballButton.transform);
		arrow.transform.localScale = Vector3.one;
		GameCamera.Instance.EnterState_Disabled();
		SetDialogTween(base.top, base.upperDialogPosition);
		StartCoroutine(SpawnEnemies());
		StartCoroutine(TutorialPart1());
	}

	private IEnumerator SpawnEnemies()
	{
		List<LevelEnemy> enemies = Level.Instance.Config.enemies;
		for (int to_spawn = 0; to_spawn < 5; to_spawn++)
		{
			LevelEnemy enemy = enemies[to_spawn];
			Level.Instance.SpawnEnemy(enemy);
			yield return new WaitForSeconds(0.5f + Random.value);
		}
	}

	private IEnumerator TutorialPart1()
	{
		yield return new WaitForSeconds(1f);
		hud.UnitButtons.ForEach(delegate(BuyUnitButton x)
		{
			x.disable = true;
		});
		yield return new WaitForSeconds(1f);
		StartAppleCap(Player.Instance.Apples);
		ShowDialog("megaball_tutorial_00");
		while (base.dialog_open)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		ShowDialog("megaball_tutorial_01");
		while (base.dialog_open)
		{
			yield return null;
		}
		StopAppleCap();
		yield return new WaitForSeconds(1f);
		arrow.In();
		focusMask.Show(megaballButton.transform.position, Vector3.one * 5.5f);
		ShowDialog("megaball_tutorial_02", true);
		megaballButton.disable = false;
		yield return null;
		int apples = Player.Instance.Apples;
		while (megaballButton.Enabled)
		{
			Player.Instance.Apples = apples;
			yield return null;
		}
		focusMask.Hide();
		arrow.Out();
		HideDialog();
		megaballButton.uiItem.enabled = false;
		BaseEntity megaball = EntityFactory.GetEntities(true).Find((BaseEntity x) => x.unitType == UnitType.MegaBall);
		GameCamera.Instance.EnterState_Following(megaball.transform, false);
		while (GameCamera.Instance.state == GameCamera.States.following)
		{
			yield return null;
		}
		StartAppleCap(Player.Instance.Apples);
		ShowDialog("megaball_tutorial_03");
		while (base.dialog_open)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.3f);
		ShowDialog("megaball_tutorial_04");
		while (base.dialog_open)
		{
			yield return null;
		}
		StopAppleCap();
		Player.changeConsumableCount(UnitType.MegaBall, 1);
		Level.Instance.EndTutorial(Level.State.playing);
		hud.ShotButtons.ForEach(delegate(ShootButton x)
		{
			x.Enabled = true;
		});
		hud.UnitButtons.ForEach(delegate(BuyUnitButton x)
		{
			x.disable = false;
		});
	}
}
