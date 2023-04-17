using System;

namespace DamnLibrary.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// string.Format implementation
        /// </summary>
        public static string Format(this string str, params object[] values) => string.Format(str, values);

        /// <summary>
        /// string.IsNullOrEmpty implementation
        /// </summary>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static string Reverse(this string str)
        {
            var charArray = str.ToCharArray();
            Array.Reverse(charArray);
            var reversed = new string(charArray);
            return reversed;
        }
    }
}