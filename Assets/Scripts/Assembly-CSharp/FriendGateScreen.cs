using System;
using UnityEngine;
using dinobash;

public class FriendGateScreen : BaseScreen
{
    private int level_id;

    private tk2dTextMesh label_timer;

    private tk2dTextMesh label_progress;

    private StandardButton btn_unlock_now;

    private bool intital_unlockedviaFriends_state;

    private void Start()
    {
        StandardButton component = base.transform.Search("btn_close").GetComponent<StandardButton>();
        component.clickSound = Sounds.main_close_popup;
        component.uiItem.OnClick += delegate
        {
            Hide();
            ScreenManager.GetScreen<MapScreen>().Show();
        };
        base.transform.Search("btn_askfriends").GetComponent<tk2dUIItem>().OnClick += delegate
        {
            Hide();
            if (isUnlockedViaFriends())
            {
                Tracking.pass_gate(Tracking.GateMethod.friends, level_id);
                ScreenManager.GetScreen<MapScreen>().unlockFriendGate(level_id);
            }
            //else if (App.Instance.facebookManager.isLoggedIn)
            //{
            //	if (App.Instance.InternetConnectivity)
            //	{
            //		ScreenManager.GetScreen<SelectFriendsScreen>().Show(FacebookManager.AppRequestType.ASK_FOR_FRIENDGATE);
            //	}
            //	else
            //	{
            //		ErrorMessageScreen.ShowError(ErrorMessages.NO_INTERNET_CONNECTIVITY_ERROR);
            //	}
            //}
            else
            {
                //ScreenManager.GetScreen<FacebookLoginRequestScreen>().Show(FacebookManager.AppRequestType.ASK_FOR_FRIENDGATE);
            }
        };
        //base.transform.Search("label_diamond_price").GetComponent<tk2dTextMesh>().text = Konfiguration.GameConfig.FriendGateDiamondCost.ToString();
        base.transform.Search("btn_buy").GetComponent<tk2dUIItem>().OnClick += delegate
        {
            Hide();
            //if (Wallet.Diamonds >= Konfiguration.GameConfig.FriendGateDiamondCost)
            //{
            //	Wallet.TakeDiamonds(Konfiguration.GameConfig.FriendGateDiamondCost);
            //	ScreenManager.GetScreen<MapScreen>().unlockFriendGate(level_id);
            //	Tracking.pass_gate(Tracking.GateMethod.paid, level_id);
            //}
            //else
            //{
            //	Tracking.store_open(Wallet.Currency.Diamonds, "friendgate_screen", "friendgate_unlock");
            //	ScreenManager.GetScreen<ShopScreenDiamonds>().Show();
            //}
        };
        label_timer = base.transform.Search("label_timer").GetComponent<tk2dTextMesh>();
        label_progress = base.transform.Search("label_progress").GetComponent<tk2dTextMesh>();
        label_progress.maxChars = 3;
        btn_unlock_now = base.transform.Search("btn_unlock_now").GetComponent<StandardButton>();
        btn_unlock_now.uiItem.OnClick += delegate
        {
            Hide();
            Tracking.pass_gate(Tracking.GateMethod.waited, level_id);
            ScreenManager.GetScreen<MapScreen>().unlockFriendGate(level_id);
        };
        btn_unlock_now.gameObject.SetActive(false);
        base.transform.localPosition += base.left;
        base.gameObject.SetActive(false);
    }

    private void Update()
    {
        TimeSpan time_span;
        Player.Instance.getFriendGateDuration(level_id, out time_span);
        if (time_span.TotalMilliseconds <= 0.0)
        {
            btn_unlock_now.gameObject.SetActive(true);
            label_timer.transform.parent.gameObject.SetActive(false);
            return;
        }
        label_timer.text = time_span.Humanize();
        label_timer.maxChars = label_timer.text.Length;
        label_timer.Commit();
        label_progress.text = Mathf.Min(Player.Instance.numberOfFriendGateHelpers, 3) + "/3";
        if (!intital_unlockedviaFriends_state && isUnlockedViaFriends())
        {
            intital_unlockedviaFriends_state = true;
            base.transform.Search("label_askfriends").GetComponent<LocalizedText>().Key = "Unlock now";
            base.transform.Search("FB").GetComponent<SpriteRenderer>().color = Colors.Invisible;
        }
    }

    public override void Hide()
    {
        ScreenManager.GetScreen<CoverScreen>(this).Hide();
        base.Interactive = false;
        HideTo(base.left, delegate
        {
            base.Hide();
            base.gameObject.SetActive(false);
        });
    }

    private bool isUnlockedViaFriends()
    {
        return Player.Instance.numberOfFriendGateHelpers >= 3;
    }

    public void Show(int level_id)
    {
        base.gameObject.SetActive(true);
        this.level_id = level_id;
        OnEscapeUp = Hide;
        base.Show();
        ShowFrom(base.right);
        ScreenManager.GetScreen<CoverScreen>(this).Show();
        TimeSpan time_span;
        if (!Player.Instance.getFriendGateDuration(level_id, out time_span))
        {
            Player.Instance.startFriendgateTimer();
        }
        intital_unlockedviaFriends_state = isUnlockedViaFriends();
        if (isUnlockedViaFriends())
        {
            base.transform.Search("label_askfriends").GetComponent<LocalizedText>().Key = "Unlock now";
            base.transform.Search("FB").GetComponent<SpriteRenderer>().color = Colors.Invisible;
        }
        else
        {
            base.transform.Search("label_askfriends").GetComponent<LocalizedText>().Key = "Ask friends";
            base.transform.Search("FB").GetComponent<SpriteRenderer>().color = Colors.Visible;
        }
    }
}
