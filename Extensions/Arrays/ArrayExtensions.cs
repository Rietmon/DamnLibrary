using System;
using System.Collections.Generic;

namespace Rietmon.Extensions
{
    public static class ArrayExtensions
    {
        public static int IndexOf<T>(this T[] array, T element) => Array.IndexOf(array, element);

        public static int[] IndicesOf<T>(this T[] array, T element)
        {
            var result = new List<int>();
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(element))
                    result.Add(i);
            }

            return result.ToArray();
        }

        public static T[] Copy<T>(this T[] array)
        {
            var newArray = new T[0];
            Array.Copy(array, newArray, array.Length);
            return newArray;
        }

        public static T[] CopyFromTo<T>(this T[] array, int indexFrom, int indexTo)
        {
            var newArray = new T[indexTo - indexFrom + 1];
            Array.Copy(array, indexFrom, newArray, 0, indexTo - indexFrom + 1);
            return newArray;
        }

        public static T[] CopyWithout<T>(this T[] array, params int[] indices)
        {
            var tempList = new List<T>();
            tempList.AddRange(array);

            foreach (var index in indices)
                tempList.RemoveAt(index);

            return tempList.ToArray();
        }
    }
}
