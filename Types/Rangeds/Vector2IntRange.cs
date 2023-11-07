#if UNITY_5_3_OR_NEWER
using System;
using DamnLibrary.Utilities;
using UnityEngine;
namespace DamnLibrary.Types.Rangeds
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

        public Vector2Int RandomValue => new(RandomUtilities.Range(MinimalValue.x, MaximalValue.x),
            RandomUtilities.Range(MinimalValue.y, MaximalValue.y));

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