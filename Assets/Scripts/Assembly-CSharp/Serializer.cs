using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public static class Serializer
{
	private static void Save<T>(T t, string path)
	{
		if (path == string.Empty)
		{
			Debug.LogWarning("saving failed - path error!");
			return;
		}
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		using (FileStream stream = new FileStream(path, FileMode.Create))
		{
			xmlSerializer.Serialize(stream, t);
		}
	}

	private static T DeserializeTextAsset<T>(string filename)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
		if (textAsset == null)
		{
			Debug.LogError("Could not load text asset " + filename);
		}
		return DeserializeString<T>(textAsset.ToString());
	}

	public static T DeserializeFileOrTextAsset<T>(string filename)
	{
		//Discarded unreachable code: IL_0042, IL_0069
		try
		{
			return DeserializeEncryptedFile<T>(Application.persistentDataPath + "/" + App.VERSION_CODE + "/" + filename + ".xml");
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message + ", loading from TextAsset");
			return DeserializeTextAsset<T>(filename);
		}
	}

	public static T DeserializeString<T>(string xml)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		StringReader stringReader = new StringReader(xml);
		XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
		T result = (T)xmlSerializer.Deserialize(xmlTextReader);
		xmlTextReader.Close();
		stringReader.Close();
		return result;
	}

	private static T DeserializeFile<T>(string path)
	{
		//Discarded unreachable code: IL_003d
		if (File.Exists(path))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using (FileStream fileStream = new FileStream(path, FileMode.Open))
			{
				T result = (T)xmlSerializer.Deserialize(fileStream);
				fileStream.Close();
				return result;
			}
		}
		throw new FileNotFoundException("file does not exist: " + path);
	}

	private static T DeserializeEncryptedFile<T>(string path)
	{
		if (File.Exists(path))
		{
			string xml = Crypto.DecryptFile(path);
			return DeserializeString<T>(xml);
		}
		throw new FileNotFoundException("file does not exist: " + path);
	}

	public static string SerializeToString<T>(T t)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.Encoding = new UTF8Encoding(false, true);
		xmlWriterSettings.Indent = true;
		xmlWriterSettings.OmitXmlDeclaration = false;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
			{
				xmlSerializer.Serialize(xmlWriter, t);
			}
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}
	}

	public static void SerializeToPlayerPrefs<T>(T t, string pKey)
	{
		string text = SerializeToString(t);
		text = text.EncryptOrDecrypt(Crypto.shared_key);
		text = text.EncodeTo64();
		PlayerPrefs.SetString(pKey, text);
		PlayerPrefs.Save();
	}

	public static T DeserializeFromPlayerPrefs<T>(string pKey)
	{
		if (PlayerPrefs.HasKey(pKey))
		{
			string @string = PlayerPrefs.GetString(pKey);
			@string = @string.DecodeFrom64();
			@string = @string.EncryptOrDecrypt(Crypto.shared_key);
			if (!string.IsNullOrEmpty(@string))
			{
				return DeserializeString<T>(@string);
			}
		}
		return default(T);
	}
}
