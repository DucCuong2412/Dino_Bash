using System.Collections.Generic;
using UnityEngine;

public class SpriteRessources : MonoBehaviour
{
	private static SpriteRessources instance;

	[SerializeField]
	private List<string> spriteKeys = new List<string>();

	[SerializeField]
	private List<Sprite> spriteList = new List<Sprite>();

	private void Awake()
	{
		instance = this;
	}

	public static Sprite GetSprite(string name)
	{
		int num = instance.spriteKeys.IndexOf(name);
		if (num > -1)
		{
			return instance.spriteList[num];
		}
		Debug.LogWarning("no sprite found for: " + name);
		return null;
	}

	public void SetRessources(List<string> pNames, List<Sprite> pSprites)
	{
		spriteKeys = pNames;
		spriteList = pSprites;
	}

	public static Sprite getDinoBuySprite(UnitType entity)
	{
		if (Konfiguration.isDinoUnit(entity) || Konfiguration.isConsumable(entity))
		{
			return GetSprite("dinobuy_" + entity.ToString().ToLower());
		}
		return null;
	}

	public static Sprite getShotBuySprite(ShotType entity)
	{
		return GetSprite("shotbuy_" + entity.ToString().ToLower());
	}

	public static Sprite getUpgradeSprite(UnitType entity)
	{
		if (Konfiguration.isUpgrade(entity))
		{
			return GetSprite("upgrade_" + entity.ToString().ToLower());
		}
		Debug.LogError("not an upgrade...");
		return null;
	}

	public static Sprite getConsumableSprite(UnitType entity)
	{
		if (Konfiguration.isConsumable(entity))
		{
			return GetSprite("consumable_" + entity.ToString().ToLower());
		}
		Debug.LogError("not an upgrade...");
		return null;
	}

	public static Sprite getSpiteForUnitType(UnitType entity)
	{
		if (Konfiguration.isConsumable(entity))
		{
			return getConsumableSprite(entity);
		}
		if (Konfiguration.isUpgrade(entity))
		{
			return getUpgradeSprite(entity);
		}
		return getDinoBuySprite(entity);
	}
}
