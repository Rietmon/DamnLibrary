namespace DamnLibrary.Types
{
    public interface IRanged<T>
    {
        public T MinimalValue { get; set; }

        public T MaximalValue { get; set; }
        
        public T this[int index] { get; }

        public T RandomValue { get; }
    }
}