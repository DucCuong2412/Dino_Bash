using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

[Serializable]
[XmlRoot("Dictionary")]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
	private const string DefaultTagItem = "Item";

	private const string DefaultTagKey = "Key";

	private const string DefaultTagValue = "Value";

	private static readonly XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));

	private static readonly XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

	protected virtual string ItemTagName
	{
		get
		{
			return "Item";
		}
	}

	protected virtual string KeyTagName
	{
		get
		{
			return "Key";
		}
	}

	protected virtual string ValueTagName
	{
		get
		{
			return "Value";
		}
	}

	public SerializableDictionary()
	{
	}

	protected SerializableDictionary(SerializationInfo info, StreamingContext context)
		: base(info, context)
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
		try
		{
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement(ItemTagName);
				try
				{
					reader.ReadStartElement(KeyTagName);
					TKey key;
					try
					{
						key = (TKey)KeySerializer.Deserialize(reader);
					}
					finally
					{
						reader.ReadEndElement();
					}
					reader.ReadStartElement(ValueTagName);
					TValue value;
					try
					{
						value = (TValue)ValueSerializer.Deserialize(reader);
					}
					finally
					{
						reader.ReadEndElement();
					}
					Add(key, value);
				}
				finally
				{
					reader.ReadEndElement();
				}
				reader.MoveToContent();
			}
		}
		finally
		{
			reader.ReadEndElement();
		}
	}

	public void WriteXml(XmlWriter writer)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, TValue> current = enumerator.Current;
				writer.WriteStartElement(ItemTagName);
				try
				{
					writer.WriteStartElement(KeyTagName);
					try
					{
						KeySerializer.Serialize(writer, current.Key);
					}
					finally
					{
						writer.WriteEndElement();
					}
					writer.WriteStartElement(ValueTagName);
					try
					{
						ValueSerializer.Serialize(writer, current.Value);
					}
					finally
					{
						writer.WriteEndElement();
					}
				}
				finally
				{
					writer.WriteEndElement();
				}
			}
		}
	}
}
