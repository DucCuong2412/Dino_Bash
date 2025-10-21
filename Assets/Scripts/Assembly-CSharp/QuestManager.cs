using System;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class QuestManager
{
	private const int MAX_QUESTS = 3;

	private static readonly QuestManager _instance = new QuestManager();

	public List<Quest> newQuests = new List<Quest>();

	private bool isInitialized;

	public static QuestManager instance
	{
		get
		{
			return _instance;
		}
	}

	public SerializableList<Quest> activeQuests
	{
		get
		{
			return Player.Instance.activeQuests;
		}
	}

	public static void Initialize()
	{
		if (!instance.isInitialized)
		{
			instance.isInitialized = true;
			Debug.Log("Quest - count: " + instance.activeQuests.Count);
			instance.printQuests();
		}
	}

	private void printQuests()
	{
		instance.activeQuests.ForEach(delegate(Quest quest)
		{
			Debug.Log(string.Format("Quest - active: {0} {1}/{2}", quest.id, quest.count, quest.max_count));
		});
	}

	public void updateQuests()
	{
		CollectRewards();
		if (Player.MaxLevelID > Konfiguration.GameConfig.First_quest_level + 1 && isInitialized)
		{
			newQuests.Clear();
			TimeSpan timeSpan = DateTime.UtcNow - Player.Instance.lastQuestCreated;
			TimeSpan timeSpan2 = new TimeSpan(Konfiguration.GameConfig.Hours_to_next_quest, 0, 0);
			while (timeSpan > timeSpan2 && activeQuests.Count < 3)
			{
				timeSpan -= timeSpan2;
				createQuest(QuestDuration.none);
			}
			Player.SavePlayer();
			Debug.Log("Quest - new quests:" + instance.newQuests.Count);
			printQuests();
		}
	}

	public void createQuest(QuestDuration durations)
	{
		activeQuests.ForEach(delegate(Quest quest)
		{
			durations |= quest.duration;
		});
		if ((durations & QuestDuration.long_term) != QuestDuration.long_term)
		{
			activeQuests.Add(getRandomQuest(QuestDuration.long_term).Clone());
			Debug.Log(string.Format("Quest - adding longterm Quest: {0}", activeQuests[activeQuests.Count - 1].id));
		}
		else if ((durations & QuestDuration.medium_term) != QuestDuration.medium_term)
		{
			activeQuests.Add(getRandomQuest(QuestDuration.medium_term).Clone());
			Debug.Log(string.Format("Quest - adding mediumterm Quest: {0}", activeQuests[activeQuests.Count - 1].id));
		}
		else
		{
			activeQuests.Add(getRandomQuest(QuestDuration.short_term).Clone());
			Debug.Log(string.Format("Quest - adding shortterm Quest: {0}", activeQuests[activeQuests.Count - 1].id));
		}
		Player.Instance.lastQuestCreated = DateTime.UtcNow;
		newQuests.Add(activeQuests[activeQuests.Count - 1]);
	}

	private Quest getRandomQuest(QuestDuration duration)
	{
		List<Quest> list = Konfiguration.quests.FindAll((Quest quest) => quest.duration == duration);
		if (!Player.hasUnlockedConsumable(UnitType.MegaBall))
		{
			list.RemoveAll((Quest quest) => quest.reward_megaballs > 0);
		}
		if (!Player.hasUnlockedConsumable(UnitType.Blizzard))
		{
			list.RemoveAll((Quest quest) => quest.reward_blizzard > 0);
		}
		if (!Player.hasUnlockedConsumable(UnitType.MeteorStorm))
		{
			list.RemoveAll((Quest quest) => quest.reward_meteorstorm > 0);
		}
		int num = Konfiguration.levels.FindIndex((LevelData level) => level.enemies.FindIndex((LevelEnemy enemy) => enemy.unittype == UnitType.Neander_Flyer) != -1);
		Debug.Log("flyer level: " + num);
		if (Player.MaxLevelID < num)
		{
			list.RemoveAll((Quest quest) => quest.objective == QuestObjective.Ground_Flyer);
		}
		int num2 = Konfiguration.levels.FindIndex((LevelData level) => level.unlockShot == ShotType.Ice);
		if (Player.MaxLevelID < num2)
		{
			list.RemoveAll((Quest quest) => quest.objective == QuestObjective.IceAge);
		}
		if (list.Count == 0)
		{
			Debug.LogError("Quest - no suitable quest found, emitting default quest");
			return get_default_quest(duration);
		}
		Debug.Log("Quest - availiable:");
		list.ForEach(delegate(Quest q)
		{
			Debug.Log("\t" + q.id);
		});
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public void ReportProgress(QuestObjective objective, int count)
	{
		activeQuests.FindAll((Quest quest) => quest.objective == objective).ForEach(delegate(Quest quest)
		{
			quest.count = Mathf.Clamp(quest.count + count, 0, quest.max_count);
			Debug.Log(string.Format("Quest - {0} set to {1}/{2}", quest.id, quest.count, quest.max_count));
		});
	}

	private void CollectRewards()
	{
		activeQuests.FindAll((Quest quest) => quest.done).ForEach(delegate(Quest quest)
		{
			string reward = string.Empty;
			int amount = 0;
			if (quest.reward_coins > 0)
			{
				reward = "coins";
				amount = quest.reward_coins;
				Wallet.GiveCoins(quest.reward_coins);
			}
			else if (quest.reward_diamonds > 0)
			{
				reward = "diamonds";
				amount = quest.reward_diamonds;
				Wallet.GiveDiamonds(quest.reward_diamonds);
			}
			else if (quest.reward_megaballs > 0)
			{
				reward = "megaballs";
				amount = quest.reward_megaballs;
				Player.changeConsumableCount(UnitType.MegaBall, quest.reward_megaballs);
			}
			else if (quest.reward_blizzard > 0)
			{
				reward = "blizzard";
				amount = quest.reward_blizzard;
				Player.changeConsumableCount(UnitType.Blizzard, quest.reward_blizzard);
			}
			else if (quest.reward_meteorstorm > 0)
			{
				reward = "meteorstorm";
				amount = quest.reward_meteorstorm;
				Player.changeConsumableCount(UnitType.MeteorStorm, quest.reward_meteorstorm);
			}
			Tracking.quest_completed(quest.id, reward, amount);
			Debug.Log(string.Format("Quest - collected: {0}", quest.id));
		});
		activeQuests.RemoveAll((Quest quest) => quest.done);
		Player.SavePlayer();
	}

	private Quest get_default_quest(QuestDuration duration)
	{
		Quest quest = new Quest();
		quest.duration = duration;
		quest.max_count = 15 * (int)duration;
		quest.id = "default_quest";
		quest.objective = QuestObjective.Kill_Neander;
		quest.reward_coins = 150 * (int)duration;
		return quest;
	}
}
