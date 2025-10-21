using UnityEngine;
using dinobash;

public class DebugInput : MonoBehaviour
{
	private void Start()
	{
		if (!Debug.isDebugBuild)
		{
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.A))
		{
			SocialGamingManager.Instance.ShowAchievementPopup(AchievementIds.COMPLETELY_UPGRADE_YOUR_APPLE_COUNTER);
		}
		if (App.State == App.States.Game)
		{
			if (Input.GetKeyUp(KeyCode.W))
			{
				Player.Instance.LevelXP += Level.Instance.Config.kill_xp;
				Player.Instance.LevelCoins += Level.Instance.Config.Kill_coins;
				Level.Instance.stateWon();
			}
			if (Input.GetKeyUp(KeyCode.L))
			{
				Level.Instance.stateLost();
			}
			if (Input.GetKeyUp(KeyCode.S))
			{
				EntityFactory.Create(UnitType.Blizzard);
			}
			if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.F))
			{
				Time.timeScale = 4f;
			}
			if (Input.GetKeyUp(KeyCode.KeypadPlus) || Input.GetKeyUp(KeyCode.F))
			{
				Time.timeScale = 1f;
			}
		}
	}
}
