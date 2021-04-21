namespace Rietmon.Extensions
{
    public static class StringExtensions
    {
        public static string Format(this string str, params object[] values) => string.Format(str, values);

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
    }
}