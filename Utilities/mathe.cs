using Unity.Mathematics;

namespace DamnLibrary.Utilities
{
    // ReSharper disable InconsistentNaming
    public static class mathe
    {
        public static bool isangleebetween(float angle, float min, float max)
        {
            if (min < 0)
            {
                return angle >= min + 360 && angle <= 360 ||
                       angle >= 0 && angle <= max;
            }

            return angle >= min && angle <= max;
        }
        
        public static bool isangleless(float angle, float min, float max)
        {
            var center = math.abs(max - min + 180);
            if (center < min)
                return !isangleebetween(angle, center, min);
            return !isangleebetween(angle, min, center);
        }
        
        public static float normalizeangle(float angle)
        {
            if (angle > 360)
                return angle - 360;
            if (angle < 0)
                return angle + 360;
            return angle;
        }
        
        public static float clampangle(float angle, float min, float max)
        {
            angle = normalizeangle(angle);
            if (isangleebetween(angle, min, max))
                return angle;
            return isangleless(angle, min, max) ? min : max;
        }
        
        public static long movetowards(long current, long target, float maxDelta)
        {
            if (math.abs(target - current) <= maxDelta)
                return target;
            return (long)(current + (long)math.sign((double)target - current) * maxDelta);
        }
    }
}