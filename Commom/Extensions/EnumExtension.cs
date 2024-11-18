using System;

namespace ArmsFW.Services.Extensions
{
    public static class EnumExtension
    {
        public static int Value(this Enum Item)
        {
            return Convert.ToInt32(Item);
        }

        public static TEnum GetValueByName<TEnum>(this Enum enumItem, string name) where TEnum:struct
        {
            var enumOutput = Enum.Parse<TEnum>(name);
            return enumOutput;
        }

        public static TEnum GetValueByName<TEnum>(string name) where TEnum : struct
        {
            var ret = Enum.TryParse<TEnum>(name, true, out TEnum enumOutput);

            if (ret)
            {
                return enumOutput;
            }
            else
            {
                return default(TEnum);
            }
        }

        public static string GetDescription(this Enum item)
        {
            return Enum.GetName(item.GetType(), item);
        }
    }
}