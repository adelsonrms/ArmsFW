using ArmsFW.Services.Shared;
using System;
using System.Globalization;

namespace ArmsFW.Services.Extensions
{
    public static class DateTimeExtension
    {
        public static string ToDate(this DateTime data)
        {
            return data.Date.ToString("dd/MM/yyyy");
        }

        public static string Formatar(this DateTime? data, string formato)
        {
            if (data.HasValue)
            {
                return data.Value.ToString(formato);
            }

            return string.Empty;
        }

        public static bool IsDate(this string date, out DateTime? dt_saida)
        {
            dt_saida = null;

            if (string.IsNullOrEmpty(date)) return false;
            
            bool dt = DateTime.TryParse(date, out DateTime dt_valida);

            if (dt)
            {
                dt_saida = dt_valida;
            }
            return dt;
        }
        public static string ToDateUniversal(this DateTime data)
        {
            return data.Date.ToString("yyyyMMdd");
        }
        public static DateTime ToDateTime(this TimeSpan? date)
        {
            return Convert.ToDateTime(date);
        }
        public static DateTime ToDatetime(this string valor)
        {
            DateTime temporaryDateTimeValue;

            bool success = DateTime.TryParseExact(
                   valor.ToString(),
                   "yyyyMMdd",
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out temporaryDateTimeValue
               );

            return temporaryDateTimeValue;
        }
        public static string ToDateTimeFormat(this double valor, bool arred = false) => TimeFunctions.DecimalToDateTime24h((decimal)valor, arred);
        public static string ToDateTimeFormat(this decimal valor, bool arred = false) => TimeFunctions.DecimalToDateTime24h(valor, arred);
        public static string ToDateTimeFormat(this float valor, bool arred = false) => TimeFunctions.DecimalToDateTime24h((decimal)valor, arred);
        public static string ToDateTimeFromTimeStanp(this long valor) => TimeFunctions.SecondtsToTime(valor);
        public static DateTime ProximoMes(this DateTime data)
        {
            return data.AddMonths(1);
        }

        public static DateTime MesAnterior(this DateTime data)
        {
            return data.AddMonths(-1);
        }

        public static DateTime InicioDoMes(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1);
        }

        public static DateTime FimDoMes(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static DateTime InicioDoAno(this DateTime data)
        {
            return new DateTime(data.Year, 1, 1);
        }

        public static DateTime FimDoAno(this DateTime data)
        {
            return new DateTime(data.Year, 12, 31);
        }
        public static int IdPeriodo(this DateTime data) => data.ToString("yyyyMM01").ToInt();
        public static TimeSpan CalcularHora(DateTime? hora_Inicio, DateTime? hora_Fim)
        {
            if (hora_Inicio.HasValue && hora_Fim.HasValue)
            {
                return (new TimeSpan(hora_Fim.Value.Ticks) - new TimeSpan(hora_Inicio.Value.Ticks));
            }

            return new TimeSpan(0);
        }

        public static int NumeroDaSemanaNoMes(this DateTime date)
        {
            try
            {
                var diaSemanaInicioDoMes = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date.InicioDoMes(), CalendarWeekRule.FirstDay,DayOfWeek.Sunday);

                var diaDaSemana = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                     new DateTime(date.Year, date.Month, date.Day),
                    CalendarWeekRule.FirstDay, 
                    DayOfWeek.Sunday
                );

                return (diaDaSemana - diaSemanaInicioDoMes) + 1;
            }
            catch 
            {
                
            }
            return 0;
        }

        public static Periodo Periodo(this DateTime data) => new Periodo(data);

        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static int ConverteHorasParaSegundos(this int horas)
        {
            return horas * 60 * 60;
        }

        public static int ConverteHorasParaSegundos(this int? horas) => ConverteHorasParaSegundos(horas.GetValueOrDefault());
    }


}