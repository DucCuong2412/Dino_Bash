using dinobash;

public class ApplesLabel : MonoSingleton<ApplesLabel>
{
	private tk2dTextMesh textMesh;

	private int currentApples;

	private Player player;

	private void Start()
	{
		textMesh = GetComponent<tk2dTextMesh>();
		player = Player.Instance;
		currentApples = 0;
	}

	private void Update()
	{
		if (currentApples != player.Apples)
		{
			currentApples = player.Apples;
			textMesh.text = currentApples.ToString();
			textMesh.Commit();
		}
	}
}
