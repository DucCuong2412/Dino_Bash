using System.Collections;
using UnityEngine;

public class SpecialMissionTutorial : AbstractTutorialScreen
{
	protected override void Start()
	{
		base.Start();
		SetDialogTween(base.top, base.upperDialogPosition);
		StartCoroutine(TutorialPart1());
	}

	private IEnumerator TutorialPart1()
	{
		HudScreen hud = ScreenManager.GetScreen<HudScreen>();
		hud.ConsumableButtons.ForEach(delegate(BuyUnitButton x)
		{
			x.disable = false;
		});
		float inital_y_pos = hud.ShotButtons[0].transform.localPosition.y;
		hud.ShotButtons.ForEach(delegate(ShootButton button)
		{
			button.transform.LocalPosY(-256f);
		});
		yield return new WaitForSeconds(1f);
		ShowDialog("special_mission_tutorial_00");
		while (base.dialog_open)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		ShowDialog("special_mission_tutorial_01");
		while (base.dialog_open)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		ShowDialog("special_mission_tutorial_02");
		while (base.dialog_open)
		{
			yield return null;
		}
		Level.Instance.EndTutorial(Level.State.intro);
		hud.ShotButtons.ForEach(delegate(ShootButton button)
		{
			Go.to(button.transform, 0.5f, new GoTweenConfig().localPosition(button.transform.localPosition.SetY(inital_y_pos)).setEaseType(GoEaseType.CubicOut));
		});
		hud.ConsumableButtons.ForEach(delegate(BuyUnitButton x)
		{
			x.disable = false;
		});
	}
}
