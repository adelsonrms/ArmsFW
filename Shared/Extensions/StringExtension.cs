using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ArmsFW.Services.Extensions
{
	public static class StringExtension
	{
		public static bool IsNumeric(this string input)
		{
			int result;
			return int.TryParse(input, out result);
		}

		

		public static bool ToBool(this string input)
		{
			bool result = false;
			if (bool.TryParse(input, out result))
			{
				return bool.Parse(input);
			}
			return result;
		}

		public static decimal ToDecimal(this string input)
		{
			decimal result = default(decimal);
			if (decimal.TryParse(input, out result))
			{
				return decimal.Parse(input);
			}
			return result;
		}

		public static float ToFloat(this string input)
		{
			float result = 0f;
			if (float.TryParse(input, out result))
			{
				return float.Parse(input);
			}
			return result;
		}

		public static string ToUpperFirst(this string String)
		{
			return String.Substring(0, 1).ToUpper() + String.Substring(1, String.Length - 1);
		}

		public static string SplitPor(this string String, string caractere, int posicao = 0)
		{
			try
			{
				String = String ?? "";

				string[] array = String.Split(new string[1] { caractere }, StringSplitOptions.None);
				if (posicao > array.Length)
				{
					return array[0];
				}
				return array[posicao];
			}
			catch
			{
				return string.Empty;
			}
		}

		public static string ToString(this string[] arrString, string separador, string delimitador)
		{
			string item = "";
			if (arrString.Length != 0)
			{
				arrString.ToList().ForEach(delegate (string X)
				{
					item = item + separador + delimitador + X + delimitador;
				});
				item = item.Substring(1);
			}
			return item;
		}
	
		public static bool IsBase64(this string String)
        {
			return String.StartsWith("data:") && String.Contains(";base64,");
		}

		public static string PegarBase64(this string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
				return "";
            }

			return valor.Split(",".ToCharArray())[1] ?? "";

		}
		public static byte[] ToBytes(this string String)
		{
            if (String.IsBase64())
            {
				var code = String.Split(",".ToCharArray())[1];
				return Convert.FromBase64String(code);
            }
			return null;
		}

		public static string Left(this object valor, int? tamanho = 0)
		{
			if (string.IsNullOrEmpty(valor.ToString())) return valor.ToString();

			if (tamanho > valor.ToString().Length) return valor.ToString();

			return valor.ToString().Substring(0, tamanho.GetValueOrDefault());
		}

		public static string Right(this object valor, int? tamanho = 0)
		{
			if (string.IsNullOrEmpty(valor.ToString())) return valor.ToString();

			if (tamanho > valor.ToString().Length) return valor.ToString();

			return valor.ToString().Substring(valor.ToString().Length - tamanho.GetValueOrDefault(), tamanho.GetValueOrDefault());
		}
	}
}