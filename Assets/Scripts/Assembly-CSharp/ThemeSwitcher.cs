using System.Collections.Generic;
using UnityEngine;

public class ThemeSwitcher : MonoBase
{
	public Transform map00;

	public Transform map01;

	public Transform map02;

	public Transform map03;

	public Transform map04;

	public GameObject SetTheme()
	{
		List<Transform> list = new List<Transform>(new Transform[6] { null, map00, map01, map02, map03, map04 });
		ChapterData chapterData = Konfiguration.GetChapterData(Level.Instance.levelid);
		Transform transform = list[(int)chapterData.theme];
		transform.transform.parent = base.transform.parent;
		transform.gameObject.name = base.gameObject.name;
		Object.Destroy(base.gameObject);
		return transform.gameObject;
	}
}
