using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public static class Extensions
    {
        public static string Pluralize(this string singularName)
        {
            if (string.IsNullOrEmpty(singularName))
            {
                return singularName;
            }

            string lowerName = singularName.ToLowerInvariant();

            if (lowerName.EndsWith("s") || lowerName.EndsWith("x") || lowerName.EndsWith("sh") || lowerName.EndsWith("ch"))
            {
                return singularName + "es";
            }

            if (lowerName.EndsWith("y") && singularName.Length > 1 &&
                !"aeiou".Contains(lowerName[lowerName.Length - 2].ToString()))
            {
                return singularName.Substring(0, singularName.Length - 1) + "ies";
            }

            return singularName + "s";
        }

        public static string Capitalize(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return char.ToUpper(str[0]).ToString();
            }

            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
