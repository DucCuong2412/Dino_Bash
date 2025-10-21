using System.Collections.Generic;

public class XMLDataPaths
{
	public const string hostURL = "https://dinobash.game-bakery.com/";

	public static KeyValuePair<string, string> konfiguration = new KeyValuePair<string, string>("https://dinobash.game-bakery.com/konfiguration.xml", "/Resources/XML/konfiguration.xml");

	public static KeyValuePair<string, string> xplevels = new KeyValuePair<string, string>("https://dinobash.game-bakery.com/xplevels.xml", "/Resources/XML/xplevels.xml");

	public static KeyValuePair<string, string> unitdata = new KeyValuePair<string, string>("https://dinobash.game-bakery.com/entitydata.xml", "/Resources/XML/entitydata.xml");

	public static KeyValuePair<string, string> shotdata = new KeyValuePair<string, string>("https://dinobash.game-bakery.com/shotdata.xml", "/Resources/XML/shotdata.xml");

	public static KeyValuePair<string, string> chapters = new KeyValuePair<string, string>("https://dinobash.game-bakery.com/chapter.xml", "/Resources/XML/chapter.xml");

	public static KeyValuePair<string, string>[] All
	{
		get
		{
			return new KeyValuePair<string, string>[5] { konfiguration, xplevels, unitdata, shotdata, chapters };
		}
	}
}
