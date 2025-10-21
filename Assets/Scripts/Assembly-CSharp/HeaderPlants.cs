using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeaderPlants : MonoBase
{
	private BaseScreen screen;

	private Animator animator;

	private void Start()
	{
		screen = base.transform.GetComponentUpwards<BaseScreen>();
		animator = GetComponent<Animator>();
		animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		screen.OnScreenShow += ResetAnimation;
	}

	private void OnDisable()
	{
		if (screen != null)
		{
			screen.OnScreenShow -= ResetAnimation;
		}
	}

	private void ResetAnimation()
	{
		animator.CrossFade("headerplants_in", 0f);
	}
}
