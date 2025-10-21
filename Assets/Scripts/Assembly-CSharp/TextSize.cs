using System.Collections;
using UnityEngine;

public class TextSize
{
	private Hashtable dict;

	private TextMesh textMesh;

	private Renderer renderer;

	public float width
	{
		get
		{
			return getTextWidth(textMesh.text);
		}
	}

	public float height
	{
		get
		{
			return renderer.bounds.size.y;
		}
	}

	public TextSize(TextMesh tm)
	{
		textMesh = tm;
		renderer = tm.renderer;
		dict = new Hashtable();
		getSpace();
	}

	private void getSpace()
	{
		string text = textMesh.text;
		textMesh.text = "a";
		float x = renderer.bounds.size.x;
		textMesh.text = "a a";
		float num = renderer.bounds.size.x - 2f * x;
		MonoBehaviour.print("char< > " + num);
		dict.Add(' ', num);
		dict.Add('a', x);
		textMesh.text = text;
	}

	public float getTextWidth(string s)
	{
		char[] array = s.ToCharArray();
		float num = 0f;
		string text = textMesh.text;
		foreach (char c in array)
		{
			if (dict.ContainsKey(c))
			{
				num += (float)dict[c];
				continue;
			}
			textMesh.text = string.Empty + c;
			float x = renderer.bounds.size.x;
			dict.Add(c, x);
			num += x;
		}
		textMesh.text = text;
		return num;
	}

	public void FitToWidth(float wantedWidth)
	{
		if (!(width <= wantedWidth))
		{
			string text = textMesh.text;
			textMesh.text = string.Empty;
			string[] array = text.Split('\n');
			string[] array2 = array;
			foreach (string s in array2)
			{
				textMesh.text += wrapLine(s, wantedWidth);
				textMesh.text += "\n";
			}
		}
	}

	private string wrapLine(string s, float w)
	{
		if (w == 0f || s.Length <= 0)
		{
			return s;
		}
		char[] array = s.ToCharArray();
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = textMesh.text;
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (dict.ContainsKey(c))
			{
				num = (float)dict[c];
			}
			else
			{
				textMesh.text = string.Empty + c;
				num = renderer.bounds.size.x;
				dict.Add(c, num);
			}
			if (c == ' ' || i == array.Length - 1)
			{
				if (c != ' ')
				{
					text += c;
					num2 += num;
				}
				if (num3 + num2 < w)
				{
					num3 += num2;
					text2 += text;
				}
				else
				{
					num3 = num2;
					text2 += text.Replace(" ", "\n");
				}
				text = string.Empty;
				num2 = 0f;
			}
			text += c;
			num2 += num;
		}
		textMesh.text = text3;
		return text2;
	}
}
