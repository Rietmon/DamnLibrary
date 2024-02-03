using System;
using DamnLibrary.Utilities;
using UnityEngine;

namespace DamnLibrary.Types
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
        
        public float this[int index] => index == 0 ? minimalValue : maximalValue;

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