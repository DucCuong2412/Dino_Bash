using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimedShot : AbstractShot
{
	[SerializeField]
	private float blastRadius = 200f;

	[SerializeField]
	private Animator crosshair_prefab;

	private Animator crosshair;

	private Transform counterScale;

	private float start_distance;

	private Vector3 target;

	private Quaternion targetRotation;

	private int animIndex;

	private string[] animStates = new string[3] { "Rocket_0", "Rocket_1", "Rocket_2" };

	private float rotateInDuration = 0.66f;

	public override void ResetSettings()
	{
		base.ResetSettings();
		base.animator.Play("Rocket_Bind");
		base.transform.localPosition = Vector3.zero;
		base.transform.localScale = Vector3.one;
		counterScale.localScale = Vector3.one;
		fx_impact.transform.parent = counterScale;
		fx_impact.transform.localPosition = Vector3.zero;
		shotCollider.enabled = false;
		shotCollider.transform.localRotation = Quaternion.identity;
	}

	public override void Fire(bool follow_shot)
	{
		base.Fire(follow_shot);
		shotCollider.enabled = true;
		target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		target.z = 0f;
		crosshair.transform.position = target;
		crosshair.transform.localScale = Vector3.zero;
		crosshair.gameObject.SetActive(true);
		crosshair.transform.parent = null;
		Go.to(crosshair.transform, 0.5f, new GoTweenConfig().scale(Vector3.one).setEaseType(GoEaseType.CubicOut));
		start_distance = Vector3.Distance(target, base.transform.position);
		Vector3 vector = target - base.transform.position;
		targetRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector.y, vector.x) * 57.29578f);
		StartCoroutine(RotateIn());
		float num = start_distance / 1000f;
		base.transform.localScale = Vector3.one * num;
		counterScale.localScale = Vector3.one * (1f / num);
		base.animator.speed = Mathf.Clamp(1f / num, 0.5f, 1.5f);
		base.animator.Play(animStates[animIndex]);
		animIndex = ++animIndex % animStates.Length;
		Debug.DrawLine(base.transform.position, target, Color.blue, 3f);
	}

	private IEnumerator RotateIn()
	{
		float t = 0f;
		while (t < rotateInDuration && !base.hitSomething)
		{
			t += Time.deltaTime;
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, targetRotation, t / rotateInDuration);
			yield return null;
		}
	}

	private void Update()
	{
		if (isDropped && counterScale.transform.position.y < -55f && !base.hitSomething)
		{
			Explode();
		}
	}

	private void Awake()
	{
		shotCollider = GetComponentInChildren<CapsuleCollider>();
		counterScale = base.transform.FindChild("counterScale");
		if (crosshair == null)
		{
			crosshair = Object.Instantiate(crosshair_prefab) as Animator;
			crosshair.gameObject.SetActive(false);
			crosshair.transform.parent = base.transform;
		}
	}

	private void Explode()
	{
		Go.to(crosshair.transform, 0.5f, new GoTweenConfig().scale(Vector3.zero).onComplete(delegate
		{
			crosshair.gameObject.SetActive(false);
			crosshair.transform.parent = base.transform;
		}));
		GameCamera.Instance.StopFollowing(base.transform);
		fx_impact.transform.parent = base.transform.parent;
		ShowFx();
		PlayHitSound();
		shotCollider.enabled = false;
		shotCollider.transform.rotation = Quaternion.identity;
		CameraShake.Shake(cameraShakeIntensity);
		HitSomething();
		Collider[] array = Physics.OverlapSphere(counterScale.position, blastRadius, 2048);
		List<BaseEntity> list = AbstractCombatBehaviour.EntitiesForCollider(array);
		foreach (BaseEntity item in list)
		{
			HitUnit(item);
		}
		base.animator.Play("Rocket_Bind");
		base.animator.Update(Time.deltaTime);
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (!base.hitUnit && other.attachedRigidbody.tag == "Body" && other.attachedRigidbody.gameObject.layer == 11)
		{
			Explode();
		}
	}
}
