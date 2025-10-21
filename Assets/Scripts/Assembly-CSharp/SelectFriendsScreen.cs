using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectFriendsScreen : BaseScreen
{
	private tk2dUIScrollableArea scrollView;

	private StandardButton closeButton;

	private tk2dUIItem ask_friends_button;

	private GameObject entryDefinition;

	private List<FriendPannel> pannels = new List<FriendPannel>();

	private SpriteRenderer icon_heart;

	private SpriteRenderer icon_friendgate;

	private SpriteRenderer icon_connecting;

	private bool started;

	private FacebookManager.AppRequestType request_type;

	protected void Start()
	{
		if (started)
		{
			return;
		}
		closeButton = base.transform.Search("btn_close").GetComponent<StandardButton>();
		closeButton.clickSound = Sounds.main_close_popup;
		closeButton.uiItem.OnClick += Hide;
		ask_friends_button = base.transform.Search("submit_button").GetComponent<tk2dUIItem>();
		ask_friends_button.OnClick += OnAskFriendsClicked;
		icon_heart = base.transform.Search("icon_heart").GetComponent<SpriteRenderer>();
		icon_friendgate = base.transform.Search("icon_friendgate").GetComponent<SpriteRenderer>();
		icon_connecting = base.transform.Search("icon_connecting").GetComponent<SpriteRenderer>();
		scrollView = base.transform.FindChild("MiddleCenter/ScrollableArea").GetComponent<tk2dUIScrollableArea>();
		entryDefinition = scrollView.transform.FindChild("Content/Friend_Entry_Definition").gameObject;
		entryDefinition.SetActive(false);
		base.transform.localPosition += base.left;
		FriendPannel select_all = FindChildComponent<FriendPannel>("MiddleCenter/select_all");
		select_all.button.uiItem.OnClick += delegate
		{
			foreach (FriendPannel pannel in pannels)
			{
				pannel.selected = !select_all.selected;
			}
		};
		started = true;
		base.gameObject.SetActive(false);
	}

	private void OnAskFriendsClicked()
	{
		List<FacebookManager.Friend> list = new List<FacebookManager.Friend>();
		foreach (FriendPannel pannel in pannels)
		{
			if (pannel.selected)
			{
				Debug.Log("Selected Friend: " + pannel.friend.name);
				list.Add(pannel.friend);
			}
		}
		if (list.Count > 0)
		{
			if (App.Instance.InternetConnectivity)
			{
				App.Instance.facebookManager.sendAppRequest(request_type, list);
				SocialGamingManager.Instance.ReportProgress(AchievementIds.GIVE_A_LIVE_TO_A_FRIEND, 1);
				Hide();
			}
			else
			{
				ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
			}
		}
		else
		{
			Debug.Log("FB-Friend-Selector: no friends selected");
		}
	}

	private void SetupEntries(List<FacebookManager.Friend> friends)
	{
		if (friends == null)
		{
			Hide();
			return;
		}
		App.Instance.facebookManager.getAppRequests(delegate(List<FacebookManager.AppRequest> requests)
		{
			Debug.Log("AppRequests:" + requests.ToPrettyString());
		});
		int count = friends.Count;
		for (int i = 0; i < count; i++)
		{
			FacebookManager.Friend friend = friends[i];
			Debug.Log("FB friend: name=" + friend.name + ", id=" + friend.id);
			GameObject gameObject = UnityEngine.Object.Instantiate(entryDefinition) as GameObject;
			gameObject.SetActive(true);
			gameObject.transform.parent = entryDefinition.transform.parent;
			gameObject.transform.position = entryDefinition.transform.position;
			gameObject.transform.LocalPosX(480 * (i % 3));
			gameObject.transform.LocalPosY(-200 * (i / 3));
			FriendPannel component = gameObject.GetComponent<FriendPannel>();
			component.selected = true;
			component.friend = friend;
			pannels.Add(component);
		}
		scrollView.ContentLength = (count / 3 + 2) * 200 + 100;
		icon_connecting.enabled = false;
	}

	public void Show(FacebookManager.AppRequestType request_type)
	{
		Start();
		base.gameObject.SetActive(true);
		ScreenManager.GetScreen<CoverScreen>(this).Show();
		this.request_type = request_type;
		icon_heart.enabled = request_type == FacebookManager.AppRequestType.ASK_FOR_LIVES;
		icon_friendgate.enabled = request_type == FacebookManager.AppRequestType.ASK_FOR_FRIENDGATE;
		icon_connecting.enabled = true;
		ShowFrom(base.right, base.Show);
		foreach (FriendPannel pannel in pannels)
		{
			UnityEngine.Object.Destroy(pannel.gameObject);
		}
		pannels.Clear();
		FriendPannel friendPannel = FindChildComponent<FriendPannel>("MiddleCenter/select_all");
		friendPannel.selected = true;
		App.Instance.facebookManager.getFriends(SetupEntries, request_type);
	}

	public override void Hide()
	{
		base.Hide();
		try
		{
			ScreenManager.GetScreen<CoverScreen>(this).Hide();
		}
		catch (Exception)
		{
		}
		HideTo(base.left, delegate
		{
			if (App.State == App.States.Game)
			{
				ScreenManager.GetScreen<GetLivesScreen>().Show_Delayed();
			}
		});
	}
}
