using System;
#if UNITY_5_3_OR_NEWER 
using Random = UnityEngine.Random;
#endif

namespace DamnLibrary.Extensions
{
    public static class RandomUtilities
    {
	    /// <summary>
	    /// Random bool
	    /// </summary>
        public static bool RandomBool => RandomByte % 2 == 0;
        
	    /// <summary>
	    /// Random byte
	    /// </summary>
        public static byte RandomByte => (byte)Range(byte.MinValue, byte.MaxValue);
        
	    /// <summary>
	    /// Random sbyte
	    /// </summary>
        public static sbyte RandomSByte => (sbyte)Range(sbyte.MinValue, sbyte.MaxValue);

	    /// <summary>
	    /// Random short
	    /// </summary>
        public static short RandomShort => (short)Range(short.MinValue, short.MaxValue);
        
	    /// <summary>
	    /// Random ushort
	    /// </summary>
        public static ushort RandomUShort => (ushort)Range(ushort.MinValue, ushort.MaxValue);

	    /// <summary>
	    /// Random int
	    /// </summary>
        public static int RandomInt => Range(int.MinValue, int.MaxValue);

	    /// <summary>
	    /// Random uint
	    /// </summary>
        public static uint RandomUInt => (uint)Range(0, uint.MaxValue);

	    /// <summary>
	    /// Random long
	    /// </summary>
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

	    /// <summary>
	    /// Random decimal
	    /// </summary>
        public static decimal RandomDecimal => new(RandomInt, RandomInt, RandomInt, RandomBool, (byte)Range(0, 28));

	    /// <summary>
	    /// Random int from min to max
	    /// </summary>
	    /// <param name="min">Min value. Included</param>
	    /// <param name="max">Max value. Excluded</param>
	    /// <returns>Random int</returns>
        public static int Range(int min, int max)
        {
#if UNITY_5_3_OR_NEWER 
            return Random.Range(min, max);
#else
            return new Random().Next(min, max);
#endif
        }
        
	    /// <summary>
	    /// Random double from min to max
	    /// </summary>
	    /// <param name="min">Min value. Included</param>
	    /// <param name="max">Max value. Excluded</param>
	    /// <returns>Random double</returns>
        public static double Range(double min, double max)
        {
#if UNITY_5_3_OR_NEWER 
            return Random.Range((float)min, (float)max);
#else
            return new Random().NextDouble() % max + min;
#endif
        }
        
	    /// <summary>
	    /// Random float from min to max
	    /// </summary>
	    /// <param name="min">Min value. Included</param>
	    /// <param name="max">Max value. Excluded</param>
	    /// <returns>Random float</returns>
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