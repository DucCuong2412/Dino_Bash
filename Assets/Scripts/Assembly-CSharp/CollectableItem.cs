using System;
using UnityEngine;
using dinobash;

public class CollectableItem : tk2dUIBaseItemControl
{
	private static Camera sceneCamera;

	private static Camera guiCamera;

	private bool touched;

	private CollectableUnit collectable;

	private float lifeTime;

	private Renderer target;

	private Action onCollect;

	[SerializeField]
	private SpriteRenderer sprite;

	private void Start()
	{
		if (sceneCamera == null || guiCamera == null)
		{
			sceneCamera = Camera.main;
			guiCamera = ScreenManager.Camera;
		}
	}

	private void OnEnable()
	{
		if (collectable == null)
		{
			uiItem = base.gameObject.AddComponent<tk2dUIItem>();
			collectable = GetComponent<CollectableUnit>();
		}
		sprite.gameObject.layer = 0;
		base.transform.localScale = Vector3.one;
		base.GetComponent<Collider>().enabled = true;
		touched = false;
		//lifeTime = Time.time + Konfiguration.GameConfig.AppleCollectTimer;
		uiItem.OnClick += OnClicked;
		Level.Instance.OnLevelWon += OnDisable;
		Level.Instance.OnLevelLost += OnDisable;
	}

	private void OnDisable()
	{
		uiItem.OnClick -= OnClicked;
		Level.Instance.OnLevelWon -= OnDisable;
		Level.Instance.OnLevelLost -= OnDisable;
	}

	protected void Update()
	{
		if (Time.time > lifeTime && collectable.GetComponent<Collider>().enabled)
		{
			collectable.stateDie();
		}
	}

	private void OnClicked()
	{
		if (touched)
		{
			return;
		}
		touched = true;
		if (collectable.unitType == UnitType.CollectableApple)
		{
			AudioPlayer.PlayGuiSFX(Sounds.game_apple_collect, 0f);
			target = MonoSingleton<ApplesLabel>.Instance.transform.parent.GetComponent<Renderer>();
			onCollect = delegate
			{
				//Player.Instance.Apples += Konfiguration.GameConfig.AppleCollectReward;
			};
		}
		if (collectable.unitType == UnitType.CollectableBlizzard)
		{
			target = ScreenManager.GetScreen<HudScreen>().ConsumableButtons.Find((BuyUnitButton x) => x.Unit == UnitType.Blizzard).GetComponent<Renderer>();
			onCollect = delegate
			{
				Player.changeConsumableCount(UnitType.Blizzard, 1);
			};
		}
		if (collectable.unitType == UnitType.CollectableMeteorStorm)
		{
			target = ScreenManager.GetScreen<HudScreen>().ConsumableButtons.Find((BuyUnitButton x) => x.Unit == UnitType.MeteorStorm).GetComponent<Renderer>();
			onCollect = delegate
			{
				Player.changeConsumableCount(UnitType.MeteorStorm, 1);
			};
		}
		if (collectable.unitType == UnitType.CollectableMegaBall)
		{
			target = ScreenManager.GetScreen<HudScreen>().ConsumableButtons.Find((BuyUnitButton x) => x.Unit == UnitType.MegaBall).GetComponent<Renderer>();
			onCollect = delegate
			{
				Player.changeConsumableCount(UnitType.MegaBall, 1);
			};
		}
		if (collectable.unitType != UnitType.CollectableApple)
		{
			Tracking.pickup_consumable(collectable.unitType.ToString(), Level.Instance.levelid);
		}
		base.GetComponent<Collider>().enabled = false;
		Vector3 position = sceneCamera.WorldToScreenPoint(base.transform.position);
		base.transform.position = guiCamera.ScreenToWorldPoint(position).SetZ(10f);
		sprite.sortingLayerID = target.sortingLayerID;
		sprite.sortingOrder = target.sortingOrder - 1;
		sprite.gameObject.layer = 12;
		Go.to(base.transform, 1f, new GoTweenConfig().position(target.transform.position).scale(Vector3.one * 0.8f).setEaseType(GoEaseType.CubicInOut)
			.onComplete(delegate
			{
				EntityFactory.Free(collectable);
				onCollect();
			}));
	}

	public static void Setup()
	{
		sceneCamera = null;
		guiCamera = null;
	}

	public static void Create(UnitType collectableItem, Vector3 pPosition)
	{
		GameObject gameObject = EntityFactory.Create(collectableItem);
		gameObject.transform.position = pPosition;
		gameObject.layer = 15;
		gameObject.tag = "CollectableApple";
		gameObject.SetActive(true);
		PlaySpawnSound(collectableItem);
	}

	private static void PlaySpawnSound(UnitType collectableItem)
	{
		if (collectableItem == UnitType.CollectableApple)
		{
			AudioPlayer.PlayGuiSFX(Sounds.game_apple_spawn, 0f);
		}
	}
}
