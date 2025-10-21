public class Invincibility : MonoBase
{
	private BaseEntity entitiy;

	private int max_health;

	private Healthbar healthbar;

	private bool keepAlive;

	private bool isInitialized;

	public bool hide_health_bar
	{
		set
		{
			healthbar.enabled = !value;
		}
	}

	private void Start()
	{
		if (!isInitialized)
		{
			entitiy = GetComponent<BaseEntity>();
			max_health = entitiy.max_health;
			healthbar = GetComponentInChildren<Healthbar>();
			isInitialized = true;
		}
	}

	private void OnEnable()
	{
		Start();
		HealUp();
		healthbar.enabled = false;
		keepAlive = true;
	}

	private void OnDisable()
	{
		healthbar.enabled = true;
		keepAlive = false;
	}

	private void HealUp()
	{
		if (entitiy.Health < max_health)
		{
			entitiy.ChangeHealth(max_health - entitiy.Health, null);
		}
	}

	private void Update()
	{
		if (keepAlive)
		{
			HealUp();
		}
	}
}
