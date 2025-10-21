using System.Collections;
using UnityEngine;

public class Healthbar : MonoBase
{
	private BaseEntity unit;

	private float currentHealth;

	private float maxhealth;

	private float timer;

	private tk2dSlicedSprite bar_bg;

	private tk2dClippedSprite bar;

	private bool isInit;

	private bool isVisible = true;

	private GoTweenFlow tween;

	private static int global_max_health = -1;

	public void Init()
	{
		if (!isInit)
		{
			unit = base.transform.parent.gameObject.GetComponent<BaseEntity>();
			base.transform.position = new Vector3(0f, unit.collider.bounds.extents.y + 200f, 0f);
			bar_bg = GetComponentInChildren<tk2dSlicedSprite>();
			bar = GetComponentInChildren<tk2dClippedSprite>();
			if (global_max_health == -1)
			{
				GetMaxHealthValue();
			}
			isInit = true;
		}
		timer = 0f;
		bar.ClipRect = new Rect(0f, 0f, 1f, 1f);
		maxhealth = unit.Config.healthpoints;
		float f = maxhealth / (float)global_max_health;
		f = Mathf.Log(f) * 0.15f + 1f;
		bar.scale = new Vector3(Mathf.Lerp(0.5f, 3f, f), 1f, 1f);
		bar_bg.dimensions = new Vector2(Mathf.Lerp(120f, 550f, f), bar_bg.dimensions.y);
		OnEnable();
	}

	private void GetMaxHealthValue()
	{
		foreach (EntityData value in Konfiguration.UnitData.Values)
		{
			global_max_health = Mathf.Max(global_max_health, value.healthpoints);
		}
	}

	private IEnumerator UpdateBar()
	{
		yield return null;
		currentHealth = unit.Health;
		while (true)
		{
			if (currentHealth != (float)unit.Health)
			{
				if (unit.Health > 0)
				{
					ShowBar();
					timer = Time.time + 2f;
					currentHealth = Mathf.RoundToInt(Mathf.Lerp(currentHealth, unit.Health, Time.deltaTime * 10f));
					bar.ClipRect = new Rect(0f, 0f, currentHealth / maxhealth, 1f);
					yield return null;
					continue;
				}
				HideBar();
			}
			if (Time.time > timer || currentHealth <= 0f)
			{
				HideBar();
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void OnEnable()
	{
		if (isInit)
		{
			timer = -1f;
			HideBar();
			currentHealth = unit.Health;
			maxhealth = unit.max_health;
			StartCoroutine(UpdateBar());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		if (isInit)
		{
			HideBar();
		}
	}

	private void OnDestroy()
	{
		maxhealth = -1f;
	}

	private void ShowBar()
	{
		if (!isVisible)
		{
			bar_bg.renderer.enabled = true;
			bar.renderer.enabled = true;
			isVisible = true;
		}
	}

	private void HideBar()
	{
		if (isVisible)
		{
			if (bar_bg != null)
			{
				bar_bg.renderer.enabled = false;
			}
			if (bar != null)
			{
				bar.renderer.enabled = false;
			}
			isVisible = false;
		}
	}
}
