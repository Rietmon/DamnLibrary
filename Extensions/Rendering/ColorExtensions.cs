using UnityEngine;

public static class ColorExtensions
{
    public static Color SetR(this Color color, float r) => new Color(r, color.g, color.b, color.a);
    public static Color SetG(this Color color, float g) => new Color(color.r, g, color.b, color.a);
    public static Color SetB(this Color color, float b) => new Color(color.r, color.g, b, color.a);
    public static Color SetA(this Color color, float a) => new Color(color.r, color.g, color.b, a);
}
