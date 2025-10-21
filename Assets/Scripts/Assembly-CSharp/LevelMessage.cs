using UnityEngine;

public class LevelMessage : MonoBase
{
	private tk2dTextMesh label;

	private GoTween scaleTween;

	private GoTween fadeTween;

	private void Start()
	{
		Level.Instance.OnLevelMessage += ShowLevelMessage;
		label = GetComponentInChildren<tk2dTextMesh>();
		scaleTween = Go.from(label.transform, 0.5f, new GoTweenConfig().scale(Vector3.one * 0.5f).setEaseType(GoEaseType.BounceOut));
		scaleTween.pause();
		scaleTween.autoRemoveOnComplete = false;
		fadeTween = Go.from(label, 0.2f, new GoTweenConfig().colorProp("color", Colors.Invisible));
		fadeTween.pause();
		fadeTween.autoRemoveOnComplete = false;
	}

	private void ShowLabel()
	{
		label.GetComponent<Renderer>().enabled = true;
		scaleTween.playForward();
		scaleTween.setOnCompleteHandler(null);
		fadeTween.playForward();
	}

	private void HideLabel()
	{
		scaleTween.playBackwards();
		scaleTween.setOnCompleteHandler(delegate
		{
			label.GetComponent<Renderer>().enabled = false;
		});
		fadeTween.playBackwards();
	}

	private void ShowLevelMessage(string message)
	{
		StopAllCoroutines();
		if (message.Length > label.maxChars)
		{
			label.maxChars = message.Length;
		}
		label.text = message;
		ShowLabel();
		WaitThen(3f, HideLabel);
	}
}
