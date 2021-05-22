using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void AddIfNotContains<T>(this List<T> array, T element)
    {
        if (!array.Contains(element))
            array.Add(element);
    }
}
