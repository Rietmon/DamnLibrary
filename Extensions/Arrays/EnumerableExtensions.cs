using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_2020
using Object = UnityEngine.Object;
#endif

namespace Rietmon.Extensions
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