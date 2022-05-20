namespace DamnLibrary.Extensions
{
    public static class FloatExtensions
    {
        public static int RoundToIntByMath(this float value) => (int)(value + (value >= 0 ? 0.5f : -0.5f));
    }
}