namespace DamnLibrary.DamnLibrary.Utilities
{
    public static class EnumUtilities
    {
        /// <summary>
        /// Minimal value of enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Minimal value</returns>
        public static T Min<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Min();

        /// <summary>
        /// Maximal value of enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static T Max<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Max();
    }
}