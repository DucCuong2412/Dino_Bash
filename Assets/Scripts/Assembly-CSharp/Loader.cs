using System;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBase
{
    private const float desired_loading_duration = 3f;

    public static Loader instance;

    [SerializeField]
    private Animator animator;

    private LocalizedText label_loading;

    private bool initialized;

    private string targetLevel = string.Empty;

    private Action onLoadedCallback;

    private bool hiddenLoading;

    private SpriteRenderer screenshot;

    private Transform screenshot_parent;

    private float loading_starded;

    private Dictionary<int, string[]> loca_mapping = new Dictionary<int, string[]>
    {
        {
            1,
            new string[4] { "Loader1_hint1", "Loader1_hint2", "Loader1_hint3", "Loader1_hint4" }
        },
        {
            2,
            new string[4] { "Loader2_hint1", "Loader2_hint2", "Loader2_hint3", "Loader2_hint4" }
        },
        {
            3,
            new string[1] { "Loader3_hint1" }
        },
        {
            4,
            new string[1] { "Loader4_hint1" }
        },
        {
            5,
            new string[1] { "Loader5_hint1" }
        },
        {
            6,
            new string[2] { "Loader6_hint1", "Loader6_hint2" }
        },
        {
            7,
            new string[1] { "Loader7_hint1" }
        }
    };

    public bool isVisible { get; private set; }

    private void Awake()
    {
        if (!initialized)
        {
            initialized = true;
            base.transform.parent = ScreenManager.Camera.transform;
            tk2dCameraAnchor[] componentsInChildren = GetComponentsInChildren<tk2dCameraAnchor>();
            foreach (tk2dCameraAnchor tk2dCameraAnchor2 in componentsInChildren)
            {
                tk2dCameraAnchor2.AnchorCamera = ScreenManager.Camera;
            }
            screenshot_parent = base.transform.Search("screenshot");
            label_loading = base.transform.Search("label_loading").GetComponent<LocalizedText>();
            Transform transform = base.transform.Search("loader bg");
            float num = 0.75000006f;
            float num2 = num * ((float)Screen.width / (float)Screen.height);
            transform.transform.localScale = new Vector3(num2, num2, 1f);
            SpriteTools.SetSortingLayerID(this, 47);
            animator.enabled = true;
            instance = this;
            base.GetComponent<Collider>().enabled = false;
            base.gameObject.SetActive(false);
        }
    }

    public static void Load(string targetLevel, Action OnLoadedCallback = null, bool hiddenLoading = false)
    {
        instance.Awake();
        if (!(targetLevel == string.Empty))
        {
            instance.gameObject.SetActive(true);
            instance.StopAllCoroutines();
            instance.targetLevel = targetLevel;
            instance.onLoadedCallback = OnLoadedCallback;
            instance.hiddenLoading = hiddenLoading;
            if (!hiddenLoading)
            {
                instance.Show();
            }
            else
            {
                instance.OnInAnimationOver();
            }
        }
    }

    private void Show()
    {
        int key = UnityEngine.Random.Range(1, loca_mapping.Keys.Count + 1);
        if (screenshot != null)
        {
            UnityEngine.Object.Destroy(screenshot);
        }
        screenshot = UnityEngine.Object.Instantiate(Resources.Load<SpriteRenderer>("GUI/ScreenShots/screenshot_" + key.ToString("00"))) as SpriteRenderer;
        screenshot.transform.RepositionAndReparent(screenshot_parent, true);
        screenshot.transform.localScale = Vector3.one;
        screenshot.transform.localPosition = Vector3.zero;
        SpriteTools.SetSortingLayerID(screenshot, 47);
        string text = loca_mapping[key][UnityEngine.Random.Range(0, loca_mapping[key].Length)];
        if (text == loca_mapping[4][0])
        {
            //label_loading.textMesh.text = string.Format(text.Localize(), Konfiguration.GameConfig.AppleBoostAmount);
        }
        else
        {
            label_loading.Key = text;
        }
        isVisible = true;
        base.GetComponent<Collider>().enabled = true;
        animator.Play("in");
        AudioPlayer.PlayGuiSFX(Sounds.main_loader_in, 0f);
    }

    private void OnInAnimationOver()
    {
        loading_starded = Time.realtimeSinceStartup;
        Go.RemoveAllTweens();
        Application.LoadLevel(targetLevel);
    }

    private void OnLevelWasLoaded()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
        Time.timeScale = 1f;
        targetLevel = string.Empty;
        if (onLoadedCallback != null)
        {
            onLoadedCallback();
            onLoadedCallback = null;
        }
        float num = Time.realtimeSinceStartup - loading_starded;
        Tracking.loading_screen_complete_tutorial(num);
        float num2 = 0.1f;
        if (num < 3f)
        {
            num2 = Mathf.Max(num2, 3f - num);
        }
        if (!hiddenLoading)
        {
            WaitThenRealtime(num2, Hide);
        }
        hiddenLoading = false;
    }

    private void Hide()
    {
        base.GetComponent<Collider>().enabled = false;
        animator.Play("out", 0);
        isVisible = false;
        WaitThenRealtime(1f, delegate
        {
            base.gameObject.SetActive(false);
        });
    }
}
