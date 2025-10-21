using System.Collections.Generic;
using UnityEngine;

public class PrefabHelper
{
	private class Config
	{
		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;

		public Sprite sprite;

		public Color spriteColor;
	}

	private const string kCloneName = "(Clone)";

	private static Dictionary<string, Dictionary<string, Config>> objectLookup = new Dictionary<string, Dictionary<string, Config>>();

	public static void RegisterPrefab<T>(T prefab) where T : Component
	{
		if (objectLookup.ContainsKey(prefab.gameObject.name + "(Clone)"))
		{
			return;
		}
		Transform[] componentsInChildren = prefab.GetComponentsInChildren<Transform>();
		Dictionary<string, Config> dictionary = new Dictionary<string, Config>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Config config = new Config();
			config.position = prefab.transform.position;
			config.rotation = prefab.transform.rotation;
			config.scale = prefab.transform.localScale;
			SpriteRenderer component = prefab.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				config.sprite = component.sprite;
				config.spriteColor = component.color;
			}
			dictionary.Add(componentsInChildren[i].name, config);
		}
		objectLookup.Add(prefab.gameObject.name + "(Clone)", dictionary);
	}

	public static void RevertPrefab<T>(T prefab) where T : Component
	{
		string name = prefab.gameObject.name;
		if (!objectLookup.ContainsKey(name))
		{
			return;
		}
		Transform[] componentsInChildren = prefab.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (objectLookup[name].ContainsKey(componentsInChildren[i].name))
			{
				Config config = objectLookup[name][componentsInChildren[i].name];
				componentsInChildren[i].transform.position = config.position;
				componentsInChildren[i].transform.rotation = config.rotation;
				componentsInChildren[i].transform.localScale = config.scale;
				SpriteRenderer component = componentsInChildren[i].GetComponent<SpriteRenderer>();
				if (component != null)
				{
					component.sprite = config.sprite;
					component.color = config.spriteColor;
				}
			}
		}
	}
}
