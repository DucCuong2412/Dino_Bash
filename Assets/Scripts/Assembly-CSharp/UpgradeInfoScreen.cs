using UnityEngine;
using dinobash;

public abstract class UpgradeInfoScreen<T> : BaseScreen
{
    protected const float hide_time = 0.75f;

    public T item;

    protected tk2dUIItem upgradeButton;

    private StandardButton closeButton;

    private LocalizedText title;

    protected LocalizedText description;

    protected LocalizedText label_upgrade;

    protected LocalizedText label_upgrade_duration;

    protected Transform itemPortraitParent;

    protected InfoContainer infoContainer;

    protected Transform stats_bg;

    protected ParticleSystem FX_onBuy;

    protected ParticleSystem FX_Upgrade;

    protected tk2dTextMesh cosumable_count_label;

    protected tk2dTextMesh locked_until_reached_level_label;

    private tk2dTextMesh labelUpgradePrice;

    private tk2dTextMesh special_offer_label;

    private tk2dTextMesh label_regular_price;

    private Animator special_offer_animator;

    protected SpriteRenderer coin_icon;

    protected SpriteRenderer diamond_icon;

    protected float upgrade_button_offset = 320f;

    protected Transform itemPortrait;

    protected bool is_special_offer;

    protected float hide_wait_duration;

    protected int specialOfferCost
    {
        get
        {
            return 0;// Mathf.RoundToInt((float)getCost() * Konfiguration.GameConfig.SpecialOfferSaveRate);
        }
    }

    protected abstract void OnClickedUpgrade();

    protected abstract int getCost();

    protected abstract bool isUpgrading();

    protected abstract bool isPremium();

    protected abstract bool isUnlock();

    protected abstract void setInfos(bool isSpecialOffer);

    protected abstract int getUpgradeLevelRequirement();

    protected void Start()
    {
        infoContainer = InfoContainer.Load(base.transform.Find("MiddleCenter/infoContainer_position"));
        upgradeButton = FindChildComponent<tk2dUIItem>("MiddleCenter/btn_upgrade_group/normal/btn_upgrade");
        closeButton = FindChildComponent<StandardButton>("MiddleCenter/btn_close");
        closeButton.clickSound = Sounds.main_close_popup;
        title = FindChildComponent<LocalizedText>("MiddleCenter/title/label_item_name");
        coin_icon = FindChildComponent<SpriteRenderer>("MiddleCenter/btn_upgrade_group/normal/icon_coin");
        diamond_icon = FindChildComponent<SpriteRenderer>("MiddleCenter/btn_upgrade_group/normal/icon_diamond");
        itemPortraitParent = FindChildComponent<Transform>("MiddleCenter/item_portrait");
        stats_bg = FindChildComponent<Transform>("MiddleCenter/stats_bg");
        FX_onBuy = FindChildComponent<ParticleSystem>("MiddleCenter/FX_Stars");
        FX_onBuy.gameObject.SetActive(false);
        FX_Upgrade = FindChildComponent<ParticleSystem>("MiddleCenter/FX_Stars/upgrade");
        FX_Upgrade.GetComponent<Renderer>().enabled = false;
        label_upgrade_duration = FindChildComponent<LocalizedText>("MiddleCenter/label_upgrade_duration");
        label_upgrade_duration.gameObject.SetActive(false);
        locked_until_reached_level_label = FindChildComponent<tk2dTextMesh>("MiddleCenter/btn_upgrade_group/locked_panel/label_level");
        locked_until_reached_level_label.transform.parent.gameObject.SetActive(false);
        label_regular_price = FindChildComponent<tk2dTextMesh>("MiddleCenter/regular_price/label_regular_price");
        label_upgrade = FindChildComponent<LocalizedText>("MiddleCenter/btn_upgrade_group/normal/btn_upgrade/label_upgrade");
        special_offer_label = FindChildComponent<tk2dTextMesh>("MiddleCenter/plants/stiehl 01/blatt 4 01/special_offer_label");
        labelUpgradePrice = FindChildComponent<tk2dTextMesh>("MiddleCenter/btn_upgrade_group/normal/label_price");
        description = FindChildComponent<LocalizedText>("MiddleCenter/stonepanel_6x2/description");
        special_offer_animator = FindChildComponent<Animator>("MiddleCenter/plants");
        cosumable_count_label = FindChildComponent<tk2dTextMesh>("MiddleCenter/item_consumable/label_cosumable_count");
        cosumable_count_label.transform.parent.gameObject.SetActive(false);
        DiamondWidget componentInChildren = GetComponentInChildren<DiamondWidget>();
        if (App.State != App.States.Game)
        {
            componentInChildren.gameObject.SetActive(false);
        }
        base.Interactive = false;
        base.transform.position += base.right;
        base.gameObject.SetActive(false);
    }

    public void Show(T t, bool isSpecialOffer = false)
    {
        Go.tweensWithTarget(base.transform).ForEach(delegate (GoTween tw)
        {
            tw.complete();
        });
        base.gameObject.SetActive(true);
        OnEscapeUp = Hide;
        if (App.State == App.States.Map)
        {
            ScreenManager.GetScreen<CoverScreen>(this).Show();
        }
        item = t;
        closeButton.uiItem.OnClick += Hide;
        upgradeButton.OnClick += OnClickedUpgrade;
        upgradeButton.GetComponent<SpriteRenderer>().color = Color.white;
        FX_onBuy.gameObject.SetActive(false);
        coin_icon.enabled = !isPremium();
        diamond_icon.enabled = isPremium();
        title.Key = t.ToString();
        hide_wait_duration = 0f;
        is_special_offer = isSpecialOffer;
        if (is_special_offer)
        {
            special_offer_animator.Play("in");
        }
        //special_offer_label.text = string.Format("Save {0:P0}".Localize().ToUpper(), 1f - Konfiguration.GameConfig.SpecialOfferSaveRate);
        label_regular_price.text = getCost().ToString();
        labelUpgradePrice.text = ((!is_special_offer) ? getCost().ToString().GetGroupedNumberString() : specialOfferCost.ToString().GetGroupedNumberString());
        infoContainer.gameObject.SetActive(!isSpecialOffer);
        setInfos(isSpecialOffer);
        TestPlayerProgressRequirement();
        itemPortrait.RepositionAndReparent(itemPortraitParent);
        EntityShadow componentInChildren = itemPortrait.GetComponentInChildren<EntityShadow>();
        if (componentInChildren != null)
        {
            componentInChildren.sprite.color = GameColors.UnlockScreenShadow;
        }
        SpriteTools.SetSortingLayerID(itemPortrait, base.SortingLayerID);
        SpriteTools.OffsetSortingOrder(itemPortrait, 1000);
        base.Show();
        ShowFrom(base.right);
    }

    protected void TestPlayerProgressRequirement()
    {
        if (Player.MaxLevelID < getUpgradeLevelRequirement())
        {
            upgradeButton.transform.parent.gameObject.SetActive(false);
            locked_until_reached_level_label.transform.parent.gameObject.SetActive(true);
            locked_until_reached_level_label.text = Konfiguration.levels[getUpgradeLevelRequirement()].display_name;
        }
        else
        {
            upgradeButton.transform.parent.gameObject.SetActive(true);
            locked_until_reached_level_label.transform.parent.gameObject.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        diamond_icon.enabled = isPremium() || isUpgrading();
        coin_icon.enabled = !diamond_icon.enabled;
        if (!is_special_offer)
        {
            labelUpgradePrice.text = getCost().ToString().GetGroupedNumberString();
        }
    }

    protected string get_tracking_origin(int cost)
    {
        string text = ((App.State != App.States.Game) ? "upgrades" : ("level_" + Player.CurrentLevelID));
        return (getCost() != cost) ? "special_offer" : text;
    }

    protected bool Debit(out int cost)
    {
        cost = getCost();
        if (isPremium() || isUpgrading())
        {
            int num = (cost = ((!is_special_offer) ? getCost() : specialOfferCost));
            if (Wallet.Diamonds < num)
            {
                Tracking.store_open(Wallet.Currency.Diamonds, "upgrade_screen", item.ToString());
                ScreenManager.GetScreen<ShopScreenDiamonds>().Show();
                return false;
            }
            Wallet.TakeDiamonds(num);
            AudioPlayer.PlayGuiSFX(Sounds.main_upgrade, 0f);
            return true;
        }
        if (Wallet.Coins < getCost())
        {
            Tracking.store_open(Wallet.Currency.Coins, "upgrade_screen", item.ToString());
            ScreenManager.GetScreen<ShopScreenCoins>().Show();
            return false;
        }
        Wallet.TakeCoins(getCost());
        AudioPlayer.PlayGuiSFX(Sounds.main_upgrade, 0f);
        return true;
    }

    protected void CheckForAchievement()
    {
        UpgradeScreen screen = ScreenManager.GetScreen<UpgradeScreen>();
        bool flag = true;
        foreach (UpgradeScreenListEntry upgrade_list_entry in screen.upgrade_list_entries)
        {
            if (upgrade_list_entry.Unit_Type != 0)
            {
                if (Konfiguration.canLevelUp(upgrade_list_entry.Unit_Type))
                {
                    flag = false;
                    break;
                }
            }
            else if (upgrade_list_entry.Shot_Type != ShotType.None && Konfiguration.canLevelUp(upgrade_list_entry.Shot_Type))
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            SocialGamingManager.Instance.ReportProgress(AchievementIds.COMPLETELY_UPGRADE_EVERYTHING_IN_THE_UPGRADE_MENU, 1);
        }
    }

    public override void Hide()
    {
        closeButton.uiItem.OnClick -= Hide;
        upgradeButton.OnClick -= OnClickedUpgrade;
        base.Hide();
        base.isVisible = true;
        WaitThen(hide_wait_duration, delegate
        {
            if (App.State == App.States.Map)
            {
                ScreenManager.GetScreen<CoverScreen>(this).Hide();
            }
            HideTo(base.left, delegate
            {
                base.isVisible = false;
                Object.Destroy(itemPortrait.gameObject);
                base.gameObject.SetActive(false);
            });
        });
    }
}
