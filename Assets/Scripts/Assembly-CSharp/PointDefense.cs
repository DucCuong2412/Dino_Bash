using System.Collections;
using UnityEngine;

public class PointDefense : GenericCombatBehaviour<BaseEntity>
{
	public Transform mDefensePoint;

	public float mRadius = 1f;

	public float mAttackRate = 1f;

	public override void Init()
	{
		if (!isInititalized)
		{
			base.Init();
			StartCoroutine(AttackAtPoint());
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	private IEnumerator AttackAtPoint()
	{
		while (true)
		{
			yield return new WaitForSeconds(mAttackRate);
			Collider[] hitColliders = Physics.OverlapSphere(mDefensePoint.position, mRadius, base.RayCastMask);
			for (int i = 0; i < hitColliders.Length; i++)
			{
				BaseEntity target = hitColliders[i].gameObject.GetComponent<BaseEntity>();
				Attack(target);
			}
		}
	}
}
