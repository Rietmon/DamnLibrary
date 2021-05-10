using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct FloatRange : IRanged<float>
{
    public float MinimalValue
    {
        get => minimalValue;
        set => minimalValue = value;
    }

    public float MaximalValue
    {
        get => maximalValue;
        set => maximalValue = value;
    }

    public float RandomValue => Random.Range(MinimalValue, MaximalValue);

    [SerializeField] private float minimalValue;

    [SerializeField] private float maximalValue;

    public FloatRange(float min, float max)
    {
        minimalValue = min;
        maximalValue = max;
    }
}
