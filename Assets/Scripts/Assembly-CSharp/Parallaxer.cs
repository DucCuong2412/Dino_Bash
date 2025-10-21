using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
	private tk2dCamera ui_camera;

	private float cam_x;

	private float CamXAnimationStart;

	private float CamXAnimationTarget_;

	private float CamXAnimationStartTime;

	private static int screen_width = 2048;

	public AnimationCurve rolloutCurve;

	public AnimationCurve focusAnimCurve;

	private List<ParallaxLayer> layers = new List<ParallaxLayer>();

	private Vector3 last_mouse_position = Vector3.zero;

	private float rollout_start_speed;

	private float rollout_start_time;

	public float d1;

	public float d2;

	public float left_edge = 35f;

	public float right_edge = -1000f;

	public float CamX
	{
		get
		{
			return cam_x;
		}
		set
		{
			cam_x = value;
		}
	}

	public float CamXAnimationTarget
	{
		get
		{
			return CamXAnimationTarget_;
		}
		set
		{
			CamXAnimationStart = CamX;
			CamXAnimationStartTime = Time.time;
			CamXAnimationTarget_ = value;
		}
	}

	public bool HandleInput { get; set; }

	public void Init()
	{
		ui_camera = ScreenManager.Camera.GetComponent<tk2dCamera>();
		layers = GetComponentsInChildren<ParallaxLayer>().ToList();
	}

	private static float mod(float x, float m)
	{
		float num = x % m;
		return (!(num < 0f)) ? num : (num + m);
	}

	private void handleInput()
	{
		float num = 5f;
		float num2 = (d1 = Time.time - CamXAnimationStartTime);
		if (num2 <= num && Time.time > num)
		{
			float time = num2 / num;
			CamX = Mathf.Lerp(CamXAnimationStart, CamXAnimationTarget, focusAnimCurve.Evaluate(time));
		}
		else if (HandleInput)
		{
			Vector3 vector = Vector3.zero;
			if (Application.isEditor)
			{
				vector = ui_camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
			}
			else if (Input.touches.Length > 0)
			{
				vector = ui_camera.GetComponent<Camera>().ScreenToWorldPoint(Input.touches[0].position);
			}
			if (Input.GetMouseButtonDown(0))
			{
				last_mouse_position = vector;
				rollout_start_speed = 0f;
			}
			if (Input.GetMouseButtonUp(0))
			{
				rollout_start_time = Time.time;
			}
			if (Input.GetMouseButton(0))
			{
				float num3 = 0.025f;
				CamX += (vector - last_mouse_position).x * num3;
				float num4 = (vector - last_mouse_position).x * num3;
				rollout_start_speed = rollout_start_speed * 0.3f + num4 * 0.7f;
				last_mouse_position = vector;
			}
			float num5 = left_edge - CamX;
			float num6 = right_edge - CamX;
			float num7 = Screen.width;
			float num8 = 4f;
			if (num5 < 0f)
			{
				rollout_start_speed = 0f;
				float num9 = (1f - 1f / (num5 * num8 / num7 + 1f)) * num7;
				CamX += num9 * Time.deltaTime;
			}
			else if (num6 > 0f)
			{
				rollout_start_speed = 0f;
				float num10 = (1f - 1f / (num6 * num8 / num7 + 1f)) * num7;
				CamX += num10 * Time.deltaTime;
			}
			float time2 = (Time.time - rollout_start_time) / (float)rolloutCurve.length;
			float num11 = Mathf.Lerp(rollout_start_speed, 0f, rolloutCurve.Evaluate(time2));
			CamX += num11;
		}
	}

	private void LateUpdate()
	{
		handleInput();
		foreach (ParallaxLayer layer in layers)
		{
			float num = layer.bounds.max.x - layer.bounds.min.x;
			float m = 2f * Mathf.Ceil((float)screen_width / num) * num;
			float num2 = (float)(-ui_camera.nativeResolutionWidth / 2) - num * 2f;
			float num3 = layer.constantSpeed * Time.deltaTime;
			float num4 = layer.speed * CamX + layer.startPosition.x + num3;
			layer.startPosition.x += num3;
			if (layer.autoRepeat)
			{
				layer.transform.LocalPosX(mod(num4 - num2, m) + num2);
			}
			else
			{
				layer.transform.LocalPosX(num4);
			}
		}
	}
}
