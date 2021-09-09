#if UNITY_5_3_OR_NEWER 
using System;
using Rietmon.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rietmon.Other
{
    [Serializable]
    public struct Vector2Range : IRanged<Vector2>
    {
        public Vector2 MinimalValue
        {
            get => minimalValue;
            set => minimalValue = value;
        }

        public Vector2 MaximalValue
        {
            get => maximalValue;
            set => maximalValue = value;
        }

        public Vector2 RandomValue => new Vector2(RandomUtilities.Range(MinimalValue.x, MaximalValue.x),
            RandomUtilities.Range(MinimalValue.y, MaximalValue.y));

        [SerializeField] private Vector2 minimalValue;

        [SerializeField] private Vector2 maximalValue;

        public Vector2Range(Vector2 min, Vector2 max)
        {
            minimalValue = min;
            maximalValue = max;
        }
    }
}
#endif