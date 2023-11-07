using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_5_3_OR_NEWER 
using Object = UnityEngine.Object;
#endif

namespace DamnLibrary.Utilities.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Return a string with the content of the array
        /// </summary>
        /// <param name="array">Array</param>
        /// <returns>Content</returns>
        public static string ToStringContent<T>(this IEnumerable<T> array) =>
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
        /// Return a random elements from the array
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="count">Count of elements</param>
        /// <typeparam name="T">Array type</typeparam>
        /// <returns>Element</returns>
        public static T[] Random<T>(this IEnumerable<T> array, int count)
        {
            var result = new T[count];
            var enumerable = array as T[] ?? array.ToArray();
            var indicesToUse = new List<int>();
            for (var i = 0; i < enumerable.Length; i++)
                indicesToUse.Add(i);

            for (var i = 0; i < count; i++)
            {
                var randomIndex = RandomUtilities.Range(0, indicesToUse.Count);
                result[i] = enumerable[indicesToUse[randomIndex]];
                indicesToUse.Remove(indicesToUse[randomIndex]);
            }

            return result;
        }

        /// <summary>
        /// Cast an IEnumerable to another type
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="castFunction">Cast function</param>
        /// <typeparam name="TOut">Out type</typeparam>
        /// <typeparam name="TIn">In type</typeparam>
        /// <returns>Casted IEnumerable</returns>
        public static IEnumerable<TOut> FuncCast<TOut, TIn>(this IEnumerable<TIn> array, Func<TIn, TOut> castFunction) =>
            array.Select(castFunction.Invoke);

        /// <summary>
        /// Check if IEnumerable contains elements and not equals null
        /// </summary>
        /// <param name="array">Array</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>True if array is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> array) => array == null || !array.Any();
    }
}