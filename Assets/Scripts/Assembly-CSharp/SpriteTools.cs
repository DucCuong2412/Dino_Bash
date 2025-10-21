using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteTools : MonoBehaviour
{
	[Serializable]
	public class SpritePair
	{
		public Sprite sourceSprite;

		public Sprite targetSprite;

		public bool isValid()
		{
			return sourceSprite != null && targetSprite != null;
		}
	}

	private const string layer = "Layer_";

	[SerializeField]
	private bool optimizeSortingOrder;

	[SerializeField]
	private bool switchSprites = true;

	[SerializeField]
	private SpritePair[] spriteSwitchPairs;

	public static void SetSortingLayer(Component pRoot, string renderLayer)
	{
		Renderer[] allSpriteRenderes = GetAllSpriteRenderes(pRoot);
		for (int i = 0; i < allSpriteRenderes.Length; i++)
		{
			allSpriteRenderes[i].sortingLayerName = renderLayer;
		}
	}

	public static void SetSortingLayerID(Component pRoot, int renderLayerID)
	{
		Renderer[] componentsInChildren = pRoot.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingLayerName = "Layer_" + renderLayerID;
		}
	}

	public static void TargetSetSortingLayerID(Component target, int renderLayerID)
	{
		Renderer component = target.GetComponent<Renderer>();
		if (component != null)
		{
			component.sortingLayerName = "Layer_" + renderLayerID;
		}
	}

	public static Renderer[] GetAllSpriteRenderes(Component pRoot)
	{
		List<Renderer> list = new List<Renderer>();
		Renderer[] componentsInChildren = pRoot.GetComponentsInChildren<Renderer>(true);
		foreach (Renderer item in componentsInChildren)
		{
			list.Add(item);
		}
		return list.ToArray();
	}

	public static void SetSortingOrder(Component pRoot, int order)
	{
		Renderer[] allSpriteRenderes = GetAllSpriteRenderes(pRoot);
		for (int i = 0; i < allSpriteRenderes.Length; i++)
		{
			allSpriteRenderes[i].sortingOrder = order;
		}
	}

	public static void OffsetSortingOrder(Component pRoot, int pOffset)
	{
		Renderer[] allSpriteRenderes = GetAllSpriteRenderes(pRoot);
		for (int i = 0; i < allSpriteRenderes.Length; i++)
		{
			allSpriteRenderes[i].sortingOrder += pOffset;
		}
	}

	public static void OptimizeSortingOrder(Component root)
	{
		List<Renderer> list = GetAllSpriteRenderes(root).ToList();
		if (list.Count == 0)
		{
			return;
		}
		list.Sort((Renderer x, Renderer y) => x.sortingOrder.CompareTo(y.sortingOrder));
		int num = 0;
		int sortingOrder = list[0].sortingOrder;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].sortingOrder > sortingOrder)
			{
				sortingOrder = list[i].sortingOrder;
				num++;
			}
			list[i].sortingOrder = num;
		}
	}

	public static Dictionary<Renderer, int> GetSortingOrderAsDictionary(Component pRoot)
	{
		Dictionary<Renderer, int> dictionary = new Dictionary<Renderer, int>();
		Renderer[] allSpriteRenderes = GetAllSpriteRenderes(pRoot);
		for (int i = 0; i < allSpriteRenderes.Length; i++)
		{
			dictionary.Add(allSpriteRenderes[i], allSpriteRenderes[i].sortingOrder);
		}
		return dictionary;
	}

	public static void SetSortingOrderFromDictionary(Dictionary<Renderer, int> pMapping)
	{
		foreach (KeyValuePair<Renderer, int> item in pMapping)
		{
			if (item.Key != null)
			{
				item.Key.sortingOrder = item.Value;
			}
		}
	}

	private static T[] GetComponentFrom<T>(Component pRoot) where T : Component
	{
		return pRoot.GetComponentsInChildren<T>();
	}

	public static void SetColor(Component pRoot, Color color)
	{
		SpriteRenderer[] componentFrom = GetComponentFrom<SpriteRenderer>(pRoot);
		for (int i = 0; i < componentFrom.Length; i++)
		{
			componentFrom[i].color = color;
		}
	}

	public static void FadeOut(Component pRoot, float time)
	{
		SpriteRenderer[] componentFrom = GetComponentFrom<SpriteRenderer>(pRoot);
		foreach (SpriteRenderer sprite in componentFrom)
		{
			if (!sprite.gameObject.activeInHierarchy)
			{
				continue;
			}
			Color color = sprite.color;
			color.a = 0f;
			Go.to(sprite, time, new GoTweenConfig().colorProp("color", color)).setOnUpdateHandler(delegate(AbstractGoTween t)
			{
				if (!sprite.gameObject.activeInHierarchy)
				{
					t.destroy();
				}
			});
		}
	}

	private void Start()
	{
		if (optimizeSortingOrder)
		{
			OptimizeSortingOrder(this);
		}
		if (!switchSprites || spriteSwitchPairs == null || spriteSwitchPairs.Length == 0)
		{
			return;
		}
		SpriteRenderer[] componentFrom = GetComponentFrom<SpriteRenderer>(this);
		foreach (SpriteRenderer spriteRenderer in componentFrom)
		{
			for (int j = 0; j < spriteSwitchPairs.Length; j++)
			{
				if (spriteSwitchPairs[j].isValid() && spriteRenderer.sprite == spriteSwitchPairs[j].sourceSprite)
				{
					spriteRenderer.sprite = spriteSwitchPairs[j].targetSprite;
					SpriteInitialSettings component = spriteRenderer.GetComponent<SpriteInitialSettings>();
					if (component != null)
					{
						component.Bind();
					}
				}
			}
		}
	}
}
