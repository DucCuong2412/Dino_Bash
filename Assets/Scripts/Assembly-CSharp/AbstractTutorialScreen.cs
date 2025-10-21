using dinobash;
using System.Collections;
using UnityEngine;

public abstract class AbstractTutorialScreen : BaseScreen
{
    protected Vector3 arrowScale = Vector3.one;

    private TutorialArrow arrowPrefab;

    private GameObject swipeArrowPrefab;

    private FocusMask focusMaskPrefab;

    protected Animator turtleAnimator;

    protected ActionHint actionHinting;

    protected GoTween dialog_tween;

    protected LocalizedText dialog_label;

    protected DinoEgg egg;

    private tk2dUIItem button_close;

    protected TextPrinter text_printer;

    private float label_pos_y;

    private Camera gameCamera;

    protected bool follow;

    protected Transform followTarget;

    private float dialog_started;

    private float minimum_dialog_time = 1.5f;

    private Vector3 textScale = Vector3.one;

    private IEnumerator close_dialog_routine;

    private PauseScreen pause_screen;

    private QuitLevelRequestScreen quit_request_screen;

    protected Vector3 dragStart;

    protected GoTween dragTween;

    protected bool DinoEggInvincible
    {
        set
        {
            if (egg == null)
            {
                egg = Object.FindObjectOfType<DinoEgg>();
            }
            egg.AddOrGetComponent<Invincibility>().enabled = value;
        }
    }

    protected Vector3 lowerDialogPosition => new Vector3(0f, 60f, 0f);

    protected Vector3 lowerCenterDialogPosition => new Vector3(0f, 360f, 0f);

    protected Vector3 centerDialogPosition => new Vector3(0f, 600f, 0f);

    protected Vector3 upperDialogPosition => new Vector3(0f, 990f, 0f);

    protected bool dialog_open
    {
        get;
        private set;
    }

    protected bool apple_cap_active
    {
        get;
        private set;
    }

    protected bool game_is_paused
    {
        get
        {
            if (pause_screen == null)
            {
                pause_screen = ScreenManager.GetScreen<PauseScreen>();
            }
            if (pause_screen != null && pause_screen.isVisible)
            {
                return true;
            }
            if (quit_request_screen == null)
            {
                quit_request_screen = ScreenManager.GetScreen<QuitLevelRequestScreen>();
            }
            if (quit_request_screen != null && quit_request_screen.isVisible)
            {
                return true;
            }
            return false;
        }
    }

    protected virtual void Start()
    {
        dialog_label = base.gameObject.transform.Search("Label").GetComponent<LocalizedText>();
        textScale = dialog_label.textMesh.scale;
        Vector3 localPosition = dialog_label.transform.localPosition;
        label_pos_y = localPosition.y;
        text_printer = dialog_label.GetComponent<TextPrinter>();
        text_printer.delay = 0.3f;
        button_close = base.gameObject.transform.Search("btn_close").GetComponent<tk2dUIItem>();
        button_close.gameObject.SetActive(value: false);
        turtleAnimator = dialog_label.transform.parent.GetComponentInChildren<Animator>();
        SetDialogTween(base.bottom, lowerDialogPosition);
        if (App.State == App.States.Game)
        {
            gameCamera = GameCamera.Instance.GetComponent<Camera>();
            actionHinting = this.AddOrGetComponent<ActionHint>();
            actionHinting.Init(getArrow(), getArrow());
        }
    }

    protected void Update()
    {
        if (!follow || !(followTarget != null))
        {
            return;
        }
        Vector3 position = followTarget.position;
        float x = position.x;
        Vector3 position2 = gameCamera.transform.position;
        if (x < position2.x + gameCamera.GetComponent<tk2dCamera>().ScreenExtents.width * 0.5f)
        {
            Transform transform = gameCamera.transform;
            Vector3 position3 = followTarget.position;
            transform.PosX(position3.x - gameCamera.GetComponent<tk2dCamera>().ScreenExtents.width * 0.5f);
            Vector3 position4 = gameCamera.transform.position;
            if (position4.x < -256f)
            {
                follow = false;
            }
        }
    }

    protected void FollowTarget(Transform target)
    {
        followTarget = target;
        follow = true;
    }

    protected void SetDialogTween(Vector3 from, Vector3 to)
    {
        if (dialog_tween != null)
        {
            dialog_tween.destroy();
        }
        dialog_label.transform.parent.localPosition = to;
        dialog_tween = Go.from(dialog_label.transform.parent, 0.3f, new GoTweenConfig().position(from).setEaseType(GoEaseType.CircOut).setUpdateType(GoUpdateType.TimeScaleIndependentUpdate));
        dialog_tween.setOnCompleteHandler(delegate
        {
            turtleAnimator.CrossFade("teach", 0.1f);
        });
        dialog_tween.autoRemoveOnComplete = false;
        dialog_tween.pause();
    }

    protected void ShowDialog(string loca_key, bool user_action = false, bool localize = true)
    {
        if (dialog_open)
        {
            UnityEngine.Debug.Log("Skipping Dialog!");
            return;
        }
        dialog_open = true;
        button_close.gameObject.SetActive(!user_action);
        dialog_started = Time.realtimeSinceStartup;
        dialog_label.textMesh.scale = textScale;
        if (close_dialog_routine != null)
        {
            StopCoroutine(close_dialog_routine);
        }
        if (!user_action)
        {
            //button_close.OnClick += HideDialog;
            close_dialog_routine = CloseDialog(6f);
            StartCoroutine(close_dialog_routine);
        }
        if (localize)
        {
            dialog_label.Key = loca_key;
        }
        else
        {
            dialog_label.textMesh.text = loca_key;
            dialog_label.textMesh.ForceBuild();
        }
        SetLineHeightOffset(dialog_label.textMesh, label_pos_y);
        text_printer.Stop();
        text_printer.Print();
        dialog_tween.playForward();
    }

    protected void HideDialog(bool force_close = false)
    {
        if (dialog_open)
        {
            if (close_dialog_routine != null)
            {
                StopCoroutine(close_dialog_routine);
            }
            float num = dialog_started + 0.3f + minimum_dialog_time;
            if (Time.realtimeSinceStartup < num && !force_close)
            {
                float wait = num - Time.realtimeSinceStartup;
                close_dialog_routine = CloseDialog(wait);
                StartCoroutine(close_dialog_routine);
            }
            else
            {
                button_close.gameObject.SetActive(value: false);
                //button_close.OnClick -= HideDialog;
                dialog_open = false;
                dialog_tween.playBackwards();
            }
        }
    }

    protected void SetLineHeightOffset(tk2dTextMesh text_mesh, float base_height)
    {
        int num = text_mesh.FormattedText.Split('\n').Length;
        float lineHeight = text_mesh.font.lineHeight;
        float lineSpacing = text_mesh.LineSpacing;
        Vector3 scale = text_mesh.scale;
        float num2 = (lineHeight + lineSpacing * scale.y) * 0.9f;
        num2 = ((num != 2) ? num2 : (num2 * 0.5f));
        num2 = ((num != 3) ? num2 : 0f);
        text_mesh.transform.LocalPosY(base_height - num2);
    }

    private IEnumerator CloseDialog(float wait)
    {
        float delay = Time.realtimeSinceStartup + wait;
        while (Time.realtimeSinceStartup < delay)
        {
            yield return null;
        }
        if (dialog_open)
        {
            HideDialog();
        }
        close_dialog_routine = null;
    }

    protected void StartAppleCap(int max)
    {
        apple_cap_active = true;
        StartCoroutine(CapApples(max));
    }

    protected void StopAppleCap()
    {
        apple_cap_active = false;
    }

    private IEnumerator CapApples(int max)
    {
        while (apple_cap_active)
        {
            yield return new WaitForEndOfFrame();
            Player.Instance.Apples = Mathf.Min(Player.Instance.Apples, max);
        }
    }

    protected FocusMask getFocusMask()
    {
        if (focusMaskPrefab == null)
        {
            focusMaskPrefab = (Resources.Load("GUI/Tutorial/FocusMask", typeof(FocusMask)) as FocusMask);
        }
        FocusMask focusMask = Object.Instantiate(focusMaskPrefab) as FocusMask;
        focusMask.transform.RepositionAndReparent(base.transform);
        Renderer componentInChildren = focusMask.GetComponentInChildren<Renderer>();
        componentInChildren.sortingLayerID = base.SortingLayerID;
        componentInChildren.sortingOrder = -20000;
        return focusMask;
    }

    protected TutorialArrow getArrow(bool flip = false)
    {
        if (arrowPrefab == null)
        {
            arrowPrefab = (Resources.Load("GUI/Tutorial/Arrow", typeof(TutorialArrow)) as TutorialArrow);
        }
        Quaternion rotation = (!flip) ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
        TutorialArrow tutorialArrow = Object.Instantiate(arrowPrefab, Vector3.zero, rotation) as TutorialArrow;
        tutorialArrow.transform.localScale = arrowScale;
        SpriteTools.SetSortingLayerID(tutorialArrow.transform, base.SortingLayerID);
        return tutorialArrow;
    }

    protected GameObject getSwipeArrow()
    {
        if (swipeArrowPrefab == null)
        {
            swipeArrowPrefab = (Resources.Load("GUI/Tutorial/SwipeArrow") as GameObject);
        }
        GameObject gameObject = Object.Instantiate(swipeArrowPrefab) as GameObject;
        SpriteTools.SetSortingLayerID(gameObject.transform, base.SortingLayerID);
        return gameObject;
    }

    protected void DragTween(Transform item, Transform target)
    {
        Vector3 position = target.position;
        position.y += 220f;
        Vector3 position2 = GameCamera.Instance.GetComponent<Camera>().WorldToScreenPoint(position);
        Vector3 endValue = ScreenManager.Camera.ScreenToWorldPoint(position2);
        item.transform.position = dragStart;
        dragTween = Go.to(item, 1f, new GoTweenConfig().position(endValue).setEaseType(GoEaseType.CubicInOut));
        dragTween.setOnCompleteHandler(delegate
        {
            DragTween(item, target);
        });
    }
}
