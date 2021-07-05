using System;
using System.Linq;

namespace Rietmon.Extensions
{
    public static class EnumUtils
    {
        public static T Min<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Min();
        }

        public static T Max<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Max();
        }
    }
}