using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseEntity))]
public class FX_UnitAttack : MonoBase
{
	[SerializeField]
	private ParticleSystem FX_Prefab;

	[SerializeField]
	private string FX_Parent_Name;

	[SerializeField]
	private int emitCount;

	[SerializeField]
	private bool spawnOnTarget;

	[SerializeField]
	private bool sharedFX;

	private ParticleSystem fx;

	private BaseEntity entity;

	private Transform transformParent;

	private static Dictionary<ParticleSystem, ParticleSystem> fx_chache = new Dictionary<ParticleSystem, ParticleSystem>();

	private void OnDestroy()
	{
		fx_chache.Clear();
	}

	private void Start()
	{
		if (FX_Parent_Name != string.Empty)
		{
			transformParent = base.transform.Search(FX_Parent_Name);
		}
		else
		{
			transformParent = base.transform;
		}
		if (fx == null)
		{
			if (sharedFX)
			{
				if (fx_chache.ContainsKey(FX_Prefab))
				{
					fx = fx_chache[FX_Prefab];
				}
				else
				{
					fx = UnityEngine.Object.Instantiate(FX_Prefab) as ParticleSystem;
					fx.gameObject.name = "SharedFX_" + fx.gameObject.name;
					SpriteTools.SetSortingLayerID(fx, 10);
					fx_chache.Add(FX_Prefab, fx);
				}
			}
			else
			{
				fx = UnityEngine.Object.Instantiate(FX_Prefab) as ParticleSystem;
				fx.transform.parent = transformParent;
				fx.transform.localPosition = Vector3.zero;
				ParticleSystem[] componentsInChildren = fx.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem c in componentsInChildren)
				{
					c.AddOrGetComponent<FX_ParticlePlayStop>();
				}
				fx.gameObject.SetActive(false);
				SpriteTools.SetSortingLayerID(fx, 10);
			}
		}
		if (fx == null)
		{
			throw new Exception("FX missing - something went really wrong here...");
		}
		entity = GetComponent<BaseEntity>();
		entity.OnAnimationEvent += OnAnimationEvent;
	}

	private void OnAnimationEvent(UnitAnimationEvents e)
	{
		if (e != UnitAnimationEvents.UnitAttack || !base.enabled)
		{
			return;
		}
		if (sharedFX)
		{
			if (!spawnOnTarget)
			{
				fx.transform.position = transformParent.position;
				fx.Emit(fx.maxParticles);
			}
			if (!spawnOnTarget || !(entity is Unit))
			{
				return;
			}
			Unit unit = (Unit)entity;
			{
				foreach (BaseEntity combatTarget in unit.CombatBehaviour.CombatTargets)
				{
					Transform[] componentsInChildren = combatTarget.animator.GetComponentsInChildren<Transform>();
					if (componentsInChildren.Length == 0)
					{
						break;
					}
					fx.Emit(emitCount);
					ParticleSystem.Particle[] array = new ParticleSystem.Particle[fx.particleCount];
					fx.GetParticles(array);
					for (int i = array.Length - emitCount; i < array.Length; i++)
					{
						array[i].position = componentsInChildren[UnityEngine.Random.Range(0, componentsInChildren.Length)].transform.position;
					}
					fx.SetParticles(array, array.Length);
				}
				return;
			}
		}
		if (!spawnOnTarget)
		{
			fx.gameObject.SetActive(true);
			WaitThen(fx.duration, delegate
			{
				fx.gameObject.SetActive(false);
			});
		}
	}
}
