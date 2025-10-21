using System;
using UnityEngine;

public class ArcProjectile : AbstractProjectile
{
	private float projectileFlightHeight = 1000f;

	public float rangeForceScale = 4f;

	public bool use_arc = true;

	public override void Fire(Vector3 start, float targetVelocity)
	{
		base.Fire(start, targetVelocity);
		base.transform.parent = null;
		base.transform.position = start;
		base.transform.rotation = Quaternion.identity;
		base.rigidbody.useGravity = true;
		base.rigidbody.isKinematic = false;
		base.rigidbody.velocity = Vector3.zero;
		float num = start.x - base.unit.transform.position.x;
		float num2 = (start.y - base.unit.transform.position.y) * 0.5f;
		Vector3 position = base.unit.CombatBehaviour.CombatTargets[0].transform.position;
		float num3 = Mathf.Abs(base.unit.transform.position.x + num - position.x);
		if (num3 == 0f)
		{
			num3 = 42f;
		}
		float y = start.y;
		float num4 = 0f;
		float num5 = 0f;
		if (use_arc)
		{
			num3 -= num2;
			float num6 = Mathf.Lerp(projectileFlightHeight, 0f, y / projectileFlightHeight);
			float num7 = Mathf.Atan(4f * (num6 / num3)) * 57.29578f;
			num4 = Mathf.Sqrt(Mathf.Abs(num3 * Physics.gravity.y) / Mathf.Sin(2f * num7 * ((float)Math.PI / 180f)));
			Debug.DrawLine(start, start + new Vector3((!base.unit.Config.isFriendly) ? (0f - num3) : num3, 0f, 0f), Color.green, 0.5f);
			num5 = ((!base.unit.Config.isFriendly) ? (180f - num7) : num7);
		}
		else
		{
			num4 = num3 / Mathf.Sqrt(Mathf.Abs(Physics.gravity.y * rangeForceScale) / (2f * (start.y - position.y)));
			num5 = ((!base.unit.Config.isFriendly) ? 180 : 0);
		}
		base.transform.Rotate(new Vector3(0f, 0f, num5));
		if (float.IsNaN(num4))
		{
			num4 = 42f;
		}
		base.rigidbody.AddForce(base.transform.right * num4, ForceMode.VelocityChange);
		hitGround = false;
	}

	private void FixedUpdate()
	{
		if (base.rigidbody.velocity != Vector3.zero && !hitGround)
		{
			base.rigidbody.rotation = Quaternion.Euler(0f, 0f, (Mathf.Atan2(base.rigidbody.velocity.y, base.rigidbody.velocity.x) - Mathf.Atan2(0f, 1f)) * 57.29578f);
		}
	}

	private void OnTriggerEnter(Collider pOther)
	{
		if (!hitGround && base.unit.CombatBehaviour.isInRayCastMask(pOther.gameObject.layer))
		{
			Attack();
			ShowHitFX();
			if (!canMultiAttack)
			{
				hitGround = true;
			}
			base.rigidbody.useGravity = false;
			base.rigidbody.isKinematic = true;
		}
	}
}
