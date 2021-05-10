using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct IntRange : IRanged<int>
{
    public int MinimalValue
    {
        get => minimalValue;
        set => minimalValue = value;
    }

    public int MaximalValue
    {
        get => maximalValue;
        set => maximalValue = value;
    }

    public int RandomValue => Random.Range(MinimalValue, MaximalValue);

    [SerializeField] private int minimalValue;

    [SerializeField] private int maximalValue;

    public IntRange(int min, int max)
    {
        minimalValue = min;
        maximalValue = max;
    }
}