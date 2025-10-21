public class IngameHinting : AbstractTutorialScreen
{
	protected override void Start()
	{
		base.Start();
		ScreenManager.LoadAndPush<WaitForRaptorTutorial>("Tutorial/Tutorial", this);
		WatchShotsOnUnitScreen watcher = ScreenManager.LoadAndPush<WatchShotsOnUnitScreen>("Tutorial/Tutorial", this);
		SetDialogTween(base.top, base.upperDialogPosition);
		ScreenManager.GetScreen<HudScreen>().Hide();
		Level.Instance.OnLevelPlay += actionHinting.StartNotification;
		Level.Instance.OnLevelPlay += delegate
		{
			watcher.StartWatching(UnitType.Neander_Shooter, "tutorial_watch_shooter");
		};
	}

	protected override void OnDestroy()
	{
		Level.Instance.OnLevelPlay -= actionHinting.StartNotification;
		base.OnDestroy();
	}
}
