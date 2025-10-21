public class WaitForRaptorTutorial : WaitForUnitTutorial
{
	protected override void Start()
	{
		unit = UnitType.Raptor;
		loca_key = "wait_for_raptor_tutorial";
		show_arrow = false;
		base.Start();
	}
}
