using UnityEngine;

public class GameCamera : MonoBase
{
	public enum States
	{
		disabled = 0,
		playing = 1,
		following = 2
	}

	private const float swipe_speed = 1.5f;

	private const float allowScrolling_delay = 0.3f;

	private const float slowDampRate = 1f;

	private const float fastDampRate = 45f;

	private bool focusLeft;

	private bool init;

	private float level_start;

	private float level_end;

	private float last_touch_position;

	private float delta;

	private Touch touch = default(Touch);

	private bool touched;

	private bool _allowScrolling;

	private float allowScrolling_timer;

	private float dampRate;

	public static GameCamera Instance { get; private set; }

	public tk2dCamera tk2d_camera { get; private set; }

	public float screen_width { get; private set; }

	public Transform followTarget { get; private set; }

	public States state { get; private set; }

	public bool allowScrolling
	{
		get
		{
			return _allowScrolling;
		}
		set
		{
			_allowScrolling = value;
			if (_allowScrolling)
			{
				allowScrolling_timer = Time.realtimeSinceStartup + 0.3f;
			}
		}
	}

	private void setState(States targetState)
	{
		state = targetState;
	}

	public void EnterState_Disabled()
	{
		followTarget = null;
		setState(States.disabled);
	}

	public void EnterState_Playing()
	{
		delta = 0f;
		followTarget = null;
		setState(States.playing);
	}

	public void EnterState_Following(Transform target, bool focusLeft)
	{
		dampRate = 1f;
		this.focusLeft = focusLeft;
		followTarget = target;
		setState(States.following);
	}

	public void StopFollowing(Transform target)
	{
		if (followTarget == target)
		{
			EnterState_Playing();
		}
	}

	public bool isInView(Transform t, float borderScale = 1f)
	{
		float num = screen_width * (1f - borderScale);
		float x = base.transform.position.x;
		return x < t.position.x + num && t.position.x < x + screen_width - num;
	}

	public GoTween PlayIntroPan(float level_width)
	{
		Start();
		return Go.to(base.transform, 3f, new GoTweenConfig().position(base.transform.position.SetX(level_width - GetComponent<tk2dCamera>().ScreenExtents.width)).setDelay(0.5f).setEaseType(GoEaseType.SineInOut));
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (!init)
		{
			init = true;
			tk2d_camera = base.gameObject.GetComponent<tk2dCamera>();
			screen_width = tk2d_camera.ScreenExtents.width;
			level_start = Konfiguration.GameConfig.levelstart;
			level_end = (float)Level.Instance.Config.levelwidth - screen_width;
			allowScrolling = true;
			Level.Instance.OnLevelPlay += OnLevelStart;
			Level.Instance.OnLevelWon += OnLevelEnd;
			Level.Instance.OnLevelLost += OnLevelEnd;
			Update_Playing();
		}
	}

	private void OnLevelStart()
	{
		EnterState_Playing();
	}

	private void OnLevelEnd()
	{
		EnterState_Disabled();
	}

	private void LateUpdate()
	{
		if (Time.timeScale != 0f)
		{
			switch (state)
			{
			case States.playing:
				Update_Playing();
				break;
			case States.following:
				Update_Following();
				break;
			}
		}
	}

	private void Update_Playing()
	{
		if (Input.touches.Length > 0 && allowScrolling && Time.realtimeSinceStartup > allowScrolling_timer)
		{
			touch = Input.touches[0];
			touched = true;
		}
		else
		{
			touched = false;
		}
		if (touched)
		{
			float x = GetDeltaPosition(touch).x;
			if (x != 0f)
			{
				delta = x;
				delta *= screen_width / (float)Screen.width;
			}
		}
		else if (Mathf.Abs(delta) > 0.01f)
		{
			delta = Mathf.Lerp(delta, 0f, Time.deltaTime * 8f);
		}
		if (level_start < base.transform.position.x - delta && base.transform.position.x - delta < level_end)
		{
			base.transform.Translate(new Vector3(0f - delta, 0f, 0f));
		}
	}

	private Vector2 GetDeltaPosition(Touch touch)
	{
		float num = Time.deltaTime / touch.deltaTime;
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			num = 1f;
		}
		return touch.deltaPosition * num;
	}

	private void Update_Following()
	{
		float x = followTarget.position.x;
		bool flag = base.transform.position.x < x - screen_width * 0.25f;
		if (focusLeft || flag)
		{
			float num = Mathf.Lerp(base.transform.position.x, x - screen_width * 0.25f, dampRate * Time.deltaTime);
			if (level_start - screen_width * 0.5f < num && num < level_end)
			{
				base.transform.PosX(num);
			}
			if (flag && dampRate < 45f)
			{
				dampRate = 45f;
			}
		}
		if (x > (float)Level.Instance.Config.levelwidth)
		{
			EnterState_Playing();
		}
	}
}
