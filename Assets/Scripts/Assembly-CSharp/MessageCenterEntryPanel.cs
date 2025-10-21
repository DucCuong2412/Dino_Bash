using System;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class MessageCenterEntryPanel : MonoBehaviour
{
	private Transform icon_acceptLife;

	private Transform icon_requestLife;

	private Transform icon_friendgate;

	private LocalizedText button_label;

	private LocalizedText entry_title;

	private TextMesh entry_description;

	private TextSize textSize;

	public Action onDeleteDone;

	private bool started;

	private FacebookManager.AppRequest _apprequest;

	public StandardButton button { get; private set; }

	public FacebookManager.AppRequest apprequest
	{
		get
		{
			return _apprequest;
		}
		set
		{
			_apprequest = value;
			SetEntry();
		}
	}

	private void Start()
	{
		if (!started)
		{
			started = true;
			icon_acceptLife = base.transform.Find("icon_acceptLife");
			icon_requestLife = base.transform.Find("icon_requestLife");
			icon_friendgate = base.transform.Find("icon_friendgate");
			entry_title = base.transform.Find("label_title").GetComponent<LocalizedText>();
			entry_description = base.transform.Find("label_description").GetComponent<TextMesh>();
			textSize = new TextSize(entry_description);
			button = base.transform.Find("btn_action").GetComponent<StandardButton>();
			button_label = base.transform.Search("label_actionButton").GetComponent<LocalizedText>();
			button.uiItem.OnClick += OnAcceptClicked;
		}
	}

	public void OnAcceptClicked()
	{
		button.uiItem.OnClick -= OnAcceptClicked;
		App.Instance.facebookManager.deleteAppRequest(_apprequest, delegate
		{
			FacebookManager.Friend item = default(FacebookManager.Friend);
			item.first_name = (item.name = _apprequest.data.from_first_name);
			item.middle_name = (item.last_name = string.Empty);
			item.id = _apprequest.data.from_id;
			List<FacebookManager.Friend> friends = new List<FacebookManager.Friend> { item };
			switch (_apprequest.data.type)
			{
			case FacebookManager.AppRequestType.ASK_FOR_LIVES:
				App.Instance.facebookManager.sendAppRequest(FacebookManager.AppRequestType.GIVE_LIVE, friends);
				break;
			case FacebookManager.AppRequestType.GIVE_LIVE:
				Player.Lives++;
				App.Instance.facebookManager.sendAppRequest(FacebookManager.AppRequestType.GIVE_LIVE, friends);
				break;
			case FacebookManager.AppRequestType.ASK_FOR_FRIENDGATE:
				App.Instance.facebookManager.sendAppRequest(FacebookManager.AppRequestType.HELP_FRIENDGATE, friends);
				break;
			case FacebookManager.AppRequestType.HELP_FRIENDGATE:
				Player.Instance.addFriendGateHelper();
				break;
			}
			if (onDeleteDone != null)
			{
				onDeleteDone();
			}
		});
	}

	private void SetEntry()
	{
		Start();
		icon_acceptLife.gameObject.SetActive(false);
		icon_requestLife.gameObject.SetActive(false);
		icon_friendgate.gameObject.SetActive(false);
		entry_title.Key = _apprequest.data.type.ToString() + ".title";
		button_label.Key = _apprequest.data.type.ToString() + ".button";
		switch (_apprequest.data.type)
		{
		case FacebookManager.AppRequestType.ASK_FOR_FRIENDGATE:
			icon_friendgate.gameObject.SetActive(true);
			break;
		case FacebookManager.AppRequestType.HELP_FRIENDGATE:
			icon_friendgate.gameObject.SetActive(true);
			break;
		case FacebookManager.AppRequestType.ASK_FOR_LIVES:
			icon_requestLife.gameObject.SetActive(true);
			break;
		case FacebookManager.AppRequestType.GIVE_LIVE:
			icon_acceptLife.gameObject.SetActive(true);
			break;
		}
		string text = string.Format(i18n.Get(_apprequest.data.type.ToString() + ".message"), apprequest.data.from_first_name);
		entry_description.text = text;
		textSize.FitToWidth(700f);
	}
}
