using System.Collections;
using UnityEngine;

public class Fx_Flash : MonoBehaviour
{
	private SpriteRenderer lightning;

	private SpriteRenderer flash;

	private Color flash_color;

	private int min_wait = 12;

	private int max_wait = 30;

	private GoTweenConfig flash_config;

	private GoTweenConfig lightning_config;

	private void Start()
	{
		lightning = GetComponent<SpriteRenderer>();
		lightning.enabled = false;
		lightning_config = new GoTweenConfig().colorProp("color", Colors.Invisible).onComplete(delegate
		{
			lightning.enabled = false;
			lightning.color = Colors.Visible;
		});
		flash = base.transform.GetChild(0).GetComponent<SpriteRenderer>();
		flash.enabled = false;
		flash_color = flash.color;
		flash_config = new GoTweenConfig().setDelay(0.1f).colorProp("color", Colors.Invisible).onComplete(delegate
		{
			flash.enabled = false;
			flash.color = flash_color;
		});
		StartCoroutine(ThunderAndLightning());
	}

	private IEnumerator ThunderAndLightning()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(min_wait, max_wait));
			flash.enabled = true;
			lightning.enabled = true;
			Go.to(flash, 0.4f, flash_config);
			yield return new WaitForSeconds(0.5f);
			Go.to(lightning, 0.1f, lightning_config);
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
