using System.Collections;
using UnityEngine;
using dinobash;

public abstract class WaitForUnitTutorial : AbstractTutorialScreen
{
	protected UnitType unit;

	protected string loca_key;

	protected bool show_arrow = true;

	private HudScreen hud;

	private TutorialArrow arrow;

	protected override void Start()
	{
		base.Start();
		hud = ScreenManager.GetScreen<HudScreen>();
		hud.Hide();
		if (hud.UnitButtons.FindIndex((BuyUnitButton x) => x.Unit == unit) >= 0)
		{
			arrow = getArrow();
			arrow.transform.RepositionAndReparent(hud.UnitButtons.Find((BuyUnitButton x) => x.Unit == unit).transform);
			arrow.transform.localScale = Vector3.one;
			hud.UnitButtons.FindAll((BuyUnitButton x) => x.Unit != unit).ForEach(delegate(BuyUnitButton x)
			{
				x.uiItem.OnClick += OnTryingToBuyWrongUnits;
				x.ignoreCommand = true;
			});
			StartCoroutine(WaitForUnit());
		}
		SetDialogTween(base.top, base.upperDialogPosition);
		Level.Instance.EndTutorial(Level.State.intro);
	}

	private void OnTryingToBuyWrongUnits()
	{
		hud.UnitButtons.FindAll((BuyUnitButton x) => x.Unit != unit).ForEach(delegate(BuyUnitButton x)
		{
			x.disable = true;
			x.uiItem.OnClick -= OnTryingToBuyWrongUnits;
		});
		ShowDialog(loca_key);
	}

	private IEnumerator WaitForUnit()
	{
		while (EntityFactory.OnStageCount[unit] == 0)
		{
			if (Player.Instance.Apples >= Konfiguration.UnitData[unit].appleCost && !arrow.Visible && show_arrow)
			{
				arrow.In();
			}
			yield return null;
		}
		if (show_arrow)
		{
			arrow.Out();
		}
		hud.UnitButtons.ForEach(delegate(BuyUnitButton x)
		{
			x.uiItem.OnClick -= OnTryingToBuyWrongUnits;
			x.ignoreCommand = false;
			x.disable = false;
		});
	}
}
