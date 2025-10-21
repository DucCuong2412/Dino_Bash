using System;
using System.Collections.Generic;
using UnityEngine;

public class StandardShot : AbstractShot
{
	[SerializeField]
	private bool strikeThrough;

	[SerializeField]
	private float bulletSpeed = 500f;

	[SerializeField]
	private float shot_angle = -45f;

	private HashSet<Rigidbody> hit_set = new HashSet<Rigidbody>();

	public override void ResetSettings()
	{
		base.ResetSettings();
		base.rigidbody.isKinematic = true;
		base.rigidbody.useGravity = false;
		base.rigidbody.drag = 0f;
	}

	public override void Fire(bool follow_shot)
	{
		base.Fire(follow_shot);
		hit_set.Clear();
		base.rigidbody.isKinematic = false;
		base.rigidbody.AddForce(Vector3.right.RotateZ(shot_angle * ((float)Math.PI / 180f)) * bulletSpeed, ForceMode.Impulse);
		base.rigidbody.useGravity = true;
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (hitGround || (!strikeThrough && base.hitUnit) || other.attachedRigidbody == null || hit_set.Contains(other.attachedRigidbody))
		{
			return;
		}
		BaseEntity component = other.attachedRigidbody.GetComponent<BaseEntity>();
		if (other.tag == "Body" && HitUnit(component))
		{
			hit_set.Add(other.attachedRigidbody);
			GameCamera.Instance.StopFollowing(base.transform);
			PlayHitSound();
			if (combatEffect == CombatEffects.slow)
			{
				QuestManager.instance.ReportProgress(QuestObjective.IceAge, 1);
			}
			if (!strikeThrough)
			{
				base.rigidbody.isKinematic = true;
				CameraShake.Shake(cameraShakeIntensity);
				ShowFx();
			}
			else if (component is Unit && (component as Unit).CombatBehaviour is ReplacerBehaviour)
			{
				HitShield();
			}
		}
		else if (hitModifier[(int)component.unitType] == 1f || other.tag == "Armor")
		{
			HitShield();
		}
	}

	private void HitShield()
	{
		GameCamera.Instance.StopFollowing(base.transform);
		shotCollider.enabled = false;
		base.rigidbody.isKinematic = true;
		PlayMissSound();
		ShowFx();
	}

	protected virtual void OnCollisionEnter(Collision other)
	{
		if (!hitGround && other.gameObject.layer == 8)
		{
			hitGround = true;
			shotCollider.enabled = false;
			base.rigidbody.isKinematic = true;
			ShowFx();
			PlayMissSound();
			GameCamera.Instance.StopFollowing(base.transform);
		}
	}
}
