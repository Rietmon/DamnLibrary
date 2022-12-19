using System;

namespace DamnLibrary.Extensions
{
    public static class ActionUtilities
    {
        public static Action GetDummy()
        {
            static void Dummy() { }
            return Dummy;
        }
    }
}