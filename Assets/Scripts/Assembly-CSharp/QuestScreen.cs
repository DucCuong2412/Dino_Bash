using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestScreen : BaseScreen
{
	private List<Transform> quest_slots = new List<Transform>();

	private List<Transform> quest_panels = new List<Transform>();

	private bool initialized;

	private StandardButton closebutton;

	private SpriteRenderer blende;

	private int quest_id;

	private bool player_made_progress;

	private float turnspeed = 0.2f;

	public int int_tween { get; set; }

	protected void Start()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		closebutton = base.transform.Search("btn_close").GetComponent<StandardButton>();
		closebutton.clickSound = Sounds.main_close_popup;
		closebutton.uiItem.OnClick += delegate
		{
			Hide();
		};
		quest_slots.Add(base.transform.Search("slot_0"));
		quest_slots.Add(base.transform.Search("slot_1"));
		quest_slots.Add(base.transform.Search("slot_2"));
		quest_panels.Add(base.transform.Search("quest_0"));
		quest_panels.Add(base.transform.Search("quest_1"));
		quest_panels.Add(base.transform.Search("quest_2"));
		quest_panels.ForEach(delegate(Transform quest)
		{
			quest.gameObject.SetActive(false);
		});
		if (QuestManager.instance.activeQuests.Count > 0)
		{
			base.transform.Search("wait_label").gameObject.SetActive(false);
			for (int i = 0; i != QuestManager.instance.activeQuests.Count; i++)
			{
				quest_panels[i].gameObject.SetActive(true);
				SetupQuest(i);
			}
		}
		blende = base.transform.Search("blende").GetComponent<SpriteRenderer>();
		base.transform.localPosition += base.right;
		base.gameObject.SetActive(false);
	}

	private void SetupQuest(int quest_number)
	{
		Transform quest_panel = quest_panels[quest_number];
		Quest quest = QuestManager.instance.activeQuests[quest_number];
		quest_panel.Search("quest_title_label").GetComponent<LocalizedText>().Key = quest.id;
		LocalizedText component = quest_panel.Search("quest_description_label").GetComponent<LocalizedText>();
		component.textMesh.text = string.Format((quest.id + ".description").Localize(), quest.max_count);
		quest_panel.Search("quest_progress_label").GetComponent<tk2dTextMesh>().text = string.Format("{0}/{1}", quest.count.ToString().GetGroupedNumberString(), quest.max_count.ToString().GetGroupedNumberString());
		Action<string, int> action = delegate(string icon_name, int amount)
		{
			Transform transform = quest_panel.Search(icon_name);
			transform.gameObject.SetActive(true);
			transform.GetComponentInChildren<tk2dTextMesh>().text = amount.ToString();
		};
		if (quest.reward_coins > 0)
		{
			action("reward_icon_coin", quest.reward_coins);
		}
		else if (quest.reward_diamonds > 0)
		{
			action("reward_icon_diamond", quest.reward_diamonds);
		}
		else if (quest.reward_megaballs > 0)
		{
			action("reward_icon_megaball", quest.reward_megaballs);
		}
		else if (quest.reward_blizzard > 0)
		{
			action("reward_icon_blizzard", quest.reward_blizzard);
		}
		else if (quest.reward_meteorstorm > 0)
		{
			action("reward_icon_meteorstorm", quest.reward_meteorstorm);
		}
	}

	private void UpdateQuests()
	{
		Action update_next_quest = delegate
		{
			quest_id++;
			if (quest_id < QuestManager.instance.activeQuests.Count)
			{
				UpdateQuests();
			}
		};
		Transform quest_panel = quest_panels[quest_id];
		Quest quest = QuestManager.instance.activeQuests[quest_id];
		tk2dTextMesh textmesh = quest_panel.Search("quest_progress_label").GetComponent<tk2dTextMesh>();
		int num = int.Parse(textmesh.text.Split('/')[0]);
		if (quest.count > num)
		{
			player_made_progress = true;
			int_tween = num;
			Go.to(quest_panel, 0.5f, new GoTweenConfig().scale(Vector3.one * 1.025f).setEaseType(GoEaseType.CircIn).onComplete(delegate
			{
				Go.to(quest_panel, 0.5f, new GoTweenConfig().scale(Vector3.one).setEaseType(GoEaseType.CircOut));
			}));
			Go.to(this, 1f, new GoTweenConfig().setDelay(0.2f).intProp("int_tween", quest.count).onUpdate(delegate
			{
				textmesh.text = string.Format("{0}/{1}", int_tween, quest.max_count);
			})
				.onComplete(delegate
				{
					if (quest.done)
					{
						if (quest_id + 1 < QuestManager.instance.activeQuests.Count)
						{
							ShowQuestDone(quest, update_next_quest);
						}
						else
						{
							ShowQuestDone(quest, Hide);
						}
					}
					else
					{
						update_next_quest();
					}
				}));
		}
		else
		{
			update_next_quest();
		}
	}

	private void ShowQuestDone(Quest quest, Action callback)
	{
		quest_slots[quest_id].transform.localScale = new Vector3(0f, 1f, 1f);
		Transform quest_panel = quest_panels[quest_id];
		Vector3 initial_position = quest_panel.transform.localPosition;
		SpriteTools.OffsetSortingOrder(quest_panel, 1000);
		Transform transform = quest_panel.transform.Search("quest_done");
		transform.gameObject.SetActive(true);
		Transform checkmark = transform.transform.Search("checkmark");
		checkmark.localScale = Vector3.zero;
		Transform fx = base.transform.Search("FX_Stars");
		fx.gameObject.SetActive(false);
		fx.localPosition = new Vector3(quest_panel.transform.localPosition.x, fx.localPosition.y, fx.localPosition.z);
		closebutton.uiItem.enabled = false;
		StandardButton button_done = transform.transform.Search("btn_done").GetComponent<StandardButton>();
		blende.transform.parent = ScreenManager.UI_Root;
		blende.enabled = true;
		Color endValue = new Color(0.6f, 0.6f, 0.6f, 0.4f);
		GoTween blende_tween = Go.to(blende, 0.3f, new GoTweenConfig().colorProp("color", endValue));
		blende_tween.autoRemoveOnComplete = false;
		GoTween btn_panel_tween = Go.to(button_done.transform.parent, 0.3f, new GoTweenConfig().localPosition(button_done.transform.parent.localPosition.SetY(-700f)).setEaseType(GoEaseType.CircOut));
		btn_panel_tween.autoRemoveOnComplete = false;
		Go.to(quest_panel, 0.3f, new GoTweenConfig().localPosition(new Vector3(initial_position.x, 128f, initial_position.z)).scale(Vector3.one * 1.3f).setEaseType(GoEaseType.CircInOut)
			.onComplete(delegate
			{
				Go.to(checkmark, 0.4f, new GoTweenConfig().scale(Vector3.one * 1.5f).setEaseType(GoEaseType.BounceOut));
				button_done.collider.enabled = true;
				button_done.uiItem.OnClick += delegate
				{
					Tracking.quest_redeemed(quest.id);
					closebutton.uiItem.enabled = true;
					button_done.collider.enabled = false;
					fx.gameObject.SetActive(true);
					btn_panel_tween.playBackwards();
					Go.to(quest_panel, 0.3f, new GoTweenConfig().setDelay(0.5f).localPosition(initial_position).scale(Vector3.one)
						.onComplete(delegate
						{
							blende_tween.playBackwards();
							Go.to(quest_panel, turnspeed, new GoTweenConfig().scale(new Vector3(0f, 1f, 1f)).setEaseType(GoEaseType.CubicIn));
							Go.to(quest_slots[quest_id], turnspeed, new GoTweenConfig().scale(new Vector3(1f, 1f, 1f)).setDelay(turnspeed).setEaseType(GoEaseType.CubicOut)
								.onComplete(delegate
								{
									callback();
								}));
						}));
				};
			}));
	}

	public override void Show()
	{
		Start();
		base.gameObject.SetActive(true);
		base.Show();
		if (App.State == App.States.Map)
		{
			ScreenManager.GetScreen<CoverScreen>(this).Show();
			RevealNewQuests();
		}
		else if (App.State == App.States.Game)
		{
			player_made_progress = false;
			quest_id = 0;
			UpdateQuests();
			if (!player_made_progress)
			{
				base.Hide();
				base.gameObject.SetActive(false);
				return;
			}
		}
		OnEscapeUp = Hide;
		ShowFrom(base.right, delegate
		{
		});
	}

	private void RevealNewQuests()
	{
		float num = 0.1f;
		float num2 = 0f;
		for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
		{
			if (QuestManager.instance.newQuests.Contains(QuestManager.instance.activeQuests[i]))
			{
				Go.to(quest_slots[i], turnspeed, new GoTweenConfig().scale(new Vector3(0f, 1f, 1f)).setDelay(0.3f + (float)i * num2).setEaseType(GoEaseType.CubicIn));
				quest_panels[i].transform.localScale = new Vector3(0f, 1f, 1f);
				Go.to(quest_panels[i], turnspeed, new GoTweenConfig().scale(new Vector3(1f, 1f, 1f)).setDelay(turnspeed + 0.3f + (float)i * num2).setEaseType(GoEaseType.CubicOut));
				num2 += num;
			}
		}
		QuestManager.instance.newQuests.Clear();
	}

	public override void Hide()
	{
		base.Hide();
		base.isVisible = true;
		if (App.State == App.States.Map)
		{
			Tracking.quest_list_closed();
			ScreenManager.GetScreen<CoverScreen>(this).Hide();
		}
		HideTo(base.left, delegate
		{
			base.gameObject.SetActive(false);
			base.isVisible = false;
		});
	}
}
