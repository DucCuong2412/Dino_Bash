using UnityEngine;

public class TutorialArrow : MonoBase
{
	private Animator animator;

	private bool isInitialized;

	public bool Visible { get; private set; }

	private void Start()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			animator = GetComponent<Animator>();
			animator.Play("Arrow_hidden");
		}
	}

	public void In()
	{
		if (!isInitialized)
		{
			Start();
		}
		if (animator != null)
		{
			animator.Play("Arrow_in");
			Visible = true;
		}
	}

	public void Out()
	{
		if (animator != null)
		{
			animator.CrossFade("Arrow_out", 0.1f);
			Visible = false;
		}
	}
}
