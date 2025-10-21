using System.Collections;
using UnityEngine;

public class FreeOrDestoryOnLevelBoundsExit : MonoBehaviour
{
	private const float lowerYBound = -512f;

	private const float upperYBound = 1300f;

	private float levelMin;

	private float levelMax;

	private void OnEnable()
	{
		levelMin = Konfiguration.GameConfig.levelstart - 1024f;
		levelMax = Level.Instance.Config.levelwidth + 1024;
		StartCoroutine(CheckIfInBounds());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator CheckIfInBounds()
	{
		while (InLevelBounds())
		{
			yield return new WaitForSeconds(0.5f);
		}
		Debug.Log("out of levelbounds: " + base.gameObject.name);
		ObjectPool.Recycle(base.transform);
	}

	private bool InLevelBounds()
	{
		return levelMin < base.transform.position.x && base.transform.position.x < levelMax && -512f < base.transform.position.y && base.transform.position.y < 1300f;
	}
}
