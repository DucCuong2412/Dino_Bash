using System.Xml.Serialization;

public class SelectionOverrideData
{
	[XmlAttribute("Unit")]
	public UnitType unit;

	[XmlAttribute("Shot")]
	public ShotType shot;

	[XmlAttribute("UnitLevel")]
	public int unit_level = -1;

	[XmlAttribute("ShotLevel")]
	public int shot_level = -1;
}
