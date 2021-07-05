using System;
using Rietmon.Extensions;
#if UNITY_2020
using UnityEngine;
#endif

namespace Rietmon.Other
{
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

        public float RandomValue => (float)RandomUtilities.Range(MinimalValue, MaximalValue);

#if UNITY_2020
        [SerializeField]
#endif
        private float minimalValue;

#if UNITY_2020
        [SerializeField]
#endif
        private float maximalValue;

        public FloatRange(float min, float max)
        {
            minimalValue = min;
            maximalValue = max;
        }
    }
}