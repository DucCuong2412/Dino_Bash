using System;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class ShotCarrier : MonoBase
{
	public enum States
	{
		disabled = 0,
		start = 1,
		flying = 2,
		shoot = 3,
		flyaway = 4
	}

	[SerializeField]
	private GameObject shotDefinition;

	private Transform shotSpawnTransform;

	[SerializeField]
	private float shot_rate = 0.1f;

	[SerializeField]
	private int ammo = 1;

	private tk2dCamera gameCamera;

	private Vector3 startPosition;

	private float lockInputTimer;

	private float speed = 1500f;

	private float levelwidth;

	private bool hasShot;

	public bool autofire = true;

	public bool randomFly;

	public AnimationCurve flyCurve;

	[HideInInspector]
	public bool tutorialMode;

	private Animator animator;

	private SpriteRenderer shadow;

	private float distance;

	private Transform currentTarget;

	private ShotType shotType;

	private bool user_touched_screen;

	private Vector3 last_postion;

	private Transform tempTarget;

	private Vector3 last_shadow_position;

	private Vector3 shadow_delta;

	private float damp_shadow = 8f;

	public States state { get; private set; }

	public Stack<AbstractShot> shots { get; private set; }

	public bool isAutoShot { get; private set; }

	private void setState(States targetState)
	{
		state = targetState;
	}

	private void Start()
	{
		levelwidth = Level.Instance.Config.levelwidth;
		if (randomFly)
		{
			base.transform.PosY(base.transform.position.y - 100f);
		}
		startPosition = base.transform.position;
		shots = new Stack<AbstractShot>();
		animator = GetComponentInChildren<Animator>();
		gameCamera = Camera.main.GetComponent<tk2dCamera>();
		shotSpawnTransform = base.transform.Search("ShotSpawnPoint");
		if (shotSpawnTransform == null)
		{
			throw new Exception("No Shot SpawnPoint defined");
		}
		shadow = UnityEngine.Object.Instantiate(Resources.Load<SpriteRenderer>("Shooting/Fly_Dino_Shadow")) as SpriteRenderer;
		shadow.transform.parent = base.transform;
		shadow.transform.localPosition = new Vector3(0f, -830f);
		shotDefinition = UnityEngine.Object.Instantiate(shotDefinition) as GameObject;
		shotDefinition.transform.parent = shotSpawnTransform;
		shotDefinition.transform.localPosition = Vector3.one;
		SpriteTools.SetSortingLayerID(this, 11);
		SpriteTools.TargetSetSortingLayerID(shadow, 4);
		shotDefinition.SetActive(false);
		ObjectPool.CreatePool(shotDefinition.transform);
		tempTarget = new GameObject("tempTarget_" + shotType).transform;
		setState(States.disabled);
	}

	private void OnEnable()
	{
		Level.Instance.OnLevelWon += OnLevelEnd;
		Level.Instance.OnLevelLost += OnLevelEnd;
	}

	private void OnDisable()
	{
		Level.Instance.OnLevelWon -= OnLevelEnd;
		Level.Instance.OnLevelLost -= OnLevelEnd;
	}

	private void OnLevelEnd()
	{
		if (state == States.flying)
		{
			stateFlyaway(false);
		}
	}

	private void stateDisabled(bool reset_cooldown = false)
	{
		base.transform.position = startPosition;
		base.transform.rotation = Quaternion.identity;
		foreach (AbstractShot shot in shots)
		{
			shot.FreeBullet();
		}
		animator.SetBool("fly_off", false);
		animator.Update(Time.deltaTime);
		if (!hasShot)
		{
			ShotFactory.ShotEnd();
		}
		hasShot = false;
		if (reset_cooldown)
		{
			HudScreen screen = ScreenManager.GetScreen<HudScreen>();
			ShootButton shootButton = screen.ShotButtons.Find((ShootButton btn) => btn.shotType == shotType);
			shootButton.EndCooldown();
		}
		setState(States.disabled);
		base.gameObject.SetActive(false);
	}

	public void stateStart(Transform target)
	{
		isAutoShot = target != null;
		if (isAutoShot)
		{
			currentTarget = FindNearestTarget(target);
		}
		setState(States.start);
		base.gameObject.SetActive(true);
		List<BaseEntity> list = EntityFactory.GetEntities(false);
		if (Player.Instance.bird_focus_on_visible_neanders)
		{
			list = list.FindAll((BaseEntity x) => GameCamera.Instance.isInView(x.transform));
		}
		if (list.Count > 0)
		{
			list.Sort((BaseEntity x, BaseEntity y) => x.transform.position.x.CompareTo(y.transform.position.x));
		}
		float num = gameCamera.transform.position.x + gameCamera.ScreenExtents.width * 0.5f;
		float x2 = num - gameCamera.ScreenExtents.width * 0.75f;
		bool focusLeft = false;
		if (list.Count > 0 && list[0].transform.position.x < num)
		{
			x2 = list[0].transform.position.x - gameCamera.ScreenExtents.width;
			focusLeft = true;
		}
		if (!isAutoShot)
		{
			GameCamera.Instance.EnterState_Following(base.transform, focusLeft);
		}
		base.transform.PosX(x2);
		distance = (float)Level.Instance.Config.levelwidth - base.transform.position.x;
		lockInputTimer = Time.time + 0.4f;
		shots.Clear();
		for (int i = 0; i < ammo; i++)
		{
			AbstractShot component = ObjectPool.Spawn(shotDefinition.transform).GetComponent<AbstractShot>();
			component.gameObject.SetActive(true);
			component.transform.parent = shotDefinition.transform.parent;
			component.transform.localPosition = shotDefinition.transform.localPosition;
			SpriteRenderer componentInChildren = component.GetComponentInChildren<SpriteRenderer>();
			component.transform.position += Quaternion.Euler(0f, 0f, 360 / ammo * shots.Count) * new Vector3(componentInChildren.bounds.extents.magnitude * 0.25f, 0f, 0f);
			component.isTutorialMode = tutorialMode;
			component.ResetSettings();
			shots.Push(component);
			shotType = component.type;
		}
		Color color = shadow.color;
		color.a = 1f;
		shadow.color = color;
		if (!isAutoShot)
		{
			FullScreenTouch.onClick += HandeOnClick;
		}
		stateFlying();
	}

	private void HandeOnClick()
	{
		user_touched_screen = true;
	}

	private void stateFlying()
	{
		setState(States.flying);
	}

	private void stateShoot()
	{
		float num = 0f;
		while (shots.Count > 0)
		{
			AbstractShot bullet = shots.Pop();
			WaitThen(num, delegate
			{
				bullet.transform.parent = base.transform.parent;
				bullet.Fire(shots.Count == 0 && !isAutoShot);
				if (bullet is BombShot && isAutoShot)
				{
					((BombShot)bullet).detonateOnHit = true;
				}
				bullet = null;
				AudioPlayer.PlayGuiSFX(Sounds.shoot_drop_shot, 0f, UnityEngine.Random.value * 0.4f + 0.8f);
			});
			num += shot_rate;
			if (!autofire && shots.Count > 0)
			{
				user_touched_screen = false;
				lockInputTimer = Time.time + shot_rate;
				return;
			}
		}
		hasShot = true;
		ShotFactory.ShotEnd();
		WaitThen(num, delegate
		{
			stateFlyaway(hasShot);
		});
	}

	private void stateFlyaway(bool did_shoot)
	{
		if (state != States.flyaway)
		{
			FullScreenTouch.onClick -= HandeOnClick;
			user_touched_screen = false;
			Color color = shadow.color;
			color.a = 0f;
			Go.to(shadow, 0.5f, new GoTweenConfig().colorProp("color", color));
			animator.SetBool("fly_off", true);
			WaitThen(2f, delegate
			{
				stateDisabled(!did_shoot && !autofire);
			});
			setState(States.flyaway);
		}
	}

	public AbstractShot TutorialShoot()
	{
		AbstractShot result = shots.Peek();
		stateShoot();
		tutorialMode = false;
		return result;
	}

	private Transform FindNearestTarget(Transform from)
	{
		Transform result = from;
		float num = 512f;
		ShotType shotType = this.shotType;
		if (shotType == ShotType.Wobble || shotType == ShotType.DoubleHunk)
		{
			num = 64f;
		}
		List<BaseEntity> entities = EntityFactory.GetEntities(this.shotType == ShotType.Heal);
		if (entities.Count > 0)
		{
			float num2 = float.MaxValue;
			foreach (BaseEntity item in entities)
			{
				float num3 = Mathf.Abs(item.transform.position.x - from.position.x);
				if (num3 < num2 && num3 < num)
				{
					num2 = num3;
					result = item.transform;
				}
			}
		}
		return result;
	}

	private void Update()
	{
		States states = state;
		if (states == States.flying)
		{
			Translate();
			if (isAutoShot)
			{
				if (currentTarget.position.x == EntityFactory.NeanderSpawnPosition || currentTarget.position.x == EntityFactory.DinoSpawnPosition)
				{
					tempTarget.position = last_postion;
					currentTarget = tempTarget;
				}
				if (!(base.transform.position.x > currentTarget.position.x - getShotHeuristic(currentTarget, shotType)))
				{
					last_postion = currentTarget.position;
					goto IL_0126;
				}
				stateShoot();
			}
			else
			{
				if (!user_touched_screen || !GameCamera.Instance.isInView(base.transform) || !(Time.time > lockInputTimer) || tutorialMode)
				{
					goto IL_0126;
				}
				stateShoot();
			}
		}
		goto IL_01bd;
		IL_0126:
		if (GameCamera.Instance.followTarget == base.transform && GameCamera.Instance.transform.position.x > levelwidth - gameCamera.ScreenExtents.width)
		{
			GameCamera.Instance.StopFollowing(base.transform);
		}
		if (base.transform.position.x >= levelwidth)
		{
			stateFlyaway(hasShot);
		}
		goto IL_01bd;
		IL_01bd:
		if (state != 0)
		{
			UpdateShadow();
		}
	}

	private float getShotHeuristic(Transform target, ShotType shot)
	{
		bool flag = target.position.y >= 700f;
		switch (shot)
		{
		case ShotType.Bomb:
			return 600f;
		case ShotType.Wobble:
		case ShotType.DoubleHunk:
			return (!flag) ? 100f : 50f;
		case ShotType.Triple:
			return (!flag) ? 650f : 150f;
		default:
			return (!flag) ? 450f : 200f;
		}
	}

	private void Translate()
	{
		float num = 1f;
		float num2 = 0f;
		if (randomFly)
		{
			num = Mathf.Sin(Time.time * 4f) * 0.5f + 0.65f;
			num2 = (Mathf.Sin(Time.time * 10f) + Mathf.Cos(Time.time * 3.3434f)) * 0.6f;
		}
		Vector3 translation = new Vector3(num * speed * Time.deltaTime, num2 * Time.deltaTime, 0f);
		if (!randomFly)
		{
			float num3 = (float)Level.Instance.Config.levelwidth - (base.transform.position.x + translation.x);
			float time = 1f - num3 / distance;
			float num4 = startPosition.y + flyCurve.Evaluate(time) * 300f;
			translation.y = num4 - base.transform.position.y;
		}
		base.transform.Translate(translation, Space.World);
		base.transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Atan2(translation.y, translation.x) - Mathf.Atan2(0f, 1f)) * 57.29578f);
	}

	private void UpdateShadow()
	{
		shadow.transform.rotation = Quaternion.identity;
		if (state == States.flying)
		{
			shadow.transform.position = new Vector3(animator.transform.position.x + 300f, 80f, 0f);
			shadow_delta = shadow.transform.position - last_shadow_position;
			last_shadow_position = shadow.transform.position;
		}
		else
		{
			shadow_delta = Vector3.Lerp(shadow_delta, Vector3.zero, Time.deltaTime * damp_shadow);
			shadow.transform.position += shadow_delta;
		}
	}
}
