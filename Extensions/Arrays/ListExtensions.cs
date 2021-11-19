using System;
using System.Collections;
using System.Collections.Generic;

namespace Rietmon.Extensions
{
    public static class ListExtensions
    {
        public static void GetGreaterList(out int greaterCount, out int greaterArrayIndex, params IList[] arrays)
        {
            greaterCount = 0;
            greaterArrayIndex = 0;
            for (var i = 0; i < arrays.Length; i++)
            {
                if (arrays[i].Count > greaterCount)
                {
                    greaterCount = arrays[i].Count;
                    greaterArrayIndex = i;
                }
            }
        }
        
        public static void AddIfNotContains<T>(this List<T> array, T element)
        {
            if (!array.Contains(element))
                array.Add(element);
        }

        public static void AddIfNotExists<T>(this List<T> array, T element, Predicate<T> existMethod)
        {
            if (!array.Exists(existMethod))
                array.Add(element);
        }
    }
}