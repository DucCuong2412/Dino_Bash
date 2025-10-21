using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTutorialScreen : AbstractTutorialScreen
{
	private HudScreen hud;

	private FocusMask mask;

	private LocalizedText text;

	private List<BaseEntity> showUnitInfo = new List<BaseEntity>();

	private float base_y_height;

	private bool swipeMoveState;

	private float time_scale = 1f;

	protected new void Start()
	{
		base.Start();
		hud = ScreenManager.GetScreen<HudScreen>();
		mask = getFocusMask();
		mask.updateType = GoUpdateType.TimeScaleIndependentUpdate;
	}

	public void Show(BaseEntity entity)
	{
		Debug.Log("Unit Tutorial adding: " + entity.unitType);
		showUnitInfo.Add(entity);
		if (showUnitInfo.Count == 1)
		{
			StartCoroutine(WaitUntilUnitGetsVisible());
		}
	}

	private IEnumerator WaitUntilUnitGetsVisible()
	{
		bool shoulUpdate = true;
		while (shoulUpdate)
		{
			yield return new WaitForSeconds(0.1f);
			if (hud.state != HudScreen.State.In)
			{
				continue;
			}
			BaseEntity removeEntry = null;
			foreach (BaseEntity entry in showUnitInfo)
			{
				if (GameCamera.Instance.isInView(entry.transform, 0.95f) && entry.Health > 0)
				{
					ShowScreen(entry);
					removeEntry = entry;
					shoulUpdate = false;
					break;
				}
			}
			if (removeEntry != null)
			{
				showUnitInfo.RemoveAll((BaseEntity entitiy) => entitiy.unitType == removeEntry.unitType);
			}
		}
	}

	private void ShowScreen(BaseEntity entity)
	{
		CameraShake.instance.enabled = false;
		GameCamera.Instance.EnterState_Disabled();
		hud.Hide();
		WaitThen(0.8f, delegate
		{
			base.Show();
			time_scale = Time.timeScale;
			Time.timeScale = 0f;
			Vector3 position = entity.transform.position;
			position.y += entity.RenderBounds.extents.y;
			Vector3 position2 = Camera.main.WorldToViewportPoint(position);
			Vector3 pos = ScreenManager.Camera.ViewportToWorldPoint(position2);
			mask.Show(pos, Vector3.one * 8f);
			StartCoroutine(Dialog(entity));
		});
	}

	private IEnumerator Dialog(BaseEntity entity)
	{
		ShowDialog(entity.unitType.ToString() + ".description");
		while (base.dialog_open)
		{
			yield return null;
		}
		PauseScreen pause_screen = ScreenManager.GetScreen<PauseScreen>();
		while (pause_screen != null && pause_screen.isVisible)
		{
			yield return null;
		}
		Hide();
	}

	public override void Hide()
	{
		Time.timeScale = time_scale;
		mask.Hide();
		GameCamera.Instance.EnterState_Playing();
		CameraShake.instance.enabled = true;
		HideTo(base.bottom, delegate
		{
			hud.Show();
			base.Hide();
			if (showUnitInfo.Count > 0)
			{
				StartCoroutine(WaitUntilUnitGetsVisible());
			}
		});
	}
}
