using System;
using System.Xml.Serialization;
using LeanplumSDK;
using UnityEngine;
using dinobash;

[Serializable]
public class Quest
{
	[XmlAttribute("id")]
	public string id;

	[XmlAttribute("objective")]
	public QuestObjective objective;

	[XmlAttribute("duration")]
	public QuestDuration duration;

	[XmlAttribute("max_count")]
	public int max_count;

	public int count;

	[XmlAttribute("coins")]
	public int reward_coins;

	[XmlAttribute("diamonds")]
	public int reward_diamonds;

	[XmlAttribute("megaballs")]
	public int reward_megaballs;

	[XmlAttribute("blizzard")]
	public int reward_blizzard;

	[XmlAttribute("meteorstorm")]
	public int reward_meteorstorm;

	[XmlIgnore]
	private Var<string> var_objective;

	[XmlIgnore]
	private Var<string> var_duration;

	[XmlIgnore]
	private Var<int> var_reward_coins;

	[XmlIgnore]
	private Var<int> var_reward_diamonds;

	[XmlIgnore]
	private Var<int> var_reward_megaballs;

	[XmlIgnore]
	private Var<int> var_reward_blizzard;

	[XmlIgnore]
	private Var<int> var_reward_meteorstorm;

	public bool done
	{
		get
		{
			return count >= max_count;
		}
	}

	private void ApplyCoinModifier()
	{
		float shop_amount_multiplier = Konfiguration.GetChapterData(Player.MaxLevelID).shop_amount_multiplier;
		reward_coins = Mathf.RoundToInt((float)reward_coins * shop_amount_multiplier);
		if (objective == QuestObjective.Collect_Coins)
		{
			max_count = Mathf.RoundToInt((float)max_count * shop_amount_multiplier);
		}
	}

	public void RegisterVars()
	{
		string text = string.Format("quest.quests.{0}.", id);
		var_objective = Var.Define(text + "objective", objective.ToString());
		var_duration = Var.Define(text + "duration", duration.ToString());
		var_reward_coins = Var.Define(text + "reward_coins", reward_coins);
		var_reward_diamonds = Var.Define(text + "reward_diamonds", reward_diamonds);
		var_reward_megaballs = Var.Define(text + "reward_megaballs", reward_megaballs);
		var_reward_blizzard = Var.Define(text + "reward_blizzard", reward_blizzard);
		var_reward_meteorstorm = Var.Define(text + "reward_meteorstorm", reward_meteorstorm);
	}

	public void ApplyVars()
	{
		try
		{
			objective = (QuestObjective)(int)Enum.Parse(typeof(QuestObjective), var_objective.Value);
			duration = (QuestDuration)(int)Enum.Parse(typeof(QuestDuration), var_duration.Value);
			reward_coins = var_reward_coins.Value;
			reward_diamonds = var_reward_diamonds.Value;
			reward_megaballs = var_reward_megaballs.Value;
			reward_blizzard = var_reward_blizzard.Value;
			reward_meteorstorm = var_reward_meteorstorm.Value;
		}
		catch (Exception ex)
		{
			Debug.LogError("Quest Leanplum input error in:" + id);
			Debug.LogError(ex.Message + "\n" + ex.StackTrace);
		}
	}

	public Quest Clone()
	{
		Quest quest = MemberwiseClone() as Quest;
		quest.ApplyCoinModifier();
		return quest;
	}
}
