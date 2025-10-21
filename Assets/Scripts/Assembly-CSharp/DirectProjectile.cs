using System.Collections;
using UnityEngine;

public class DirectProjectile : AbstractProjectile
{
	[SerializeField]
	private float speed;

	[SerializeField]
	private float angle;

	public override void Fire(Vector3 start, float targetVelocity)
	{
		base.Fire(start, targetVelocity);
		base.transform.parent = null;
		base.transform.position = start;
		float z = ((!base.unit.Config.isFriendly) ? (180f - angle) : (0f + angle));
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, z));
		StartCoroutine(Fly());
	}

	private void OnTriggerEnter(Collider pOther)
	{
		if (!hitGround && base.unit.CombatBehaviour.isInRayCastMask(pOther.gameObject.layer))
		{
			Attack();
			ShowHitFX();
			hitGround = true;
		}
	}

	private IEnumerator Fly()
	{
		hitGround = false;
		while (!hitGround)
		{
			base.transform.Translate(base.transform.right * speed * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
	}
}
