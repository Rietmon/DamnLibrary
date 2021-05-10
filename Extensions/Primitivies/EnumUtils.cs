using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumUtils
{
    public static T Max<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>().Max();
    }
}
