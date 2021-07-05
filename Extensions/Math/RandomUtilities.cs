using System;
#if UNITY_2020
using Random = UnityEngine.Random;
#endif

namespace Rietmon.Extensions
{
    public static class RandomUtilities
    {
        public static bool RandomBool => RandomByte % 2 == 0;
        
#if UNITY_2020
        public static byte RandomByte => (byte)Random.Range(byte.MinValue, byte.MaxValue);
#else
        public static byte RandomByte => (byte)Range(byte.MinValue, byte.MaxValue);
#endif

#if UNITY_2020
        public static short RandomShort => (short)Random.Range(short.MinValue, short.MaxValue);
#else
        public static short RandomShort => (short)Range(short.MinValue, short.MaxValue);
#endif

#if UNITY_2020
        public static int RandomInt => Random.Range(int.MinValue, int.MaxValue);
#else
        public static int RandomInt => Range(int.MinValue, int.MaxValue);
#endif

#if UNITY_2020
        public static uint RandomUInt => (uint)Random.Range(0, uint.MaxValue);
#else
        public static uint RandomUInt => (uint)Range(0, uint.MaxValue);
#endif

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
            new decimal(RandomInt, RandomInt, RandomInt, RandomBool, (byte)Range(0, 28));

        public static int Range(int min, int max)
        {
#if UNITY_2020
            return Random.Range(0, 28);
#else
            return new Random().Next(min, max);
#endif
        }
        
        public static double Range(double min, double max)
        {
#if UNITY_2020
            return Random.Range(0, 28);
#else
            return new Random().NextDouble() % max + min;
#endif
        }
    }
}