using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

[Serializable]
[XmlRoot("List")]
public class SerializableList<TValue> : List<TValue>, IXmlSerializable
{
	private static readonly XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

	public SerializableList()
	{
	}

	public SerializableList(List<TValue> o)
		: base((IEnumerable<TValue>)o)
	{
	}

	public XmlSchema GetSchema()
	{
		return null;
	}

	public void ReadXml(XmlReader reader)
	{
		bool isEmptyElement = reader.IsEmptyElement;
		reader.Read();
		if (isEmptyElement)
		{
			return;
		}
		while (reader.NodeType != XmlNodeType.EndElement)
		{
			try
			{
				TValue item = (TValue)ValueSerializer.Deserialize(reader);
				Add(item);
			}
			finally
			{
			}
			reader.MoveToContent();
		}
	}

	public void WriteXml(XmlWriter writer)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				TValue current = enumerator.Current;
				ValueSerializer.Serialize(writer, current);
			}
		}
	}
}
