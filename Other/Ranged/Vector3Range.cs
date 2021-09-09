#if UNITY_5_3_OR_NEWER 
using System;
using Rietmon.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rietmon.Other
{
    [Serializable]
    public struct Vector3Range : IRanged<Vector3>
    {
        public Vector3 MinimalValue
        {
            get => minimalValue;
            set => minimalValue = value;
        }

        public Vector3 MaximalValue
        {
            get => maximalValue;
            set => maximalValue = value;
        }

        public Vector3 RandomValue => new Vector3(RandomUtilities.Range(MinimalValue.x, MaximalValue.x),
            RandomUtilities.Range(MinimalValue.y, MaximalValue.y), RandomUtilities.Range(MinimalValue.z, MaximalValue.z));

        [SerializeField] private Vector3 minimalValue;

        [SerializeField] private Vector3 maximalValue;

        public Vector3Range(Vector3 min, Vector3 max)
        {
            minimalValue = min;
            maximalValue = max;
        }
    }
}
#endif