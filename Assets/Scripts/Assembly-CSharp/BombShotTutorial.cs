using System;
using System.Collections;
using UnityEngine;
using dinobash;

public class BombShotTutorial : AbstractTutorialScreen
{
    private TutorialArrow btnArrow;

    private ShootButton bombButton;

    private ShootButton standardBulletBtn;

    private BuyUnitButton raptorBtn;

    private ShotCarrier shot_carrier;

    private Unit neander;

    private Transform dragArrow;

    private GoTween raptorButtonTween;

    private HudScreen hud;

    private WatchShotsOnUnitScreen watcher;

    private bool useDragShots;

    protected override void Start()
    {
        base.Start();
        //useDragShots = Konfiguration.GameConfig.Use_dragshot_feature;
        base.DinoEggInvincible = true;
        hud = ScreenManager.GetScreen<HudScreen>();
        hud.Show();
        shot_carrier = ShotFactory.GetCarrier(ShotType.Bomb);
        shot_carrier.tutorialMode = true;
        bombButton = hud.ShotButtons.Find((ShootButton btn) => btn.shotType == ShotType.Bomb);
        btnArrow = getArrow();
        btnArrow.transform.RepositionAndReparent(bombButton.transform);
        dragArrow = getSwipeArrow().transform;
        dragArrow.transform.parent = base.transform;
        Animator component = dragArrow.GetComponent<Animator>();
        component.updateMode = AnimatorUpdateMode.UnscaledTime;
        dragArrow.gameObject.SetActive(false);
        standardBulletBtn = hud.ShotButtons.Find((ShootButton x) => x.shotType == ShotType.Normal);
        standardBulletBtn.Enabled = false;
        hud.UnitButtons.ForEach(delegate (BuyUnitButton button)
        {
            button.disable = true;
        });
        SetDialogTween(base.top, base.centerDialogPosition);
        watcher = ScreenManager.LoadAndPush<WatchShotsOnUnitScreen>("Tutorial/Tutorial", this);
        if (useDragShots)
        {
            StartCoroutine(TutorialDragshotPart1());
        }
        else
        {
            StartCoroutine(TutorialPart1());
        }
    }

    private void OnDisable()
    {
        ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(OnFired));
    }

    private IEnumerator TutorialDragshotPart1()
    {
        SetDialogTween(base.top, base.upperDialogPosition);
        bombButton.uiItem.enabled = false;
        yield return new WaitForSeconds(0.1f);
        GameCamera.Instance.EnterState_Disabled();
        GameCamera.Instance.PlayIntroPan(Level.Instance.Config.levelwidth);
        neander = SpawnCavemen();
        yield return new WaitForSeconds(3.1f);
        StartAppleCap(Player.Instance.Apples);
        FollowTarget(neander.transform);
        GameCamera.Instance.EnterState_Disabled();
        dragArrow.transform.position = bombButton.transform.position;
        dragArrow.gameObject.SetActive(true);
        dragArrow.GetComponent<Animator>().Play("SwipeArrow_Drag");
        dragStart = dragArrow.transform.position;
        DragTween(dragArrow, neander.transform);
        bombButton.uiItem.OnUp += HandleOnStartDrag;
        bombButton.uiItem.enabled = true;
        bombButton.SetDragTutorial(neander.transform, DragAction);
        ShowDialog("Fire your new bomb!", true);
    }

    private void HandleOnStartDrag()
    {
        HideDialog();
        bombButton.uiItem.OnUp -= HandleOnStartDrag;
        if (dragTween != null)
        {
            dragTween.destroy();
            dragArrow.gameObject.SetActive(false);
        }
    }

    private void DragAction(bool success)
    {
        StartCoroutine(DragResult(success));
    }

    private IEnumerator DragResult(bool success)
    {
        if (success)
        {
            if (dragTween != null)
            {
                dragTween.destroy();
                dragArrow.gameObject.SetActive(false);
            }
            follow = false;
            bombButton.SetDragTutorial(null, null);
            yield return new WaitForSeconds(3f);
            StartCoroutine(TutorialDragshotPart2());
        }
        else
        {
            ShowDialog("dragshot.tutorial.3", true);
            yield return new WaitForSeconds(1f);
            dragArrow.gameObject.SetActive(true);
            dragArrow.GetComponent<Animator>().Play("SwipeArrow_Drag");
            DragTween(dragArrow, neander.transform);
            bombButton.uiItem.OnUp += HandleOnStartDrag;
        }
    }

    private IEnumerator TutorialDragshotPart2()
    {
        HideDialog();
        Tracking.fire_volcano_bomb();
        yield return new WaitForSeconds(0.2f);
        Tracking.drop_volcano_bomb();
        yield return new WaitForSeconds(0.2f);
        Tracking.detonate_volcano_bomb();
        StopAppleCap();
        GameCamera.Instance.EnterState_Disabled();
        StartCoroutine(TutorialPart3());
    }

    private IEnumerator TutorialPart1()
    {
        yield return new WaitForSeconds(0.1f);
        GameCamera.Instance.EnterState_Disabled();
        yield return new WaitForSeconds(1f);
        StartAppleCap(Player.Instance.Apples);
        ShowDialog("Fire your new bomb!", true);
        btnArrow.In();
        ShotFactory.OnShotStart = (Action)Delegate.Combine(ShotFactory.OnShotStart, new Action(OnFired));
    }

    private Unit SpawnCavemen()
    {
        Unit component = EntityFactory.Create(UnitType.Neander_Weak).GetComponent<Unit>();
        component.transform.LocalPosX(component.transform.localPosition.x - 1300f);
        Unit component2 = EntityFactory.Create(UnitType.Neander_Weak).GetComponent<Unit>();
        component2.transform.LocalPosX(component2.transform.localPosition.x - 1050f);
        return component;
    }

    private IEnumerator TutorialPart2()
    {
        Tracking.fire_volcano_bomb();
        StopAppleCap();
        btnArrow.Out();
        neander = SpawnCavemen();
        HideDialog();
        yield return new WaitForSeconds(0.3f);
        SetDialogTween(base.bottom, base.lowerDialogPosition);
        while (shot_carrier.transform.position.x < neander.transform.position.x - 1100f)
        {
            yield return null;
        }
        ShowDialog("Tap anywhere to drop the bomb", true);
        Time.timeScale = 0f;
        while (base.game_is_paused || (!Input.GetMouseButton(0) && Input.touchCount <= 0))
        {
            yield return null;
        }
        AbstractShot bombShot = shot_carrier.GetComponent<ShotCarrier>().TutorialShoot();
        bombShot.isTutorialMode = true;
        Time.timeScale = 1f;
        HideDialog(true);
        Tracking.drop_volcano_bomb();
        yield return new WaitForSeconds(1.5f);
        ShowDialog("Tap to detonate!", true);
        bombShot.isTutorialMode = false;
        Time.timeScale = 0f;
        while (base.game_is_paused || (!Input.GetMouseButton(0) && Input.touchCount <= 0))
        {
            yield return null;
        }
        Time.timeScale = 1f;
        HideDialog();
        Tracking.detonate_volcano_bomb();
        yield return new WaitForSeconds(3f);
        StartCoroutine(TutorialPart3());
    }

    private IEnumerator TutorialPart3()
    {
        GameCamera.Instance.EnterState_Playing();
        StartAppleCap(Player.Instance.Apples + 10);
        ShowDialog("Rearming takes some time");
        while (base.dialog_open)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        ShowDialog("Now surive and protect the egg!");
        while (base.dialog_open)
        {
            yield return null;
        }
        StopAppleCap();
        standardBulletBtn.Enabled = true;
        hud.UnitButtons.ForEach(delegate (BuyUnitButton button)
        {
            button.disable = false;
        });
        actionHinting.StartNotification();
        Level.Instance.EndTutorial(Level.State.playing);
        watcher.StartWatching(UnitType.Neander_Healer, "tutorial_watch_healer");
    }

    private void OnFired()
    {
        ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(OnFired));
        if (useDragShots)
        {
            StartCoroutine(TutorialDragshotPart2());
        }
        else
        {
            StartCoroutine(TutorialPart2());
        }
    }
}
