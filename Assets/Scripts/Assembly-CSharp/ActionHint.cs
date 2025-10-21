using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHint : MonoBehaviour
{
    private TutorialArrow buyUnitArrow;

    private TutorialArrow shotArrow;

    private float lastTimeShootingHappend;

    private float lastTimeBuyUnitHappend;

    private float unitBuildCooldown;

    private float shotCooldown;

    private bool hintIsShown;

    private HudScreen hud;

    public float hintingInterval = 7f;

    private BuyUnitButton unitButton;

    public void Init(TutorialArrow buyUnitArrow, TutorialArrow shotArrow)
    {
        this.buyUnitArrow = buyUnitArrow;
        this.shotArrow = shotArrow;
        hud = ScreenManager.GetScreen<HudScreen>();
        if (hud.UnitButtons.Count > 0)
        {
            unitBuildCooldown = Konfiguration.UnitData[hud.UnitButtons[0].Unit].buildcooldown;
        }
        if (hud.ShotButtons.Count > 0)
        {
            shotCooldown = Konfiguration.ShotData[hud.ShotButtons[0].shotType].cooldown;
        }
    }

    private void OnDisable()
    {
        StopNotification();
    }

    public void StartNotification()
    {
        lastTimeShootingHappend = Time.time + hintingInterval;
        ShotFactory.OnShotStart = (Action)Delegate.Combine(ShotFactory.OnShotStart, new Action(LastShotTime));
        lastTimeBuyUnitHappend = Time.time;
        EntityFactory.OnPlayerBuiltUnit += LastBuyUnitTime;
        if (hud.ShotButtons.Count > 0)
        {
            StartCoroutine(MonitorShooting());
        }
        if (hud.UnitButtons.Count > 0)
        {
            unitButton = hud.UnitButtons[hud.UnitButtons.Count - 1];
            StartCoroutine(MonitorBuyUnits());
        }
    }

    public void StopNotification()
    {
        StopAllCoroutines();
        buyUnitArrow.Out();
        HideShotArrow();
        ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(LastShotTime));
        ShotFactory.OnShotEnd = (Action)Delegate.Remove(ShotFactory.OnShotEnd, new Action(OnShotEnd));
        EntityFactory.OnPlayerBuiltUnit -= LastBuyUnitTime;
        EntityFactory.OnPlayerBuiltUnit -= OnPlayerBuiltUnit;
    }

    private IEnumerator MonitorBuyUnits()
    {
        while (!unitButton.Enabled || !hud.isVisible || hintIsShown || lastTimeBuyUnitHappend + hintingInterval > Time.time)
        {
            yield return new WaitForSeconds(0.5f);
        }
        buyUnitArrow.transform.RepositionAndReparent(unitButton.transform);
        buyUnitArrow.In();
        hintIsShown = true;
        EntityFactory.OnPlayerBuiltUnit += OnPlayerBuiltUnit;
    }

    private void LastBuyUnitTime()
    {
        lastTimeBuyUnitHappend = Mathf.Max(lastTimeBuyUnitHappend, Time.time);
    }

    private void OnPlayerBuiltUnit()
    {
        EntityFactory.OnPlayerBuiltUnit -= OnPlayerBuiltUnit;
        buyUnitArrow.Out();
        hintIsShown = false;
        lastTimeBuyUnitHappend = Time.time + unitBuildCooldown;
        StartCoroutine(MonitorBuyUnits());
    }

    private IEnumerator MonitorShooting()
    {
        bool neanders_visible = false;
        while (!hud.isVisible || !hud.ShotButtons[0].Enabled || hintIsShown || !neanders_visible || lastTimeShootingHappend + hintingInterval > Time.time)
        {
            yield return new WaitForSeconds(0.5f);
            List<BaseEntity> neanders = EntityFactory.GetEntities(false);
            neanders_visible = neanders.Find((BaseEntity n) => GameCamera.Instance.isInView(n.transform, 0.8f)) != null;
        }
        shotArrow.transform.RepositionAndReparent(hud.ShotButtons[0].transform);
        shotArrow.In();
        hintIsShown = true;
        //if (Konfiguration.GameConfig.Use_dragshot_feature)
        //{
        //	hud.ShotButtons[0].uiItem.OnUp += HideShotArrow;
        //}
        //else
        //{
        //	ShotFactory.OnShotStart = (Action)Delegate.Combine(ShotFactory.OnShotStart, new Action(HideShotArrow));
        //}
        ShotFactory.OnShotEnd = (Action)Delegate.Combine(ShotFactory.OnShotEnd, new Action(OnShotEnd));
    }

    private void LastShotTime()
    {
        lastTimeShootingHappend = Mathf.Max(lastTimeShootingHappend, Time.time);
    }

    private void HideShotArrow()
    {
        hud.ShotButtons[0].uiItem.OnUp -= HideShotArrow;
        ShotFactory.OnShotStart = (Action)Delegate.Remove(ShotFactory.OnShotStart, new Action(HideShotArrow));
        shotArrow.Out();
    }

    private void OnShotEnd()
    {
        ShotFactory.OnShotEnd = (Action)Delegate.Remove(ShotFactory.OnShotEnd, new Action(OnShotEnd));
        hintIsShown = false;
        lastTimeShootingHappend = Time.time + shotCooldown;
        StartCoroutine(MonitorShooting());
    }
}
