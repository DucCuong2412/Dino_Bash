using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LeanplumSDK;
using UnityEngine;

public class LevelData
{
	public readonly int levelwidth;

	public string name;

	public readonly int level_coins;

	[XmlIgnore]
	private Var<int> var_level_coins;

	public readonly int kill_coins;

	[XmlIgnore]
	private Var<int> var_kill_coins;

	public readonly int level_xp;

	public readonly int kill_xp;

	public readonly bool is_friend_gate;

	[XmlIgnore]
	public Var<float> var_cuttoff;

	[XmlIgnore]
	private Var<float> var_endlessScale;

	public readonly int friend_gate_duration_in_seconds;

	[XmlIgnore]
	private Var<int> var_friend_gate_duration_in_seconds;

	public readonly UnitType unlockUnit;

	public readonly ShotType unlockShot = ShotType.None;

	public readonly bool endless_mode;

	public readonly bool override_shots_selection;

	public readonly bool override_dino_selection;

	[XmlArrayItem("Override")]
	public readonly List<SelectionOverrideData> selection_override = new List<SelectionOverrideData>();

	public List<LevelEnemy> enemies;

	public string display_name
	{
		get
		{
			return name.Split("_".ToCharArray())[1].TrimStart('0');
		}
	}

	public int Level_coins
	{
		get
		{
			if (var_level_coins == null)
			{
				Debug.LogError("var_level_coins is null: " + name + " / " + display_name);
				return 0;
			}
			return var_level_coins.Value;
		}
	}

	public int Kill_coins
	{
		get
		{
			if (var_kill_coins == null)
			{
				Debug.LogError("var_level_coins is null");
				return 0;
			}
			return var_kill_coins.Value;
		}
	}

	public float EndlessScale
	{
		get
		{
			//Discarded unreachable code: IL_0013, IL_002e
			try
			{
				return var_endlessScale.Value;
			}
			catch (Exception)
			{
				Debug.LogError("Var_endlessScale is invalid! - using default 0.03f");
				return 0.03f;
			}
		}
	}

	public int FriendGateDurationInSeconds
	{
		get
		{
			if (!is_friend_gate)
			{
				return 0;
			}
			if (var_friend_gate_duration_in_seconds == null)
			{
				Debug.LogError("var_friend_gate_duration_in_seconds is null");
				return 0;
			}
			return var_friend_gate_duration_in_seconds.Value;
		}
	}

	public List<ShotType> getShotOverrides
	{
		get
		{
			List<ShotType> shots = new List<ShotType>();
			selection_override.ForEach(delegate(SelectionOverrideData x)
			{
				shots.Add(x.shot);
			});
			return shots;
		}
	}

	public List<UnitType> getDinoOverrides
	{
		get
		{
			List<UnitType> dinos = new List<UnitType>();
			selection_override.ForEach(delegate(SelectionOverrideData x)
			{
				dinos.Add(x.unit);
			});
			return dinos;
		}
	}

	private LevelData()
	{
	}

	public LevelData(LevelTheme pTheme, int pLevelwidth, List<LevelEnemy> pEnemies)
	{
		levelwidth = pLevelwidth;
		enemies = pEnemies;
	}

	public void init()
	{
		if (is_friend_gate)
		{
			var_friend_gate_duration_in_seconds = Var.Define("friendgate.duration_in_seconds." + name, friend_gate_duration_in_seconds);
			return;
		}
		var_level_coins = Var.Define("level.level_coins." + name, level_coins);
		var_kill_coins = Var.Define("level.kill_coins." + name, kill_coins);
		var_cuttoff = Var.Define("level.cutoff." + name, 1f);
		var_endlessScale = Var.Define("level.endless_scale." + name, 0.03f);
	}

	public int getOverrideLevel(ShotType shot = ShotType.None, UnitType unit = UnitType.None)
	{
		if (shot != ShotType.None && unit != 0)
		{
			Debug.Log("please ask only for one thing..");
			return -1;
		}
		if (selection_override.Contains(selection_override.Find((SelectionOverrideData x) => x.shot == shot)))
		{
			return selection_override.Find((SelectionOverrideData x) => x.shot == shot).shot_level;
		}
		if (selection_override.Contains(selection_override.Find((SelectionOverrideData x) => x.unit == unit)))
		{
			return selection_override.Find((SelectionOverrideData x) => x.unit == unit).unit_level;
		}
		return -1;
	}
}
