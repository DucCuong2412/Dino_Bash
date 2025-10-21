using UnityEngine;

public class FX_PlayOnEnable : MonoBase
{
	private Animator animator;

	private bool isInit;

	[SerializeField]
	private string animatorBoolParameter = "play";

	private void Init()
	{
		if (!isInit)
		{
			animator = GetComponent<Animator>();
			isInit = true;
		}
	}

	private void OnEnable()
	{
		if (!isInit)
		{
			Init();
		}
		animator.SetBool(animatorBoolParameter, true);
	}

	private void OnDisable()
	{
		animator.SetBool(animatorBoolParameter, false);
	}
}
