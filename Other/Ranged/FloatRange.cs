using DamnLibrary.Extensions;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

namespace DamnLibrary.Other
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

        public float RandomValue => RandomUtilities.Range(MinimalValue, MaximalValue);

#if UNITY_5_3_OR_NEWER 
        [SerializeField]
#endif
        private float minimalValue;

#if UNITY_5_3_OR_NEWER 
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