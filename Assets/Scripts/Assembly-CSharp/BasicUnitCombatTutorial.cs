using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class BasicUnitCombatTutorial : AbstractTutorialScreen
{
	private TutorialArrow buyUnitArrow;

	private Transform dinoButton;

	private Transform standardBulletBtn;

	private UnitType tutorial_dino;

	private ActionHint hinting;

	private FocusMask focusMask;

	private HudScreen hud;

	private List<SpriteRenderer> apple_cost = new List<SpriteRenderer>();

	protected override void Start()
	{
		base.Start();
		base.DinoEggInvincible = true;
		hud = ScreenManager.GetScreen<HudScreen>();
		hud.Show();
		focusMask = getFocusMask();
		standardBulletBtn = hud.ShotButtons[0].transform;
		dinoButton = hud.UnitButtons[0].transform;
		tutorial_dino = hud.UnitButtons[0].Unit;
		SpriteRenderer component = hud.transform.Search("applebar_apple").GetComponent<SpriteRenderer>();
		for (int i = 0; i < Konfiguration.UnitData[tutorial_dino].appleCost; i++)
		{
			SpriteRenderer spriteRenderer = new GameObject("apple").AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = component.sprite;
			spriteRenderer.sortingLayerID = component.sortingLayerID;
			spriteRenderer.sortingOrder = -2;
			spriteRenderer.gameObject.layer = component.gameObject.layer;
			spriteRenderer.transform.parent = component.transform;
			spriteRenderer.transform.localPosition = Vector3.zero;
			spriteRenderer.enabled = false;
			apple_cost.Add(spriteRenderer);
		}
		buyUnitArrow = getArrow();
		buyUnitArrow.transform.RepositionAndReparent(dinoButton);
		SetDialogTween(base.top, base.centerDialogPosition);
		StartCoroutine(TutorialPart1());
	}

	private IEnumerator TutorialPart1()
	{
		GameCamera.Instance.EnterState_Playing();
		GoTween shotButtonTween = Go.from(standardBulletBtn, 1f, new GoTweenConfig().localPosition(standardBulletBtn.localPosition.SetY(-256f)));
		shotButtonTween.pause();
		GoTween dinoBuyButtonTween = Go.from(dinoButton, 1f, new GoTweenConfig().localPosition(dinoButton.localPosition.SetY(-256f)));
		dinoBuyButtonTween.pause();
		yield return new WaitForSeconds(2f);
		Vector3 appleCounterLabel_Position = hud.transform.Search("AppleCountLabel").position;
		focusMask.Show(appleCounterLabel_Position, Vector3.one * 5.5f);
		StartAppleCap(Konfiguration.UnitData[UnitType.Raptor].appleCost);
		ShowDialog("you steadily gain apples");
		while (base.dialog_open)
		{
			yield return null;
		}
		focusMask.Hide();
		yield return new WaitForSeconds(1f);
		dinoButton.gameObject.SetActive(true);
		dinoBuyButtonTween.play();
		yield return new WaitForSeconds(1f);
		string text2 = "Buy a raptor for # apples".Localize();
		text2 = string.Format(text2, tutorial_dino.ToString().Localize(), Konfiguration.UnitData[tutorial_dino].appleCost);
		ShowDialog(text2, true, false);
		while (Player.Instance.Apples < Konfiguration.UnitData[tutorial_dino].appleCost)
		{
			yield return new WaitForSeconds(0.5f);
		}
		buyUnitArrow.In();
		while (EntityFactory.OnStageCount[tutorial_dino] <= 0)
		{
			yield return null;
		}
		Tracking.buy_snappy();
		int i = 0;
		apple_cost.ForEach(delegate(SpriteRenderer apple)
		{
			apple.transform.parent = null;
			apple.enabled = true;
			Go.to(apple.transform, 0.75f, new GoTweenConfig().position(hud.UnitButtons[0].transform.position).setEaseType(GoEaseType.QuadIn).setDelay((float)i * 0.2f)
				.onComplete(delegate
				{
					apple.enabled = false;
				}));
			i++;
		});
		buyUnitArrow.Out();
		HideDialog();
		EntityFactory.Create(UnitType.Neander_Weak);
		yield return new WaitForSeconds(0.5f);
		SetDialogTween(base.top, base.upperDialogPosition);
		yield return new WaitForSeconds(0.5f);
		ShowDialog("Dinos will automatically attack the humans");
		while (base.dialog_open)
		{
			yield return null;
		}
		yield return new WaitForSeconds(4f);
		Level.Instance.EndTutorial(Level.State.playing);
		ShowDialog("Finish all humans!");
		while (base.dialog_open)
		{
			yield return null;
		}
		StopAppleCap();
		shotButtonTween.play();
		yield return new WaitForSeconds(1f);
		actionHinting.hintingInterval = 3f;
		actionHinting.StartNotification();
	}
}
