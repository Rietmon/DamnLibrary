#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace DamnLibrary.Utilities.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Return abs of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Abs of vector</returns>
        public static Vector3 Abs(this Vector3 vector) => new(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        
        /// <summary>
        /// Return sum of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Sum of vector</returns>
        public static float Sum(this Vector3 vector) => vector.x + vector.y + vector.z;

        /// <summary>
        /// Add x, y and z to vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <returns>Result vector</returns>
        public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0) => vector + new Vector3(x, y, z);
        
        /// <summary>
        /// Check if vector has invalid float values
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>True if vector is invalid</returns>
        public static bool IsInvalid(this Vector3 vector)
        {
            for (var i = 0; i < 3; i++)
            {
                var vectorValue = vector[i];
                if (float.IsNaN(vectorValue) || float.IsInfinity(vectorValue) ||
                    float.IsNegativeInfinity(vectorValue) || float.IsPositiveInfinity(vectorValue))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Return abs of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Abs of vector</returns>
        public static Vector3Int Abs(this Vector3Int vector) => new(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        
        /// <summary>
        /// Return sum of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Sum of vector</returns>
        public static int Sum(this Vector3Int vector) => vector.x + vector.y + vector.z;
        
        /// <summary>
        /// Add x, y and z to vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <returns>Result vector</returns>
        public static Vector3Int Add(this Vector3Int vector, int x = 0, int y = 0, int z = 0) => vector + new Vector3Int(x, y, z);
    }
}
#endif