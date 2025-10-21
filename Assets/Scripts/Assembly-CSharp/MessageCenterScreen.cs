using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageCenterScreen : BaseScreen
{
	private tk2dUIScrollableArea scrollView;

	private StandardButton closeButton;

	private tk2dUIItem accept_all_button;

	private GameObject entryDefinition;

	private List<MessageCenterEntryPanel> pannels = new List<MessageCenterEntryPanel>();

	private SpriteRenderer icon_connecting;

	private bool started;

	protected void Start()
	{
		if (!started)
		{
			closeButton = base.transform.Search("btn_close").GetComponent<StandardButton>();
			closeButton.uiItem.OnClick += OnCloseClick;
			closeButton.clickSound = Sounds.main_close_popup;
			icon_connecting = base.transform.Search("icon_connecting").GetComponent<SpriteRenderer>();
			accept_all_button = base.transform.Search("accept_all_button").GetComponent<tk2dUIItem>();
			accept_all_button.OnClick += OnAcceptAllClicked;
			scrollView = base.transform.FindChild("MiddleCenter/ScrollableArea").GetComponent<tk2dUIScrollableArea>();
			entryDefinition = scrollView.transform.FindChild("Content/message_Definition").gameObject;
			entryDefinition.SetActive(false);
			base.transform.localPosition += base.left;
			base.gameObject.SetActive(false);
			started = true;
		}
	}

	private void OnCloseClick()
	{
		Hide();
		ScreenManager.GetScreen<MapScreen>().Show();
	}

	private void OnAcceptAllClicked()
	{
		foreach (MessageCenterEntryPanel pannel in pannels)
		{
			pannel.onDeleteDone = null;
			pannel.OnAcceptClicked();
		}
		Hide();
	}

	private void SetupEntries(List<FacebookManager.AppRequest> app_requests)
	{
		if (app_requests == null)
		{
			Hide();
			return;
		}
		int count = app_requests.Count;
		for (int i = 0; i < count; i++)
		{
			FacebookManager.AppRequest apprequest = app_requests[i];
			GameObject gameObject = UnityEngine.Object.Instantiate(entryDefinition) as GameObject;
			gameObject.SetActive(true);
			gameObject.transform.parent = entryDefinition.transform.parent;
			gameObject.transform.position = entryDefinition.transform.position;
			gameObject.transform.LocalPosY(-250 * i);
			MessageCenterEntryPanel component = gameObject.GetComponent<MessageCenterEntryPanel>();
			component.apprequest = apprequest;
			component.onDeleteDone = (Action)Delegate.Combine(component.onDeleteDone, new Action(ReloadEntries));
			pannels.Add(component);
		}
		scrollView.ContentLength = (count + 1) * 250;
		icon_connecting.enabled = false;
	}

	private void ReloadEntries()
	{
		foreach (MessageCenterEntryPanel pannel in pannels)
		{
			UnityEngine.Object.Destroy(pannel.gameObject);
		}
		pannels.Clear();
		App.Instance.facebookManager.getAppRequests(SetupEntries);
	}

	public override void Show()
	{
		Start();
		base.gameObject.SetActive(true);
		icon_connecting.enabled = true;
		OnEscapeUp = Hide;
		ShowFrom(base.right);
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		ReloadEntries();
		base.Show();
	}

	public override void Hide()
	{
		base.Hide();
		List<FacebookManager.AppRequest> list = new List<FacebookManager.AppRequest>();
		foreach (MessageCenterEntryPanel pannel in pannels)
		{
			if (pannel.apprequest.data.type == FacebookManager.AppRequestType.HELP_FRIENDGATE)
			{
				pannel.OnAcceptClicked();
			}
			list.Add(pannel.apprequest);
		}
		App.Instance.facebookManager.deleteAppRequests(list, delegate
		{
		});
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		HideTo(base.left, delegate
		{
			base.gameObject.SetActive(false);
		});
	}
}
