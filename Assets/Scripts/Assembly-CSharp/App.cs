using System;
using UnityEngine;
using dinobash;

public class App : MonoBase
{
	public enum States
	{
		Invalid = 0,
		StartScreen = 1,
		Menu = 2,
		Map = 3,
		Game = 4
	}

	public static readonly string VERSION_CODE = "1.2.40";

	public CloudSaveGames cloudSaveGameManager;

	public FacebookManager facebookManager;

	public PaymentManager paymentManager;

	public static States State { get; private set; }

	public static States LastState { get; private set; }

	public static App Instance { get; private set; }

	public bool InternetConnectivity
	{
		get
		{
			return Application.internetReachability != NetworkReachability.NotReachable;
		}
	}

	public static event Action<States> OnStateChange;

	private static void SetState(States target_state)
	{
		LastState = State;
		State = target_state;
		if (App.OnStateChange != null)
		{
			App.OnStateChange(State);
		}
	}

	public static void stateStartScreen()
	{
		AudioPlayer.StopMusic();
		GameInput.ResetEvents();
		Action action = delegate
		{
			StartScreen startScreen = ScreenManager.LoadAndPush<StartScreen>();
			ScreenManager.LoadAndPush<StartInfoScreen>();
			ScreenManager.LoadAndPush<QuitGameScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<MoreGamesScreen>();
			ScreenManager.LoadAndPush<UpdateAppScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<NewCloudSaveAvailableScreen>();
			ScreenManager.LoadAndPush<ErrorMessageScreen>();
			ScreenManager.LoadAndPush<AchievementPopUp>();
			AudioPlayer.PlayMusic(Music.menu);
			startScreen.Show();
		};
		switch (State)
		{
		case States.StartScreen:
			return;
		case States.Invalid:
			action();
			break;
		default:
			Loader.Load(States.StartScreen.ToString(), action);
			break;
		}
		SetState(States.StartScreen);
	}

	public static void stateMap(bool show_upgrade_screen = false, bool force = false)
	{
		if (State == States.Map && !force)
		{
			return;
		}
		if (State == States.Game)
		{
			Player.Instance.LevelEnd();
		}
		GameInput.ResetEvents();
		AudioPlayer.StopMusic();
		QuestManager.instance.updateQuests();
		Loader.Load(States.Map.ToString(), delegate
		{
			AudioResources.UnLoadSoundClips(Sounds.INGAME, Sounds.INGAME_END);
			AudioResources.UnloadEntitiySounds();
			AudioResources.LoadSoundClips(Sounds.MAP, Sounds.MAP_END);
			ScreenManager.Camera.AddOrGetComponent<AudioListener>().enabled = true;
			MapScreen mapScreen = ScreenManager.LoadAndPush<MapScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<MapTutorial>("Tutorial/Tutorial");
			ScreenManager.LoadAndPush<SelectDinoScreen>("SelectDinoOrShotScreen");
			ScreenManager.LoadAndPush<SelectShotScreen>("SelectDinoOrShotScreen");
			UpgradeScreen upgradeScreen = ScreenManager.LoadAndPush<UpgradeScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<UpgradeInfoDinoScreen>("UpgradeInfoScreen");
			ScreenManager.LoadAndPush<UpgradeInfoShotScreen>("UpgradeInfoScreen");
			ScreenManager.LoadAndPush<UpgradeInfoUpgradesScreen>("UpgradeInfoScreen");
			ScreenManager.LoadAndPush<QuestScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<FriendGateScreen>();
			ScreenManager.LoadAndPush<BundlePromotionScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<ResourceBarScreen>().Show();
			ScreenManager.LoadAndPush<GetLivesScreen>();
			ScreenManager.LoadAndPush<ShopScreenCoins>("ShopScreen");
			ScreenManager.LoadAndPush<ShopScreenDiamonds>("ShopScreen");
			ScreenManager.LoadAndPush<FacebookLoginRequestScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<MessageCenterScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<SelectFriendsScreen>();
			ScreenManager.LoadAndPush<MoreLevelsScreen>();
			ScreenManager.LoadAndPush<RewardedFBLoginScreen>();
			ScreenManager.LoadAndPush<RewardedVideoScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<UpgradeReminderScreen>("Tutorial/UpgradeReminderScreen");
			ScreenManager.LoadAndPush<NewCloudSaveAvailableScreen>();
			ScreenManager.LoadAndPush<ErrorMessageScreen>();
			ScreenManager.LoadAndPush<RewardedVideoPopup>();
			ScreenManager.LoadAndPush<AchievementPopUp>();
			AudioPlayer.PlayMusic(Music.map);
			mapScreen.SetInputLockerScreens();
			if (show_upgrade_screen)
			{
				Tracking.upgrades_open();
				upgradeScreen.Show(UpgradeScreen.Tab.invalid, show_upgrade_screen);
			}
		});
		SetState(States.Map);
	}

	public static void stateGame(int level, bool skipLoader = false)
	{
		Player.Lives--;
		if (State == States.Game)
		{
			Player.Instance.LevelEnd();
		}
		if (Player.CurrentLevelID != level)
		{
			Player.LooseCount = 0;
		}
		Tracking.level(level, Tracking.LevelAction.start, Player.LooseCount, 0f, 0, 0);
		AudioPlayer.StopMusic();
		GameInput.ResetEvents();
		Action onLoadedCallback = delegate
		{
			Level level2 = new GameObject("Level").AddComponent<Level>();
			level2.SetUpGame(level);
			ScreenManager.LoadAndPush<HudScreen>();
			ScreenManager.LoadAndPush<UnitTutorialScreen>("Tutorial/Tutorial");
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<PauseScreen>();
			ScreenManager.LoadAndPush<QuitLevelRequestScreen>();
			ScreenManager.LoadAndPush<RefillConsumablesScreen>();
			ScreenManager.LoadAndPush<DinoRageScreen>();
			ScreenManager.LoadAndPush<ResultScreen>();
			ScreenManager.LoadAndPush<RateAppScreen>();
			ScreenManager.LoadAndPush<LevelupScreen>();
			ScreenManager.LoadAndPush<UnlockScreen>();
			ScreenManager.LoadAndPush<UpgradeInfoDinoScreen>("UpgradeInfoScreen");
			ScreenManager.LoadAndPush<QuestScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<ResourceBarScreen>();
			ScreenManager.LoadAndPush<GetLivesScreen>();
			ScreenManager.LoadAndPush<ShopScreenCoins>("ShopScreen");
			ScreenManager.LoadAndPush<ShopScreenDiamonds>("ShopScreen");
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<RewardedVideoScreen>();
			ScreenManager.LoadAndPush<CoverScreen>();
			ScreenManager.LoadAndPush<FacebookLoginRequestScreen>();
			ScreenManager.LoadAndPush<MessageCenterScreen>();
			ScreenManager.LoadAndPush<SelectFriendsScreen>();
			ScreenManager.LoadAndPush<ErrorMessageScreen>();
			ScreenManager.LoadAndPush<AchievementPopUp>();
			ScreenManager.LoadAndPush<RewardedVideoPopup>();
			ScreenManager.Camera.GetComponent<AudioListener>().enabled = false;
			UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Level/SceneCamera"));
			AudioResources.UnLoadSoundClips(Sounds.MAP, Sounds.MAP_END);
			AudioResources.LoadSoundClips(Sounds.INGAME, Sounds.INGAME_END);
			if (level > 0)
			{
				AudioPlayer.PlayMusic(AudioPlayer.GetTheme(level));
			}
			level2.Play();
			if (level == Player.MaxLevelID && !Player.HasPlayedMaxLevelID)
			{
				Tracking.level_002_start_tutorial();
				Tracking.level_003_start_tutorial();
				Player.HasPlayedMaxLevelID = true;
			}
		};
		Loader.Load(States.Game.ToString(), onLoadedCallback, skipLoader);
		SetState(States.Game);
	}

	private void Start()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Debug.Log("------------ APP START ------------");
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		facebookManager = ScriptableObject.CreateInstance<FacebookManager>();
		facebookManager.Init(delegate
		{
		});
		UnityEngine.Object.DontDestroyOnLoad(facebookManager);
		paymentManager = ScriptableObject.CreateInstance<PaymentManager>();
		paymentManager.Init();
		UnityEngine.Object.DontDestroyOnLoad(paymentManager);
		GameObject gameObject = new GameObject("Cloud Save Game Manager");
		cloudSaveGameManager = gameObject.AddComponent<CloudSaveGames>();
		ChartboostWrapper.Init();
		SocialGamingManager.Instance.Initialize();
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		Screen.sleepTimeout = -1;
		Debug.Log("maxTexSize: " + SystemInfo.maxTextureSize);
		Debug.Log("persistentDataPath: " + Application.persistentDataPath);
		Time.timeScale = 1f;
		Konfiguration.Init();
		i18n.Init();
		ShopItems.Init();
		Tutorials.Init();
		base.gameObject.AddComponent<Wallet>();
		Player.Create();
		Player.LoadPlayer();
		Wallet.Load();
		base.gameObject.AddComponent<TimeCheatDetector>();
		base.gameObject.AddComponent<GameInput>();
		base.gameObject.AddComponent<AudioPlayer>();
		base.gameObject.AddComponent<DebugInput>();
		base.gameObject.AddComponent<BundleOffer>();
		base.gameObject.AddComponent<ShopPromotions>();
		QuestManager.Initialize();
		ScreenManager.Initialize();
		UnityEngine.Object.Instantiate(Resources.Load("GUI/Loader"));
		ScreenManager.Camera.AddOrGetComponent<AudioListener>();
		AudioResources.LoadSoundClips(Sounds.GUI, Sounds.GUI_END);
		NotificationManager.Init();
		Try(delegate
		{
			NotificationManager.OnApplicationPause(false);
		});
		Tracking.Initialize();
		RewardedVideosWrapper.Init();
		LeanplumWrapper.Init();
		Debug.Log("App initialized");
		StartGame();
	}

	private void StartGame()
	{
		if (Player.MaxLevelID > Tutorials.LevelID("BasicShooting_Tutorial") /*|| !Konfiguration.GameConfig.Use_skip_intro*/)
		{
			stateStartScreen();
			return;
		}
		WaitThenRealtime(0.1f, delegate
		{
			Tracking.play_button_pressed_tutorial();
			stateGame(Player.MaxLevelID, true);
		});
	}

	private void OnApplicationQuit()
	{
		OnApplicationFocus(false);
	}

	private void Try(Action f)
	{
		try
		{
			f();
		}
		catch (Exception ex)
		{
			Debug.LogError("Error in OnApplicationFocus: " + ex.Message + ex.StackTrace);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		Try(delegate
		{
			NotificationManager.OnApplicationPause(!hasFocus);
		});
		if (hasFocus)
		{
			OnEnter();
		}
		else
		{
			OnPaused();
		}
	}

	private void OnEnter()
	{
		Debug.Log("Game got focus");
		Player.CheckForTimeManipulation();
		Try(delegate
		{
			WaitThen(2f, delegate
			{
				ShopScreenCoins screen = ScreenManager.GetScreen<ShopScreenCoins>();
				if (screen != null)
				{
					Debug.Log("shop_screen_coins: appenter");
					screen.OnAppEnter();
				}
				ShopScreenDiamonds screen2 = ScreenManager.GetScreen<ShopScreenDiamonds>();
				if (screen2 != null)
				{
					Debug.Log("shop_screen_diamonds: appenter");
					screen2.OnAppEnter();
				}
			});
		});
		RewardedVideosWrapper.Resume();
	}

	private void OnPaused()
	{
		Debug.Log("Game lost focus or quit");
		if (Player.Instance != null)
		{
			Try(delegate
			{
				Player.SavePlayer();
			});
		}
		RewardedVideosWrapper.Pause();
		PauseScreen screen = ScreenManager.GetScreen<PauseScreen>();
		RefillConsumablesScreen screen2 = ScreenManager.GetScreen<RefillConsumablesScreen>();
		QuitLevelRequestScreen screen3 = ScreenManager.GetScreen<QuitLevelRequestScreen>();
		DinoRageScreen screen4 = ScreenManager.GetScreen<DinoRageScreen>();
		if (State == States.Game && !Loader.instance.isVisible && (Level.Instance.state == Level.State.playing || Level.Instance.state == Level.State.tutorial) && screen3 != null && !screen3.isVisible && screen2 != null && !screen2.isVisible && screen4 != null && !screen4.isVisible && screen != null && !screen.isVisible && !screen.isTransitioning)
		{
			screen.Show(true);
		}
	}
}
