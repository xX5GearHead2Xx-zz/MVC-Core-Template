using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Text;

namespace Ecommerce
{
    public static class ExtensionMethods
    {
        public static string SanitizeInput(this string value)
        {
            return value.Replace("'", "''");
        }

        public static string Encrypt(this string value)
        {
            return Security.Encrypt(value);
        }

        public static string Decrypt(this string value)
        {
            return Security.Decrypt(value);
        }

        public static string ToDBDate(this DateTime Date)
        {
            //All dates are saved as UTC, this will be converted to local time when displaying
            return Date.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToDBDate(this DateTime? Date)
        {
            if (Date.HasValue)
            {
                //All dates are saved as UTC, this will be converted to local time when displaying
                return Convert.ToDateTime(Date).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return "";
            }
        }

        public static StringBuilder RemoveLastComma(this StringBuilder String)
        {
            int CharPosition = String.ToString().LastIndexOf(',');
            return String.Remove(CharPosition, 1);
        }
    }
}
