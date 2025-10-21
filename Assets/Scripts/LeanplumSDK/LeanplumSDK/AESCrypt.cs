using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LeanplumSDK
{
	internal class AESCrypt
	{
		private const int iterations = 1000;

		private const int keySize = 256;

		private const string salt = "L3@nP1Vm";

		private const string vector = "__l3anplum__iv__";

		public static string Encrypt(string plaintext, string key)
		{
			return Encrypt<AesManaged>(plaintext, key);
		}

		private static string Encrypt<T>(string plaintext, string key) where T : SymmetricAlgorithm, new()
		{
			//Discarded unreachable code: IL_0104
			byte[] bytes = Encoding.ASCII.GetBytes("__l3anplum__iv__");
			byte[] bytes2 = Encoding.ASCII.GetBytes("L3@nP1Vm");
			byte[] bytes3 = Encoding.ASCII.GetBytes(plaintext);
			T val = new T();
			byte[] inArray;
			try
			{
				try
				{
					Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, bytes2, 1000);
					byte[] bytes4 = rfc2898DeriveBytes.GetBytes(32);
					val.Mode = CipherMode.CBC;
					using (ICryptoTransform transform = val.CreateEncryptor(bytes4, bytes))
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
							{
								cryptoStream.Write(bytes3, 0, bytes3.Length);
								cryptoStream.FlushFinalBlock();
								inArray = memoryStream.ToArray();
							}
						}
					}
				}
				catch (Exception ex)
				{
					LeanplumNative.CompatibilityLayer.LogError("Error performing encryption. " + ex.ToString());
					return string.Empty;
				}
				val.Clear();
			}
			finally
			{
				if (val != null)
				{
					((IDisposable)val).Dispose();
				}
			}
			return Convert.ToBase64String(inArray);
		}

		public static string Decrypt(string ciphertext, string key)
		{
			return Decrypt<AesManaged>(ciphertext, key);
		}

		private static string Decrypt<T>(string ciphertext, string key) where T : SymmetricAlgorithm, new()
		{
			//Discarded unreachable code: IL_00ff
			byte[] bytes = Encoding.ASCII.GetBytes("__l3anplum__iv__");
			byte[] bytes2 = Encoding.ASCII.GetBytes("L3@nP1Vm");
			byte[] array = Convert.FromBase64String(ciphertext);
			int count = 0;
			T val = new T();
			byte[] array2;
			try
			{
				try
				{
					Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, bytes2, 1000);
					byte[] bytes3 = rfc2898DeriveBytes.GetBytes(32);
					val.Mode = CipherMode.CBC;
					using (ICryptoTransform transform = val.CreateDecryptor(bytes3, bytes))
					{
						using (MemoryStream stream = new MemoryStream(array))
						{
							using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
							{
								array2 = new byte[array.Length];
								count = cryptoStream.Read(array2, 0, array2.Length);
							}
						}
					}
				}
				catch (Exception ex)
				{
					LeanplumNative.CompatibilityLayer.LogError("Error performing decryption. " + ex.ToString());
					return string.Empty;
				}
				val.Clear();
			}
			finally
			{
				if (val != null)
				{
					((IDisposable)val).Dispose();
				}
			}
			return Encoding.UTF8.GetString(array2, 0, count);
		}
	}
}
