using System;
using System.Collections;
using UnityEngine;
using dinobash;

public abstract class AbstractShot : MonoBase
{
	[SerializeField]
	private Transform shotTransform;

	[SerializeField]
	private bool doesDirectDamage = true;

	protected bool isDropped;

	protected bool hitGround;

	[SerializeField]
	protected CameraShake.Intensity cameraShakeIntensity;

	protected CapsuleCollider shotCollider;

	public CombatEffects combatEffect = CombatEffects.none;

	public float combatEffectDuration = 5f;

	protected BitArray ignoreMask;

	protected float[] hitModifier;

	[SerializeField]
	protected ParticleSystem fx_impact;

	[SerializeField]
	protected ParticleSystem fx_groundimpact;

	private bool isInit;

	public bool isTutorialMode { get; set; }

	public ShotType type { get; private set; }

	protected Animator animator { get; private set; }

	protected ShotData Config { get; set; }

	protected bool hitUnit { get; private set; }

	protected bool hitSomething { get; private set; }

	public event Action OnStartCoolDown;

	public virtual void Fire(bool follow_shot)
	{
		isDropped = true;
		shotCollider.enabled = true;
		if (follow_shot)
		{
			GameCamera.Instance.EnterState_Following(base.transform, false);
		}
		StartCoroutine(CheckLevelBounds());
	}

	private IEnumerator CheckLevelBounds()
	{
		do
		{
			yield return new WaitForSeconds(0.2f);
		}
		while (Level.Instance.inLevelBounds(base.transform.position.x));
		FreeBullet();
	}

	public virtual void ResetSettings()
	{
		Init();
		SetRenderer(true);
		SetActiveChildren(false, fx_impact.transform);
		if (fx_groundimpact != null)
		{
			SetActiveChildren(false, fx_groundimpact.transform);
		}
		isDropped = false;
		hitSomething = false;
		isTutorialMode = false;
		hitUnit = false;
		hitGround = false;
		shotCollider.enabled = false;
	}

	public virtual void FreeBullet()
	{
		HitSomething();
		GameCamera.Instance.StopFollowing(base.transform);
		ObjectPool.Recycle(base.transform);
	}

	protected void HitSomething()
	{
		if (!hitSomething)
		{
			hitSomething = true;
			if (this.OnStartCoolDown != null)
			{
				this.OnStartCoolDown();
				this.OnStartCoolDown = null;
			}
		}
	}

	protected void PlayHitSound()
	{
		AudioPlayer.PlayGuiSFX(AudioResources.GetShotSounds(type)[1], 0f);
	}

	protected void PlayMissSound()
	{
		AudioPlayer.PlayGuiSFX(Sounds.shoot_miss, 0f);
	}

	private void OnDisable()
	{
		HitSomething();
		StopAllCoroutines();
	}

	protected bool HitUnit(BaseEntity entity)
	{
		if (ignoreMask[(int)entity.unitType])
		{
			return false;
		}
		if (Config.damage < 0 && entity.Health == entity.max_health)
		{
			return false;
		}
		hitUnit = true;
		HitSomething();
		AbstractCombatEffect.Apply(combatEffect, entity, combatEffectDuration, Config.damage);
		if (entity is Unit)
		{
			((Unit)entity).stateStun(Config.stunDuration);
		}
		int correct_damage = ((!doesDirectDamage) ? (-1) : (-Config.damage));
		correct_damage = getTutorialDamage(correct_damage, entity);
		if (type == ShotType.Meteor)
		{
			correct_damage = Konfiguration.scaleConsumableWithPlayerProgress(correct_damage);
		}
		correct_damage = Mathf.RoundToInt((float)correct_damage * (1f - hitModifier[(int)entity.unitType]));
		entity.ChangeHealth(correct_damage, null);
		return true;
	}

	private int getTutorialDamage(int correct_damage, BaseEntity target)
	{
		if (Player.CurrentLevelID < 5)
		{
			if (target.unitType == UnitType.Neander_Healer)
			{
				return -target.max_health;
			}
			if (target.unitType == UnitType.Neander_Shooter)
			{
				return -target.max_health;
			}
		}
		return correct_damage;
	}

	private ParticleSystem SetupFX(ParticleSystem fx)
	{
		fx = UnityEngine.Object.Instantiate(fx) as ParticleSystem;
		fx.transform.RepositionAndReparent(base.transform, true);
		fx.transform.localPosition = Vector3.zero;
		return fx;
	}

	protected virtual void ShowFx()
	{
		HitSomething();
		SetRenderer(false);
		if (hitGround && fx_groundimpact != null)
		{
			StartParticleFX(fx_groundimpact);
		}
		else
		{
			StartParticleFX(fx_impact);
		}
	}

	private void StartParticleFX(ParticleSystem ps)
	{
		SetActiveChildren(true, ps.transform);
		ps.Play();
		WaitThen(ps.duration, delegate
		{
			SetActiveChildren(false, ps.transform);
			ps.Stop();
			ps.Clear();
			FreeBullet();
		});
	}

	private void SetActiveChildren(bool state, Transform parent)
	{
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActive(state);
		}
	}

	protected virtual void SetRenderer(bool visible)
	{
		Renderer[] componentsInChildren = shotTransform.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = visible;
		}
	}

	protected virtual void Init()
	{
		if (!isInit)
		{
			isInit = true;
			ShotCarrier componentUpwards = this.GetComponentUpwards<ShotCarrier>();
			if (componentUpwards != null)
			{
				string text = componentUpwards.gameObject.name;
				text = text.Substring(0, text.IndexOf("(Clone)"));
				type = (ShotType)(int)Enum.Parse(typeof(ShotType), text);
			}
			else
			{
				string text2 = base.gameObject.name;
				text2 = text2.Substring(0, text2.IndexOf("(Clone)"));
				type = (ShotType)(int)Enum.Parse(typeof(ShotType), text2);
			}
			Config = Konfiguration.ShotData[type];
			ignoreMask = HitMasks.GetIgnoreMask((int)type);
			hitModifier = HitMasks.GetHitModifier((int)type);
			if ((bool)base.collider)
			{
				shotCollider = (CapsuleCollider)base.collider;
			}
			fx_impact = SetupFX(fx_impact);
			if (fx_groundimpact != null)
			{
				fx_groundimpact = SetupFX(fx_groundimpact);
			}
			SpriteTools.SetSortingLayerID(this, 11);
			animator = GetComponent<Animator>();
		}
	}
}
