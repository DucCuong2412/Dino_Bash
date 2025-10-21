using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dinobash;

public class ResourceBarScreen : BaseScreen
{
	private const string heart_empty = "heart empty";

	private List<Animator> hearts = new List<Animator>();

	private tk2dTextMesh xpLevel;

	private tk2dTextMesh coins;

	private tk2dTextMesh diamonds;

	private tk2dTextMesh timer;

	private tk2dClippedSprite xpProgressBar;

	private tk2dUIItem btn_getlifes;

	private tk2dUIItem btn_coinshop;

	private tk2dUIItem btn_diamondshop;

	public SpriteRenderer icon_coins;

	public SpriteRenderer icon_diamonds;

	public SpriteRenderer icon_xp;

	private int xpLevelOnStart;

	private bool isInitialized;

	public bool switch_in_progress;

	public float tweenDelay;

	public int tweenCoins { get; set; }

	public int tweenDiamonds { get; set; }

	public float tweenProgressbar { get; set; }

	protected void Start()
	{
		if (isInitialized)
		{
			return;
		}
		isInitialized = true;
		timer = base.transform.Search("refill_timer_label").GetComponent<tk2dTextMesh>();
		xpLevel = base.transform.Search("xpLevelLabel").GetComponent<tk2dTextMesh>();
		xpLevel.text = (Player.XPLevel + 1).ToString();
		xpLevel.Commit();
		coins = base.transform.Search("coinsLabel").GetComponent<tk2dTextMesh>();
		coins.text = Wallet.Coins.ToString().GetGroupedNumberString();
		coins.Commit();
		diamonds = base.transform.Search("diamondsLabel").GetComponent<tk2dTextMesh>();
		diamonds.text = Wallet.Diamonds.ToString().GetGroupedNumberString();
		diamonds.Commit();
		icon_coins = FindChildComponent<SpriteRenderer>("UpperCenter/Coins/icon_coin");
		icon_diamonds = FindChildComponent<SpriteRenderer>("UpperCenter/Diamonds/icon_diamond");
		icon_xp = FindChildComponent<SpriteRenderer>("UpperCenter/XP/icon_xp");
		btn_getlifes = FindChildComponent<tk2dUIItem>("UpperCenter/Hearts/btn_RefillHearts");
		btn_getlifes.OnClick += delegate
		{
			if (Player.Lives < 5)
			{
				SwitchOpenScreen(ScreenManager.GetScreen<GetLivesScreen>());
			}
		};
		btn_coinshop = FindChildComponent<tk2dUIItem>("UpperCenter/Coins/btn_BuyCoins");
		btn_coinshop.OnClick += delegate
		{
			SwitchOpenScreen(ScreenManager.GetScreen<ShopScreenCoins>());
		};
		btn_diamondshop = FindChildComponent<tk2dUIItem>("UpperCenter/Diamonds/btn_BuyDiamonds");
		btn_diamondshop.OnClick += delegate
		{
			SwitchOpenScreen(ScreenManager.GetScreen<ShopScreenDiamonds>());
		};
		GenerateHearts();
		Player.OnGenerateLife += RefillHearts;
		tweenCoins = Wallet.Coins;
		tweenDiamonds = Wallet.Diamonds;
		Wallet.OnBalanceChanged += onBalanceChanged;
		xpProgressBar = base.transform.Search("progressbar").GetComponent<tk2dClippedSprite>();
		tweenProgressbar = GetXpProgress(Player.XPLevel);
		xpProgressBar.ClipRect = new Rect(0f, 0f, tweenProgressbar, 1f);
		xpLevelOnStart = Player.XPLevel;
		base.transform.localPosition += base.top;
		base.gameObject.SetActive(false);
	}

	private void HideOpenScreen(BaseScreen screen)
	{
		if (screen != null && screen.isVisible)
		{
			screen.Hide();
		}
	}

	private void SwitchOpenScreen(BaseScreen target)
	{
		if (!target.isVisible && !switch_in_progress)
		{
			UpgradeScreen screen = ScreenManager.GetScreen<UpgradeScreen>();
			UpgradeInfoDinoScreen screen2 = ScreenManager.GetScreen<UpgradeInfoDinoScreen>();
			UpgradeInfoShotScreen screen3 = ScreenManager.GetScreen<UpgradeInfoShotScreen>();
			UpgradeInfoUpgradesScreen screen4 = ScreenManager.GetScreen<UpgradeInfoUpgradesScreen>();
			QuestScreen screen5 = ScreenManager.GetScreen<QuestScreen>();
			FacebookLoginRequestScreen screen6 = ScreenManager.GetScreen<FacebookLoginRequestScreen>();
			SelectFriendsScreen screen7 = ScreenManager.GetScreen<SelectFriendsScreen>();
			GetLivesScreen screen8 = ScreenManager.GetScreen<GetLivesScreen>();
			ShopScreenCoins screen9 = ScreenManager.GetScreen<ShopScreenCoins>();
			ShopScreenDiamonds screen10 = ScreenManager.GetScreen<ShopScreenDiamonds>();
			HideOpenScreen(screen);
			HideOpenScreen(screen2);
			HideOpenScreen(screen3);
			HideOpenScreen(screen4);
			HideOpenScreen(screen);
			HideOpenScreen(screen5);
			HideOpenScreen(screen6);
			HideOpenScreen(screen7);
			HideOpenScreen(screen);
			HideOpenScreen(screen);
			if (screen8 != target && screen8.isVisible)
			{
				screen8.Hide();
			}
			if (screen9 != target && screen9.isVisible)
			{
				screen9.Hide();
			}
			if (screen10 != target && screen10.isVisible)
			{
				screen10.Hide();
			}
			switch_in_progress = true;
			WaitThen(0.3f, delegate
			{
				target.Show();
				switch_in_progress = false;
			});
		}
	}

	private IEnumerator WatchPromotionTime()
	{
		bool running = true;
		while (running)
		{
			running = ShopPromotions.remainingTime.Seconds > 0;
			yield return new WaitForSeconds(1f);
		}
		SetSalesbadges();
	}

	private void SetSalesbadges()
	{
		btn_coinshop.transform.parent.Find("icon_sale_badge").gameObject.SetActive(ShopPromotions.is_sale);
		btn_diamondshop.transform.parent.Find("icon_sale_badge").gameObject.SetActive(ShopPromotions.is_sale);
	}

	private void GenerateHearts()
	{
		hearts = new List<Animator>();
		Transform transform = base.transform.Search("heart_definition");
		Vector3 vector = new Vector3(transform.Find("heart_full").GetComponent<Renderer>().bounds.size.x * 1.05f, 0f, 0f);
		for (int i = 0; i < 5; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(transform.gameObject, transform.position + i * vector, Quaternion.identity) as GameObject;
			SpriteTools.SetSortingLayerID(gameObject.transform, base.SortingLayerID);
			gameObject.transform.parent = transform.parent;
			gameObject.name = "heart_" + i;
			hearts.Add(gameObject.GetComponent<Animator>());
		}
		UnityEngine.Object.Destroy(transform.gameObject);
	}

	public void SetHearts(bool animate = true)
	{
		StandardButton component = btn_getlifes.GetComponent<StandardButton>();
		component.Enabled = Player.Lives < 5;
		btn_getlifes.enabled = component.Enabled;
		if (!animate)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			if (i < Player.Lives)
			{
				hearts[i].Play("heart full");
			}
			else
			{
				hearts[i].Play("heart empty");
			}
			hearts[i].Update(Time.deltaTime);
		}
	}

	public void RefillHearts()
	{
		for (int i = 0; i < hearts.Count; i++)
		{
			SetHeartFullSortingOrder(hearts[i], 45);
			if (hearts[i].GetCurrentAnimatorStateInfo(0).IsName("heart empty") && i < Player.Lives)
			{
				hearts[i].Play("heart full in");
			}
		}
		SetHearts(false);
	}

	private void SetHeartFullSortingOrder(Animator heart, int order)
	{
		heart.transform.Find("heart_full").GetComponent<SpriteRenderer>().sortingOrder = order;
	}

	public void RemoveHeart()
	{
		int index = Mathf.Clamp(Player.Lives, 0, hearts.Count);
		SetHeartFullSortingOrder(hearts[index], 100);
		hearts[index].Play("heart empty in");
	}

	protected override void OnDestroy()
	{
		Player.OnGenerateLife -= RefillHearts;
		Wallet.OnBalanceChanged -= onBalanceChanged;
		base.OnDestroy();
	}

	private void onBalanceChanged()
	{
		if (!base.isVisible)
		{
			return;
		}
		GoTweenConfig goTweenConfig = new GoTweenConfig();
		goTweenConfig.scale(Vector3.one * 1.25f);
		goTweenConfig.setIterations(4);
		goTweenConfig.loopType = GoLoopType.PingPong;
		if (tweenCoins != Wallet.Coins)
		{
			Go.to(icon_coins.transform, 0.15f, goTweenConfig);
			Go.to(this, 1f, new GoTweenConfig().intProp("tweenCoins", Wallet.Coins).setDelay(tweenDelay).onUpdate(delegate
			{
				coins.text = tweenCoins.ToString().GetGroupedNumberString();
			}));
		}
		if (tweenDiamonds != Wallet.Diamonds)
		{
			Go.to(icon_diamonds.transform, 0.15f, goTweenConfig);
			Go.to(this, 1f, new GoTweenConfig().intProp("tweenDiamonds", Wallet.Diamonds).setDelay(tweenDelay).onUpdate(delegate
			{
				diamonds.text = tweenDiamonds.ToString().GetGroupedNumberString();
			}));
		}
		tweenDelay = 0f;
	}

	private IEnumerator UpdateTimerLabel()
	{
		while (base.isVisible && Player.Lives != 5)
		{
			TimeSpan ts = TimeSpan.FromSeconds(Player.GetSecondsToNextLive());
			string hours = "New life in:".Localize() + " ";
			if (ts.Hours > 0)
			{
				hours = hours + ts.Hours.ToString("00") + ":";
			}
			timer.text = hours + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
			yield return new WaitForSeconds(1f);
		}
		if (base.isVisible)
		{
			Go.to(timer.transform.parent, 0.4f, new GoTweenConfig().localPosition(timer.transform.parent.localPosition.SetY(100f)).setEaseType(GoEaseType.CircOut));
		}
	}

	public override void Show()
	{
		Show(false);
	}

	public void Show(bool animate)
	{
		Start();
		base.gameObject.SetActive(true);
		SetSalesbadges();
		StopAllCoroutines();
		StartCoroutine(WatchPromotionTime());
		SetHearts();
		base.Show();
		bool flag = Player.Lives != 5;
		timer.transform.parent.gameObject.SetActive(flag);
		if (flag)
		{
			StartCoroutine(UpdateTimerLabel());
		}
		ShowFrom(base.top, null, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
		if (animate)
		{
			float delay = 3f;
			Go.to(this, 1f, new GoTweenConfig().setDelay(delay).intProp("tweenCoins", Wallet.Coins).onUpdate(delegate
			{
				coins.text = tweenCoins.ToString().GetGroupedNumberString();
				coins.Commit();
			}));
			WaitThen(delay, TweenProgressBar);
			diamonds.text = Wallet.Diamonds.ToString().GetGroupedNumberString();
			diamonds.Commit();
		}
		else
		{
			coins.text = Wallet.Coins.ToString().GetGroupedNumberString();
			coins.Commit();
			diamonds.text = Wallet.Diamonds.ToString().GetGroupedNumberString();
			diamonds.Commit();
		}
	}

	public override void Hide()
	{
		base.Hide();
		StopAllCoroutines();
		HideTo(base.top, delegate
		{
			base.gameObject.SetActive(false);
		}, 0.3f, GoUpdateType.TimeScaleIndependentUpdate);
	}

	public void AnimateDiamondsDifference()
	{
		Go.to(this, 0.5f, new GoTweenConfig().setDelay(1.5f).intProp("tweenDiamonds", Wallet.Diamonds).onUpdate(delegate
		{
			diamonds.text = tweenDiamonds.ToString().GetGroupedNumberString();
			diamonds.Commit();
		}));
	}

	private float GetXpProgress(int level)
	{
		int num = Mathf.Clamp(level + 1, 0, Konfiguration.XpLevels.Count - 1);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			num2 += Konfiguration.XpLevels[i].xp;
		}
		int num3 = Player.XP - num2;
		return (float)num3 / (float)Konfiguration.XpLevels[num].xp;
	}

	private void TweenProgressBar()
	{
		float xpProgress = GetXpProgress(xpLevelOnStart);
		Debug.Log("xp progress: " + xpProgress);
		float duration = (xpProgress - tweenProgressbar) * 2f;
		Go.to(this, duration, new GoTweenConfig().floatProp("tweenProgressbar", Mathf.Clamp01(xpProgress)).setEaseType(GoEaseType.Linear).onUpdate(delegate
		{
			xpProgressBar.ClipRect = new Rect(0f, 0f, tweenProgressbar, 1f);
		})
			.onComplete(delegate
			{
				if (xpProgress >= 1f)
				{
					xpLevelOnStart++;
					xpLevel.text = (xpLevelOnStart + 1).ToString();
					xpLevel.Commit();
					tweenProgressbar = 0f;
					xpProgressBar.ClipRect = new Rect(0f, 0f, tweenProgressbar, 1f);
					TweenProgressBar();
				}
			}));
	}
}
