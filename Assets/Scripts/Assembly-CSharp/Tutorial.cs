using System;
using System.Xml.Serialization;

[Serializable]
public class Tutorial
{
	[XmlAttribute("name")]
	public readonly string name;

	[XmlAttribute("level")]
	public readonly string level;

	[XmlAttribute("onMap")]
	public readonly bool onMap;
}
