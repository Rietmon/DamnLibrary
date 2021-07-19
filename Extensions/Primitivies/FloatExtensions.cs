namespace Rietmon.Extensions
{
    public static class FloatExtensions
    {
        public static int RoundToIntByMath(this float value) => (int)(value + (value >= 0 ? 0.5f : -0.5f));

        public static int ToMilliseconds(this float value) => (int)value * 1000;
    }
}