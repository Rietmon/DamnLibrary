namespace Unity.Mathematics
{
    // ReSharper disable InconsistentNaming
    public static class mathe
    {
        public static long movetowards(long current, long target, float maxDelta)
        {
            if (math.abs(target - current) <= maxDelta)
                return target;
            return (long)(current + (long)math.sign((double)target - current) * maxDelta);
        }
    }
}