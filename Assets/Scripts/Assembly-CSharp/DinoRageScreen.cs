using System;
using UnityEngine;
using dinobash;

public class DinoRageScreen : BaseScreen
{
    private tk2dCameraAnchor upper_center;

    private tk2dCameraAnchor middle_center;

    private float screen_width;

    private tk2dTextMesh cost_label;

    private GoTween title_color_tween;

    private int level_relativ_to_progress
    {
        get
        {
            int a = Mathf.RoundToInt((float)Level.Instance.levelid / (float)Konfiguration.levels.Count * 10f);
            a = Mathf.Max(a, Player.GetUnitLevel(UnitType.TRex_Jr));
            return Mathf.Clamp(a, 0, 9);
        }
    }

    private void SetUpgradeSetters(Transform root)
    {
        EntityUpgradeSetter[] componentsInChildren = root.GetComponentsInChildren<EntityUpgradeSetter>();
        EntityUpgradeSetter[] array = componentsInChildren;
        foreach (EntityUpgradeSetter entityUpgradeSetter in array)
        {
            entityUpgradeSetter.ForceUpdate(level_relativ_to_progress);
        }
    }

    protected void Start()
    {
        upper_center = FindChildComponent<tk2dCameraAnchor>("UpperCenter");
        middle_center = FindChildComponent<tk2dCameraAnchor>("MiddleCenter");
        screen_width = ScreenManager.Camera.GetComponent<tk2dCamera>().ScreenExtents.width;
        tk2dTextMesh component = base.transform.Search("label_benefits2").GetComponent<tk2dTextMesh>();
        //component.text = string.Format("DINO_RAGE_EFFECT_1".Localize(), Konfiguration.GameConfig.Dino_rage_apples);
        cost_label = base.transform.Search("label_price").GetComponent<tk2dTextMesh>();
        //cost_label.text = Konfiguration.GameConfig.Dino_rage_cost.ToString();
        base.transform.GetComponentInChildren<EntityShadow>().GetComponent<Renderer>().enabled = false;
        SetUpgradeSetters(base.transform);
        base.transform.Search("btn_dino_rage").GetComponent<tk2dUIItem>().OnClick += delegate
        {
            //int dino_rage_cost = Konfiguration.GameConfig.Dino_rage_cost;
            //if (Wallet.Diamonds >= dino_rage_cost)
            //{
            //	Wallet.TakeDiamonds(dino_rage_cost);
            //	int spent = dino_rage_cost;
            //	Tracking.buy_item(UnitType.None, ShotType.None, "dinorage", spent, "level");
            //	AudioPlayer.PlayGuiSFX(AudioResources.GetDinoSelectSound(UnitType.Blizzard), 0f);
            //	CameraShake.Shake(CameraShake.Intensity.strong);
            //	EntityFactory.Create(UnitType.Blizzard);
            //	Unit component2 = EntityFactory.Create(UnitType.TRex_Jr).GetComponent<Unit>();
            //	EntityData config = component2.Config;
            //	config.override_unit_level = level_relativ_to_progress;
            //	component2.Config = config;
            //	MeleeCombatBehaviour combat_behaviour = component2.GetComponent<MeleeCombatBehaviour>();
            //	combat_behaviour.SetIgnoredMaskForUnit(UnitType.Neander_Disguise, false);
            //	combat_behaviour.SetCannotHitMaskForUnit(UnitType.Neander_Disguise, false);
            //	Action dino_rage_end_callback = delegate
            //	{
            //		combat_behaviour.SetIgnoredMaskForUnit(UnitType.Neander_Disguise, true);
            //		combat_behaviour.SetCannotHitMaskForUnit(UnitType.Neander_Disguise, true);
            //	};
            //	SetUpgradeSetters(component2.transform);
            //	EntityFactory.Dino_Egg.Dino_Rage(dino_rage_end_callback);
            //	Hide();
            //}
            //else
            //{
            //	Tracking.store_open(Wallet.Currency.Diamonds, "dinorage_screen", "dinorage");
            //	ScreenManager.GetScreen<ShopScreenDiamonds>().Show();
            //}
        };
        base.transform.Search("btn_give_up").GetComponent<tk2dUIItem>().OnClick += delegate
        {
            EntityFactory.Dino_Egg.level_end();
            Hide();
        };
        Level.Instance.OnLevelLost += delegate
        {
            StopAllCoroutines();
        };
        middle_center.AnchorOffsetPixels = new Vector2(screen_width, 0f);
        upper_center.AnchorOffsetPixels = new Vector2(0f, 768f);
        base.gameObject.SetActive(false);
    }

    private void ShowCenterDialog()
    {
        middle_center.AnchorOffsetPixels = new Vector2(screen_width, 0f);
        Go.to(middle_center, 0.3f, new GoTweenConfig().vector2Prop("AnchorOffsetPixels", new Vector2(0f, 0f)).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
    }

    private void HideCenterDialog(Action callback)
    {
        Go.to(middle_center, 0.3f, new GoTweenConfig().vector2Prop("AnchorOffsetPixels", new Vector2(0f - screen_width, 0f)).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate).onComplete(delegate
        {
            callback();
        }));
    }

    public override void Show()
    {
        base.gameObject.SetActive(true);
        base.Show();
        Tracking.dino_rage_offer(Player.CurrentLevelID, Level.Instance.getProgess());
        Time.timeScale = 0f;
        OnEscapeUp = Hide;
        ScreenManager.GetScreen<CoverScreen>(this).Show();
        ShowCenterDialog();
        upper_center.AnchorOffsetPixels = new Vector2(0f, 768f);
        Go.to(upper_center, 0.3f, new GoTweenConfig().vector2Prop("AnchorOffsetPixels", Vector2.zero).setEaseType(GoEaseType.CircOut).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
    }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1f;
        ScreenManager.GetScreen<CoverScreen>(this).Hide();
        ScreenManager.GetScreen<HudScreen>().Show();
        HideCenterDialog(delegate
        {
            base.gameObject.SetActive(false);
        });
        Go.to(upper_center, 0.3f, new GoTweenConfig().vector2Prop("AnchorOffsetPixels", new Vector2(0f, 768f)).setEaseType(GoEaseType.CircIn).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
    }
}
