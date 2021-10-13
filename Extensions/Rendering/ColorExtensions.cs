#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Will return new color value with changed R
        /// </summary>
        public static Color R(this Color color, float r) => new Color(r, color.g, color.b, color.a);
        /// <summary>
        /// Will return new color value with changed G
        /// </summary>
        public static Color G(this Color color, float g) => new Color(color.r, g, color.b, color.a);
        /// <summary>
        /// Will return new color value with changed B
        /// </summary>
        public static Color B(this Color color, float b) => new Color(color.r, color.g, b, color.a);
        /// <summary>
        /// Will return new color value with changed A
        /// </summary>
        public static Color A(this Color color, float a) => new Color(color.r, color.g, color.b, a);
    }
}
#endif