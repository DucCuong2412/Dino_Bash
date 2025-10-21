using UnityEngine;

public static class GameSorting
{
	public const int Background = 0;

	public const int DinoEgg = 4;

	public const int Entities = 5;

	public const int CombatFX = 10;

	public const int Shooting = 11;

	public const int Collectables = 20;

	public const int Foreground = 30;

	private static int largeCounter;

	private static int smallCounter;

	public static void Set(Unit unit, int force_layer = -1)
	{
		SpriteTools.SetSortingOrderFromDictionary(unit.initialSortingOrder);
		bool flag = UnitType.DinoEgg < unit.unitType && unit.unitType < UnitType.Raptor;
		bool flag2 = UnitType.FIRSTNEANDER < unit.unitType && unit.unitType < UnitType.Neander_Shooter;
		if (force_layer >= 2 || ((flag || flag2) && force_layer == -1))
		{
			largeCounter = ++largeCounter % 100;
			int num = largeCounter % 2;
			switch (unit.unitType)
			{
			case UnitType.Brachio:
			case UnitType.Tricer:
			case UnitType.Neander_ShotShield:
			case UnitType.Neander_ShotShield_Fast:
				num = 1;
				break;
			}
			int layer = ((force_layer != -1) ? force_layer : (num + 2));
			SetTransform(unit.transform, layer);
			int num2 = ((num != 0) ? (-10000) : 0);
			num2 -= largeCounter * 100;
			UnitType unitType = unit.unitType;
			if (unitType == UnitType.Brachio)
			{
				num2 = -20000;
			}
			SpriteTools.OffsetSortingOrder(unit, num2);
			return;
		}
		smallCounter = ++smallCounter % 100;
		int num3 = smallCounter % 2;
		switch (unit.unitType)
		{
		case UnitType.TRex:
		case UnitType.TRex_Jr:
			num3 = 1;
			break;
		case UnitType.MegaBall:
		case UnitType.Neander_Healer:
		case UnitType.Neander_Bush:
		case UnitType.Neander_Fire:
			num3 = 0;
			break;
		}
		num3 = ((force_layer != -1) ? force_layer : num3);
		SetTransform(unit.transform, num3);
		int num4 = ((num3 != 0) ? 10000 : 20000);
		num4 -= smallCounter * 100;
		switch (unit.unitType)
		{
		case UnitType.TRex:
			num4 = 1000;
			break;
		case UnitType.TRex_Jr:
			num4 = 2000;
			break;
		case UnitType.Neander_Bush:
			num4 = 21000;
			break;
		case UnitType.MegaBall:
			num4 = 22000;
			break;
		}
		SpriteTools.OffsetSortingOrder(unit, num4);
	}

	private static void SetTransform(Transform t, int layer)
	{
		float y = (float)layer * 42f + 44f;
		t.PosY(y);
		t.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.925f, (float)layer / 4f);
	}
}
