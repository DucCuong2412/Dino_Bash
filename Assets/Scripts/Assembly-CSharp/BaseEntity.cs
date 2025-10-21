using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBase
{
	private EntityData _config;

	private UnitAnimationEvent animEvents;

	[EnumFlags]
	public CombatEffects combat_effects_mask;

	private int _health;

	private float health_modifier;

	private AudioSource audio_source;

	private Dictionary<EntitySound, AudioResources.SFX> entity_sounds;

	private List<EntitySound> attack_sounds = new List<EntitySound>();

	private List<EntitySound> die_sounds = new List<EntitySound>();

	private Bounds _renderBounds;

	private Animator _animator;

	public UnitType unitType { get; private set; }

	public EntityData Config
	{
		get
		{
			return _config;
		}
		set
		{
			_config = value;
			SetMaxHealth(health_modifier);
		}
	}

	public virtual int Health
	{
		get
		{
			return _health;
		}
		protected set
		{
			_health = value;
		}
	}

	public int max_health
	{
		get
		{
			return Mathf.RoundToInt((float)Config.healthpoints * health_modifier);
		}
	}

	public EntityColorTinter colorTinter { get; private set; }

	public Bounds RenderBounds
	{
		get
		{
			if (_renderBounds.extents != Vector3.zero)
			{
				return _renderBounds;
			}
			Component component = colorTinter;
			if (component == null)
			{
				component = this;
			}
			Renderer[] componentsInChildren = component.GetComponentsInChildren<Renderer>();
			Bounds bounds = default(Bounds);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (bounds.extents == Vector3.zero)
				{
					bounds = renderer.bounds;
					continue;
				}
				bounds.Encapsulate(renderer.bounds.min);
				bounds.Encapsulate(renderer.bounds.max);
			}
			_renderBounds = bounds;
			return bounds;
		}
	}

	public Animator animator
	{
		get
		{
			if (_animator == null)
			{
				_animator = GetComponentInChildren<Animator>();
			}
			return _animator;
		}
	}

	public event Action<UnitAnimationEvents> OnAnimationEvent;

	public event Action<BaseEntity> OnEntityHit;

	public event Action OnEntityDied;

	public void FireAnimationEvent(UnitAnimationEvents e)
	{
		if (e == UnitAnimationEvents.UnitPlaySpecialSound)
		{
			PlaySound(EntitySound.Special);
		}
		if (this.OnAnimationEvent != null)
		{
			this.OnAnimationEvent(e);
		}
	}

	public void SetMaxHealth(float health_modifier = 1f)
	{
		this.health_modifier = health_modifier;
		Health = Mathf.RoundToInt((float)Config.healthpoints * this.health_modifier);
	}

	public virtual void ChangeHealth(int amount, BaseEntity entitiy, bool showTintFX = true)
	{
		_health += amount;
		_health = Mathf.Clamp(_health, 0, max_health);
		if (_health == 0)
		{
			stateDie();
		}
		else if (amount < 0)
		{
			if (showTintFX)
			{
				colorTinter.FlashColor(EntityColorTinter.hitColor, 0.1f);
			}
			if (this.OnEntityHit != null)
			{
				this.OnEntityHit(entitiy);
			}
		}
		else
		{
			if (entitiy == null)
			{
				CheckForHealAchievement();
			}
			if (showTintFX)
			{
				colorTinter.FlashColor(EntityColorTinter.healColor, 0.3f);
			}
		}
	}

	private void CheckForHealAchievement()
	{
		if (Konfiguration.isDinoUnit(unitType) && unitType != UnitType.DinoEgg)
		{
			SocialGamingManager.Instance.ReportProgress(AchievementIds.HEAL_A_DINOSAUR, 1);
		}
	}

	private EntitySound GetAttackSound()
	{
		if (attack_sounds.Count == 0)
		{
			if (entity_sounds.ContainsKey(EntitySound.Attack1))
			{
				attack_sounds.Add(EntitySound.Attack1);
			}
			if (entity_sounds.ContainsKey(EntitySound.Attack2))
			{
				attack_sounds.Add(EntitySound.Attack2);
			}
			if (entity_sounds.ContainsKey(EntitySound.Attack3))
			{
				attack_sounds.Add(EntitySound.Attack3);
			}
			if (entity_sounds.ContainsKey(EntitySound.Attack4))
			{
				attack_sounds.Add(EntitySound.Attack4);
			}
		}
		if (attack_sounds.Count > 0)
		{
			return attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Count)];
		}
		return EntitySound.None;
	}

	private EntitySound GetDieSound()
	{
		if (die_sounds.Count == 0)
		{
			if (entity_sounds.ContainsKey(EntitySound.Die1))
			{
				die_sounds.Add(EntitySound.Die1);
			}
			if (entity_sounds.ContainsKey(EntitySound.Die2))
			{
				die_sounds.Add(EntitySound.Die2);
			}
			if (entity_sounds.ContainsKey(EntitySound.Die3))
			{
				die_sounds.Add(EntitySound.Die3);
			}
			if (entity_sounds.ContainsKey(EntitySound.Die4))
			{
				die_sounds.Add(EntitySound.Die4);
			}
		}
		if (die_sounds.Count > 0)
		{
			return die_sounds[UnityEngine.Random.Range(0, die_sounds.Count)];
		}
		return EntitySound.None;
	}

	public void PlaySound(EntitySound sound)
	{
		EntitySound entitySound = EntitySound.None;
		switch (sound)
		{
		case EntitySound.Spawn:
			if (entity_sounds.ContainsKey(EntitySound.Spawn) && Level.Instance.state == Level.State.playing)
			{
				AudioPlayer.PlayGuiSFX(entity_sounds[sound], 0f);
			}
			return;
		case EntitySound.ATTACK:
			audio_source.Stop();
			entitySound = GetAttackSound();
			break;
		case EntitySound.DIE:
			audio_source.Stop();
			entitySound = GetDieSound();
			break;
		case EntitySound.Move:
			if (entity_sounds.ContainsKey(EntitySound.Move) && audio_source.clip != AudioResources.GetClip(entity_sounds[sound].Path))
			{
				AudioPlayer.PlayGameSFX(audio_source, entity_sounds[sound]);
			}
			return;
		case EntitySound.Special:
			if (entity_sounds.ContainsKey(EntitySound.Special) && Level.Instance.state == Level.State.playing)
			{
				AudioPlayer.PlayGameSFX(audio_source, entity_sounds[sound]);
			}
			return;
		}
		if (entitySound != 0 && !audio_source.isPlaying && entity_sounds.ContainsKey(entitySound))
		{
			AudioPlayer.PlayGameSFX(audio_source, entity_sounds[entitySound]);
		}
	}

	protected virtual void OnEnable()
	{
		animEvents = GetComponentInChildren<UnitAnimationEvent>();
		if (animEvents != null)
		{
			animEvents.OnAnimationEvent += FireAnimationEvent;
		}
	}

	protected virtual void OnDisable()
	{
		if (animEvents != null)
		{
			animEvents.OnAnimationEvent -= FireAnimationEvent;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void Awake()
	{
		string text = base.gameObject.name;
		int num = text.IndexOf("(Recyled)");
		if (num == -1)
		{
			num = text.IndexOf("(Clone)");
		}
		text = text.Substring(0, num);
		unitType = (UnitType)(int)Enum.Parse(typeof(UnitType), text);
		if (Konfiguration.isCollectable(unitType))
		{
			Config = Konfiguration.UnitData[UnitType.CollectableApple];
		}
		else
		{
			Config = Konfiguration.UnitData[unitType];
		}
		SetMaxHealth();
		foreach (Transform item in base.transform)
		{
			if (item.tag == "EntityGraphics")
			{
				colorTinter = item.AddOrGetComponent<EntityColorTinter>();
				break;
			}
		}
		audio_source = base.gameObject.AddComponent<AudioSource>();
		AudioPlayer.Setup3DAudioSource(audio_source);
		_renderBounds = RenderBounds;
		entity_sounds = AudioResources.LoadAndGetEntitiySounds(unitType);
		SpriteTools.SetSortingLayerID(this, 5);
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("GUI/HealthBar", typeof(GameObject)));
		gameObject.transform.parent = base.transform;
		gameObject.transform.position = base.transform.position;
		GetComponentInChildren<Healthbar>().Init();
	}

	public virtual void stateDie()
	{
		PlaySound(EntitySound.DIE);
		if (this.OnEntityDied != null)
		{
			this.OnEntityDied();
		}
	}
}
