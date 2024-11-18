using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ArmsFW.Services.Shared.Util
{
	public class SecurityUtil
	{
		public static string Key => "a8736d09e7d64be5b31cf0de6ac1de23";

		public static string EncryptString(string plainText, string key = "")
		{
			if (string.IsNullOrEmpty(plainText))
			{
				return "";
			}
			if (string.IsNullOrEmpty(key))
			{
				key = Key;
			}
			byte[] iV = new byte[16];
			byte[] inArray;
			using (Aes aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(key);
				aes.IV = iV;
				ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
				using MemoryStream memoryStream = new MemoryStream();
				using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					streamWriter.Write(plainText);
				}
				inArray = memoryStream.ToArray();
			}
			return Convert.ToBase64String(inArray);
		}

		public static string DecryptString(string cipherText, string key = "")
		{
			if (string.IsNullOrEmpty(cipherText))
			{
				return "";
			}
			if (string.IsNullOrEmpty(key))
			{
				key = Key;
			}
			byte[] iV = new byte[16];
			byte[] buffer = Convert.FromBase64String(cipherText);
			using Aes aes = Aes.Create();
			aes.Key = Encoding.UTF8.GetBytes(key);
			aes.IV = iV;
			ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
			using MemoryStream stream = new MemoryStream(buffer);
			using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
			using StreamReader streamReader = new StreamReader(stream2);
			return streamReader.ReadToEnd();
		}
	}
}
