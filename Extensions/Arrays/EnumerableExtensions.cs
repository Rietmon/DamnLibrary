using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_5_3_OR_NEWER 
using Object = UnityEngine.Object;
#endif

namespace DamnLibrary.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Return a string with the content of the array
        /// </summary>
        /// <param name="array">Array</param>
        /// <returns>Content</returns>
        public static string ToStringContent(this IEnumerable<object> array) =>
            array.Aggregate("", (current, obj) => current + $"{obj}\n");

        /// <summary>
        /// Return a random element from the array
        /// </summary>
        /// <param name="array">Array</param>
        /// <typeparam name="T">Array type</typeparam>
        /// <returns>Element</returns>
        public static T Random<T>(this IEnumerable<T> array)
        {
            var enumerable = array as T[] ?? array.ToArray();
            var randomIndex = RandomUtilities.Range(0, enumerable.Length);
            return enumerable.ElementAtOrDefault(randomIndex);
        }

        /// <summary>
        /// Cast an array to another type
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="castFunction">Cast function</param>
        /// <typeparam name="TOut">Out type</typeparam>
        /// <typeparam name="TIn">In type</typeparam>
        /// <returns></returns>
        public static IEnumerable<TOut> FuncCast<TOut, TIn>(this IEnumerable<TIn> array, Func<TIn, TOut> castFunction) => 
            array.Select(castFunction.Invoke).ToList();
    }
}