#if UNITY_2020
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static float GetSum(this Vector2 vector) => vector.x + vector.y;

        public static Vector2 Add(this Vector2 vector, float x, float y) => vector + new Vector2(x, y);

        public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.x, vector.y);
        
        public static Vector3 ToVector3(this Vector2Int vector) => new Vector3(vector.x, vector.y);
    }
}
#endif