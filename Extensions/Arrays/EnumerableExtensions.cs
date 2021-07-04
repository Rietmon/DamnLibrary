using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Rietmon.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToStringContent(this IEnumerable<object> array)
        {
            var result = "";

            foreach (var obj in array)
                result += obj.ToString();

            return result;
        }

        public static T Random<T>(this IEnumerable<T> array)
        {
            var enumerable = array as T[] ?? array.ToArray();
            var randomIndex = UnityEngine.Random.Range(0, enumerable.Length);
            return enumerable.ElementAtOrDefault(randomIndex);
        }

        public static T GetObjectByName<T>(this IEnumerable<T> array, string name) where T : Object
        {
            return array.FirstOrDefault(obj => obj.name == name);
        }

        public static IEnumerable<TOut> SmartCast<TOut, TIn>(this IEnumerable<TIn> array, Func<TIn, TOut> castFunction) => 
            array.Select(castFunction.Invoke).ToList();
    }
}