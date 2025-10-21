using System.Collections.Generic;
using System.Linq;
using dinobash;

public class SelectShotScreen : SelectDinoOrShotScreen<ShotType>
{
	protected override int SelectionSlots
	{
		get
		{
			return Player.shotSlots;
		}
	}

	protected override List<ShotType> getAvailable()
	{
		return Player.AvailableShotTypes;
	}

	protected override List<ShotType> getSelectedInPlayer()
	{
		return Player.SelectedShotTypes;
	}

	protected override string getSpriteName(ShotType shot)
	{
		return "shotbuy_" + shot.ToString().ToLower();
	}

	protected override float getIconScale()
	{
		return 1.215f;
	}

	protected override string getTitle()
	{
		return "select shots";
	}

	protected override int enumToInt(ShotType t)
	{
		return (int)t;
	}

	protected override bool isUpgrading(ShotType shot)
	{
		return false;
	}

	protected override void OnButtonNextClicked(List<int> selected_)
	{
		Player.SelectedShotTypes = new SerializableList<ShotType>(selected_.Select((int x) => (ShotType)x).ToList());
		Hide();
		App.stateGame(level_id);
	}

	protected override void OnButtonBackClicked(List<int> selected_)
	{
		List<ShotType> o = selected_.Select((int x) => (ShotType)x).ToList();
		Player.SelectedShotTypes = new SerializableList<ShotType>(o);
		Hide();
		base.isVisible = true;
		LevelData levelData = Konfiguration.GetLevelData(level_id);
		if (Player.AvailableUnitTypes.Count > 4 && !levelData.override_dino_selection)
		{
			HideTo(base.right, delegate
			{
				base.isVisible = false;
				CleanUpDinoSelectButtons();
				ScreenManager.GetScreen<SelectDinoScreen>().Show(level_id, base.left);
				base.gameObject.SetActive(false);
			});
			return;
		}
		ScreenManager.GetScreen<CoverScreen>(this).Hide();
		HideTo(base.right, delegate
		{
			CleanUpDinoSelectButtons();
			ScreenManager.GetScreen<ResourceBarScreen>().Show();
			ScreenManager.GetScreen<MapScreen>().Show();
			base.gameObject.SetActive(false);
		});
	}

	protected override DinoShotUpgradeAdapter GetAdapter(ShotType shot)
	{
		return new ShotAdapter(shot);
	}

	protected override void SetInfoContainer(ShotType shot)
	{
		infoContainer.Set(shot, base.SortingLayerID, false);
	}

	protected override void PlaySelectSound(ShotType shot)
	{
		AudioPlayer.PlayGuiSFX(AudioResources.GetShotSounds(shot)[0], 0f);
	}

	protected override void PlayDeselectSound()
	{
		AudioPlayer.PlayGuiSFX(Sounds.map_shot_unselect, 0f);
	}
}
