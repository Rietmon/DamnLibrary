#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace DamnLibrary.Extensions
{
    public static class Vector4Extensions
    {
        public static Vector4 Abs(this Vector4 vector) => new Vector4(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z), Mathf.Abs(vector.w));
        public static float Sum(this Vector4 vector) => vector.x + vector.y + vector.z;
        public static Vector4 Add(this Vector4 vector, float x = 0, float y = 0, float z = 0, float w = 0) => vector + new Vector4(x, y, z, w);
    }
}
#endif