#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace DamnLibrary.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Return new color value with changed R
        /// </summary>
        public static Color R(this Color color, float r) => new(r, color.g, color.b, color.a);
        /// <summary>
        /// Return new color value with changed G
        /// </summary>
        public static Color G(this Color color, float g) => new(color.r, g, color.b, color.a);
        /// <summary>
        /// Return new color value with changed B
        /// </summary>
        public static Color B(this Color color, float b) => new(color.r, color.g, b, color.a);
        /// <summary>
        /// Return new color value with changed A
        /// </summary>
        public static Color A(this Color color, float a) => new(color.r, color.g, color.b, a);
    }
}
#endif