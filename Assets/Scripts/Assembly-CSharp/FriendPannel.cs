using System.Collections.Generic;
using UnityEngine;

public class FriendPannel : MonoBehaviour
{
	private TextMesh name_label;

	public StandardButton button;

	private SpriteRenderer haeckchen;

	private FacebookManager.Friend friend_;

	public bool selected
	{
		get
		{
			return haeckchen.enabled;
		}
		set
		{
			haeckchen.enabled = value;
		}
	}

	public string friend_name
	{
		get
		{
			return name_label.text;
		}
		set
		{
			base.name = value;
			name_label.text = base.name;
		}
	}

	public FacebookManager.Friend friend
	{
		get
		{
			return friend_;
		}
		set
		{
			friend_ = value;
			friend_name = buildNiceName();
		}
	}

	private void Awake()
	{
		name_label = base.transform.Search("name").GetComponent<TextMesh>();
		button = base.transform.Search("button").GetComponent<StandardButton>();
		haeckchen = base.transform.Search("haeckchen").GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		button.uiItem.OnClick += delegate
		{
			selected = !selected;
		};
	}

	private string buildNiceName()
	{
		string text = friend_.first_name.Trim();
		string text2 = friend_.middle_name.Trim();
		string text3 = friend_.last_name.Trim();
		List<string> list = new List<string>();
		if (text.Length > 0)
		{
			list.Add(text);
		}
		if (text2.Length > 0)
		{
			list.Add(text2);
		}
		if (text3.Length > 0)
		{
			list.Add(text3);
		}
		return string.Join("\n", list.ToArray());
	}
}
