using System;
using System.Collections;
using UnityEngine;

public class WatchShotsOnUnitScreen : AbstractTutorialScreen
{
	private UnitType unit_type;

	private string loca_entry;

	private int shot_count;

	private int shots_fired;

	protected override void Start()
	{
		base.Start();
	}

	public void StartWatching(UnitType unit, string loca, int shots = 3)
	{
		unit_type = unit;
		loca_entry = loca;
		shot_count = shots;
		StartCoroutine(WatchShotsOnHealer());
	}

	private IEnumerator WatchShotsOnHealer()
	{
		while (EntityFactory.OnStageCount[unit_type] <= 0)
		{
			yield return null;
		}
		BaseEntity entity = EntityFactory.GetEntities(false).Find((BaseEntity x) => x.unitType == unit_type);
		ShotFactory.OnShotStart = (Action)Delegate.Combine(ShotFactory.OnShotStart, new Action(count_shots));
		shots_fired = 0;
		while (shots_fired < shot_count)
		{
			yield return null;
		}
		if (entity.Health == entity.max_health)
		{
			while (GameCamera.Instance.state != GameCamera.States.playing)
			{
				yield return null;
			}
			Time.timeScale = 0f;
			ShowDialog(loca_entry);
			while (base.dialog_open)
			{
				yield return null;
			}
			Time.timeScale = 1f;
		}
	}

	private void count_shots()
	{
		shots_fired++;
	}
}
