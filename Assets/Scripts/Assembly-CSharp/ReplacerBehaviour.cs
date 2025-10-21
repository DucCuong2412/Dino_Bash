using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacerBehaviour : AbstractUnitCombatBehaviour
{
	public override void Init()
	{
		if (!isInititalized)
		{
			base.Init();
			SpriteTools.OffsetSortingOrder(this, 10000);
		}
	}

	protected override void SyncToState(Unit unit, Unit.State state)
	{
		StopAllCoroutines();
		switch (state)
		{
		case Unit.State.move:
			StartCoroutine(CheckForLevelEnd());
			break;
		case Unit.State.die:
			HandleOnEntityDied();
			break;
		case Unit.State.combat:
			break;
		}
	}

	private IEnumerator CheckForLevelEnd()
	{
		do
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (!base.Entity.hasReachedLevelEnd(128f));
		base.Entity.stateDie();
	}

	private void HandleOnEntityDied()
	{
		List<UnitType> list = EntityFactory.StringToUnittype(base.Entity.Config.command);
		foreach (UnitType item in list)
		{
			if (item != 0)
			{
				GameObject gameObject = EntityFactory.Create(item);
				if (gameObject != null)
				{
					gameObject.transform.PosX(base.transform.position.x + Random.Range(-64f, 64f));
					GameSorting.Set(gameObject.GetComponent<Unit>(), 0);
				}
			}
		}
	}
}
