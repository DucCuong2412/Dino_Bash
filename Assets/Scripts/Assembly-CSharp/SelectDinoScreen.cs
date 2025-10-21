using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using dinobash;

public class SelectDinoScreen : SelectDinoOrShotScreen<UnitType>
{
    protected override int SelectionSlots
    {
        get
        {
            return 4;
        }
    }

    protected override List<UnitType> getAvailable()
    {
        return Player.AvailableUnitTypes;
    }

    protected override List<UnitType> getSelectedInPlayer()
    {
        return Player.SelectedUnitTypes;
    }

    protected override string getSpriteName(UnitType unit_type)
    {
        return "dinobuy_" + unit_type.ToString().ToLower();
    }

    protected override float getIconScale()
    {
        return 1f;
    }

    protected override string getTitle()
    {
        return "select dinos";
    }

    protected override int enumToInt(UnitType t)
    {
        return (int)t;
    }

    protected override bool isUpgrading(UnitType dino)
    {
        //if (Konfiguration.GameConfig.Use_upgrade_locks)
        //{
        //	return EntityTimers.is_upgrading(dino);
        //}
        return false;
    }

    public override void Show(int level_id, Vector3 showFrom)
    {
        base.Show(level_id, showFrom);
        EntityTimers.onUpgradeComplete += HandleonUpgradeComplete;
    }

    public override void Hide()
    {
        EntityTimers.onUpgradeComplete -= HandleonUpgradeComplete;
        base.Hide();
    }

    private void HandleonUpgradeComplete(UnitType dino)
    {
        DinoSelectButton dinoSelectButton = all_dino_buttons.Find((DinoSelectButton x) => x.sprite_name == getSpriteName(dino));
        if (dinoSelectButton != null)
        {
            dinoSelectButton.is_upgrading = false;
        }
    }

    protected override void OnButtonNextClicked(List<int> selected_)
    {
        Player.SelectedUnitTypes = new SerializableList<UnitType>(selected_.Select((int x) => (UnitType)x).ToList());
        List<UnitType> list = Player.SelectedUnitTypes.FindAll((UnitType x) => EntityTimers.is_upgrading(x));
        //if (Konfiguration.GameConfig.Use_upgrade_locks && list.Count > 0)
        //{
        //	ShowInTrainingError();
        //	return;
        //}
        Hide();
        HideInTrainingError();
        LevelData levelData = Konfiguration.GetLevelData(level_id);
        if (Player.AvailableShotTypes.Count > Player.shotSlots && !levelData.override_shots_selection)
        {
            HideTo(base.left, delegate
            {
                CleanUpDinoSelectButtons();
                ScreenManager.GetScreen<SelectShotScreen>().Show(level_id, base.right);
                base.gameObject.SetActive(false);
            });
        }
        else
        {
            Player.SelectedShotTypes = new SerializableList<ShotType>(Player.AvailableShotTypes);
            App.stateGame(level_id);
        }
    }

    protected override void OnButtonBackClicked(List<int> selected_)
    {
        Player.SelectedUnitTypes = new SerializableList<UnitType>(selected_.Select((int x) => (UnitType)x).ToList());
        Hide();
        base.isVisible = true;
        HideInTrainingError();
        ScreenManager.GetScreen<CoverScreen>(this).Hide();
        HideTo(base.right, delegate
        {
            base.isVisible = false;
            CleanUpDinoSelectButtons();
            ScreenManager.GetScreen<ResourceBarScreen>().Show();
            ScreenManager.GetScreen<MapScreen>().Show();
            base.gameObject.SetActive(false);
        });
    }

    protected override DinoShotUpgradeAdapter GetAdapter(UnitType dino)
    {
        return new DinoAdapter(dino);
    }

    protected override void SetInfoContainer(UnitType dino)
    {
        infoContainer.Set(dino, base.SortingLayerID, false);
    }

    protected override void PlayDeselectSound()
    {
        AudioPlayer.PlayGuiSFX(Sounds.map_dino_unselect, 0f);
    }

    protected override void PlaySelectSound(UnitType dino)
    {
        AudioPlayer.PlayGuiSFX(AudioResources.GetDinoSelectSound(dino), 0f);
    }
}
