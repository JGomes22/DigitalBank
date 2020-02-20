using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DigitalBank.Infra.CrossCutting.Extensions
{
    public static class EnumExtension
    {
        public static string Description(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
                return attributes.First().Description;

            return value.ToString();
        }
    }
}
