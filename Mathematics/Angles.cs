using System;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

namespace Rietmon.Mathematics
{
    public static class Angles
    {
        public static bool IsAngleBetween(float angle, float min, float max)
        {
            if (min < 0)
            {
                return angle >= min + 360 && angle <= 360 ||
                       angle >= 0 && angle <= max;
            }

            return angle >= min && angle <= max;
        }

        public static float Clamp(float angle, float min, float max)
        {
            angle = NormalizeAngle(angle);
        
            if (IsAngleBetween(angle, min, max))
                return angle;

            return IsAngleLess(angle, min, max) ? min : max;
        }

        private static bool IsAngleLess(float angle, float min, float max)
        {
#if UNITY_5_3_OR_NEWER 
            var center = Mathf.Abs(max - min + 180);
#else
            var center = Math.Abs(max - min + 180);
#endif
        
            if (center < min)
                return !IsAngleBetween(angle, center, min);

            return !IsAngleBetween(angle, min, center);
        }

        public static float NormalizeAngle(float angle)
        {
            if (angle > 360)
                return angle - 360;
            if (angle < 0)
                return angle + 360;

            return angle;
        }
    }
}
