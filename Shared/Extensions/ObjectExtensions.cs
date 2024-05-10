using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArmsFW.Services.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Força a conversão de uma expressao String para Int.
        /// </summary>
        /// <param name="String">A expressão numerica a ser convertida</param>
        /// <returns>Retorna o valor convertido em Int. Se ocorrer erro, retorna 0</returns>
        public static string ObjectToJson(this object obj)
        {
            try
            {
                var strObject = JsonConvert.SerializeObject(obj);
                return System.Text.RegularExpressions.Regex.Unescape(strObject);
            }
            catch
            {
                return null;
            }
        }

        public static int ToInt(this object input)
        {
            if (input==null)
            {
                return 0;
            }


            if (int.TryParse(input.ToString(), out int result)) return result;
            return 0;
        }
    }
}