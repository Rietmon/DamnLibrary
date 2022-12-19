#if UNITY_5_3_OR_NEWER 
using Object = UnityEngine.Object;
#endif

namespace DamnLibrary.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToStringContent(this IEnumerable<object> array)
        {
            var result = "";

            foreach (var obj in array)
                result += $"{obj}\n";

            return result;
        }

        public static T Random<T>(this IEnumerable<T> array)
        {
            var enumerable = array as T[] ?? array.ToArray();
            var randomIndex = RandomUtilities.Range(0, enumerable.Length);
            return enumerable.ElementAtOrDefault(randomIndex);
        }

        public static IEnumerable<TOut> SmartCast<TOut, TIn>(this IEnumerable<TIn> array, Func<TIn, TOut> castFunction) => 
            array.Select(castFunction.Invoke).ToList();
    }
}