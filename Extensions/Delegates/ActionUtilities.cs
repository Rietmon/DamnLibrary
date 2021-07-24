using System;

namespace Rietmon.Extensions
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