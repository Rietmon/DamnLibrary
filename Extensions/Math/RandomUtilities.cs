#if UNITY_2020
using Random = UnityEngine.Random;

namespace Rietmon.Extensions
{
    public static class RandomUtilities
    {
        public static bool RandomBool => RandomByte % 2 == 0;
        public static byte RandomByte => (byte)Random.Range(byte.MinValue, byte.MaxValue);

        public static short RandomShort => (short)Random.Range(short.MinValue, short.MaxValue);

        public static int RandomInt => Random.Range(int.MinValue, int.MaxValue);

        public static uint RandomUInt => (uint)Random.Range(0, uint.MaxValue);

        public static long RandomLong
        {
            get
            {
                long b = RandomInt;
                b <<= 32;
                b |= (uint)RandomInt;
                return b;
            }
        }

        public static decimal RandomDecimal =>
            new decimal(RandomInt, RandomInt, RandomInt, RandomBool, (byte)Random.Range(0, 28));
    }
}
#endif