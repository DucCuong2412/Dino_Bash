using System.Xml.Serialization;

public struct LevelEnemy
{
	[XmlAttribute("Unit")]
	public UnitType unittype;

	[XmlAttribute("Delay")]
	public float delay;

	[XmlAttribute("Level")]
	public int unit_level;

	[XmlAttribute("Command")]
	public string command;

	public LevelEnemy(UnitType unittype, float delay, int unit_level, string command)
	{
		this.unittype = unittype;
		this.delay = delay;
		this.unit_level = unit_level;
		this.command = command;
	}
}
