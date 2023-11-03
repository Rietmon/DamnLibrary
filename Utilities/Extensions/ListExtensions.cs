using System.Collections;

namespace DamnLibrary.DamnLibrary.Utilities.Extensions
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

        /// <summary>
        /// Remove elements from the list if they exists
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="elements">Elements to remove</param>
        /// <typeparam name="T">List type</typeparam>
        public static void RemoveRange<T>(this List<T> list, IEnumerable<T> elements)
        {
            foreach (var element in elements)
                list.Remove(element);
        }
        
        /// <summary>
        /// Cast an array to another type
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="castFunction">Cast function</param>
        /// <typeparam name="TOut">Out type</typeparam>
        /// <typeparam name="TIn">In type</typeparam>
        /// <returns>Casted list</returns>
        public static List<TOut> FuncCast<TOut, TIn>(this List<TIn> list, Func<TIn, TOut> castFunction) => 
            list.Select(castFunction.Invoke).ToList();
    }
}