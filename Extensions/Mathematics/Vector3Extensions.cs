#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 Abs(this Vector3 vector) => new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        public static float Sum(this Vector3 vector) => vector.x + vector.y + vector.z;
        public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0) => vector + new Vector3(x, y, z);
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

        public static Vector3Int Abs(this Vector3Int vector) => new Vector3Int(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        public static int Sum(this Vector3Int vector) => vector.x + vector.y + vector.z;
        public static Vector3Int Add(this Vector3Int vector, int x = 0, int y = 0, int z = 0) => vector + new Vector3Int(x, y, z);
    }
}
#endif