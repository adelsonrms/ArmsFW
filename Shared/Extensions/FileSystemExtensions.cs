using System;
using System.IO;

namespace ArmsFW.Services.Extensions.FileSystem
{
	public enum eFileNameType
	{
		FullName,
		NameWithoutExtension
	}
	public static class FileSystemExtensions
	{
		public static string GetFileName(this string strFile, eFileNameType type = eFileNameType.FullName)
		{
			return type switch
			{
				eFileNameType.FullName => Path.GetFileName(strFile), 
				eFileNameType.NameWithoutExtension => Path.GetFileNameWithoutExtension(strFile), 
				_ => strFile, 
			};
		}


		public static string GetFileText(this string strFile)
		{
			return File.ReadAllText(strFile);
		}

		public static string GetExtension(this string strFile)
		{
			try
			{
				return Path.GetExtension(strFile);
			}
			catch (Exception)
			{
			}
			return "";
		}

		public static FileInfo GetFileInfo(this string strFile)
		{
			try
			{
				return new FileInfo(strFile);
			}
			catch (Exception)
			{
			}
			return new FileInfo("");
		}

		public static string GetFolder(this string strFile, eFileNameType type = eFileNameType.FullName)
		{
			return Path.GetDirectoryName(strFile);
		}

		public static int GetLength(this string strFile)
		{
			if (strFile == null)
			{
				return 0;
			}
			return (int)new FileInfo(strFile).Length;
		}

		public static string ChangeExtension(this string strFile, string newExtension)
		{
			return Path.GetDirectoryName(strFile) + "\\" + Path.GetFileNameWithoutExtension(strFile) + newExtension;
		}

		public static bool FileExists(this string strFile)
		{
			return File.Exists(strFile);
		}

		public static string FileFormat(this byte[] bytes)
		{
			return NumberFileFormat(bytes.Length);
		}

		public static string FileFormat(this int bytes)
		{
			return NumberFileFormat(bytes);
		}

		public static string FileFormat(this long bytes)
		{
			return NumberFileFormat(bytes);
		}

		private static string NumberFileFormat(long bytes)
		{
			if (bytes < 0)
			{
				throw new ArgumentException("bytes");
			}
			double num;
			string text;
			if (bytes >= 1152921504606846976L)
			{
				num = bytes >> 50;
				text = "EB";
			}
			else if (bytes >= 1125899906842624L)
			{
				num = bytes >> 40;
				text = "PB";
			}
			else if (bytes >= 1099511627776L)
			{
				num = bytes >> 30;
				text = "TB";
			}
			else if (bytes >= 1073741824)
			{
				num = bytes >> 20;
				text = "GB";
			}
			else if (bytes >= 1048576)
			{
				num = bytes >> 10;
				text = "MB";
			}
			else
			{
				if (bytes < 1024)
				{
					return bytes.ToString("0 B");
				}
				num = bytes;
				text = "KB";
			}
			return (num / 1024.0).ToString("0.## ") + text;
		}

		public static string FormatarTamanho(this int bytes)
		{
			return NumberFileFormat(bytes);
		}

		public static string FormatarTamanho(this int? bytes)
		{
            if (!bytes.HasValue)
            {
				return "0 Kb";
            }

			return NumberFileFormat(bytes.Value);
		}
	}
}
