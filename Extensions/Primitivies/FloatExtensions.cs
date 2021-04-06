using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static int RoundToIntByMath(this float value) => (int)(value + 0.5f);
}
