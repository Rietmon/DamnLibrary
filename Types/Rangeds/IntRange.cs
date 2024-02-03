using System;
using DamnLibrary.Utilities;
using UnityEngine;

namespace DamnLibrary.Types
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
        
        public int this[int index] => index == 0 ? minimalValue : maximalValue;

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