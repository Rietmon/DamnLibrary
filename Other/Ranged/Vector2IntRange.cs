#if UNITY_2020
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rietmon.Other
{
    [Serializable]
    public struct Vector2IntRange : IRanged<Vector2Int>
    {
        public Vector2Int MinimalValue
        {
            get => minimalValue;
            set => minimalValue = value;
        }

        public Vector2Int MaximalValue
        {
            get => maximalValue;
            set => maximalValue = value;
        }

        public Vector2Int RandomValue => new Vector2Int(Random.Range(MinimalValue.x, MaximalValue.x),
            Random.Range(MinimalValue.y, MaximalValue.y));

        [SerializeField] private Vector2Int minimalValue;

        [SerializeField] private Vector2Int maximalValue;

        public Vector2IntRange(Vector2Int min, Vector2Int max)
        {
            minimalValue = min;
            maximalValue = max;
        }
    }
}
#endif