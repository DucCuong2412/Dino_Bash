using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class AppleLevelUpgradeTutorial : AbstractTutorialScreen
{
    private TutorialArrow arrow;

    private FocusMask focusMask;

    private List<SpriteRenderer> appleSprites = new List<SpriteRenderer>();

    private HudScreen hud;

    private tk2dUIItem upgrade_button;

    private SpriteRenderer applebar_apple;

    protected override void Start()
    {
        base.Start();
        hud = ScreenManager.GetScreen<HudScreen>();
        upgrade_button = hud.transform.Search("Upgrade_Button").GetComponent<tk2dUIItem>();
        arrow = getArrow(true);
        arrow.transform.RepositionAndReparent(upgrade_button.transform.GetChild(0));
        arrow.transform.localScale = Vector3.one;
        focusMask = getFocusMask();
        applebar_apple = hud.transform.Search("applebar_apple").GetComponent<SpriteRenderer>();
        SetDialogTween(base.bottom, base.centerDialogPosition);
        //for (int i = 0; i < Konfiguration.GameConfig.AppleUpgradeCost[0]; i++)
        //{
        //	GameObject gameObject = new GameObject("apple");
        //	SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        //	spriteRenderer.sprite = applebar_apple.sprite;
        //	spriteRenderer.sortingLayerID = applebar_apple.sortingLayerID;
        //	spriteRenderer.sortingOrder = applebar_apple.sortingOrder - 2;
        //	spriteRenderer.gameObject.layer = applebar_apple.gameObject.layer;
        //	spriteRenderer.enabled = false;
        //	spriteRenderer.transform.parent = base.transform;
        //	spriteRenderer.transform.position = new Vector3(Random.Range(-600f, 768f), 1024f, 0f);
        //	appleSprites.Add(spriteRenderer);
        //}
        //appleSprites.Sort((SpriteRenderer x, SpriteRenderer y) => x.transform.position.x.CompareTo(y.transform.position.x));
        appleSprites.ForEach(delegate (SpriteRenderer x)
        {
            x.sortingOrder = -Mathf.RoundToInt(x.transform.position.x);
        });
        StartCoroutine(Part1());
    }

    private IEnumerator Part1()
    {
        hud.UnitButtons.ForEach(delegate (BuyUnitButton x)
        {
            x.disable = true;
        });
        hud.ShotButtons.ForEach(delegate (ShootButton x)
        {
            x.Enabled = false;
        });
        hud.ConsumableButtons.ForEach(delegate (BuyUnitButton x)
        {
            x.disable = true;
        });
        GameCamera.Instance.EnterState_Disabled();
        //StartAppleCap(Konfiguration.GameConfig.AppleUpgradeCost[0]);
        yield return new WaitForSeconds(1f);
        ShowDialog("Let's try something different..");
        while (base.dialog_open)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        string text = "Here are # apples for you".Localize();
        //text = text.Replace("#", Konfiguration.GameConfig.AppleUpgradeCost[0].ToString());
        ShowDialog(text, false, false);
        while (base.dialog_open)
        {
            yield return null;
        }
        foreach (SpriteRenderer apple in appleSprites)
        {
            apple.enabled = true;
            Vector3 target = apple.transform.position;
            target.y = Random.Range(-280f, -380f);
            Go.to(apple.transform, 0.6f, new GoTweenConfig().position(target).setEaseType(GoEaseType.BounceOut));
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        foreach (SpriteRenderer apple2 in appleSprites)
        {
            Go.to(apple2.transform, 0.5f, new GoTweenConfig().position(applebar_apple.transform.position).setEaseType(GoEaseType.CubicInOut).onComplete(delegate
            {
                apple2.enabled = false;
                Player.Instance.Apples++;
            }));
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        appleSprites.ForEach(delegate (SpriteRenderer x)
        {
            x.enabled = false;
        });
        focusMask.Show(upgrade_button.transform.GetChild(0).position, 6f);
        arrow.In();
        while (upgrade_button.enabled)
        {
            yield return null;
        }
        focusMask.Hide();
        arrow.Out();
        ShowDialog("Now, you get apples faster");
        while (base.dialog_open)
        {
            yield return null;
        }
        StopAppleCap();
        Level.Instance.EndTutorial(Level.State.playing);
        float introduration = 3f;
        tk2dCamera gamecamera = Camera.main.GetComponent<tk2dCamera>();
        Go.to(gamecamera.transform, introduration, new GoTweenConfig().position(gamecamera.transform.position.SetX((float)Level.Instance.Config.levelwidth - gamecamera.ScreenExtents.width)).setEaseType(GoEaseType.QuintInOut));
        yield return new WaitForSeconds(introduration);
        hud.UnitButtons.ForEach(delegate (BuyUnitButton x)
        {
            x.disable = false;
        });
        hud.ShotButtons.ForEach(delegate (ShootButton x)
        {
            x.Enabled = true;
        });
        hud.ConsumableButtons.ForEach(delegate (BuyUnitButton x)
        {
            x.disable = false;
        });
        GameCamera.Instance.EnterState_Playing();
    }
}
