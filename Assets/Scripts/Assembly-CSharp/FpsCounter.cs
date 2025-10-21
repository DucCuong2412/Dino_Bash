using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	private string label = string.Empty;

	private Color darkRed = new Color(0.2f, 0f, 0f, 1f);

	private int frameCount;

	private float dt;

	private float fps;

	private float updateRate = 2f;

	private void Start()
	{
		GUI.depth = 2;
	}

	private void Update()
	{
		frameCount++;
		dt += Time.deltaTime;
		if (dt > 1f / updateRate)
		{
			fps = (float)frameCount / dt;
			frameCount = 0;
			dt -= 1f / updateRate;
		}
		label = "FPS :" + Mathf.Round(fps);
	}

	private void OnGUI()
	{
		GUI.color = darkRed;
		GUI.Label(new Rect(6f, 6f, 100f, 25f), label);
		GUI.color = Color.red;
		GUI.Label(new Rect(5f, 5f, 100f, 25f), label);
	}
}
