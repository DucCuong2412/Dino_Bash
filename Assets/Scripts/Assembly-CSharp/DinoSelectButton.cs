using UnityEngine;

public class DinoSelectButton : StandardButton
{
	private SpriteRenderer dino_slot;

	private SpriteRenderer selected_sprite;

	private tk2dTextMesh price_label;

	private Transform price_panel;

	private Transform upgrade_icon;

	private string sprite_name_;

	private int price_value;

	public string sprite_name
	{
		get
		{
			return sprite_name_;
		}
		set
		{
			sprite_name_ = value;
			Sprite sprite = SpriteRessources.GetSprite(value);
			if (sprite != null)
			{
				dino_slot.sprite = sprite;
				_normal = sprite;
				_disabled = sprite;
			}
		}
	}

	public int price
	{
		set
		{
			price_value = value;
			price_panel.gameObject.SetActive(price_value > 0);
			price_label.gameObject.SetActive(price_value > 0);
			price_label.text = price_value.ToString();
			price_label.Commit();
		}
	}

	public bool selected
	{
		set
		{
			Go.to(selected_sprite, 0.25f, new GoTweenConfig().colorProp("color", (!value) ? Colors.Invisible : Colors.Visible));
			base.uiItem.enabled = !value;
		}
	}

	public bool is_upgrading
	{
		set
		{
			price_panel.gameObject.SetActive(!value && price_value > 0);
			price_label.gameObject.SetActive(!value && price_value > 0);
			base.Enabled = !value;
			upgrade_icon.gameObject.SetActive(value);
		}
	}

	public void init()
	{
		dino_slot = GetComponent<SpriteRenderer>();
		selected_sprite = base.transform.parent.FindChild("selected").GetComponent<SpriteRenderer>();
		price_label = base.transform.parent.FindChild("price_label").GetComponent<tk2dTextMesh>();
		price_panel = price_label.transform.parent.Find("dinobuy_panel");
		upgrade_icon = base.transform.parent.FindChild("upgrading");
		selected = false;
	}
}
