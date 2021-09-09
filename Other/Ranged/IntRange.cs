using System;
using Rietmon.Extensions;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

namespace Rietmon.Other
{
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

        public int RandomValue => RandomUtilities.Range(MinimalValue, MaximalValue);

#if UNITY_5_3_OR_NEWER 
        [SerializeField]
#endif
        private int minimalValue;

#if UNITY_5_3_OR_NEWER 
        [SerializeField]
#endif
        private int maximalValue;

        public IntRange(int min, int max)
        {
            minimalValue = min;
            maximalValue = max;
        }
    }
}