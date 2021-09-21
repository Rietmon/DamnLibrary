using Rietmon.Extensions;
#pragma warning disable 660,661

namespace Rietmon.Other
{
    internal abstract class Identification
    {
        public abstract byte Size { get; }
        
        public abstract object Id { get; }
        
        protected abstract bool Compare(Identification other);

        public static bool operator ==(Identification left, Identification right)
        {
            var preCompare = CompareUtilities.PreCompare(left, right);
            if (preCompare != null)
                return preCompare.Value;
            
            var bigger = left.Size > right.Size ? left : right;
            var smaller = left.Size > right.Size ? right : left;

            return bigger.Compare(smaller);
        }

        public static bool operator !=(Identification left, Identification right) => !(left == right);
    }
}