using System;
using System.Collections.Generic;

namespace Rietmon.Extensions
{
    public static class ArrayExtensions
    {
        public static bool TryGetValue<T>(this T[] array, int index, out T value)
        {
            if (index < array.Length && index >= 0)
            {
                value = array[index];
                return true;
            }

            value = default;
            return false;
        }
        
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
            var newArray = Array.Empty<T>();
            Array.Copy(array, newArray, array.Length);
            return newArray;
        }

        public static T[] CopyFromTo<T>(this T[] array, int indexFrom, int indexTo)
        {
            if (indexFrom >= array.Length || indexTo >= array.Length)
                return null;
            
            var newArray = new T[indexTo - indexFrom + 1];
            Array.Copy(array, indexFrom, newArray, 0, indexTo - indexFrom + 1);
            return newArray;
        }

        public static T[] CopyWithout<T>(this T[] array, params int[] indices)
        {
            var tempList = new List<T>(array);
            if (indices.Length > 0)
            {
                var objectsToRemove = new T[indices.Length];
                for (var i = 0; i < indices.Length; i++)
                    objectsToRemove[i] = array[indices[i]];
                
                foreach (var obj in objectsToRemove)
                    tempList.Remove(obj);
            }
            return tempList.ToArray();
        }

        public static T GetArgument<T>(this T[] array, int index, T defaultValue = default) =>
            array.Length <= index ? defaultValue : array[index];
        
        public static T GetArgument<T>(this object[] array, int index, T defaultValue = default) =>
            array.Length <= index ? defaultValue : (T)array[index];

        public static TOut[] SmartCast<TOut, TIn>(this TIn[] array, Func<TIn, TOut> castFunction)
        {
            var result = new TOut[array.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = castFunction.Invoke(array[i]);
            }

            return result;
        }

        public static T Find<T>(this T[] array, Func<T, bool> condition)
        {
            foreach (var element in array)
            {
                if (condition.Invoke(element))
                    return element;
            }

            return default;
        }
        
        public static T FindOr<T>(this T[] array, Func<T, bool> condition, T or)
        {
            foreach (var element in array)
            {
                if (condition.Invoke(element))
                    return element;
            }

            return or;
        }

        public static T CentralOrDefault<T>(this T[] array)
        {
            if (array.Length == 0)
                return default;

            if (array.Length == 1)
                return array[0];

            var centralIndex = array.Length / 2f;

            return array[(int)centralIndex];
        }
    }
}
