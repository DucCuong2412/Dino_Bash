public class WaitForTricerTutorial : WaitForUnitTutorial
{
	protected override void Start()
	{
		unit = UnitType.Tricer;
		loca_key = "wait_for_tank_tutorial";
		base.Start();
	}
}
