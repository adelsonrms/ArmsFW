using System;
using System.IO;

namespace app.web.Bussines.Api
{
    public static class IFormFileExtensions
	{
		public static string FileExtension(this ArmsFW.Services.Upload.IFormFile file)
		{
			string text = InverteString(file.FileName);
			return InverteString(text.Substring(0, text.IndexOf(".", StringComparison.Ordinal)));
		}

		private static string InverteString(string s)
		{
			char[] array = s.ToCharArray();
			Array.Reverse(array);
			return new string(array);
		}

		public static byte[] GetBytes(this Microsoft.AspNetCore.Http.IFormFile file)
		{
			try
			{
				return new BinaryReader(file.OpenReadStream()).ReadBytes((int)file.Length);
			}
			catch
			{
				return null;
			}
		}
	}
}
