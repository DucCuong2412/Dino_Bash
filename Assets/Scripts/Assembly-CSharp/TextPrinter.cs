using System.Collections;
using UnityEngine;

[RequireComponent(typeof(tk2dTextMesh))]
public class TextPrinter : MonoBehaviour
{
	private tk2dTextMesh label;

	public bool print_on_enable = true;

	public float delay;

	public float duration = 0.5f;

	private string complete_text;

	private int line_count;

	private void OnEnable()
	{
		if (label == null)
		{
			label = GetComponent<tk2dTextMesh>();
		}
		if (print_on_enable)
		{
			Print();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void Print()
	{
		Stop();
		complete_text = label.FormattedText;
		line_count = label.LineCount;
		label.LineCount = 0;
		label.text = string.Empty;
		label.GetComponent<Renderer>().enabled = false;
		StartCoroutine(Run());
	}

	public void Stop()
	{
		StopAllCoroutines();
	}

	private IEnumerator Run()
	{
		float time2 = Time.realtimeSinceStartup + delay;
		while (Time.realtimeSinceStartup < time2)
		{
			yield return null;
		}
		time2 = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < time2 + duration)
		{
			float value = (Time.realtimeSinceStartup - time2) / duration;
			int index2 = Mathf.FloorToInt(value * (float)complete_text.Length - 1f);
			index2 = Mathf.Clamp(index2, 0, complete_text.Length - 1);
			if (index2 > label.text.Length)
			{
				label.text = complete_text.Substring(0, index2);
				label.ForceBuild();
				label.GetComponent<Renderer>().enabled = true;
			}
			yield return null;
		}
		label.text = complete_text;
		label.LineCount = line_count;
	}
}
