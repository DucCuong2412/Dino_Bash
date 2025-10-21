using System;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public static class EntityFactory
{
	private static Dictionary<UnitType, BaseEntity> unitPool;

	public static Transform dinoSpawnRoot;

	public static Transform neanderSpawnRoot;

	public static Dictionary<UnitType, int> OnStageCount;

	public static Dictionary<UnitType, bool> CanSpawn;

	private static List<BaseEntity> liveEntities;

	private static Vector3 EGGPOSITION = new Vector3(0f, 164f, 0f);

	public static float DinoSpawnPosition
	{
		get
		{
			return Konfiguration.GameConfig.levelstart;
		}
	}

	public static float NeanderSpawnPosition
	{
		get
		{
			return (float)Level.Instance.Config.levelwidth + 256f;
		}
	}

	public static int NeanderKilled { get; private set; }

	public static DinoEgg Dino_Egg { get; private set; }

	public static event Action OnPlayerBuiltUnit;

	public static event Action<Unit> OnNeanderBuilt;

	public static List<BaseEntity> GetEntities(bool isFriendly)
	{
		return liveEntities.FindAll((BaseEntity x) => x.Config.isFriendly == isFriendly);
	}

	public static GameObject Create(UnitType entity)
	{
		if (!CanSpawn[entity])
		{
			return null;
		}
		Dictionary<UnitType, int> onStageCount;
		Dictionary<UnitType, int> dictionary = (onStageCount = OnStageCount);
		UnitType key;
		UnitType key2 = (key = entity);
		int num = onStageCount[key];
		dictionary[key2] = num + 1;
		GameObject gameObject = ObjectPool.Spawn(unitPool[entity].transform).gameObject;
		BaseEntity component = gameObject.GetComponent<BaseEntity>();
		if (component.animator != null)
		{
			InitFirstTimeOnly(component);
			component.animator.enabled = true;
			component.animator.Update(Time.deltaTime);
		}
		if (Player.ActiveUpgrades.Contains(component.unitType))
		{
			SetupDefense(component);
		}
		if (component is Unit)
		{
			SetupUnit(component as Unit);
			if (EntityFactory.OnNeanderBuilt != null && Konfiguration.isNeander(entity))
			{
				EntityFactory.OnNeanderBuilt(component as Unit);
			}
		}
		gameObject.SetActive(true);
		liveEntities.Add(component);
		return gameObject;
	}

	private static void SetupDefense(BaseEntity entity)
	{
		switch (entity.unitType)
		{
		default:
			return;
		case UnitType.EggProtection:
			entity.transform.position = EGGPOSITION;
			entity.GetComponent<CounterAttackMeelee>().mTarget = Dino_Egg;
			break;
		case UnitType.SmallBarricade:
			entity.transform.PosX((float)Level.Instance.Config.levelwidth * 0.2f);
			entity.transform.PosY(110f);
			break;
		case UnitType.Stone:
			entity.transform.PosX((float)Level.Instance.Config.levelwidth * 0.5f);
			entity.transform.PosY(110f);
			break;
		case UnitType.PoisonTrap:
			entity.transform.PosX((float)Level.Instance.Config.levelwidth * 0.3f);
			entity.transform.PosY(EGGPOSITION.y);
			break;
		case UnitType.DynamiteSmall:
			entity.transform.PosX((float)Level.Instance.Config.levelwidth * 0.7f);
			entity.transform.PosY(120f);
			break;
		case UnitType.Dynamite:
			entity.transform.PosX((float)Level.Instance.Config.levelwidth * 0.6f);
			entity.transform.PosY(120f);
			break;
		case UnitType.RainCloud:
			entity.transform.PosX((float)Level.Instance.Config.levelwidth * 0.55f);
			entity.transform.PosY(110f);
			break;
		case UnitType.AppleStartBonus:
		case UnitType.CoinDoubler:
		case UnitType.XpDoubler:
		case UnitType.AdditionalShotSlot:
		case UnitType.FastShotCooldown:
		case UnitType.RaindowTrail:
			return;
		}
		AbstractCombatBehaviour component = entity.GetComponent<AbstractCombatBehaviour>();
		if (component != null)
		{
			component.Init();
		}
		entity.transform.parent = dinoSpawnRoot;
	}

	private static void SetupUnit(Unit unit)
	{
		if (unit.Config.isFriendly)
		{
			unit.transform.parent = dinoSpawnRoot;
			unit.transform.PosX(DinoSpawnPosition - unit.RenderBounds.extents.x * 1.2f);
			if (EntityFactory.OnPlayerBuiltUnit != null)
			{
				EntityFactory.OnPlayerBuiltUnit();
			}
		}
		else
		{
			unit.transform.parent = neanderSpawnRoot;
			unit.transform.PosX(NeanderSpawnPosition + unit.RenderBounds.extents.x);
		}
		unit.stateDisabled();
		unit.stateStand();
	}

	private static void InitFirstTimeOnly(BaseEntity entity)
	{
		if (!(entity.animator.GetComponent<SpriteInitialSettings>() == null))
		{
			return;
		}
		Transform[] componentsInChildren = entity.animator.GetComponentsInChildren<Transform>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				transform.gameObject.AddComponent<SpriteInitialSettings>();
			}
		}
	}

	public static void Free(BaseEntity entity)
	{
		if (entity is Unit)
		{
			if (entity.Config.isFriendly)
			{
				entity.transform.PosX(DinoSpawnPosition - entity.RenderBounds.extents.x * 1.2f);
			}
			else
			{
				NeanderKilled++;
				entity.transform.PosX(NeanderSpawnPosition);
			}
		}
		if (entity.animator != null)
		{
			entity.animator.enabled = false;
		}
		ResetUnit(entity);
		liveEntities.Remove(entity);
		Dictionary<UnitType, int> onStageCount;
		Dictionary<UnitType, int> dictionary = (onStageCount = OnStageCount);
		UnitType unit;
		UnitType key = (unit = entity.Config.unit);
		int num = onStageCount[unit];
		dictionary[key] = num - 1;
		CanSpawn[entity.Config.unit] = true;
		ObjectPool.Recycle(entity.transform);
	}

	private static void ResetUnit(BaseEntity entity)
	{
		SpriteInitialSettings[] componentsInChildren = entity.GetComponentsInChildren<SpriteInitialSettings>();
		foreach (SpriteInitialSettings spriteInitialSettings in componentsInChildren)
		{
			spriteInitialSettings.Revert();
		}
	}

	public static List<UnitType> StringToUnittype(string s)
	{
		List<UnitType> list = new List<UnitType>();
		string[] array = new string[1] { s };
		if (s.Contains("+"))
		{
			array = s.Split('+');
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			try
			{
				list.Add((UnitType)(int)Enum.Parse(typeof(UnitType), text.Trim()));
			}
			catch
			{
				Debug.LogError("Not a valid Unittype supplied: " + s);
				list.Add(UnitType.None);
			}
		}
		return list;
	}

	public static void Init()
	{
		dinoSpawnRoot = GameObject.FindGameObjectWithTag("DinoBase").transform;
		neanderSpawnRoot = GameObject.FindGameObjectWithTag("NeanderBase").transform;
		NeanderKilled = 0;
		if (dinoSpawnRoot == null || neanderSpawnRoot == null)
		{
			throw new Exception("No Spawn Bases found!");
		}
		unitPool = new Dictionary<UnitType, BaseEntity>();
		CanSpawn = new Dictionary<UnitType, bool>();
		OnStageCount = new Dictionary<UnitType, int>();
		liveEntities = new List<BaseEntity>();
		foreach (int unitType in Enum.GetValues(typeof(UnitType)))
		{
			switch ((UnitType)unitType)
			{
			case UnitType.None:
			case UnitType.AppleStartBonus:
			case UnitType.CoinDoubler:
			case UnitType.XpDoubler:
			case UnitType.AdditionalShotSlot:
			case UnitType.FastShotCooldown:
			case UnitType.RaindowTrail:
			case UnitType.Message:
			case UnitType.Wait:
				CanSpawn.Add((UnitType)unitType, false);
				continue;
			case UnitType.DinoEgg:
				CanSpawn.Add((UnitType)unitType, false);
				Dino_Egg = UnityEngine.Object.Instantiate(Resources.Load<DinoEgg>("Units/" + (UnitType)unitType), EGGPOSITION, Quaternion.identity) as DinoEgg;
				Dino_Egg.transform.parent = dinoSpawnRoot;
				continue;
			case UnitType.TRex_Jr:
				if (Level.Instance.levelid > Konfiguration.GameConfig.Dino_rage_start_level)
				{
					CanSpawn.Add((UnitType)unitType, true);
					continue;
				}
				break;
			}
			Action<UnitType> addEntity = delegate(UnitType entitiy)
			{
				if (CanSpawn.ContainsKey(entitiy))
				{
					CanSpawn[entitiy] = true;
				}
				else
				{
					CanSpawn.Add(entitiy, true);
				}
			};
			if (Level.Instance.Config.enemies.FindIndex((LevelEnemy x) => x.unittype == (UnitType)unitType) >= 0)
			{
				addEntity((UnitType)unitType);
				if (unitType == 68)
				{
					List<LevelEnemy> list = Level.Instance.Config.enemies.FindAll((LevelEnemy x) => x.unittype == UnitType.Neander_Bush);
					foreach (LevelEnemy item in list)
					{
						if (item.command != string.Empty)
						{
							List<UnitType> list2 = StringToUnittype(item.command);
							list2.ForEach(delegate(UnitType x)
							{
								addEntity(x);
							});
						}
					}
				}
			}
			else if (!CanSpawn.ContainsKey((UnitType)unitType) && Konfiguration.isNeander((UnitType)unitType))
			{
				CanSpawn.Add((UnitType)unitType, false);
				continue;
			}
			if (Konfiguration.isDinoUnit((UnitType)unitType) && Level.Instance.AvailableDinos.FindIndex((UnitType x) => x == (UnitType)unitType) == -1 && !Konfiguration.isCollectable((UnitType)unitType) && Level.Instance.Config.enemies.FindIndex((LevelEnemy x) => x.unittype == (UnitType)unitType) == -1 && !CanSpawn.ContainsKey((UnitType)unitType))
			{
				CanSpawn.Add((UnitType)unitType, false);
			}
			else if (Konfiguration.isUpgrade((UnitType)unitType) && Player.ActiveUpgrades.FindIndex((UnitType x) => x == (UnitType)unitType) == -1)
			{
				CanSpawn.Add((UnitType)unitType, false);
			}
			else if (!Konfiguration.isDinoUnit((UnitType)unitType) && !Konfiguration.isNeander((UnitType)unitType) && !Konfiguration.isUpgrade((UnitType)unitType) && !Konfiguration.isConsumable((UnitType)unitType))
			{
				CanSpawn.Add((UnitType)unitType, false);
			}
			else if (!CanSpawn.ContainsKey((UnitType)unitType))
			{
				CanSpawn.Add((UnitType)unitType, true);
			}
		}
		List<UnitType> list3 = new List<UnitType>();
		foreach (KeyValuePair<UnitType, bool> item2 in CanSpawn)
		{
			if (item2.Value)
			{
				list3.Add(item2.Key);
			}
		}
		list3.ForEach(delegate(UnitType entity)
		{
			Debug.Log("Setup for: " + entity);
			GameObject gameObject = (GameObject)Resources.Load("Units/" + entity, typeof(GameObject));
			OnStageCount.Add(entity, 0);
			BaseEntity component = gameObject.GetComponent<BaseEntity>();
			ObjectPool.CreatePool(gameObject.transform);
			unitPool.Add(entity, component);
			BaseEntity component2 = Create(entity).GetComponent<BaseEntity>();
			if (component2 is Unit)
			{
				(component2 as Unit).stateDisabled();
			}
			Free(component2);
		});
	}
}
