using System;
using System.Collections;
using UnityEngine;

public class FriendGate : MonoBehaviour
{
	private Animator animator;

	private static readonly int opened = Animator.StringToHash("friendgate open");

	private static readonly int closed = Animator.StringToHash("friendgate closed");

	private static readonly int open = Animator.StringToHash("friendgate animation");

	private Action callback;

	public bool locked
	{
		set
		{
			animator.Play((!value) ? opened : closed);
		}
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void unlock(Action callback = null)
	{
		animator.Play(open);
		this.callback = callback;
		StartCoroutine(PollAnimState());
	}

	private IEnumerator PollAnimState()
	{
		yield return new WaitForSeconds(0.1f);
		while (animator.GetCurrentAnimatorStateInfo(0).IsName("friendgate animation"))
		{
			yield return new WaitForSeconds(0.2f);
		}
		if (callback != null)
		{
			callback();
		}
	}
}
