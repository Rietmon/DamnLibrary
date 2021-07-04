using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static void AddIfNotContains<T>(this List<T> array, T element)
    {
        if (!array.Contains(element))
            array.Add(element);
    }
    
    public static void AddIfNotExists<T>(this List<T> array, T element, Predicate<T> existMethod)
    {
        if (!array.Exists(existMethod))
            array.Add(element);
    }
}
