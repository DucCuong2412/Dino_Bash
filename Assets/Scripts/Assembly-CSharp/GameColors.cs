using UnityEngine;

public class GameColors : MonoBehaviour
{
	private static GameColors instance;

	[SerializeField]
	private Color[] environmentShadow;

	[SerializeField]
	private Color unlockScreenShadow;

	public static Color[] EnvironmentShadow
	{
		get
		{
			return instance.environmentShadow;
		}
	}

	public static Color UnlockScreenShadow
	{
		get
		{
			return instance.unlockScreenShadow;
		}
	}

	private void Start()
	{
		instance = this;
	}
}
