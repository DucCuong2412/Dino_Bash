using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShootingTutorial : AbstractTutorialScreen
{
	private TutorialArrow btnArrow;

	private GameObject swipeArrow;

	private Transform dragArrow;

	private ShootButton shot_button;

	private ShotCarrier shot_carrier;

	private FocusMask focusMask;

	private Camera gameCamera;

	private float btnOriginalYPosition;

	private Unit neander;

	private int numberOfShootsNeededToKillNeander = 2;

	private bool useDragShots;

	private GoTween camera_sequence;

	private Vector3 level_end_position;

	private Vector3 shot_button_position;

	protected override void Start()
	{
		base.Start();
		useDragShots = Konfiguration.GameConfig.Use_dragshot_feature;
		base.DinoEggInvincible = true;
		HudScreen screen = ScreenManager.GetScreen<HudScreen>();
		shot_button = screen.ShotButtons[0].GetComponent<ShootButton>();
		foreach (ShootButton shotButton in screen.ShotButtons)
		{
			shotButton.gameObject.SetActive(false);
		}
		foreach (BuyUnitButton unitButton in screen.UnitButtons)
		{
			unitButton.gameObject.SetActive(false);
		}
		btnArrow = getArrow();
		btnArrow.transform.RepositionAndReparent(shot_button.transform);
		focusMask = getFocusMask();
		dragArrow = getSwipeArrow().transform;
		dragArrow.transform.parent = base.transform;
		Animator component = dragArrow.GetComponent<Animator>();
		component.updateMode = AnimatorUpdateMode.UnscaledTime;
		dragArrow.gameObject.SetActive(false);
		swipeArrow = getSwipeArrow();
		swipeArrow.transform.position = ScreenManager.Camera.ViewportToWorldPoint(new Vector3(0.2f, 0.5f, 0f));
		swipeArrow.transform.parent = base.transform;
		swipeArrow.SetActive(false);
		gameCamera = Camera.main;
		screen.transform.Search("AppleCounter").gameObject.SetActive(false);
		screen.transform.Search("skipIntroLabel").gameObject.SetActive(false);
		shot_carrier = ShotFactory.GetCarrier(ShotType.Normal);
		shot_carrier.tutorialMode = true;
		SetDialogTween(base.top, base.upperDialogPosition);
		Level.Instance.OnLevelPlay += delegate
		{
			StartCoroutine(TutorialPart1());
		};
	}

	private void OnDisable()
	{
		ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(OnFired));
		ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(OnFiredSecondTime));
	}

	private IEnumerator TutorialPart1()
	{
		GameCamera.Instance.EnterState_Disabled();
		yield return new WaitForSeconds(1f);
		ShowDialog("Protect the egg!");
		Vector3 eggPos = Camera.main.WorldToViewportPoint(egg.transform.position + new Vector3(-75f, 230f, 0f));
		Vector3 screenPos = ScreenManager.Camera.ViewportToWorldPoint(eggPos);
		focusMask.Show(screenPos, new Vector3(7f, 7f, 7f));
		while (base.dialog_open)
		{
			yield return null;
		}
		focusMask.Hide();
		level_end_position = gameCamera.transform.position.SetX((float)Level.Instance.Config.levelwidth - gameCamera.GetComponent<tk2dCamera>().ScreenExtents.width);
		Go.to(Camera.main.transform, 2f, new GoTweenConfig().localPosition(level_end_position));
		neander = EntityFactory.Create(Level.Instance.Config.enemies[0].unittype).GetComponent<Unit>();
		neander.ChangeHealth(-neander.Health + (Konfiguration.ShotData[ShotType.Normal].damage * numberOfShootsNeededToKillNeander - 1), null);
		neander.transform.position += new Vector3(-200f, 0f, 0f);
		yield return new WaitForSeconds(3f);
		shot_button.gameObject.SetActive(true);
		shot_button.uiItem.enabled = false;
		shot_button_position = shot_button.transform.localPosition;
		Go.from(shot_button.transform, 1f, new GoTweenConfig().localPosition(shot_button_position.SetY(-256f)));
		yield return new WaitForSeconds(1f);
		FollowTarget(neander.transform);
		if (useDragShots)
		{
			yield return new WaitForSeconds(1f);
			ShowDialog("dragshot.tutorial.1", true);
			dragArrow.transform.position = shot_button.transform.position;
			dragArrow.gameObject.SetActive(true);
			dragArrow.GetComponent<Animator>().Play("SwipeArrow_Drag");
			dragStart = dragArrow.transform.position;
			DragTween(dragArrow, neander.transform);
			shot_button.uiItem.OnUp += HandleOnStartDrag;
			shot_button.uiItem.enabled = true;
			shot_button.SetDragTutorial(neander.transform, DragAction);
		}
		else
		{
			shot_button.uiItem.enabled = true;
			btnArrow.In();
			ShowDialog("Shoot on offender!", true);
			ShotFactory.OnShotStart = (Action)Delegate.Combine(ShotFactory.OnShotStart, new Action(OnFired));
		}
	}

	private void HandleOnStartDrag()
	{
		HideDialog();
		shot_button.uiItem.OnUp -= HandleOnStartDrag;
		if (dragTween != null)
		{
			dragTween.destroy();
			dragArrow.gameObject.SetActive(false);
		}
	}

	private void DragAction(bool success)
	{
		StartCoroutine(DragResult(success));
	}

	private IEnumerator DragResult(bool success)
	{
		if (success)
		{
			shot_button.gameObject.SetActive(false);
			if (dragTween != null)
			{
				dragTween.destroy();
				dragArrow.gameObject.SetActive(false);
			}
			shot_button.SetDragTutorial(null, null);
			yield return new WaitForSeconds(2f);
			ShowDialog("dragshot.tutorial.2");
			while (base.dialog_open)
			{
				yield return null;
			}
			StartCoroutine(TutorialPart2());
		}
		else
		{
			ShowDialog("dragshot.tutorial.3", true);
			DragTween(dragArrow, neander.transform);
			dragArrow.gameObject.SetActive(true);
			dragArrow.GetComponent<Animator>().Play("SwipeArrow_Drag");
			shot_button.uiItem.OnUp += HandleOnStartDrag;
		}
	}

	private void HideArrow()
	{
		shot_button.uiItem.OnUp -= HideArrow;
		btnArrow.Out();
	}

	private IEnumerator TutorialPart2()
	{
		ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(OnFired));
		if (!useDragShots)
		{
			btnArrow.Out();
		}
		follow = false;
		HideDialog();
		yield return new WaitForSeconds(0.3f);
		SetDialogTween(base.bottom, base.lowerDialogPosition);
		if (!useDragShots)
		{
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
			shot_button.gameObject.SetActive(false);
			HideDialog();
			GameCamera.Instance.EnterState_Disabled();
			Time.timeScale = 1f;
		}
		if (useDragShots)
		{
			yield return new WaitForSeconds(1f);
		}
		else
		{
			yield return new WaitForSeconds(2f);
		}
		swipeArrow.SetActive(true);
		ShowDialog("swipe to scroll left", true);
		GameCamera.Instance.EnterState_Playing();
		while (!(gameCamera.transform.position.x < -300f))
		{
			yield return null;
		}
		swipeArrow.SetActive(false);
		HideDialog();
		yield return new WaitForSeconds(1f);
		shot_button.transform.localPosition = shot_button_position;
		shot_button.gameObject.SetActive(true);
		shot_button.uiItem.enabled = true;
		Go.from(shot_button.transform, 1f, new GoTweenConfig().localPosition(shot_button_position.SetY(-256f)));
		if (neander.state == Unit.State.move)
		{
			ShowDialog("shoot again!", true);
			btnArrow.In();
			if (useDragShots)
			{
				shot_button.uiItem.OnUp += HideArrow;
			}
			ShotFactory.OnShotStart = (Action)Delegate.Combine(ShotFactory.OnShotStart, new Action(OnFiredSecondTime));
		}
		else
		{
			StartCoroutine(TutorialPart4());
		}
	}

	private IEnumerator TutorialPart3()
	{
		HideDialog();
		yield return new WaitForSeconds(0.1f);
		bool playerHasNotShot = true;
		while (shot_carrier.transform.position.x < neander.transform.position.x - 500f)
		{
			if (Input.GetMouseButton(0) || Input.touchCount > 0)
			{
				playerHasNotShot = false;
				break;
			}
			yield return null;
		}
		if (!useDragShots)
		{
			if (playerHasNotShot)
			{
				ShowDialog("don't forget to release!", true);
				Time.timeScale = 0f;
				while (base.game_is_paused || (!Input.GetMouseButton(0) && Input.touchCount <= 0))
				{
					yield return null;
				}
				Time.timeScale = 1f;
			}
			yield return new WaitForSeconds(0.3f);
			HideDialog();
		}
		while (neander.state != Unit.State.die)
		{
			yield return null;
		}
		actionHinting.hintingInterval = 4f;
		actionHinting.StartNotification();
		StartCoroutine(TutorialPart4());
	}

	private IEnumerator TutorialPart4()
	{
		List<Unit> enemies = new List<Unit>
		{
			neander,
			EntityFactory.Create(Level.Instance.Config.enemies[1].unittype).GetComponent<Unit>()
		};
		yield return new WaitForSeconds(2f);
		Go.to(Camera.main.transform, 1f, new GoTweenConfig().localPosition(level_end_position));
		ShowDialog("Take out the neander!", true);
		ShotFactory.OnShotStart = (Action)Delegate.Combine(ShotFactory.OnShotStart, new Action(HideAndUnsubscribe));
		while (enemies.Exists((Unit x) => x.Health > 0))
		{
			yield return null;
		}
		Level.Instance.EndTutorial(Level.State.won);
	}

	private void HideAndUnsubscribe()
	{
		ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(HideAndUnsubscribe));
		HideDialog();
	}

	private void OnFired()
	{
		StartCoroutine(TutorialPart2());
	}

	private void OnFiredSecondTime()
	{
		ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(OnFiredSecondTime));
		HideDialog();
		if (!useDragShots)
		{
			btnArrow.Out();
		}
		StartCoroutine(TutorialPart3());
	}
}
