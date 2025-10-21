using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShot : AbstractShot
{
	[SerializeField]
	private float blastRadius = 200f;

	[SerializeField]
	public bool detonateOnHit;

	[SerializeField]
	private float bulletSpeed = 3000f;

	private float startTime;

	private bool exploded;

	public override void ResetSettings()
	{
		base.ResetSettings();
		exploded = false;
		base.rigidbody.isKinematic = true;
		base.rigidbody.useGravity = false;
		base.rigidbody.drag = 0f;
		base.rigidbody.angularDrag = 0f;
	}

	public override void Fire(bool follow_shot)
	{
		base.Fire(follow_shot);
		base.rigidbody.isKinematic = false;
		base.rigidbody.AddForce(new Vector3(1f, -0.75f, 0f) * bulletSpeed, ForceMode.Impulse);
		base.rigidbody.useGravity = true;
		startTime = Time.time + 0.5f;
		exploded = false;
	}

	protected virtual void Update()
	{
		if (isDropped && !exploded && !detonateOnHit && (Input.GetMouseButtonUp(0) || Input.touchCount > 0) && Time.time > startTime && !base.isTutorialMode)
		{
			StopAllCoroutines();
			StartCoroutine(Explode(true));
		}
	}

	protected virtual void OnCollisionEnter(Collision pOther)
	{
		if (!hitGround && pOther.gameObject.layer == 8)
		{
			hitGround = true;
			base.rigidbody.AddForce(Vector3.right * 300f, ForceMode.Impulse);
			base.rigidbody.drag = 0.5f;
			base.rigidbody.angularDrag = 0.5f;
			StartCoroutine(Explode(detonateOnHit));
		}
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (base.type != ShotType.Meteor)
		{
			return;
		}
		BaseEntity baseEntity = AbstractCombatBehaviour.EntityForCollider(other);
		if (baseEntity != null && baseEntity is Unit)
		{
			Unit unit = (Unit)baseEntity;
			if (unit.state == Unit.State.fly)
			{
				unit.stateFall();
			}
		}
	}

	private IEnumerator Explode(bool explodeNow = false)
	{
		if (hitGround && !explodeNow)
		{
			yield return new WaitForSeconds(2f);
		}
		exploded = true;
		base.rigidbody.isKinematic = true;
		base.transform.rotation = Quaternion.identity;
		PlayHitSound();
		ShowFx();
		GameCamera.Instance.StopFollowing(base.transform);
		CameraShake.Shake(cameraShakeIntensity);
		Collider[] targets = Physics.OverlapSphere(base.transform.position, blastRadius, 3072);
		targets = Array.FindAll(targets, (Collider x) => x.tag == "Body");
		List<BaseEntity> entities = AbstractCombatBehaviour.EntitiesForCollider(targets);
		foreach (BaseEntity target in entities)
		{
			HitUnit(target);
		}
		QuestManager.instance.ReportProgress(QuestObjective.Blast_Neander, entities.Count);
		if (combatEffect == CombatEffects.slow)
		{
			QuestManager.instance.ReportProgress(QuestObjective.IceAge, entities.Count);
		}
		shotCollider.enabled = false;
	}
}
