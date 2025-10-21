using System;
using System.Collections;
using UnityEngine;
using dinobash;

public class DinoEgg : BaseEntity
{
	private string currentAnimation;

	private int healthParameter;

	public bool player_used_dinorage { get; private set; }

	protected override void Start()
	{
		base.Start();
		player_used_dinorage = false;
		healthParameter = Animator.StringToHash("health");
		SpriteTools.SetSortingLayerID(this, 5);
		SpriteTools.OffsetSortingOrder(this, -10000);
		base.OnEntityHit += SetAnimation;
	}

	public void Dino_Rage(Action dino_rage_end_callback)
	{
		player_used_dinorage = true;
		Debug.Log("DINORAGE START");
		Invincibility invincibility = base.transform.AddOrGetComponent<Invincibility>();
		invincibility.hide_health_bar = false;
		SetAnimation(null);
		StartCoroutine(give_apples());
		StartCoroutine(wait_for_rage_end(dino_rage_end_callback));
	}

	private IEnumerator give_apples()
	{
		int apples_to_give = Konfiguration.GameConfig.Dino_rage_apples;
		while (apples_to_give > 0)
		{
			if (Player.Instance.Apples < Player.Instance.get_max_apples())
			{
				Player.Instance.Apples++;
				apples_to_give--;
				yield return new WaitForSeconds(0.05f);
			}
			yield return null;
		}
	}

	private IEnumerator wait_for_rage_end(Action callback)
	{
		yield return new WaitForSeconds(Konfiguration.GameConfig.Dino_rage_duration * 0.5f);
		base.transform.AddOrGetComponent<Invincibility>().enabled = false;
		yield return new WaitForSeconds(Konfiguration.GameConfig.Dino_rage_duration * 0.5f);
		callback();
		Debug.Log("DINORAGE OVER");
	}

	public override void stateDie()
	{
		if (player_used_dinorage)
		{
			level_end();
		}
		else if (!Level.Instance.Config.endless_mode && Level.Instance.levelid > Konfiguration.GameConfig.Dino_rage_start_level && Player.LooseCount + 1 >= Konfiguration.GameConfig.Dino_rage_loose_count && Level.Instance.getProgess() >= Konfiguration.GameConfig.Dino_rage_threshold)
		{
			ScreenManager.GetScreen<DinoRageScreen>().Show();
		}
		else
		{
			level_end();
		}
	}

	public void level_end()
	{
		base.stateDie();
		base.animator.SetInteger(healthParameter, 0);
		base.GetComponent<Collider>().enabled = false;
		Level.Instance.stateLost();
	}

	private void SetAnimation(BaseEntity entity)
	{
		int num = Mathf.RoundToInt(Mathf.Clamp((float)Health / (float)base.max_health * 100f, 0f, 100f));
		if (Health > 0)
		{
			num = Mathf.Max(num, 1);
		}
		base.animator.SetInteger(healthParameter, num);
		PlaySound(EntitySound.DIE);
	}
}
