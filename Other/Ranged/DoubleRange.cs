using System;
using Rietmon.Extensions;
#if UNITY_2020
using UnityEngine;
#endif

namespace Rietmon.Other
{
    [Serializable]
    public struct DoubleRange : IRanged<double>
    {
        public double MinimalValue
        {
            get => minimalValue;
            set => minimalValue = value;
        }

        public double MaximalValue
        {
            get => maximalValue;
            set => maximalValue = value;
        }

        public double RandomValue => RandomUtilities.Range(MinimalValue, MaximalValue);

#if UNITY_2020
        [SerializeField]
#endif
        private double minimalValue;

#if UNITY_2020
        [SerializeField]
#endif
        private double maximalValue;

        public DoubleRange(double min, double max)
        {
            minimalValue = min;
            maximalValue = max;
        }
    }
}