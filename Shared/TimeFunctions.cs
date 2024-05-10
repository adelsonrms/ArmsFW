using System;
using System.Collections.Generic;
using System.Text;

namespace ArmsFW.Services.Extensions
{
    public static class TimeFunctions
    {
        public static string DecimalToDateTime(decimal value)
        {
            return DateTime.FromOADate(Convert.ToDouble(value)).ToString("HH:mm");
        }
        public static string SecondtsToTime(object value)
        {
            TimeSpan ts = TimeSpan.FromSeconds((long)value);

            return string.Format("{0}:{1}", ts.ToString("hh"), ts.ToString("mm"));
        }
        public static string DecimalToDateTime24h(decimal value, bool arred = false)
        {
            decimal hour = Math.Floor(value), minute = (value - hour);

            decimal hr = (hour * 24) + Math.Floor(minute * 24);
            decimal min = Math.Round((((minute * 24) - Math.Floor(minute * 24))) * 60);

            if (min>=60)
            {
                hr += 1;min = 0;
            }

            string h = $"{hr}";
            string m = $"{min.ToString("00")}";

            if (h.Length<2)
            {
                h = Convert.ToInt32(h).ToString("00");
            }

            if (arred)
            {
                return $"{h}";
            }
            else
            {
                return $"{h}:{m}";
            }
        }
    }


}
