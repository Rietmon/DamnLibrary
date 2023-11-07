#if UNITY_5_3_OR_NEWER
namespace DamnLibrary.Utilities.Extensions
{
    public static class Vector4Extensions
    {
        /// <summary>
        /// Return abs of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Abs of vector</returns>
        public static Vector4 Abs(this Vector4 vector) => new(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z), Mathf.Abs(vector.w));
        
        /// <summary>
        /// Return sum of vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Sum of vector</returns>
        public static float Sum(this Vector4 vector) => vector.x + vector.y + vector.z;

        /// <summary>
        /// Add x, y, z and w to vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <param name="w">W</param>
        /// <returns>Result vector</returns>
        public static Vector4 Add(this Vector4 vector, float x = 0, float y = 0, float z = 0, float w = 0) => vector + new Vector4(x, y, z, w);
    }
}
#endif