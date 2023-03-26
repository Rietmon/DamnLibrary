namespace DamnLibrary.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Round float to int by math principles
        /// </summary>
        /// <param name="value">Float</param>
        /// <returns>Rounded int</returns>
        public static int RoundToIntByMath(this float value) => (int)(value + (value >= 0 ? 0.5f : -0.5f));
    }
}