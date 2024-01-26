using System;
using DamnLibrary.Utilities;

#pragma warning disable 660,661

namespace DamnLibrary.Managements.Signals
{
    internal readonly struct SignalSystemId
    {
        private readonly IntPtr methodPointer;

        private SignalSystemId(IntPtr methodPointer)
        {
            this.methodPointer = methodPointer;
        }
        
        public static implicit operator SignalSystemId(RuntimeMethodHandle handler) =>
            new(handler.Value);

        public static bool operator ==(SignalSystemId left, SignalSystemId right) =>
            left.methodPointer == right.methodPointer;

        public static bool operator !=(SignalSystemId left, SignalSystemId right) => !(left == right);
    }
}
