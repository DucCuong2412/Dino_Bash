using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class Unit : BaseEntity
{
	public enum State
	{
		invalid = 0,
		disabled = 1,
		stand = 2,
		move = 3,
		combat = 4,
		die = 5,
		stun = 6,
		fly = 7,
		fall = 8
	}

	public const float flyHeight = 700f;

	private bool isInitialized;

	public BoxCollider bodyCollider;

	[HideInInspector]
	public bool StopAtLevelEnd = true;

	public int hits_to_kill = -1;

	public bool immune_to_stun;

	private float randomValue;

	private float height;

	private int die_animation_alternatives;

	public State state;

	protected float state_blend_duration = 0.1f;

	private UnitType drop_collectable;

	private int level_collectable_index = -1;

	private float last_hit;

	public AbstractUnitCombatBehaviour CombatBehaviour { get; private set; }

	public Vector3 Speed { get; set; }

	public bool AllowStandToWalk { get; set; }

	public Dictionary<Renderer, int> initialSortingOrder { get; private set; }

	public State lastState { get; private set; }

	public event Action<Unit, State> OnStateChanged;

	protected void SetState(State pTargetState)
	{
		lastState = state;
		state = pTargetState;
		UpdateAnimationState();
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(this, state);
		}
	}

	private void UpdateAnimationState()
	{
		string text = state.ToString();
		float normalizedTime = 0f;
		switch (state)
		{
		case State.die:
			if (die_animation_alternatives > 0)
			{
				int num = UnityEngine.Random.Range(0, die_animation_alternatives + 1);
				if (num > 0)
				{
					text = text + " " + num;
				}
			}
			break;
		case State.stand:
		case State.move:
			normalizedTime = UnityEngine.Random.value;
			break;
		}
		base.animator.CrossFade(text, state_blend_duration, 0, normalizedTime);
		base.animator.Update(Time.deltaTime);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		base.OnAnimationEvent += AnimationEventHandler;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		base.OnAnimationEvent -= AnimationEventHandler;
	}

	private void AnimationEventHandler(UnitAnimationEvents pEvent)
	{
		if (pEvent == UnitAnimationEvents.UnitDied)
		{
			EntityFactory.Free(this);
		}
	}

	protected virtual void Update()
	{
		switch (state)
		{
		case State.move:
			base.transform.Translate(Speed * Time.deltaTime);
			if (hasReachedLevelEnd(0f) && StopAtLevelEnd)
			{
				Level.Instance.UnitReachedLevelEnd(this);
				stateStand();
			}
			break;
		case State.fly:
			base.transform.Translate(Speed * Time.deltaTime);
			if (hasReachedLevelEnd(300f))
			{
				stateFall();
			}
			break;
		}
	}

	public virtual void stateDisabled()
	{
		if (state != State.disabled)
		{
			SetMaxHealth();
			Init();
			base.colorTinter.Revert();
			CombatBehaviour = GetComponent<AbstractUnitCombatBehaviour>();
			if (CombatBehaviour == null)
			{
				throw new Exception("No combat behaviour found on unit:" + base.gameObject.name);
			}
			CombatBehaviour.Init();
			base.rigidbody.useGravity = false;
			base.rigidbody.isKinematic = true;
			bodyCollider.isTrigger = true;
			bodyCollider.enabled = true;
			Speed = ((!base.Config.isFriendly) ? new Vector3(0f - base.Config.walkspeed, 0f, 0f) : new Vector3(base.Config.walkspeed, 0f, 0f));
			SetState(State.disabled);
		}
	}

	private void Init()
	{
		if (isInitialized)
		{
			return;
		}
		isInitialized = true;
		randomValue = UnityEngine.Random.value;
		SpriteTools.OptimizeSortingOrder(base.animator);
		initialSortingOrder = SpriteTools.GetSortingOrderAsDictionary(this);
		AllowStandToWalk = true;
		bodyCollider = base.gameObject.GetComponent<BoxCollider>();
		if (bodyCollider == null)
		{
			throw new Exception("No unit-body collider found!");
		}
		if (!(base.animator.runtimeAnimatorController is AnimatorOverrideController))
		{
			return;
		}
		AnimatorOverrideController animatorOverrideController = base.animator.runtimeAnimatorController as AnimatorOverrideController;
		AnimationClipPair[] clips = animatorOverrideController.clips;
		foreach (AnimationClipPair animationClipPair in clips)
		{
			if (animationClipPair.overrideClip != null && animationClipPair.overrideClip != animationClipPair.originalClip && (animationClipPair.originalClip.name == "dummy_die 1" || animationClipPair.originalClip.name == "dummy_die 2" || animationClipPair.originalClip.name == "dummy_die 3"))
			{
				die_animation_alternatives++;
			}
		}
	}

	public virtual void stateStand()
	{
		if (state == State.die)
		{
			return;
		}
		if (state == State.disabled || state == State.invalid)
		{
			StopAllCoroutines();
			GameSorting.Set(this);
			height = base.transform.position.y;
			base.animator.transform.localPosition = new Vector3(128f * randomValue - 64f, 0f, 0f);
			PlaySound(EntitySound.Spawn);
			if (CombatBehaviour is FlyerCombatBehaviour)
			{
				stateFly();
			}
			else
			{
				stateMove();
			}
		}
		else
		{
			if (AllowStandToWalk && !hasReachedLevelEnd(0f))
			{
				StartCoroutine(WaitForMove());
			}
			SetState(State.stand);
		}
	}

	private IEnumerator WaitForMove()
	{
		yield return new WaitForSeconds(UnityEngine.Random.value * 0.2f + 0.1f);
		if (state == State.stand)
		{
			stateMove();
		}
	}

	public virtual void stateMove()
	{
		if (state != State.die)
		{
			PlaySound(EntitySound.Move);
			SetState(State.move);
		}
	}

	public virtual void stateCombat()
	{
		if (state != State.die)
		{
			SetState(State.combat);
		}
	}

	public virtual void stateFly()
	{
		if (state != State.die)
		{
			base.transform.LocalPosY(700f);
			SetState(State.fly);
		}
	}

	public virtual void stateFall()
	{
		if (state == State.fly)
		{
			SetState(State.fall);
			if (!base.Config.isFriendly)
			{
				QuestManager.instance.ReportProgress(QuestObjective.Ground_Flyer, 1);
			}
			float num = FallToGround();
			WaitThen(num - state_blend_duration, stateStand);
		}
	}

	public override void stateDie()
	{
		if (state != State.die)
		{
			base.stateDie();
			float delay = FallToGround();
			bodyCollider.enabled = false;
			if (!base.Config.isFriendly)
			{
				Player.Instance.LevelCoins += Level.Instance.KillCoins;
				Player.Instance.LevelXP += Level.Instance.KillXP;
				WaitThen(delay, SpawnCollectable);
			}
			if (!base.Config.isFriendly && Level.Instance.state == Level.State.playing)
			{
				SocialGamingManager.Instance.ReportProgress(AchievementIds.KILL_100_CAVEMEN, 1, 100);
				QuestManager.instance.ReportProgress(QuestObjective.Kill_Neander, 1);
			}
			SetState(State.die);
		}
	}

	public virtual void stateStun(float pStunDuration)
	{
		if (immune_to_stun)
		{
			return;
		}
		switch (state)
		{
		case State.die:
		case State.stun:
		case State.fly:
		case State.fall:
			stateFall();
			return;
		}
		if (!(pStunDuration <= 0f))
		{
			StartCoroutine(WaitForStunEnd(pStunDuration));
			SetState(State.stun);
		}
	}

	public void setDropCollectable(UnitType collectable, int index)
	{
		if (Konfiguration.isCollectable(collectable) && Player.MaxLevelID == Level.Instance.levelid && !Player.CollectableDropProgress[index])
		{
			drop_collectable = collectable;
			level_collectable_index = index;
		}
	}

	private void SpawnCollectable()
	{
		if (drop_collectable != 0)
		{
			CollectableItem.Create(drop_collectable, base.transform.position.SetY(UnityEngine.Random.Range(140f, 160f)));
			Player.CollectableDropProgress[level_collectable_index] = true;
			drop_collectable = UnitType.None;
		}
		else if (UnityEngine.Random.value < base.Config.dropChance && !Level.Instance.Config.override_shots_selection && !Level.Instance.Config.override_dino_selection && Level.Instance.CurrentEnemyIndex < Level.Instance.Config.enemies.Count - 5)
		{
			CollectableItem.Create(UnitType.CollectableApple, base.transform.position.SetY(UnityEngine.Random.Range(140f, 160f)));
		}
	}

	private float FallToGround()
	{
		float num = base.transform.position.y - height;
		if (num < 1f)
		{
			return 0f;
		}
		float num2 = 980f;
		float num3 = num / num2;
		Go.to(base.transform, num3, new GoTweenConfig().position(base.transform.position.SetY(height)));
		return num3;
	}

	private IEnumerator WaitForStunEnd(float pStunDuration)
	{
		yield return new WaitForSeconds(pStunDuration);
		if (state == State.stun)
		{
			stateStand();
		}
	}

	public bool hasReachedLevelEnd(float extra_distance = 0f)
	{
		float num = bodyCollider.bounds.extents.x + bodyCollider.center.x + (float)base.Config.attackRange * 0.95f;
		if (base.Config.isFriendly)
		{
			Debug.DrawLine(new Vector3((float)Level.Instance.Config.levelwidth - 160f - 1f - num - extra_distance, 2048f), new Vector3((float)Level.Instance.Config.levelwidth - 160f - num - extra_distance, -1000f), Color.yellow, 1f);
			return base.transform.position.x > (float)Level.Instance.Config.levelwidth - 160f - 1f - num - extra_distance;
		}
		return base.transform.position.x < 149f + num * 0.5f + extra_distance;
	}

	public override void ChangeHealth(int amount, BaseEntity entitiy, bool showTintFX = true)
	{
		if (hits_to_kill == -1)
		{
			base.ChangeHealth(amount, entitiy, showTintFX);
		}
		else if (!(Time.time < last_hit))
		{
			last_hit = Time.time + 1f;
			int num = Mathf.CeilToInt((float)base.max_health / (float)hits_to_kill);
			base.ChangeHealth(Mathf.RoundToInt(Mathf.Sign(amount) * (float)num), entitiy, showTintFX);
		}
	}
}
