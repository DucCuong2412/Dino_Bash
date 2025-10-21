using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using dinobash;

public abstract class SelectDinoOrShotScreen<T> : BaseScreen where T : struct, IConvertible
{
	private class DinoSelectionSlot
	{
		public Transform transform;

		public int unit = -1;

		public DinoSelectionSlot(Transform t)
		{
			transform = t;
		}
	}

	private const float error_speed = 5f;

	protected Transform error_dialog;

	private List<DinoSelectionSlot> selected_slots = new List<DinoSelectionSlot>();

	protected List<DinoSelectButton> all_dino_buttons = new List<DinoSelectButton>();

	private StandardButton btn_back;

	private StandardButton btn_next;

	private Vector3 button_scale;

	protected InfoContainer infoContainer;

	protected int level_id;

	protected GoTween error_tween;

	private Transform dino_slot_template;

	private Transform dino_button_template;

	private bool reducedToThreeSlots;

	private float slot_size_scale = 1.15f;

	private StandardButton btn_buy_fourth_slot;

	private Transform slot4;

	private Vector3 error_start_pos;

	private Vector3 error_end_pos;

	private float show_error_time = -1f;

	private tk2dTextMesh info_health_label;

	private tk2dTextMesh info_attack_label;

	private LocalizedText info_dino_name_label;

	private LocalizedText info_dino_description_label;

	private SpriteRenderer info_dino_portrait;

	private GoTween buttonTween;

	protected abstract int SelectionSlots { get; }

	protected abstract List<T> getAvailable();

	protected abstract List<T> getSelectedInPlayer();

	protected abstract string getSpriteName(T unit_type);

	protected abstract float getIconScale();

	protected abstract string getTitle();

	protected abstract int enumToInt(T t);

	protected abstract bool isUpgrading(T t);

	protected abstract void OnButtonNextClicked(List<int> selected);

	protected abstract void OnButtonBackClicked(List<int> selected);

	protected abstract DinoShotUpgradeAdapter GetAdapter(T t);

	protected abstract void SetInfoContainer(T t);

	protected abstract void PlaySelectSound(T t);

	protected abstract void PlayDeselectSound();

	public virtual void Show(int level_id, Vector3 showFrom)
	{
		base.gameObject.SetActive(true);
		this.level_id = level_id;
		createButtons();
		setSlots();
		OnEscapeUp = delegate
		{
			OnButtonBackClicked(getSelected());
		};
		List<T> selectedInPlayer = getSelectedInPlayer();
		if (selectedInPlayer.Count > 0)
		{
			setDinoInfo(getSelectedInPlayer().Last());
		}
		else
		{
			setDinoInfo(getAvailable().Last());
		}
		base.Show();
		ShowFrom(showFrom, init_slots);
	}

	private void init_slots()
	{
		List<T> selectedInPlayer = getSelectedInPlayer();
		T t;
		foreach (T item in selectedInPlayer)
		{
			t = item;
			DinoSelectButton button = all_dino_buttons.Find((DinoSelectButton x) => x.sprite_name == getSpriteName(t));
			OnDinoSelected(button, t);
		}
	}

	protected void Start()
	{
		dino_slot_template = base.transform.Find("MiddleCenter/dino_select/dino_slot");
		dino_button_template = base.transform.Find("MiddleCenter/dino_select/dino_button");
		error_dialog = base.transform.Find("MiddleCenter/error");
		CreateErrorTween();
		Transform target = base.transform.Find("MiddleCenter/dino_info/infoContainerPosition");
		infoContainer = InfoContainer.Load(target);
		btn_back = FindChildComponent<StandardButton>("MiddleCenter/btn_back");
		btn_back.uiItem.OnClick += delegate
		{
			OnButtonBackClicked(getSelected());
		};
		btn_next = FindChildComponent<StandardButton>("MiddleCenter/btn_next");
		btn_next.uiItem.OnClick += delegate
		{
			OnButtonNextClicked(getSelected());
		};
		updateBtnNextState();
		FindChildComponent<LocalizedText>("MiddleCenter/title/title_label").Key = getTitle();
		btn_buy_fourth_slot = base.transform.Find("MiddleCenter/selected_dinos/shot_slot_3").GetComponent<StandardButton>();
		tk2dTextMesh component = btn_buy_fourth_slot.transform.Find("dinobuy_panel/price_label").GetComponent<tk2dTextMesh>();
		component.text = Konfiguration.getUpgradeBuyCost(UnitType.AdditionalShotSlot).ToString();
		btn_buy_fourth_slot.uiItem.OnClick += delegate
		{
			UpgradeInfoUpgradesScreen screen = ScreenManager.GetScreen<UpgradeInfoUpgradesScreen>();
			screen.OnScreenHide += HandleOnUpgradeClose;
			screen.Show(UnitType.AdditionalShotSlot);
		};
		btn_buy_fourth_slot.gameObject.SetActive(false);
		setSlots();
		info_dino_name_label = FindChildComponent<LocalizedText>("MiddleCenter/dino_info/label_dino_name");
		info_dino_description_label = FindChildComponent<LocalizedText>("MiddleCenter/dino_info/label_dino_description");
		info_dino_portrait = FindChildComponent<SpriteRenderer>("MiddleCenter/dino_info/dino_portrait");
		base.transform.localPosition += base.left;
		base.gameObject.SetActive(false);
	}

	private void createButtons()
	{
		all_dino_buttons.Clear();
		dino_slot_template.gameObject.SetActive(true);
		dino_button_template.gameObject.SetActive(true);
		float num = 325f;
		float num2 = 0f;
		float num3 = 230f;
		int num4 = 5;
		int num5 = 2;
		if (getAvailable().Count > num4 * num5)
		{
			num4++;
			num *= 0.885f;
			num2 = 48f;
		}
		for (int i = 0; i != num5; i++)
		{
			for (int j = 0; j != num4; j++)
			{
				Vector3 position = dino_slot_template.position + new Vector3(Mathf.Round((float)j * num) - num2, (float)i * (0f - num3), 0f);
				int num6 = i * num4 + j;
				if (num6 < getAvailable().Count)
				{
					Transform transform = UnityEngine.Object.Instantiate(dino_button_template) as Transform;
					transform.gameObject.SetActive(true);
					transform.position = position;
					transform.transform.parent = dino_button_template.transform.parent;
					DinoSelectButton btn = transform.GetComponentInChildren<DinoSelectButton>();
					btn.init();
					T unit_type = getAvailable()[num6];
					btn.uiItem.OnClick += delegate
					{
						if (Konfiguration.GameConfig.Use_upgrade_locks && isUpgrading(unit_type))
						{
							ShowInTrainingError();
						}
						else
						{
							OnDinoSelected(btn, unit_type);
						}
					};
					button_scale = (btn.transform.localScale *= getIconScale());
					btn.sprite_name = getSpriteName(unit_type);
					btn.price = GetAdapter(unit_type).ApplePrice;
					btn.is_upgrading = isUpgrading(unit_type);
					all_dino_buttons.Add(btn);
				}
				else
				{
					Transform transform2 = UnityEngine.Object.Instantiate(dino_slot_template) as Transform;
					transform2.position = position;
					transform2.transform.parent = dino_slot_template.transform.parent;
				}
			}
		}
		dino_slot_template.gameObject.SetActive(false);
		dino_button_template.gameObject.SetActive(false);
	}

	private void setSlots()
	{
		selected_slots.Clear();
		selected_slots.Add(new DinoSelectionSlot(base.transform.Find("MiddleCenter/selected_dinos/slot0")));
		selected_slots.Add(new DinoSelectionSlot(base.transform.Find("MiddleCenter/selected_dinos/slot1")));
		selected_slots.Add(new DinoSelectionSlot(base.transform.Find("MiddleCenter/selected_dinos/slot2")));
		slot4 = base.transform.Find("MiddleCenter/selected_dinos/slot3");
		if (Konfiguration.GameConfig.Use_ShotSlot_upsell)
		{
			if (this is SelectDinoScreen)
			{
				selected_slots.Add(new DinoSelectionSlot(slot4));
			}
			else if (Player.ActiveUpgrades.Contains(UnitType.AdditionalShotSlot))
			{
				selected_slots.Add(new DinoSelectionSlot(slot4));
				btn_buy_fourth_slot.gameObject.SetActive(false);
			}
			else
			{
				btn_buy_fourth_slot.gameObject.SetActive(true);
			}
			return;
		}
		if (SelectionSlots > 3)
		{
			slot4.gameObject.SetActive(true);
			if (reducedToThreeSlots)
			{
				selected_slots.ForEach(delegate(DinoSelectionSlot x)
				{
					x.transform.localPosition -= new Vector3(slot4.GetComponent<Renderer>().bounds.extents.x * slot_size_scale, 0f);
				});
				reducedToThreeSlots = false;
			}
			selected_slots.Add(new DinoSelectionSlot(slot4));
			return;
		}
		slot4.gameObject.SetActive(false);
		if (!reducedToThreeSlots)
		{
			selected_slots.ForEach(delegate(DinoSelectionSlot x)
			{
				x.transform.localPosition += new Vector3(slot4.GetComponent<Renderer>().bounds.extents.x * slot_size_scale, 0f);
			});
			reducedToThreeSlots = true;
		}
	}

	private void HandleOnUpgradeClose()
	{
		UpgradeInfoUpgradesScreen screen = ScreenManager.GetScreen<UpgradeInfoUpgradesScreen>();
		if (screen != null)
		{
			screen.OnScreenHide -= HandleOnUpgradeClose;
		}
		if (Player.ActiveUpgrades.Contains(UnitType.AdditionalShotSlot))
		{
			btn_buy_fourth_slot.uiItem.enabled = false;
			Go.to(btn_buy_fourth_slot.transform, 0.3f, new GoTweenConfig().scale(Vector3.zero));
			selected_slots.Add(new DinoSelectionSlot(slot4));
		}
	}

	private void Update()
	{
		if (Konfiguration.GameConfig.Use_upgrade_locks)
		{
			if (Time.time < show_error_time)
			{
				error_dialog.localPosition = Vector3.Lerp(error_dialog.localPosition, error_end_pos, Time.deltaTime * 5f);
			}
			else
			{
				error_dialog.localPosition = Vector3.Lerp(error_dialog.localPosition, error_start_pos, Time.deltaTime * 5f);
			}
		}
	}

	protected void ShowInTrainingError()
	{
		show_error_time = Time.time + 5f;
	}

	protected void HideInTrainingError()
	{
		show_error_time = Time.time;
	}

	private void CreateErrorTween()
	{
		error_start_pos = error_dialog.localPosition - new Vector3(0f, 256f, 0f);
		error_end_pos = error_dialog.localPosition;
		error_dialog.localPosition = error_start_pos;
	}

	private DinoSelectionSlot findFreeSlot(T type)
	{
		if (selected_slots.Find((DinoSelectionSlot x) => x.unit == enumToInt(type)) != null)
		{
			return null;
		}
		return selected_slots.Find((DinoSelectionSlot x) => x.unit == -1);
	}

	private void setDinoInfo(T unit_type)
	{
		foreach (DinoSelectButton all_dino_button in all_dino_buttons)
		{
			all_dino_button.selected = all_dino_button.sprite_name == getSpriteName(unit_type);
		}
		SetInfoContainer(unit_type);
		info_dino_name_label.Key = unit_type.ToString();
		info_dino_description_label.Key = unit_type.ToString() + ".description";
		info_dino_portrait.sprite = SpriteRessources.GetSprite(getSpriteName(unit_type));
	}

	public override void Hide()
	{
		if (buttonTween != null && buttonTween.state != GoTweenState.Complete)
		{
			buttonTween.complete();
		}
		base.Hide();
	}

	protected void CleanUpDinoSelectButtons()
	{
		foreach (Transform item in dino_slot_template.parent)
		{
			if (item != dino_slot_template && item != dino_button_template)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
	}

	private void OnDinoSelected(DinoSelectButton button, T unit_type)
	{
		DinoSelectionSlot slot = findFreeSlot(unit_type);
		setDinoInfo(unit_type);
		if (slot == null)
		{
			return;
		}
		slot.unit = enumToInt(unit_type);
		PlaySelectSound(unit_type);
		GameObject gameObject = UnityEngine.Object.Instantiate(button.gameObject) as GameObject;
		gameObject.transform.position = button.transform.position;
		gameObject.transform.localScale = button_scale;
		gameObject.transform.parent = dino_button_template.transform.parent;
		gameObject.GetComponent<SpriteRenderer>().color = (gameObject.GetComponent<DinoSelectButton>().originalColor = Colors.Visible);
		DinoSelectButton new_button = gameObject.GetComponent<DinoSelectButton>();
		new_button.uiItem.enabled = false;
		button.GetComponent<SpriteRenderer>().color = (button.GetComponent<DinoSelectButton>().originalColor = Colors.Deactivated);
		buttonTween = Go.to(gameObject.transform, 0.2f, new GoTweenConfig().position(slot.transform.position).setEaseType(GoEaseType.CircInOut).onComplete(delegate
		{
			new_button.uiItem.enabled = true;
			new_button.uiItem.OnClick += delegate
			{
				OnDinoDeselected(slot, button, new_button, unit_type);
			};
			updateBtnNextState();
		}));
	}

	private List<int> getSelected()
	{
		return (from x in selected_slots.FindAll((DinoSelectionSlot x) => x.unit != -1)
			select x.unit).ToList();
	}

	private void updateBtnNextState()
	{
		List<int> selected = getSelected();
		string text = string.Empty;
		foreach (int item in selected)
		{
			text = text + item + ", ";
		}
		bool isFocused = selected.Count >= 1;
		btn_next.Enabled = isFocused;
		btn_next.isFocused = isFocused;
	}

	private void OnDinoDeselected(DinoSelectionSlot slot, DinoSelectButton selection_button, DinoSelectButton button, T unit_type)
	{
		all_dino_buttons.ForEach(delegate(DinoSelectButton btn)
		{
			btn.selected = false;
		});
		PlayDeselectSound();
		slot.unit = -1;
		updateBtnNextState();
		button.uiItem.enabled = false;
		buttonTween = Go.to(button.transform, 0.2f, new GoTweenConfig().position(selection_button.transform.position).setEaseType(GoEaseType.CircInOut).onComplete(delegate
		{
			UnityEngine.Object.Destroy(button.gameObject);
			selection_button.GetComponent<SpriteRenderer>().color = Colors.Visible;
			selection_button.GetComponent<StandardButton>().originalColor = Colors.Visible;
		}));
	}
}
