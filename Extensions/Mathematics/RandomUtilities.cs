using System;
#if UNITY_5_3_OR_NEWER 
using Random = UnityEngine.Random;
#endif

namespace DamnLibrary.Extensions
{
    public static class RandomUtilities
    {
        public static bool RandomBool => RandomByte % 2 == 0;
        
        public static byte RandomByte => (byte)Range(byte.MinValue, byte.MaxValue);
        
        public static sbyte RandomSByte => (sbyte)Range(sbyte.MinValue, sbyte.MaxValue);

        public static short RandomShort => (short)Range(short.MinValue, short.MaxValue);
        
        public static ushort RandomUShort => (ushort)Range(ushort.MinValue, ushort.MaxValue);

        public static int RandomInt => Range(int.MinValue, int.MaxValue);

        public static uint RandomUInt => (uint)Range(0, uint.MaxValue);

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
#if UNITY_5_3_OR_NEWER 
            return Random.Range(min, max);
#else
            return new Random().Next(min, max);
#endif
        }
        
        public static double Range(double min, double max)
        {
#if UNITY_5_3_OR_NEWER 
            return Random.Range((float)min, (float)max);
#else
            return new Random().NextDouble() % max + min;
#endif
        }
        
        public static float Range(float min, float max)
        {
#if UNITY_5_3_OR_NEWER 
            return Random.Range(min, max);
#else
            return (float)(new Random().NextDouble() % max + min);
#endif
        }
    }
}