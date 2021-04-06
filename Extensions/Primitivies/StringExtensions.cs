using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
    public static string Format(this string str, params object[] values) => string.Format(str, values);
}
