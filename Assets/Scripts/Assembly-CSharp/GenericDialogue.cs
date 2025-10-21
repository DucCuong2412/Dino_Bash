using System.Collections;
using UnityEngine;

public class GenericDialogue : AbstractTutorialScreen
{
	private HudScreen hud;

	private GoTween intro;

	protected override void Start()
	{
		base.Start();
		SetDialogTween(base.top, base.upperDialogPosition);
		hud = ScreenManager.GetScreen<HudScreen>();
		hud.Hide();
		GameCamera.Instance.EnterState_Disabled();
		intro = GameCamera.Instance.PlayIntroPan(Level.Instance.Config.levelwidth);
		if (Tutorials.isTutorialLevel(Level.Instance.levelid))
		{
			StartCoroutine(TutorialPart1());
		}
		else
		{
			Debug.LogError("wrongly setup dialog in level:" + Level.Instance.Config.name);
		}
	}

	private IEnumerator TutorialPart1()
	{
		ShowDialog(Tutorials.LocaKeyForLevel(Level.Instance.levelid));
		while (base.dialog_open)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		Level.Instance.EndTutorial(Level.State.playing);
		hud.Show();
		if (intro != null && intro.state != GoTweenState.Complete)
		{
			intro.complete();
		}
		GameCamera.Instance.EnterState_Playing();
	}
}
