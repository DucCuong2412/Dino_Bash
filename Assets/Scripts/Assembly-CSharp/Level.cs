using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class Level : MonoBase
{
	public enum State
	{
		invalid = 0,
		intro = 1,
		playing = 2,
		tutorial = 3,
		abort = 4,
		won = 5,
		lost = 6
	}

	private const float endlessScorStepSize = 50f;

	private GoTween introSequence;

	private bool introSequenceComplete;

	private HashSet<UnitType> unkown_enemies;

	private HashSet<UnitType> discovered_enemies;

	private float coin_multiplier = 1f;

	private float xp_multiplier = 1f;

	private List<Unit> dinosAtLevelEnd = new List<Unit>();

	private int rush_unit_count;

	private float next_rush_wave = -1f;

	private bool waitUntilAllNeandersDied;

	private float difficultyModifier = 1f;

	private int waveCount = 1;

	private int collectableItemIndex = -1;

	private UnitType assignCollectable;

	public static Level Instance { get; private set; }

	public bool tutorialMode { get; private set; }

	public LevelData Config { get; private set; }

	public State state { get; private set; }

	public float playTime { get; private set; }

	public int totalEnemies
	{
		get
		{
			List<LevelEnemy> list = Config.enemies.FindAll((LevelEnemy item) => Konfiguration.isNeander(item.unittype));
			List<LevelEnemy> list2 = list.FindAll((LevelEnemy item) => item.unittype == UnitType.Neander_Bush);
			int toSpawn = 0;
			list2.ForEach(delegate(LevelEnemy bush)
			{
				toSpawn += EntityFactory.StringToUnittype(bush.command).Count;
			});
			return list.Count + toSpawn;
		}
	}

	private float replayFactor
	{
		get
		{
			return Konfiguration.GameConfig.ReplayFactor;
		}
	}

	private float winlossFactor
	{
		get
		{
			return Konfiguration.GameConfig.Win_loss_influce;
		}
	}

	private int maxPlayCountInflunce
	{
		get
		{
			return Konfiguration.GameConfig.Win_loss_maxCount;
		}
	}

	private int replay_spread
	{
		get
		{
			return Konfiguration.GameConfig.Level_grind_spread;
		}
	}

	private float grind_factor
	{
		get
		{
			return Konfiguration.GameConfig.Level_grind_factor;
		}
	}

	public static int xp_recieved { get; private set; }

	public static int coins_recieved { get; private set; }

	public int levelid { get; private set; }

	public List<ShotType> AvailableShots { get; private set; }

	public List<UnitType> AvailableDinos { get; private set; }

	public Dictionary<ShotType, int> ShotLevels { get; private set; }

	public Dictionary<UnitType, int> UnitLevels { get; private set; }

	public int CurrentEnemyIndex
	{
		get
		{
			return enemyIndex % Config.enemies.Count;
		}
	}

	public int enemyIndex { get; private set; }

	private float level_cutoff
	{
		get
		{
			return Config.var_cuttoff.Value;
		}
	}

	public int EndlessScore
	{
		get
		{
			if (Instance.Config.endless_mode)
			{
				return calculateEndlessScore(playTime);
			}
			return 0;
		}
	}

	public int KillCoins
	{
		get
		{
			return Mathf.RoundToInt((float)Config.Kill_coins / (float)totalEnemies);
		}
	}

	public int KillXP
	{
		get
		{
			return Mathf.RoundToInt((float)Config.kill_xp / (float)totalEnemies);
		}
	}

	public int usedConsumableCount { get; set; }

	public event Action OnLevelStart;

	public event Action OnLevelPlay;

	public event Action OnLevelAbort;

	public event Action OnLevelLost;

	public event Action OnLevelWon;

	public event Action<string> OnLevelMessage;

	private void Awake()
	{
		Instance = this;
	}

	private void SetState(State targetState)
	{
		Debug.Log("LEVEL: " + targetState);
		state = targetState;
	}

	private void stateIntro()
	{
		introSequence = GameCamera.Instance.PlayIntroPan(Config.levelwidth);
		introSequence.setOnCompleteHandler(delegate
		{
			statePlaying();
		});
		SetState(State.intro);
	}

	private void statePlaying()
	{
		switch (state)
		{
		case State.intro:
			if (this.OnLevelPlay != null)
			{
				this.OnLevelPlay();
			}
			break;
		default:
			Debug.LogError("state change not allowed!");
			return;
		case State.tutorial:
			break;
		}
		playTime = 0f;
		StartCoroutine(PlayLevel());
		SetState(State.playing);
	}

	private void stateTutorial()
	{
		Tracking.finish_intro_cutscene_tutorial();
		if (this.OnLevelPlay != null)
		{
			this.OnLevelPlay();
		}
		SetState(State.tutorial);
	}

	public void stateWon()
	{
		State state = this.state;
		if (state == State.playing || state == State.tutorial)
		{
			if (this.OnLevelWon != null)
			{
				this.OnLevelWon();
			}
			if (Player.MaxLevelID == levelid)
			{
				Player.WinCount++;
				QuestManager.instance.ReportProgress(QuestObjective.Win_Level, 1);
			}
			QuestManager.instance.ReportProgress(QuestObjective.Play_Level, 1);
			Player.LooseCount = 0;
			GiveXPandCoins(true);
			foreach (UnitType discovered_enemy in discovered_enemies)
			{
				Player.DiscoveredNeanders.Add(discovered_enemy);
			}
			Player.CompletedLevel(levelid);
			SetState(State.won);
			SetState(State.won);
		}
		else
		{
			Debug.LogError("state change not allowed!");
		}
	}

	public void stateLost()
	{
		State state = this.state;
		if (state == State.playing || state == State.tutorial)
		{
			if (!GameCamera.Instance.isInView(EntityFactory.Dino_Egg.transform))
			{
				Go.to(GameCamera.Instance.transform, 1f, new GoTweenConfig().position(GameCamera.Instance.transform.position.SetX((0f - GameCamera.Instance.screen_width) * 0.5f)).setEaseType(GoEaseType.QuintInOut));
			}
			StopAllCoroutines();
			coins_recieved = 0;
			xp_recieved = 0;
			int looseCount = Player.LooseCount;
			if (Config.endless_mode)
			{
				QuestManager.instance.ReportProgress(QuestObjective.Play_Survival, 1);
				Player.RecordEndlessLevel(levelid, playTime);
				UpdateLeaderboard();
			}
			else
			{
				Player.LooseCount++;
				GiveXPandCoins(false);
			}
			Debug.Log("cleared: " + getProgess());
			Tracking.level(levelid, Tracking.LevelAction.loose, looseCount, getProgess(), coins_recieved, xp_recieved);
			if (Player.MaxLevelID == levelid)
			{
				Player.WinCount = 0;
			}
			QuestManager.instance.ReportProgress(QuestObjective.Play_Level, 1);
			if (Player.Lives == 4)
			{
				Player.Instance.PlayerData.timeOfLostLive = DateTime.UtcNow;
			}
			if (this.OnLevelLost != null)
			{
				this.OnLevelLost();
			}
			SetState(State.lost);
			SetState(State.lost);
		}
		else
		{
			Debug.LogError("state change not allowed!");
		}
	}

	private void UpdateLeaderboard()
	{
		//Discarded unreachable code: IL_0091
		string leaderboard_id = string.Empty;
		switch (Konfiguration.ChapterForLevel(levelid))
		{
		case 0:
			leaderboard_id = LeaderboardIds._1ST_SURVIVAL_LEVEL;
			break;
		case 1:
			leaderboard_id = LeaderboardIds._2ND_SURVIVAL_LEVEL;
			break;
		case 2:
			leaderboard_id = LeaderboardIds._3RD_SURVIVAL_LEVEL;
			break;
		case 3:
			leaderboard_id = LeaderboardIds._4TH_SURVIVAL_LEVEL;
			break;
		case 4:
			leaderboard_id = LeaderboardIds._5TH_SURVIVAL_LEVEL;
			break;
		}
		long num = -1L;
		try
		{
			num = Convert.ToInt64(Player.getBestEndlessLevelScore(levelid));
		}
		catch (OverflowException)
		{
			Debug.LogError("outside the range of the Int64 type.");
			return;
		}
		SocialGamingManager.Instance.ReportScore(leaderboard_id, num);
	}

	public float getProgess()
	{
		return (float)enemyIndex / (float)Config.enemies.Count;
	}

	private void GiveXPandCoins(bool game_won)
	{
		float value = ((!game_won) ? (winlossFactor * (float)(Player.LooseCount - 1)) : ((0f - winlossFactor) * (float)(Player.WinCount - 1)));
		value = Mathf.Clamp(value, (float)(-maxPlayCountInflunce) * winlossFactor, (float)maxPlayCountInflunce * winlossFactor);
		float t = 1f / (float)replay_spread * (float)(replay_spread - Mathf.Clamp(Player.MaxLevelID - levelid, 0, replay_spread));
		float num = Mathf.Lerp(replayFactor, 1f + value, t);
		if (Player.MaxLevelID != levelid)
		{
			num *= grind_factor;
		}
		num = Mathf.Max(num, replayFactor);
		Debug.Log(string.Format("coin & xp modification: {0}", num));
		if (game_won)
		{
			Player.Instance.LevelXP = Mathf.RoundToInt((float)(Config.kill_xp + Config.level_xp) * num);
			Player.Instance.LevelCoins = Mathf.RoundToInt((float)(Config.Kill_coins + Config.Level_coins) * num);
		}
		else
		{
			Player.Instance.LevelXP = 0;
			Player.Instance.LevelCoins = Mathf.RoundToInt((float)Player.Instance.LevelCoins * num);
		}
		Player.Instance.LevelXP = Mathf.Min(Player.Instance.LevelXP, Config.kill_xp + Config.level_xp);
		Player.Instance.LevelCoins = Mathf.Min(Player.Instance.LevelCoins, Config.Kill_coins + Config.Level_coins);
		Player.Instance.LevelXP = Mathf.RoundToInt((float)Player.Instance.LevelXP * xp_multiplier);
		Player.Instance.LevelCoins = Mathf.RoundToInt((float)Player.Instance.LevelCoins * coin_multiplier);
		Player.XP += Player.Instance.LevelXP;
		Wallet.GiveCoins(Player.Instance.LevelCoins);
		QuestManager.instance.ReportProgress(QuestObjective.Collect_Coins, Player.Instance.LevelCoins);
		xp_recieved = Player.Instance.LevelXP;
		coins_recieved = Player.Instance.LevelCoins;
	}

	public void stateAbort()
	{
		if (this.OnLevelAbort != null)
		{
			this.OnLevelAbort();
		}
		Tracking.level(levelid, Tracking.LevelAction.quit, Player.LooseCount, getProgess(), 0, 0);
		Debug.Log("Abort Level!");
		SetState(State.abort);
		App.stateMap();
	}

	public void EndTutorial(State pTargetState)
	{
		if (state == State.tutorial)
		{
			switch (pTargetState)
			{
			case State.intro:
				stateIntro();
				break;
			case State.playing:
				statePlaying();
				break;
			case State.won:
				stateWon();
				break;
			case State.lost:
				stateLost();
				break;
			default:
				Debug.LogError("state change not allowed!");
				break;
			}
		}
	}

	private void Update()
	{
		switch (state)
		{
		case State.intro:
			if ((Input.GetMouseButtonUp(0) || Input.touchCount > 0) && introSequence != null)
			{
				introSequence.complete();
			}
			break;
		case State.playing:
			playTime += Time.deltaTime;
			dinosAtLevelEnd.RemoveAll((Unit x) => x.state == Unit.State.die || x.state == Unit.State.disabled);
			break;
		}
	}

	public void SetUpGame(int level)
	{
		levelid = level;
		SetSelectionOverrides();
		SetDinoShotLevelOverrides();
	}

	private void SetSelectionOverrides()
	{
		if (Konfiguration.levels[levelid].override_shots_selection)
		{
			AvailableShots = Konfiguration.levels[levelid].getShotOverrides;
		}
		else
		{
			AvailableShots = Player.SelectedShotTypes;
		}
		if (Konfiguration.levels[levelid].override_dino_selection)
		{
			AvailableDinos = Konfiguration.levels[levelid].getDinoOverrides;
		}
		else
		{
			AvailableDinos = Player.SelectedUnitTypes;
		}
	}

	private void SetDinoShotLevelOverrides()
	{
		ShotLevels = new Dictionary<ShotType, int>();
		ShotLevels.Add(ShotType.Meteor, Player.GetShotLevel(ShotType.Meteor));
		AvailableShots.ForEach(delegate(ShotType shot)
		{
			if (!ShotLevels.ContainsKey(shot))
			{
				ShotLevels.Add(shot, Player.GetShotLevel(shot));
			}
		});
		if (Konfiguration.levels[levelid].override_shots_selection)
		{
			AvailableShots.ForEach(delegate(ShotType shot)
			{
				int overrideLevel2 = Konfiguration.levels[levelid].getOverrideLevel(shot);
				if (overrideLevel2 != -1)
				{
					ShotLevels[shot] = overrideLevel2;
				}
			});
		}
		UnitLevels = new Dictionary<UnitType, int>();
		AvailableDinos.ForEach(delegate(UnitType unit)
		{
			if (!UnitLevels.ContainsKey(unit))
			{
				UnitLevels.Add(unit, Player.GetUnitLevel(unit));
			}
		});
		UnitLevels.Add(UnitType.MegaBall, 0);
		if (!Konfiguration.levels[levelid].override_dino_selection)
		{
			return;
		}
		AvailableDinos.ForEach(delegate(UnitType dino)
		{
			int overrideLevel = Konfiguration.levels[levelid].getOverrideLevel(ShotType.None, dino);
			if (overrideLevel != -1)
			{
				UnitLevels[dino] = overrideLevel;
			}
		});
	}

	public void Play()
	{
		Player.CurrentLevelID = levelid;
		Player.Instance.LevelStart();
		usedConsumableCount = 0;
		Config = Konfiguration.levels[levelid];
		float num = Config.levelwidth;
		base.gameObject.name = string.Format("Level: {0} ID: {1}", Config.name, levelid);
		string text = Konfiguration.GetChapterData(levelid).theme.ToString();
		string text2 = text;
		if (Screen.width <= 960)
		{
			text2 += "_low";
		}
		UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Level/" + text2), new Vector3(0f, 0f, 256f), Quaternion.identity);
		GameObject gameObject = new GameObject("Dino_Base");
		gameObject.tag = "DinoBase";
		GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Level/NeanderBase_" + text)) as GameObject;
		gameObject2.transform.PosX(num);
		gameObject2.tag = "NeanderBase";
		gameObject2.name = "Neander_Base";
		BoxCollider boxCollider = new GameObject("GroundCollision").AddComponent<BoxCollider>();
		boxCollider.gameObject.layer = 8;
		boxCollider.gameObject.tag = "Ground";
		boxCollider.material = Resources.Load<PhysicMaterial>("Physics/Ground");
		boxCollider.size = new Vector3(num + Mathf.Abs(Konfiguration.GameConfig.levelstart), 512f, 512f);
		boxCollider.center = new Vector3(boxCollider.size.x * 0.5f, -128f, 0f);
		boxCollider.transform.position = new Vector3(Konfiguration.GameConfig.levelstart, -50f, 0f);
		UnityEngine.Object.Instantiate(Resources.Load("FX/FX_CoinCollect"));
		unkown_enemies = Konfiguration.GetUnknownNeanders();
		discovered_enemies = new HashSet<UnitType>();
		EntityFactory.Init();
		ShotFactory.Setup();
		CollectableItem.Setup();
		Debug.Log("start level: " + base.gameObject.name);
		if (this.OnLevelStart != null)
		{
			this.OnLevelStart();
		}
		SetupUpgrades();
		if (Tutorials.isTutorialLevel(levelid))
		{
			Tutorials.LoadAndPush(levelid, ScreenManager.GetScreen<HudScreen>());
			if (levelid > 0)
			{
				stateTutorial();
			}
			else if (levelid == 0)
			{
				CutSceneScreen.PlayCutscene("GUI/Intro", stateTutorial);
				Tracking.start_intro_cutscene_tutorial();
			}
		}
		else
		{
			stateIntro();
		}
	}

	private void SetupUpgrades()
	{
		List<UnitType> list = Player.ActiveUpgrades.FindAll((UnitType x) => Konfiguration.getPreviousUpgradeStage(x) != UnitType.None);
		List<UnitType> list2 = new List<UnitType>();
		foreach (UnitType item in list)
		{
			list2.Add(Konfiguration.getPreviousUpgradeStage(item));
		}
		foreach (UnitType activeUpgrade in Player.ActiveUpgrades)
		{
			if (!list2.Contains(activeUpgrade))
			{
				switch (activeUpgrade)
				{
				case UnitType.AppleStartBonus:
					Player.Instance.Apples += Konfiguration.GameConfig.AppleBoostAmount;
					break;
				case UnitType.CoinDoubler:
					coin_multiplier = 2f;
					break;
				case UnitType.XpDoubler:
					xp_multiplier = 2f;
					break;
				default:
					EntityFactory.Create(activeUpgrade);
					break;
				}
			}
		}
	}

	public void UnitReachedLevelEnd(Unit unit)
	{
		if (Konfiguration.isDinoUnit(unit.unitType))
		{
			dinosAtLevelEnd.Add(unit);
		}
	}

	private IEnumerator PlayLevel()
	{
		enemyIndex = 0;
		while (Config.endless_mode || enemyIndex < Config.enemies.Count)
		{
			while (waitUntilAllNeandersDied)
			{
				List<BaseEntity> neanders = EntityFactory.GetEntities(false);
				waitUntilAllNeandersDied = neanders.Count > 0;
				yield return null;
			}
			LevelEnemy enemy = Config.enemies[CurrentEnemyIndex];
			float delay = enemy.delay;
			if (dinosAtLevelEnd.Count > 0 && !Config.endless_mode)
			{
				if (Time.time > next_rush_wave)
				{
					rush_unit_count++;
					delay = 0.5f;
					if (rush_unit_count >= Konfiguration.GameConfig.NeanderRushUnits)
					{
						rush_unit_count = 0;
						next_rush_wave = Time.time + Konfiguration.GameConfig.NeanderRushInterval;
					}
				}
				else
				{
					delay = next_rush_wave - Time.time;
				}
			}
			else
			{
				rush_unit_count = 0;
				next_rush_wave = -1f;
			}
			yield return new WaitForSeconds(delay * difficultyModifier);
			SpawnEnemy(enemy);
			if (enemyIndex == 0)
			{
				AudioPlayer.PlayGuiSFX(Sounds.game_start_horn, 0f);
			}
			enemyIndex++;
			if (Config.endless_mode)
			{
				difficultyModifier -= 1f / (float)Config.enemies.Count / 5f;
				difficultyModifier = Mathf.Clamp(difficultyModifier, 0.1f, 1f);
			}
			if (level_cutoff < 1f && (float)enemyIndex / (float)Config.enemies.Count > level_cutoff)
			{
				break;
			}
		}
		while (EntityFactory.GetEntities(false).FindIndex((BaseEntity x) => Konfiguration.isNeander(x.unitType)) != -1)
		{
			yield return new WaitForSeconds(0.2f);
		}
		yield return new WaitForSeconds(0.5f);
		stateWon();
	}

	public void SpawnEnemy(LevelEnemy enemy)
	{
		if (Konfiguration.isCollectable(enemy.unittype))
		{
			collectableItemIndex++;
			if (Player.MaxLevelID == levelid && !Player.CollectableDropProgress[collectableItemIndex])
			{
				assignCollectable = enemy.unittype;
			}
			return;
		}
		switch (enemy.unittype)
		{
		case UnitType.Message:
		{
			string text = enemy.command.Localize();
			if (enemy.command == "EndlessLevel" && enemyIndex > 0)
			{
				return;
			}
			if (enemy.command == "EndlessModeWave")
			{
				waveCount++;
				text = string.Format(text, waveCount);
			}
			if (enemy.command == "RaptorComes")
			{
				if (Player.AvailableUnitTypes.Contains(UnitType.Raptor))
				{
					return;
				}
			}
			else if (enemy.command == "SpittyComes")
			{
				if (Player.AvailableUnitTypes.Contains(UnitType.Spitty))
				{
					return;
				}
			}
			else if (enemy.command == "TricerComes")
			{
				if (Player.AvailableUnitTypes.Contains(UnitType.Tricer))
				{
					return;
				}
			}
			else if (enemy.command == "StegoComes")
			{
				if (Player.AvailableUnitTypes.Contains(UnitType.Stego))
				{
					return;
				}
			}
			else if (enemy.command == "BrachioComes" && Player.AvailableUnitTypes.Contains(UnitType.Brachio))
			{
				return;
			}
			if (this.OnLevelMessage != null)
			{
				this.OnLevelMessage(text);
			}
			if (enemy.command == "FinalWave")
			{
				AudioPlayer.PlayGuiSFX(Sounds.game_finalwave_horn, 0f);
			}
			return;
		}
		case UnitType.Wait:
			waitUntilAllNeandersDied = ((!Config.endless_mode) ? true : false);
			return;
		}
		GameObject gameObject = EntityFactory.Create(enemy.unittype);
		if (gameObject == null)
		{
			Debug.LogError("Entitiy is null!");
			return;
		}
		BaseEntity component = gameObject.GetComponent<BaseEntity>();
		EntityData config = component.Config;
		config.level = enemy.unit_level;
		config.command = enemy.command;
		component.Config = config;
		if (component is Unit)
		{
			Unit unit = component as Unit;
			unit.setDropCollectable(assignCollectable, collectableItemIndex);
			assignCollectable = UnitType.None;
			if (unit.Config.isFriendly)
			{
				float num = Camera.main.transform.position.x - unit.RenderBounds.extents.x;
				float num2 = float.PositiveInfinity;
				List<BaseEntity> entities = EntityFactory.GetEntities(false);
				if (entities.Count > 0)
				{
					entities.Sort((BaseEntity x, BaseEntity y) => x.transform.position.x.CompareTo(y.transform.position.x));
					num2 = entities[0].transform.position.x;
				}
				unit.transform.PosX((!(num2 < num)) ? num : (num2 - unit.RenderBounds.extents.x * 1.2f));
			}
			else if (Config.endless_mode)
			{
				float num3 = playTime / 60f;
				float num4 = num3 * num3 * Config.EndlessScale + 1f;
				unit.SetMaxHealth(num4);
				unit.CombatBehaviour.AttackModifier = num4;
				Debug.Log(string.Format("Endless Mode Stats - spawn_scale: {0} - strenght scale {1}", difficultyModifier, num4));
			}
		}
		if (unkown_enemies.Contains(enemy.unittype))
		{
			unkown_enemies.Remove(enemy.unittype);
			discovered_enemies.Add(enemy.unittype);
			ScreenManager.GetScreen<UnitTutorialScreen>().Show(gameObject.GetComponent<BaseEntity>());
		}
	}

	public static int calculateEndlessScore(float time)
	{
		int num = Mathf.FloorToInt(time / 60f * 500f);
		return Mathf.RoundToInt(Mathf.Floor((float)num / 50f) * 50f);
	}

	public static float oldScoreToTime(float old_time)
	{
		int num = Mathf.RoundToInt(Mathf.Floor(old_time / 50f) * 50f);
		return (float)num / 500f * 60f;
	}

	public bool inLevelBounds(float pos_x)
	{
		return Konfiguration.GameConfig.levelstart - 256f < pos_x && pos_x < (float)Instance.Config.levelwidth + 256f;
	}
}
