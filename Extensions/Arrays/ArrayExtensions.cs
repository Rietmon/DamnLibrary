using System;
using System.Collections.Generic;

namespace DamnLibrary.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Return greater array length and index of these arrays
        /// </summary>
        /// <param name="greaterCount">Out! Greater array length</param>
        /// <param name="greaterArrayIndex">Out! Greater array index</param>
        /// <param name="arrays">Arrays</param>
        public static void GetGreaterArray(out int greaterCount, out int greaterArrayIndex, params Array[] arrays)
        {
            greaterCount = 0;
            greaterArrayIndex = 0;
            for (var i = 0; i < arrays.Length; i++)
            {
                if (arrays[i].Length > greaterCount)
                {
                    greaterCount = arrays[i].Length;
                    greaterArrayIndex = i;
                }
            }
        }
        
        /// <summary>
        /// Try get value from array by index
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="index">Index to get</param>
        /// <param name="value">Out! Result value</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>True if we found, false if is not</returns>
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
        
        /// <summary>
        /// Array.IndexOf implementation
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="element">Element which could be found</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Index of element or -1 if not found</returns>
        public static int IndexOf<T>(this T[] array, T element) => Array.IndexOf(array, element);
        
        /// <summary>
        /// Array.FindIndex implementation
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="match">Predicate to compare</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Index of the element or -1 if not found</returns>
        public static int FindIndex<T>(this T[] array, Predicate<T> match) => Array.FindIndex(array, match);

        /// <summary>
        /// Indices of element in array
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="element">Element which could be found</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Indices of elements. Count can be 0 but not null</returns>
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

        /// <summary>
        /// Array.Copy implementation
        /// </summary>
        /// <param name="array">Array</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Copied array</returns>
        public static T[] Copy<T>(this T[] array)
        {
            var newArray = Array.Empty<T>();
            Array.Copy(array, newArray, array.Length);
            return newArray;
        }

        /// <summary>
        /// Array.Copy implementation with index from and index to
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="indexFrom">Start index</param>
        /// <param name="indexTo">End index</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Copied array</returns>
        public static T[] CopyFromTo<T>(this T[] array, int indexFrom, int indexTo)
        {
            if (indexFrom >= array.Length || indexTo >= array.Length)
                return null;
            
            var newArray = new T[indexTo - indexFrom + 1];
            Array.Copy(array, indexFrom, newArray, 0, indexTo - indexFrom + 1);
            return newArray;
        }

        /// <summary>
        /// Copy array without elements with indices
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="indices">Indices to exclude</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Copied array</returns>
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

        /// <summary>
        /// Copy array with elements with provided indices
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="indices">Indices to include</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Copied array</returns>
        public static T[] CopyWith<T>(this T[] array, params int[] indices)
        {
            var result = new T[indices.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = array[indices[i]];
            return result;
        }

        /// <summary>
        /// Get element from array by index or default value
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="index">Index of the element</param>
        /// <param name="defaultValue">Default value</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Element with provided index or provided defaultValue</returns>
        public static T GetObject<T>(this T[] array, int index, T defaultValue = default) =>
            array.Length <= index || index < 0 ? defaultValue : array[index];
        
        /// <summary>
        /// Get casted element from array by index or default value
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="index">Index of the element</param>
        /// <param name="defaultValue">Default value</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Element with provided index or provided defaultValue</returns>
        public static T GetObject<T>(this object[] array, int index, T defaultValue = default) =>
            array.Length <= index || index < 0 ? defaultValue : (T)array[index];

        /// <summary>
        /// Cast array to another type with provided cast function
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="castFunction">Cast function</param>
        /// <typeparam name="TOut">Out array type</typeparam>
        /// <typeparam name="TIn">In array type</typeparam>
        /// <returns>Casted array</returns>
        public static TOut[] FuncCast<TOut, TIn>(this TIn[] array, Func<TIn, TOut> castFunction)
        {
            var result = new TOut[array.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = castFunction.Invoke(array[i]);
            }

            return result;
        }

        /// <summary>
        /// Find element in array by condition or return default value
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="condition">Condition function</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Found element or default</returns>
        public static T FindOrDefault<T>(this T[] array, Func<T, bool> condition)
        {
            foreach (var element in array)
            {
                if (condition.Invoke(element))
                    return element;
            }

            return default;
        }

        /// <summary>
        /// Find element in array by condition or return provided value
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="condition">Condition function</param>
        /// <param name="or">Default return value</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Found element or provided value</returns>
        public static T FindOr<T>(this T[] array, Func<T, bool> condition, T or)
        {
            foreach (var element in array)
            {
                if (condition.Invoke(element))
                    return element;
            }

            return or;
        }

        /// <summary>
        /// Return central element of the array
        /// </summary>
        /// <param name="array">Array</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns></returns>
        public static T CentralOrDefault<T>(this T[] array)
        {
            if (array.Length == 0)
                return default;

            if (array.Length == 1)
                return array[0];

            var centralIndex = array.Length / 2f;

            return array[(int)centralIndex];
        }

        /// <summary>
        /// Array.Sort implementation
        /// </summary>
        /// <param name="array">Array</param>
        /// <typeparam name="T">Type of the array</typeparam>
        public static void Sort<T>(this T[] array) => Array.Sort(array);

        /// <summary>
        /// Check if array contains elements and not equals null
        /// </summary>
        /// <param name="array">Array</param>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>True if array is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this T[] array) => array == null || array.Length == 0;
    }
}
