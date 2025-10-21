using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class MapTutorial : AbstractTutorialScreen
{
    private TutorialArrow arrow;

    private FocusMask mask;

    private LevelButton[] levels;

    private MapScreen map_screen;

    private Action map_screenOnEscapeCallback;

    private UpgradeScreen upgrade_screen;

    private UpgradeInfoDinoScreen info_screen;

    private ResourceBarScreen ressource_bar;

    private tk2dUIScrollableArea dino_scroll_view;

    private tk2dUIItem bg_panel;

    private tk2dUIItem button_upgrade;

    private int raptor_level;

    private tk2dUIItem close_button;

    private List<tk2dUIItem> chached_uiItems = new List<tk2dUIItem>();

    private int tut_coins;

    private int tut_diamonds;

    protected override void Start()
    {
        levels = UnityEngine.Object.FindObjectsOfType<LevelButton>();
        base.Start();
        arrow = getArrow();
        mask = getFocusMask();
        if (Player.MaxLevelID == Tutorials.LevelID("PanOverMap"))
        {
            DisableUpgradeButton();
            if (!Player.HasPlayedMaxLevelID)
            {
                PanOverMap();
            }
        }
        else if (Player.MaxLevelID == Tutorials.LevelID("UpgradeTutorial"))
        {
            if (!Player.WatchedUpgradeTutorial && Player.LooseCount > 0 && Player.GetUnitLevel(UnitType.Raptor) == 0 && !EntityTimers.is_upgrading(UnitType.Raptor) && Wallet.Coins >= Konfiguration.getDinoUpgradeCost(UnitType.Raptor))
            {
                StartCoroutine(UpgradeTutorial_OpenUpgradeScreen());
            }
            else if (!Player.WatchedUpgradeTutorial)
            {
                DisableUpgradeButton();
            }
        }
        else if (Player.MaxLevelID == Tutorials.LevelID("UpgradeTutorial") + 1)
        {
            if (!Player.WatchedUpgradeTutorial && Player.GetUnitLevel(UnitType.Raptor) == 0 && !EntityTimers.is_upgrading(UnitType.Raptor) && Wallet.Coins >= Konfiguration.getDinoUpgradeCost(UnitType.Raptor))
            {
                StartCoroutine(UpgradeTutorial_OpenUpgradeScreen());
            }
        }
        else if (Player.MaxLevelID == Tutorials.LevelID("FriendGate_Tutorial"))
        {
            StartCoroutine(FriendgatTutorial());
        }
        else if (Player.MaxLevelID == Tutorials.LevelID("dialog_map3_start") || Player.MaxLevelID == Tutorials.LevelID("dialog_map4_start") || Player.MaxLevelID == Tutorials.LevelID("dialog_map5_start"))
        {
            if (!Player.HasPlayedMaxLevelID)
            {
                StartCoroutine(StartDialogue(Player.MaxLevelID));
            }
        }
        else
        {
            if (Player.MaxLevelID < Tutorials.LevelID("UpgradeTutorial"))
            {
                DisableUpgradeButton();
                WaitThen(1.25f, delegate
                {
                    MarkCurrentLevel();
                });
            }
            if (Player.MaxLevelID > Tutorials.LevelID("UpgradeTutorial") && Player.WatchedUpgradeTutorial && !Player.WatchedUpgradeTutorial2nd)
            {
                Start_UpgradeTutorial_2nd();
            }
        }
        map_screen = ScreenManager.GetScreen<MapScreen>();
        Transform transform = map_screen.transform.Find("LowerRight/UpgradeTip");
        bool flag = Player.MaxLevelID < Tutorials.LevelID("FriendGate_Tutorial") && Player.WatchedUpgradeTutorial;
        transform.gameObject.SetActive(flag);
        //if (Player.MaxLevelID <= Konfiguration.GameConfig.First_quest_level)
        //{
        //	DisableQuestButton();
        //}
    }

    private LevelButton MarkCurrentLevel()
    {
        LevelButton levelButton = Array.Find(levels, (LevelButton x) => x.Level_ID == Player.MaxLevelID);
        arrow.transform.RepositionAndReparent(levelButton.transform.Find("top/dino_icon"));
        arrow.transform.rotation = Quaternion.identity;
        SpriteTools.SetSortingLayerID(arrow.transform, base.SortingLayerID);
        SpriteTools.SetSortingOrder(arrow.transform, 950);
        arrow.In();
        return levelButton;
    }

    private StandardButton getMapButton(string name)
    {
        MapScreen screen = ScreenManager.GetScreen<MapScreen>();
        return screen.transform.Search(name).GetComponent<StandardButton>();
    }

    private void DisableUpgradeButton(bool hide = true)
    {
        DisableButton(getMapButton("Upgrade_Button"), hide);
    }

    private void DisableQuestButton(bool hide = true)
    {
        DisableButton(getMapButton("Quest_Button"), hide);
    }

    private void DisableButton(StandardButton button, bool hide = true)
    {
        if (hide)
        {
            button.gameObject.SetActive(false);
        }
        else
        {
            button.uiItem.enabled = false;
        }
    }

    private void PanOverMap()
    {
        SetDialogTween(base.top, new Vector3(0f, 880f, 0f));
        SetMapScrollingInput(false);
        DisableMapScreens();
        Parallaxer parallaxer = ScreenManager.GetScreen<MapScreen>().parallaxer;
        parallaxer.CamX = -500f;
        Go.to(parallaxer, 6f, new GoTweenConfig().setEaseType(GoEaseType.SineInOut).floatProp("CamX", -180f).onComplete(delegate
        {
            ShowDialog("PanOverMap_00", true);
            Go.to(parallaxer, 3f, new GoTweenConfig().setDelay(4f).setEaseType(GoEaseType.CubicInOut).floatProp("CamX", 28f)
                .onIterationStart(delegate
                {
                    HideDialog();
                })
                .onComplete(delegate
                {
                    MarkCurrentLevel();
                    EnableMapScreens();
                    WaitThen(3f, delegate
                    {
                        if (App.State == App.States.Map)
                        {
                            App.stateGame(Tutorials.LevelID("PanOverMap"));
                        }
                    });
                }));
        }));
    }

    protected override void OnDestroy()
    {
        Wallet.OnBalanceChanged -= update_balance;
        base.OnDestroy();
    }

    private void DisableMapScreens()
    {
        map_screen = ScreenManager.GetScreen<MapScreen>();
        SetMapScrollingInput(false);
        map_screenOnEscapeCallback = map_screen.OnEscapeUp;
        map_screen.OnEscapeUp = null;
        upgrade_screen = ScreenManager.GetScreen<UpgradeScreen>();
        info_screen = ScreenManager.GetScreen<UpgradeInfoDinoScreen>();
        ressource_bar = ScreenManager.GetScreen<ResourceBarScreen>();
        DisableUiItems(ressource_bar);
        DisableUiItems(map_screen);
    }

    private void EnableMapScreens()
    {
        map_screen.OnEscapeUp = map_screenOnEscapeCallback;
        SetMapScrollingInput(true);
        EnableUiItems();
    }

    private IEnumerator UpgradeTutorial_OpenUpgradeScreen()
    {
        Tracking.level_004_start_tutorial();
        SetDialogTween(base.top, new Vector3(0f, 625f, 0f));
        DisableMapScreens();
        yield return null;
        ShowDialog("Upgrade your raptor!", true);
        Transform upgradeButton = map_screen.transform.Find("LowerRight/Upgrade_Button");
        arrow.transform.RepositionAndReparent(upgradeButton.transform);
        arrow.transform.position += new Vector3(220f, 0f, 0f);
        yield return new WaitForSeconds(1f);
        mask.Show(upgradeButton.transform.position + new Vector3(220f, 0f, 0f), Vector3.one * 7f);
        yield return new WaitForSeconds(0.75f);
        arrow.In();
        upgradeButton.GetComponent<tk2dUIItem>().enabled = true;
        upgrade_screen.OnScreenShow += Show_SwitchTab;
    }

    private void Show_SwitchTab()
    {
        upgrade_screen.OnScreenShow -= Show_SwitchTab;
        arrow.Out();
        mask.Hide();
        HideDialog();
        upgrade_screen.OnEscapeUp = null;
        StartCoroutine(UpgradeTutorial_SwitchTab());
    }

    private IEnumerator UpgradeTutorial_SwitchTab()
    {
        DisableUiItems(upgrade_screen);
        yield return new WaitForSeconds(1f);
        SpriteTools.SetSortingLayerID(arrow, ressource_bar.SortingLayerID);
        SpriteTools.OffsetSortingOrder(arrow, 20000);
        SpriteTools.SetSortingLayerID(mask, ressource_bar.SortingLayerID);
        SpriteTools.OffsetSortingOrder(mask, 19000);
        mask.transform.LocalPosZ(ressource_bar.transform.localPosition.z);
        mask.transform.localScale = new Vector3(1f, 1f, 0f);
        dino_scroll_view = upgrade_screen.transform.Find("MiddleCenter/Dinos/ScrollableArea").GetComponent<tk2dUIScrollableArea>();
        dino_scroll_view.enabled = false;
        StandardButton[] upgrade_buttons = dino_scroll_view.GetComponentsInChildren<StandardButton>();
        Array.ForEach(upgrade_buttons, delegate (StandardButton x)
        {
            x.uiItem.enabled = false;
        });
        tk2dUIItem raptor_button = Array.Find(upgrade_buttons, (StandardButton x) => x.transform.parent.name == UnitType.Raptor.ToString()).uiItem;
        bg_panel = raptor_button.transform.parent.Find("bg_panel").GetComponent<tk2dUIItem>();
        bg_panel.enabled = true;
        bg_panel.OnClick += Show_UpgradeRaptor;
        arrow.transform.RepositionAndReparent(raptor_button.transform);
        arrow.transform.localPosition += new Vector3(0f, 0f, -10f);
        arrow.In();
        mask.Show(raptor_button.transform.position, 6f);
        while (Player.GetUnitLevel(UnitType.Raptor) == 0)
        {
            yield return null;
        }
        Show_CloseScreen();
    }

    private void Show_UpgradeRaptor()
    {
        bg_panel.OnClick -= Show_UpgradeRaptor;
        StopAllCoroutines();
        mask.Hide();
        arrow.Out();
        info_screen.OnEscapeUp = null;
        StartCoroutine(UpgradeTutorial_UpgradeRaptor());
    }

    private IEnumerator UpgradeTutorial_UpgradeRaptor()
    {
        DisableUiItems(info_screen);
        yield return new WaitForSeconds(1f);
        button_upgrade = info_screen.transform.Find("MiddleCenter/btn_upgrade_group/normal/btn_upgrade").GetComponent<tk2dUIItem>();
        button_upgrade.enabled = true;
        arrow.transform.RepositionAndReparent(button_upgrade.transform);
        arrow.transform.parent = null;
        arrow.In();
        mask.Show(button_upgrade.transform.position, 6f);
        raptor_level = Player.GetUnitLevel(UnitType.Raptor);
        button_upgrade.OnClick += Show_CloseScreen;
    }

    private void Show_CloseScreen()
    {
        button_upgrade.OnClick -= Show_CloseScreen;
        button_upgrade.enabled = false;
        arrow.Out();
        mask.Hide();
        StartCoroutine(UpgradeTutorial_CloseScreen());
    }

    private IEnumerator UpgradeTutorial_CloseScreen()
    {
        yield return new WaitForSeconds(0.5f);
        //if (Konfiguration.GameConfig.Use_upgrade_timers)
        //{
        //	SpriteTools.SetSortingLayerID(dialog_label.transform.parent, ressource_bar.SortingLayerID);
        //	SpriteTools.OffsetSortingOrder(dialog_label.transform.parent, 20000);
        //	base.transform.position = ressource_bar.transform.position;
        //	SetDialogTween(base.top, new Vector3(0f, 1072f, 0f));
        //	ShowDialog("skip_training_with_diamonds", true);
        //	button_upgrade.enabled = true;
        //	arrow.In();
        //}
        while (raptor_level == Player.GetUnitLevel(UnitType.Raptor))
        {
            yield return null;
        }
        //if (Konfiguration.GameConfig.Use_upgrade_timers)
        //{
        //	if (base.dialog_open)
        //	{
        //		HideDialog();
        //	}
        //	button_upgrade.enabled = false;
        //	arrow.Out();
        //}
        yield return new WaitForSeconds(1f);
        //if (Konfiguration.GameConfig.Use_upgrade_timers)
        //{
        //	close_button = info_screen.transform.Find("MiddleCenter/btn_close").GetComponent<tk2dUIItem>();
        //	close_button.enabled = true;
        //	arrow.transform.RepositionAndReparent(close_button.transform);
        //	arrow.transform.parent = null;
        //	arrow.transform.localScale = Vector3.one;
        //	arrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        //	arrow.In();
        //	mask.Show(close_button.transform.position, 3f);
        //}
        while (info_screen.Interactive)
        {
            yield return null;
        }
        //if (Konfiguration.GameConfig.Use_upgrade_timers)
        //{
        //	arrow.Out();
        //	mask.Hide();
        //}
        button_upgrade.enabled = true;
        DisableUiItems(upgrade_screen);
        yield return new WaitForSeconds(0.5f);
        close_button = upgrade_screen.transform.Find("MiddleCenter/btn_close").GetComponent<tk2dUIItem>();
        close_button.enabled = true;
        arrow.transform.RepositionAndReparent(close_button.transform);
        arrow.transform.parent = null;
        arrow.transform.localScale = Vector3.one;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        arrow.In();
        mask.Show(close_button.transform.position, 3f);
        close_button.OnClick += UpgradeTutorial_End;
    }

    private void UpgradeTutorial_End()
    {
        close_button.OnClick -= UpgradeTutorial_End;
        arrow.Out();
        mask.Hide();
        dino_scroll_view.enabled = true;
        EnableMapScreens();
        SetDialogTween(base.top, new Vector3(0f, 880f, 0f));
        ShowDialog("Now teach them a lesson!");
        Player.WatchedUpgradeTutorial = true;
    }

    private void DisableUiItems(Component c)
    {
        tk2dUIItem[] componentsInChildren = c.GetComponentsInChildren<tk2dUIItem>(true);
        tk2dUIItem[] array = componentsInChildren;
        foreach (tk2dUIItem tk2dUIItem2 in array)
        {
            if (tk2dUIItem2.enabled)
            {
                tk2dUIItem2.enabled = false;
                if (!chached_uiItems.Contains(tk2dUIItem2))
                {
                    chached_uiItems.Add(tk2dUIItem2);
                }
            }
        }
    }

    private void SetMapScrollingInput(bool state)
    {
        ScreenManager.GetScreen<MapScreen>().parallaxer.HandleInput = state;
        ScreenManager.GetScreen<MapScreen>().enabled = state;
    }

    private void EnableUiItems()
    {
        chached_uiItems.ForEach(delegate (tk2dUIItem x)
        {
            x.enabled = true;
        });
        chached_uiItems.Clear();
    }

    private void Start_UpgradeTutorial_2nd()
    {
        if (Wallet.Coins >= Konfiguration.getDinoUpgradeCost(UnitType.Raptor))
        {
            upgrade_screen = ScreenManager.GetScreen<UpgradeScreen>();
            upgrade_screen.OnScreenShow -= UpgradeTutorial_2nd_Proxy;
            upgrade_screen.OnScreenShow += UpgradeTutorial_2nd_Proxy;
        }
    }

    private void UpgradeTutorial_2nd_Proxy()
    {
        if (!Player.WatchedUpgradeTutorial2nd)
        {
            StartCoroutine(UpgradeTutorial_2nd());
        }
    }

    private void HandleTabChanged(UpgradeScreen.Tab tab)
    {
        if (tab == UpgradeScreen.Tab.dinos)
        {
            arrow.In();
        }
    }

    private void update_balance()
    {
        if (Wallet.Coins > tut_coins)
        {
            tut_coins = Wallet.Coins;
        }
        if (Wallet.Diamonds > tut_diamonds)
        {
            tut_diamonds = Wallet.Diamonds;
        }
    }

    private IEnumerator UpgradeTutorial_2nd()
    {
        dino_scroll_view = upgrade_screen.transform.Find("MiddleCenter/Dinos/ScrollableArea").GetComponent<tk2dUIScrollableArea>();
        StandardButton[] upgrade_buttons = dino_scroll_view.GetComponentsInChildren<StandardButton>();
        tk2dUIItem raptor_button = Array.Find(upgrade_buttons, (StandardButton x) => x.transform.parent.name == UnitType.Raptor.ToString()).uiItem;
        SpriteTools.SetSortingLayerID(arrow, upgrade_screen.SortingLayerID);
        SpriteTools.OffsetSortingOrder(arrow, 500);
        arrow.transform.RepositionAndReparent(raptor_button.transform);
        arrow.transform.localPosition += new Vector3(100f, 0f, 0f);
        arrow.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 270f));
        arrow.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        arrow.In();
        upgrade_screen.OnTabChanged -= HandleTabChanged;
        upgrade_screen.OnTabChanged += HandleTabChanged;
        Wallet.OnBalanceChanged -= update_balance;
        Wallet.OnBalanceChanged += update_balance;
        update_balance();
        bool did_upgrade = false;
        while (!did_upgrade && App.State == App.States.Map)
        {
            did_upgrade = Wallet.Coins < tut_coins || Wallet.Diamonds < tut_diamonds;
            yield return new WaitForSeconds(0.5f);
        }
        if (did_upgrade)
        {
            Player.WatchedUpgradeTutorial2nd = true;
            arrow.Out();
            Wallet.OnBalanceChanged -= update_balance;
            upgrade_screen.OnTabChanged -= HandleTabChanged;
        }
    }

    private IEnumerator FriendgatTutorial()
    {
        StandardButton upgrade_button = getMapButton("Upgrade_Button");
        GoTween upgrade_button_tween = Go.to(upgrade_button.transform, 0.5f, new GoTweenConfig().localPosition(upgrade_button.transform.localPosition.SetX(512f)).setEaseType(GoEaseType.CubicIn));
        upgrade_button_tween.autoRemoveOnComplete = false;
        StandardButton quest_button = getMapButton("Quest_Button");
        GoTween quest_button_tween = null;
        if (quest_button != null)
        {
            quest_button_tween = Go.to(quest_button.transform, 0.5f, new GoTweenConfig().localPosition(quest_button.transform.localPosition.SetX(512f)).setEaseType(GoEaseType.CubicIn));
            quest_button_tween.autoRemoveOnComplete = false;
        }
        SetDialogTween(base.top, base.upperDialogPosition);
        yield return null;
        DisableMapScreens();
        yield return new WaitForSeconds(1.5f);
        LevelButton level_button = MarkCurrentLevel();
        level_button.button.uiItem.enabled = true;
        SpriteTools.SetSortingOrder(arrow.transform, -1);
        SpriteTools.SetSortingLayerID(dialog_label.transform.parent, ressource_bar.SortingLayerID);
        SpriteTools.OffsetSortingOrder(dialog_label.transform.parent, 20000);
        ShowDialog("Friendgate_Tutorial_00", true);
        FriendGateScreen friendgate_screen = ScreenManager.GetScreen<FriendGateScreen>();
        while (!friendgate_screen.isVisible)
        {
            yield return null;
        }
        DisableUiItems(friendgate_screen);
        HideDialog();
        yield return new WaitForSeconds(0.3f);
        SetDialogTween(base.top, new Vector3(0f, 1125f, 0f));
        SpriteTools.SetSortingLayerID(mask, ressource_bar.SortingLayerID);
        SpriteTools.OffsetSortingOrder(mask, 15000);
        Transform btn_unlock = friendgate_screen.transform.Search("btn_askfriends");
        yield return new WaitForSeconds(1f);
        arrow.Out();
        mask.Show(btn_unlock.position, 6f);
        ShowDialog("Friendgate_Tutorial_02", true);
        bool arrow_repositioned = false;
        while (friendgate_screen.isVisible)
        {
            yield return new WaitForSeconds(2f);
            if (!friendgate_screen.isVisible)
            {
                break;
            }
            int sorting_order = 16000;
            Action<string, string> show_portrait_sprite = delegate (string target, string spritename)
            {
                SpriteRenderer component = friendgate_screen.transform.Search(target).GetComponent<SpriteRenderer>();
                SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate(component) as SpriteRenderer;
                spriteRenderer.transform.RepositionAndReparent(component.transform);
                spriteRenderer.sprite = SpriteRessources.GetSprite(spritename);
                spriteRenderer.sortingLayerID = ressource_bar.SortingLayerID;
                spriteRenderer.sortingOrder = ++sorting_order;
                Go.from(spriteRenderer.transform, 0.5f, new GoTweenConfig().position(turtleAnimator.transform.position).setEaseType(GoEaseType.CubicOut));
            };
            show_portrait_sprite("icon_person_1", "person_dino_a");
            yield return new WaitForSeconds(0.5f);
            Player.Instance.addFriendGateHelper();
            if (!friendgate_screen.isVisible)
            {
                break;
            }
            show_portrait_sprite("icon_person_2", "person_dino_b");
            yield return new WaitForSeconds(0.5f);
            Player.Instance.addFriendGateHelper();
            if (!friendgate_screen.isVisible)
            {
                break;
            }
            show_portrait_sprite("icon_person_3", "person_dino_c");
            yield return new WaitForSeconds(0.5f);
            Player.Instance.addFriendGateHelper();
            btn_unlock.GetComponent<tk2dUIItem>().enabled = true;
            if (!friendgate_screen.isVisible)
            {
                break;
            }
            yield return new WaitForSeconds(1.5f);
            if (!friendgate_screen.isVisible)
            {
                break;
            }
            HideDialog();
            yield return new WaitForSeconds(0.3f);
            if (!friendgate_screen.isVisible)
            {
                break;
            }
            SpriteTools.SetSortingLayerID(arrow, ressource_bar.SortingLayerID);
            SpriteTools.OffsetSortingOrder(arrow, 20000);
            arrow.transform.RepositionAndReparent(btn_unlock);
            arrow.transform.localPosition += new Vector3(0f, 0f, -10f);
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            arrow.In();
            arrow_repositioned = true;
            ShowDialog("Friendgate_Tutorial_03", true);
            while (friendgate_screen.isVisible)
            {
                yield return null;
            }
        }
        if (base.dialog_open)
        {
            HideDialog();
        }
        if (arrow_repositioned)
        {
            arrow.Out();
        }
        mask.Hide();
        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(4f);
        SetDialogTween(base.top, base.upperDialogPosition);
        ShowDialog("Friendgate_Tutorial_04");
        while (base.dialog_open)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        EnableMapScreens();
        upgrade_button_tween.playBackwards();
        if (quest_button_tween != null)
        {
            quest_button_tween.playBackwards();
        }
        MarkCurrentLevel();
        DisableQuestButton(false);
        WaitThen(3f, delegate
        {
            if (App.State == App.States.Map)
            {
                App.stateGame(Tutorials.LevelID("FriendGate_Tutorial") + 2);
            }
        });
    }

    public IEnumerator StartDialogue(int level_id)
    {
        SetDialogTween(base.top, new Vector3(0f, 880f, 0f));
        SetMapScrollingInput(false);
        DisableMapScreens();
        yield return new WaitForSeconds(1.5f);
        ShowDialog(Tutorials.LocaKeyForLevel(level_id));
        while (base.dialog_open)
        {
            yield return null;
        }
        EnableMapScreens();
        SetMapScrollingInput(true);
    }
}
