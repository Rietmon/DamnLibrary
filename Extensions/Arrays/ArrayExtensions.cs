using System;

namespace Rietmon.Extensions
{
    public static class ArrayExtensions
    {
        public static int IndexOf<T>(this Array array, T element) => Array.IndexOf(array, element);
    }
}
