#if UNITY_2020
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static float GetSum(this Vector3 vector) => vector.x + vector.y + vector.z;
        
        public static float GetSum(this Vector3Int vector) => vector.x + vector.y + vector.z;

        public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0) => vector + new Vector3(x, y, z);
    }
}
#endif