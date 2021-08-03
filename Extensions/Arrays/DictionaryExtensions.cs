using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryExtensions
{
    public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other)
    {
        foreach (var otherPair in other)
        {
            dictionary.Add(otherPair.Key, otherPair.Value);
        }
    }

    public static Dictionary<TKey, TNewValue> SmartCast<TNewValue, TKey, TCurrentValue>(this Dictionary<TKey, TCurrentValue> dictionary,
        Func<TCurrentValue, TNewValue> castFunction)
    {
        var result = new Dictionary<TKey, TNewValue>();
        foreach (var pair in dictionary)
        {
            result.Add(pair.Key, castFunction.Invoke(pair.Value));
        }

        return result;
    }
}
