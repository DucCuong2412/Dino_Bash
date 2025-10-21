using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class Crypto
{
	private static readonly string encrypted_key = "ow4YAHUAZQBgAB8AIgCYAFEArQAgADwANAACAM4AigDfAAUA6ABUADwAdABNAIgAEQDAADYA4ABlAAEAlwDDDhAAYABuABAAKAAVAP0A3QCHAAUAMABrAAoAhADLAMMAJwDVAGcAbwBDAFEA3QA0AP4AbgDjAGIAHwDiAJkOeQBkAF4AOQBYABIAvAAFAKoAPgBeADYAdgDMAMUAqgAbANcAQQA/ABcATQCZAEwA4AAkAP4AFABKAKUAlw54AFgAKQAAAGAAMgC4ABsAugB0AC8AdQB4AJkAgwD/ABAAxgAhACQAIwBBAIsATgDNAF8A+QBwAH0ArwC0DjgANwA=";

	private static readonly string runtime_key = "\u0efbsQ\r@ky±j\u008e\\u\u0004#ñå\u008cY£\f\v/\u0015ño\u0087\u001d\u00a0&'Ø";

	public static string shared_key
	{
		get
		{
			return encrypted_key.DecodeFrom64().EncryptOrDecrypt(runtime_key);
		}
	}

	public static string EncryptOrDecrypt(this string text, string key)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			stringBuilder.Append((char)(text[i] ^ key[i % key.Length]));
		}
		return stringBuilder.ToString();
	}

	public static string EncodeTo64(this string toEncode)
	{
		byte[] bytes = Encoding.Unicode.GetBytes(toEncode);
		return Convert.ToBase64String(bytes);
	}

	public static string DecodeFrom64(this string encodedData)
	{
		byte[] bytes = Convert.FromBase64String(encodedData);
		return Encoding.Unicode.GetString(bytes);
	}

	public static void EncryptFile(string path)
	{
		string text;
		using (StreamReader streamReader = new StreamReader(path))
		{
			text = streamReader.ReadToEnd();
			text = text.Encrypt(shared_key);
		}
		using (StreamWriter streamWriter = new StreamWriter(path))
		{
			streamWriter.Write(text);
		}
	}

	public static string DecryptFile(string path)
	{
		//Discarded unreachable code: IL_001f
		using (StreamReader streamReader = new StreamReader(path))
		{
			string s = streamReader.ReadToEnd();
			return s.Decrypt(shared_key);
		}
	}

	public static string Encrypt(this string s, string key)
	{
		s = s.EncryptOrDecrypt(key);
		s = s.EncryptOrDecrypt(SystemInfo.deviceUniqueIdentifier);
		s = s.EncodeTo64();
		return s;
	}

	public static string Decrypt(this string s, string key)
	{
		s = s.DecodeFrom64();
		s = s.EncryptOrDecrypt(SystemInfo.deviceUniqueIdentifier);
		s = s.EncryptOrDecrypt(key);
		return s;
	}
}
