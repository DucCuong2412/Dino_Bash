using System;
using UnityEngine;

public class ShootButton : StandardButton
{
	private float timer;

	private float cooldownTime;

	private bool cooldown;

	private Transform cooldownTransform;

	private HudScreen hud;

	private Animator crosshair;

	private Transform dummyTarget;

	private bool dragTutorial;

	private Transform tutorialTarget;

	private Action<bool> tutorialCallback;

	private GoTween wobbleTween;

	private bool dragging;

	private static Transform swipeArrowPrefab;

	private static bool hint_is_shown;

	private Transform dragArrow;

	private GoTween dragTween;

	public ShotType shotType { get; private set; }

	public void SetDragTutorial(Transform target, Action<bool> callback)
	{
		if (target != null)
		{
			dragTutorial = true;
			tutorialTarget = target;
			tutorialCallback = callback;
		}
		else
		{
			dragTutorial = false;
		}
	}

	private void OnDestroy()
	{
		swipeArrowPrefab = null;
	}

	public void Init(ShotType pShotType)
	{
		shotType = pShotType;
		hud = ScreenManager.GetScreen<HudScreen>();
		dummyTarget = new GameObject("target_" + shotType).transform;
		crosshair = base.transform.FindChild("crosshair").GetComponent<Animator>();
		crosshair.transform.parent = dummyTarget;
		crosshair.transform.localPosition = new Vector3(-64f, 64f, 0f);
		SpriteTools.SetSortingLayerID(crosshair, 30);
		base.uiItem.OnDown += HandleOnDown;
		base.uiItem.OnClick += HandleOnClick;
		base.uiItem.OnUp += HandleOnUp;
		base.uiItem.OnRelease += HandleOnRelease;
		cooldownTransform = base.transform.FindChild("sprite/cooldown");
		cooldownTransform.localScale = new Vector3(1f, 0f, 1f);
		string text = "shotbuy_" + shotType.ToString().ToLower();
		base.sprite = base.transform.FindChild("sprite").GetComponent<SpriteRenderer>();
		Sprite sprite = SpriteRessources.GetSprite(text);
		if (sprite != null)
		{
			_normal = sprite;
			_pressed = _normal;
			_disabled = _normal;
			originalColor = base.sprite.color;
			base.sprite.sprite = _normal;
		}
		cooldown = false;
		cooldownTime = Konfiguration.ShotData[shotType].cooldown;
		clickSound = Sounds.shoot_select;
		GoTweenConfig goTweenConfig = new GoTweenConfig();
		goTweenConfig.localRotation(Quaternion.Euler(0f, 0f, 10f));
		goTweenConfig.scale(Vector3.one * 1.15f);
		goTweenConfig.loopType = GoLoopType.PingPong;
		goTweenConfig.iterations = 2;
		wobbleTween = Go.to(base.sprite.transform, 0.3f, goTweenConfig);
		wobbleTween.autoRemoveOnComplete = false;
		wobbleTween.pause();
		UnitTutorialScreen screen = ScreenManager.GetScreen<UnitTutorialScreen>();
		screen.OnScreenShow += HandleOnTutorialShow;
	}

	private void HandleOnTutorialShow()
	{
		dragging = false;
		GameCamera.Instance.allowScrolling = true;
	}

	private void OnCoolDownStart()
	{
		cooldown = true;
		timer = Time.time + cooldownTime;
	}

	public void EndCooldown()
	{
		cooldownTransform.localScale = new Vector3(1f, 0f, 1f);
		base.Enabled = true;
		cooldown = false;
		wobbleTween.restart();
	}

	protected void Update()
	{
		if (cooldown)
		{
			if (Time.time < timer)
			{
				float y = Mathf.Clamp01(Helper.MapRange(cooldownTime, 0f, 1f, 0f, timer - Time.time));
				cooldownTransform.localScale = new Vector3(1f, y, 1f);
			}
			else
			{
				EndCooldown();
			}
		}
		else if (dragging)
		{
			Vector3 position = new Vector3(base.uiItem.Touch.position.x, base.uiItem.Touch.position.y);
			position = ScreenManager.Camera.ScreenToWorldPoint(position);
			sprite.transform.position = position;
		}
		else
		{
			sprite.transform.localPosition = Vector3.zero;
		}
	}

	private void HandleOnDown()
	{
		GameCamera.Instance.allowScrolling = false;
	}

	private void HandleOnRelease()
	{
		GameCamera.Instance.allowScrolling = true;
		if (!dragging)
		{
			return;
		}
		dragging = false;
		Vector3 localPosition = sprite.transform.localPosition;
		localPosition.z = 0f;
		if (!(localPosition.magnitude < 128f))
		{
			Vector3 position = new Vector3(base.uiItem.Touch.position.x, base.uiItem.Touch.position.y);
			Vector3 vector = GameCamera.Instance.camera.ScreenToWorldPoint(position);
			vector.z = 0f;
			if (handleTutorial(vector))
			{
				dummyTarget.transform.position = vector;
				crosshair.Play("crosshair_rotate");
				LaunchShot(dummyTarget);
			}
		}
	}

	private void HandleOnUp()
	{
		if (Konfiguration.GameConfig.Use_dragshot_feature)
		{
			dragging = true;
		}
	}

	private void HandleOnClick()
	{
		if (Konfiguration.GameConfig.Use_dragshot_feature)
		{
			ShowDragHint();
		}
		else
		{
			LaunchShot(null);
		}
	}

	private void LaunchShot(Transform target)
	{
		if (hud.shotbarState != HudScreen.State.Out)
		{
			ShotCarrier shotCarrier = ShotFactory.Fire(shotType, target);
			shotCarrier.shots.ToArray()[0].OnStartCoolDown += OnCoolDownStart;
			base.Enabled = false;
		}
	}

	private void ShowDragHint()
	{
		if (swipeArrowPrefab == null)
		{
			swipeArrowPrefab = Resources.Load<Transform>("GUI/Tutorial/SwipeArrow");
		}
		if (dragArrow == null)
		{
			dragArrow = UnityEngine.Object.Instantiate(swipeArrowPrefab) as Transform;
			SpriteTools.SetSortingLayerID(dragArrow, ScreenManager.GetScreen<HudScreen>().SortingLayerID);
			SpriteTools.SetSortingOrder(dragArrow, 256);
			dragArrow.parent = base.transform.parent.parent;
			dragArrow.position = base.transform.position;
			Vector3 endValue = dragArrow.position + new Vector3(-768f, 512f, 0f);
			Animator component = dragArrow.GetComponent<Animator>();
			component.Play("SwipeArrow_Drag");
			component.Update(Time.deltaTime);
			UnityEngine.Object.Destroy(component);
			GoTweenConfig goTweenConfig = new GoTweenConfig().position(endValue).setEaseType(GoEaseType.CubicInOut);
			goTweenConfig.iterations = 2;
			dragTween = Go.to(dragArrow, 1f, goTweenConfig);
			dragTween.autoRemoveOnComplete = false;
			dragTween.setOnCompleteHandler(delegate
			{
				hint_is_shown = false;
				dragArrow.gameObject.SetActive(false);
			});
		}
		if (dragTween.state == GoTweenState.Complete && !hint_is_shown)
		{
			hint_is_shown = true;
			dragTween.restart();
			dragArrow.gameObject.SetActive(true);
		}
	}

	private bool handleTutorial(Vector3 target_pos)
	{
		if (dragTutorial)
		{
			if (Mathf.Abs(tutorialTarget.transform.position.x - target_pos.x) > 400f)
			{
				Debug.Log("Tutorial Missed DragShot");
				if (tutorialCallback != null)
				{
					tutorialCallback(false);
				}
				return false;
			}
			if (tutorialCallback != null)
			{
				tutorialCallback(true);
			}
		}
		return true;
	}
}
