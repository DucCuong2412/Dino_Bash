using System;
using UnityEngine;

public class StandardButton : MonoBase
{
	private BoxCollider boxCollider;

	private Vector3 boxcolliderSize;

	protected SpriteRenderer sprite;

	protected Sprite _normal;

	[SerializeField]
	protected Sprite _pressed;

	[SerializeField]
	protected Sprite _disabled;

	[HideInInspector]
	public Color originalColor;

	[HideInInspector]
	public Color disabledColor = Colors.Deactivated;

	private GoTween colorTween;

	private bool _enabled = true;

	private GoTween focusTween;

	private static GoTweenConfig inConfig;

	private static GoTweenConfig loopConfig;

	private bool _isFocused;

	[NonSerialized]
	[HideInInspector]
	public Sounds clickSound = Sounds.main_play_button;

	private GoTween _tween;

	public float darken_on_down = 0.8f;

	public tk2dUIItem uiItem { get; private set; }

	public bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			if (_enabled != value)
			{
				_enabled = value;
				uiItem.enabled = value;
				if (_enabled)
				{
					base.gameObject.tag = "Default";
				}
				else
				{
					base.gameObject.tag = "UI_Disabled";
				}
				if (colorTween != null)
				{
					colorTween.complete();
				}
				if (_enabled)
				{
					sprite.sprite = _normal;
					colorTween = Go.to(sprite, 0.15f, new GoTweenConfig().colorProp("color", originalColor));
				}
				else
				{
					sprite.sprite = _disabled;
					Color endValue = originalColor * disabledColor;
					colorTween = Go.to(sprite, 0.15f, new GoTweenConfig().colorProp("color", endValue));
				}
			}
		}
	}

	public bool isFocused
	{
		get
		{
			return _isFocused;
		}
		set
		{
			_isFocused = value;
			if (_isFocused)
			{
				Focus();
			}
			else if (focusTween != null)
			{
				focusTween.destroy();
				base.transform.localScale = Vector3.one;
			}
		}
	}

	public GoTween tween
	{
		get
		{
			if (_tween == null)
			{
				_tween = Go.to(base.transform, 0.2f, new GoTweenConfig().scale(base.transform.localScale * 0.9f).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate).setEaseType(GoEaseType.ExpoOut));
				_tween.pause();
				_tween.autoRemoveOnComplete = false;
			}
			return _tween;
		}
	}

	private void Focus()
	{
		if (focusTween == null)
		{
			inConfig = new GoTweenConfig().scale(new Vector3(1.05f, 0.975f, 1f)).setEaseType(GoEaseType.SineInOut);
			inConfig.loopType = GoLoopType.PingPong;
			inConfig.iterations = 2;
			loopConfig = new GoTweenConfig().scale(new Vector3(1.05f, 0.975f, 1f)).setEaseType(GoEaseType.SineInOut);
			loopConfig.loopType = GoLoopType.PingPong;
			loopConfig.iterations = -1;
		}
		focusTween = Go.to(base.transform, 0.6f, loopConfig);
		focusTween.pause();
		GoTween goTween = Go.to(base.transform, 0.4f, inConfig);
		goTween.setOnCompleteHandler(delegate
		{
			focusTween.play();
		});
	}

	protected void Awake()
	{
		uiItem = this.AddOrGetComponent<tk2dUIItem>();
		uiItem.isHoverEnabled = false;
		sprite = GetComponent<SpriteRenderer>();
		if (sprite != null)
		{
			originalColor = sprite.color;
			_normal = sprite.sprite;
			if (_disabled == null)
			{
				_disabled = _normal;
			}
			if (_pressed == null)
			{
				_pressed = _normal;
			}
		}
		boxCollider = GetComponent<BoxCollider>();
		boxcolliderSize = boxCollider.size;
	}

	private void OnEnable()
	{
		uiItem.OnDown += OnDown;
		uiItem.OnRelease += OnRelease;
	}

	private void OnDisable()
	{
		uiItem.OnDown -= OnDown;
		uiItem.OnRelease -= OnRelease;
	}

	private void OnDown()
	{
		if (Enabled)
		{
			if (isFocused)
			{
				focusTween.pause();
			}
			Color color = darken_on_down * originalColor;
			color.a = originalColor.a;
			sprite.color = color;
			tween.playForward();
			boxCollider.size = boxcolliderSize * 1.35f;
		}
	}

	private void OnRelease()
	{
		if (Enabled)
		{
			if (isFocused)
			{
				focusTween.play();
			}
			if (clickSound != 0)
			{
				AudioPlayer.PlayGuiSFX(clickSound, 0f);
			}
			sprite.color = originalColor;
			tween.playBackwards();
			boxCollider.size = boxcolliderSize;
		}
	}
}
