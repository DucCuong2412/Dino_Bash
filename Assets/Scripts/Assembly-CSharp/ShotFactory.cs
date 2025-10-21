using System;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public static class ShotFactory
{
	private const float startheight = 850f;

	private static Dictionary<ShotType, List<ShotCarrier>> carriers;

	public static Action OnShotStart;

	public static Action OnShotEnd;

	public static ShotCarrier Current { get; private set; }

	public static float globaleCooldownModifier
	{
		get
		{
			if (Player.AvailiableUpgrades.Contains(UnitType.FastShotCooldown))
			{
				return Konfiguration.UnitData[UnitType.FastShotCooldown].buildcooldown;
			}
			return 1f;
		}
	}

	public static ShotCarrier GetCarrier(ShotType type)
	{
		return carriers[type].Find((ShotCarrier x) => x.state == ShotCarrier.States.disabled);
	}

	public static ShotCarrier Fire(ShotType type, Transform target)
	{
		ShotCarrier shotCarrier = carriers[type].Find((ShotCarrier x) => x.state == ShotCarrier.States.disabled);
		if (shotCarrier == null)
		{
			return null;
		}
		shotCarrier.transform.PosY(850f);
		shotCarrier.stateStart(target);
		Current = shotCarrier;
		if (OnShotStart != null)
		{
			OnShotStart();
		}
		return shotCarrier;
	}

	public static void ShotEnd()
	{
		if (OnShotEnd != null)
		{
			OnShotEnd();
		}
		Current = null;
	}

	public static void Setup()
	{
		carriers = new Dictionary<ShotType, List<ShotCarrier>>();
		foreach (ShotType availableShot in Level.Instance.AvailableShots)
		{
			if (availableShot != ShotType.None)
			{
				ShotCarrier original = Resources.Load<ShotCarrier>("Shooting/" + availableShot);
				original = UnityEngine.Object.Instantiate(original, new Vector3(-3000f, 850f, 0f), Quaternion.identity) as ShotCarrier;
				original.transform.parent = EntityFactory.dinoSpawnRoot;
				Transform original2 = Resources.Load<Transform>("Shooting/Fly_Dino_standard");
				original2 = UnityEngine.Object.Instantiate(original2) as Transform;
				original2.transform.parent = original.transform;
				original2.transform.localPosition = Vector3.zero;
				if (Player.AvailiableUpgrades.Contains(UnitType.RaindowTrail) && Player.GetUpgradeState(UnitType.RaindowTrail))
				{
					TrailRenderer original3 = Resources.Load<TrailRenderer>("Shooting/RainbowTrail");
					original3 = UnityEngine.Object.Instantiate(original3) as TrailRenderer;
					original3.transform.parent = original2.GetComponentInChildren<Animator>().transform;
					original3.transform.localPosition = new Vector3(0f, 15f, 1f);
				}
				if (!carriers.ContainsKey(availableShot))
				{
					carriers.Add(availableShot, new List<ShotCarrier>());
				}
				carriers[availableShot].Add(original);
			}
		}
	}
}
