using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainTrigger : GenericCombatBehaviour<BaseEntity>
{
	private const string rain_anim = "rain";

	private const string idle_anim = "idle";

	private const float anim_crossfade = 0.1f;

	public float min_idle;

	public float max_idle;

	public float min_rain;

	public float max_rain;

	private Animator animator;

	public override void Init()
	{
		base.Init();
		base.gameObject.layer = 19;
		animator = GetComponentInChildren<Animator>();
	}

	private void OnEnable()
	{
		StartCoroutine(Rain());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private float getIdleTime()
	{
		return Random.Range(min_idle, max_idle);
	}

	private float getRainTime()
	{
		return Random.Range(min_rain, max_rain);
	}

	private IEnumerator Rain()
	{
		yield return new WaitForSeconds(2f);
		while (true)
		{
			base.GetComponent<Collider>().enabled = true;
			animator.CrossFade("rain", 0.1f, 0);
			List<BaseEntity> neanders = EntityFactory.GetEntities(false);
			neanders.FindAll((BaseEntity neander) => neander.transform.position.x > base.GetComponent<Collider>().bounds.center.x - base.GetComponent<Collider>().bounds.extents.x && neander.transform.position.x < base.GetComponent<Collider>().bounds.center.x + base.GetComponent<Collider>().bounds.extents.x).ForEach(delegate(BaseEntity neander)
			{
				Collider component = neander.GetComponent<Collider>();
				if (component != null)
				{
					OnTriggerEnter(component);
				}
			});
			yield return new WaitForSeconds(getRainTime());
			base.GetComponent<Collider>().enabled = false;
			animator.CrossFade("idle", 0.1f, 0);
			yield return new WaitForSeconds(getIdleTime());
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (base.gameObject.layer == other.gameObject.layer)
		{
			return;
		}
		BaseEntity componentInChildren = other.GetComponentInChildren<BaseEntity>();
		if (componentInChildren != null && isValidAttackTarget(componentInChildren.unitType))
		{
			FireNeander_Helper componentInChildren2 = componentInChildren.GetComponentInChildren<FireNeander_Helper>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.ExtinguishFire();
			}
		}
	}
}
