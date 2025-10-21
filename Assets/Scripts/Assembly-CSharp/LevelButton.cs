using UnityEngine;
using dinobash;

public class LevelButton : MonoBehaviour
{
	private Transform border;

	private tk2dTextMesh label;

	private SpriteRenderer unlock_icon;

	public Transform selection;

	private Transform friend_gate_sprite;

	public StandardButton button;

	private SpriteRenderer special_icon;

	public Sprite lv_tatze;

	public Sprite lv_fragezeichen;

	public Sprite lv_endlos_modus;

	private bool _levelComplete;

	private bool _focused;

	private int level_id;

	public UnitType unlockUnit
	{
		set
		{
			if (value != 0)
			{
				friend_gate_sprite.gameObject.SetActive(false);
				unlock_icon.gameObject.SetActive(true);
				unlock_icon.sprite = SpriteRessources.getSpiteForUnitType(value);
				if (Konfiguration.isUpgrade(value))
				{
					unlock_icon.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
				}
				else if (Konfiguration.isConsumable(value))
				{
					unlock_icon.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
				}
				else
				{
					unlock_icon.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				}
				special_icon.enabled = false;
			}
			else
			{
				unlock_icon.gameObject.SetActive(false);
			}
		}
	}

	public ShotType unlockShot
	{
		set
		{
			if (value != ShotType.None)
			{
				friend_gate_sprite.gameObject.SetActive(false);
				unlock_icon.gameObject.SetActive(true);
				unlock_icon.sprite = SpriteRessources.getShotBuySprite(value);
				unlock_icon.transform.localScale = new Vector3(0.97f, 0.97f, 1f);
				special_icon.enabled = false;
			}
			else
			{
				unlock_icon.gameObject.SetActive(false);
			}
		}
	}

	public bool levelComplete
	{
		get
		{
			return _levelComplete;
		}
		set
		{
			_levelComplete = value;
			if (value)
			{
				if (level_id == Player.MaxLevelID - 1 && !Player.HasPlayedMaxLevelID)
				{
					Animator component = base.transform.Find("top/LevelButtonWin").GetComponent<Animator>();
					component.Play("win in", 0, Random.value);
				}
				else
				{
					Animator component2 = base.transform.Find("top/LevelButtonWin").GetComponent<Animator>();
					component2.Play("win loop", 0, Random.value);
				}
			}
			else
			{
				Animator component3 = base.transform.Find("top/LevelButtonWin").GetComponent<Animator>();
				component3.Play("level_button_no_haekchen");
			}
		}
	}

	public bool focused
	{
		get
		{
			return _focused;
		}
		set
		{
			_focused = value;
			selection.gameObject.SetActive(_focused);
		}
	}

	public bool clickable
	{
		get
		{
			return button.Enabled;
		}
		set
		{
			SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				spriteRenderer.color = ((!value) ? Colors.Deactivated : Colors.Visible);
			}
			MeshRenderer[] componentsInChildren2 = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren2)
			{
				meshRenderer.material.color = ((!value) ? Colors.Deactivated : Colors.Visible);
			}
			button.Enabled = value;
		}
	}

	public bool isFriendGate
	{
		set
		{
			friend_gate_sprite.gameObject.SetActive(value);
			unlock_icon.gameObject.SetActive(!value);
			border.gameObject.SetActive(!value);
		}
	}

	public int Level_ID
	{
		get
		{
			return level_id;
		}
		set
		{
			level_id = value;
			base.gameObject.name = "level_button_" + level_id;
			LevelData levelData = Konfiguration.GetLevelData(level_id);
			clickable = (level_id <= Player.MaxLevelID && (!levelData.is_friend_gate || level_id == Player.MaxLevelID)) || Cheats.UnlockLevels;
			focused = Player.MaxLevelID == level_id;
			levelComplete = level_id < Player.MaxLevelID && !levelData.endless_mode;
			string text = "--";
			int bestEndlessLevelScore = Player.getBestEndlessLevelScore(level_id);
			if (bestEndlessLevelScore != -1)
			{
				text = bestEndlessLevelScore.ToString();
				border.GetComponent<tk2dSlicedSprite>().dimensions = new Vector2(275f, 118f);
			}
			label.text = ((!levelData.endless_mode) ? levelData.display_name : text);
			label.Commit();
			isFriendGate = levelData.is_friend_gate;
			if (levelData.unlockUnit != 0)
			{
				unlockUnit = levelData.unlockUnit;
			}
			else if (levelData.unlockShot != ShotType.None)
			{
				unlockShot = levelData.unlockShot;
			}
			else
			{
				unlockUnit = UnitType.None;
				special_icon.enabled = true;
				if (levelData.override_dino_selection || levelData.override_shots_selection)
				{
					special_icon.sprite = lv_fragezeichen;
				}
				else if (levelData.endless_mode)
				{
					special_icon.sprite = lv_endlos_modus;
				}
				else
				{
					special_icon.sprite = lv_tatze;
				}
				base.transform.Find("top/LevelButtonWin/check").GetComponent<SpriteRenderer>().enabled = false;
			}
			if ((levelData.is_friend_gate || level_id < Player.MaxLevelID) && !levelData.endless_mode)
			{
				special_icon.enabled = false;
			}
		}
	}

	public void Start()
	{
		border = base.transform.Find("top/border");
		label = base.transform.Find("top/border/label").GetComponent<tk2dTextMesh>();
		unlock_icon = base.transform.Find("top/dino_icon").GetComponent<SpriteRenderer>();
		selection = base.transform.Find("top/selection");
		special_icon = base.transform.Find("top/lv/special_icon").GetComponent<SpriteRenderer>();
		friend_gate_sprite = base.transform.Find("top/friendgate");
		button = GetComponentInChildren<StandardButton>();
		button.clickSound = Sounds.map_waypoint;
	}
}
