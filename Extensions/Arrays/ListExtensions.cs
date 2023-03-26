using System;
using System.Collections;
using System.Collections.Generic;

namespace DamnLibrary.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Return a greater list from the lists
        /// </summary>
        /// <param name="greaterCount">Greater list length</param>
        /// <param name="greaterArrayIndex">Greater list index</param>
        /// <param name="lists">Lists</param>
        public static void GetGreaterList(out int greaterCount, out int greaterArrayIndex, params IList[] lists)
        {
            greaterCount = 0;
            greaterArrayIndex = 0;
            for (var i = 0; i < lists.Length; i++)
            {
                if (lists[i].Count > greaterCount)
                {
                    greaterCount = lists[i].Count;
                    greaterArrayIndex = i;
                }
            }
        }
        
        /// <summary>
        /// Add an element to the list if it doesn't contains it
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="element">Element</param>
        /// <typeparam name="T">List type</typeparam>
        public static void AddIfNotContains<T>(this List<T> list, T element)
        {
            if (!list.Contains(element))
                list.Add(element);
        }

        /// <summary>
        /// Add an element to the list if it doesn't exists
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="element">Element</param>
        /// <param name="existMethod">Predicate</param>
        /// <typeparam name="T">List type</typeparam>
        public static void AddIfNotExists<T>(this List<T> list, T element, Predicate<T> existMethod)
        {
            if (!list.Exists(existMethod))
                list.Add(element);
        }
    }
}