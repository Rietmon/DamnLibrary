using DamnLibrary.Extensions;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

namespace DamnLibrary.Other
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

#if UNITY_5_3_OR_NEWER 
        [SerializeField]
#endif
        private double minimalValue;

#if UNITY_5_3_OR_NEWER 
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