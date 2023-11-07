#if UNITY_5_3_OR_NEWER
namespace DamnLibrary.Types.Rangeds
{
    [Serializable]
    public struct Vector3IntRange : IRanged<Vector3Int>
    {
        public Vector3Int MinimalValue
        {
            get => minimalValue;
            set => minimalValue = value;
        }

        public Vector3Int MaximalValue
        {
            get => maximalValue;
            set => maximalValue = value;
        }

        public Vector3Int RandomValue => new(RandomUtilities.Range(MinimalValue.x, MaximalValue.x),
            RandomUtilities.Range(MinimalValue.y, MaximalValue.y), RandomUtilities.Range(MinimalValue.z, MaximalValue.z));

        [SerializeField] private Vector3Int minimalValue;

        [SerializeField] private Vector3Int maximalValue;

        public Vector3IntRange(Vector3Int min, Vector3Int max)
        {
            minimalValue = min;
            maximalValue = max;
        }
    }
}
#endif