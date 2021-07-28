using System;
using System.Reflection;
using Rietmon.Extensions;

namespace Rietmon.Management
{
    internal readonly struct SignalSystemId
    {
        private readonly IntPtr originalMethodPointer;

        private SignalSystemId(IntPtr originalMethodPointer)
        {
            this.originalMethodPointer = originalMethodPointer;
        }

        public static implicit operator SignalSystemId(IntPtr originalMethodPointer) =>
            new SignalSystemId(originalMethodPointer);
        
        public static implicit operator SignalSystemId(RuntimeMethodHandle handler) =>
            new SignalSystemId(handler.Value);

        public static bool operator ==(SignalSystemId left, SignalSystemId right)
        {
            var preCompare = CompareUtilities.PreCompare(left, right);
            if (preCompare != null)
                return preCompare.Value;

            return left.originalMethodPointer == right.originalMethodPointer;
        }

        public static bool operator !=(SignalSystemId left, SignalSystemId right)
        {
            return !(left == right);
        }
    }
}
