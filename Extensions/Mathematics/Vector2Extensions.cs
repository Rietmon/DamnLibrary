#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace DamnLibrary.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Return abs of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Abs of vector</returns>
        public static Vector2 Abs(this Vector2 vector) => new(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        
        /// <summary>
        /// Return sum of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Sum of vector</returns>
        public static float Sum(this Vector2 vector) => vector.x + vector.y;
        
        /// <summary>
        /// Add x and y to vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Result vector</returns>
        public static Vector2 Add(this Vector2 vector, float x = 0, float y = 0) => vector + new Vector2(x, y);
        
        /// <summary>
        /// Return abs of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Abs of vector</returns>
        public static Vector2Int Abs(this Vector2Int vector) => new(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        
        /// <summary>
        /// Return sum of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Sum of vector</returns>
        public static int Sum(this Vector2Int vector) => vector.x + vector.y;
        
        /// <summary>
        /// Return abs of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Abs vector</returns>
        public static Vector2Int Add(this Vector2Int vector, int x = 0, int y = 0) => vector + new Vector2Int(x, y);
    }
}
#endif