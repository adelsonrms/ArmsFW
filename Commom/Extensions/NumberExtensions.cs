using System;

namespace ArmsFW.Services.Extensions
{
    public static class NumberExtensions
    {
        public static string FileFormat(this byte[] bytes) => NumberFileFormat(bytes.Length);
        public static string FileFormat(this int bytes) => NumberFileFormat(bytes);

        public static decimal ToDecimal(this object valor)
        {
            try
            {
                return Convert.ToDecimal(valor);
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public static string ToCurrency(this object valor)
        {
            try
            {
                return Convert.ToDecimal(valor).ToString("C", System.Globalization.CultureInfo.CurrentCulture);
            }
            catch
            {
                return valor.ToString();
            }
        }
        private static string NumberFileFormat(long bytes)
        {
            if (bytes < 0) throw new ArgumentException("bytes");

            double humano;
            string sufixo;

            if (bytes >= 1152921504606846976L) // Exabyte (1024^6)
            {
                humano = bytes >> 50;
                sufixo = "EB";
            }
            else if (bytes >= 1125899906842624L) // Petabyte (1024^5)
            {
                humano = bytes >> 40;
                sufixo = "PB";
            }
            else if (bytes >= 1099511627776L) // Terabyte (1024^4)
            {
                humano = bytes >> 30;
                sufixo = "TB";
            }
            else if (bytes >= 1073741824) // Gigabyte (1024^3)
            {
                humano = bytes >> 20;
                sufixo = "GB";
            }
            else if (bytes >= 1048576) // Megabyte (1024^2)
            {
                humano = bytes >> 10;
                sufixo = "MB";
            }
            else if (bytes >= 1024) // Kilobyte (1024^1)
            {
                humano = bytes;
                sufixo = "KB";
            }
            else return bytes.ToString("0 B"); // Byte

            humano /= 1024;
            return humano.ToString("0.## ") + sufixo;
        }
    }
}
