#if UNITY_2020
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class ColorExtensions
    {
        public static Color R(this Color color, float r) => new Color(r, color.g, color.b, color.a);
        public static Color G(this Color color, float g) => new Color(color.r, g, color.b, color.a);
        public static Color B(this Color color, float b) => new Color(color.r, color.g, b, color.a);
        public static Color A(this Color color, float a) => new Color(color.r, color.g, color.b, a);
    }
}
#endif