using UnityEngine;

public class FlyerHelper : MonoBase
{
	private const string drop = "drop";

	private const string fly = "fly";

	private Unit unit;

	private Animator animator;

	public string parentTransformName;

	private Transform parent;

	private Vector3 start_position;

	private Quaternion start_rotation;

	private void Start()
	{
		unit = GetComponentInParent<Unit>();
		animator = GetComponentInChildren<Animator>();
		parent = unit.animator.transform.Search(parentTransformName);
		base.transform.parent = parent;
		start_position = base.transform.localPosition;
		start_rotation = base.transform.rotation;
		unit.OnStateChanged += HandleOnStateChanged;
	}

	private void HandleOnStateChanged(Unit unit, Unit.State state)
	{
		switch (state)
		{
		case Unit.State.fly:
			base.transform.parent = parent;
			base.transform.rotation = start_rotation;
			base.transform.localPosition = start_position;
			base.gameObject.SetActive(true);
			animator.Play("fly");
			break;
		case Unit.State.fall:
			base.transform.parent = unit.transform;
			animator.Play("drop");
			break;
		case Unit.State.die:
			if (unit.lastState == Unit.State.fly)
			{
				base.transform.parent = unit.transform;
			}
			animator.Play("drop");
			break;
		default:
			base.gameObject.SetActive(false);
			break;
		}
	}
}
