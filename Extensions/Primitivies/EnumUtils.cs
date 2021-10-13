using System;
using System.Linq;

namespace Rietmon.Extensions
{
    public static class EnumUtils
    {
        /// <summary>
        /// Minimal value of enum
        /// </summary>
        public static T Min<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Min();

        /// <summary>
        /// Maximal value of enum
        /// </summary>
        public static T Max<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Max();
    }
}