using UnityEngine;
using dinobash;

public class EntityShadow : MonoBase
{
	private int envcolor_index;

	public SpriteRenderer sprite { get; private set; }

	private Color shadow_color
	{
		get
		{
			return GameColors.EnvironmentShadow[envcolor_index];
		}
	}

	private void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		ChapterData chapter = Konfiguration.GetChapterData(Player.CurrentLevelID);
		int num = Konfiguration.chapters.FindIndex((ChapterData x) => x == chapter);
		if (num >= 0)
		{
			envcolor_index = num;
		}
		sprite.color = shadow_color;
	}

	public void Revert()
	{
		sprite.color = shadow_color;
	}
}
