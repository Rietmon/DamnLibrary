using System;

namespace DamnLibrary.Extensions
{
    public unsafe interface IUnsafeArray<out TArrayType, TSaveType> : IDisposable 
        where TArrayType : struct where TSaveType : unmanaged
    {
        int Length { get; set; }
        
        internal ushort StructSize { get; }

        internal TSaveType* ArrayPointer { get; set; }
        
        TArrayType[] ToArray();

        public void Resize(int newLength, int oldLength)
        {
            var oldPointer = ArrayPointer;
            ArrayPointer = (TSaveType*)MemoryUtilities.Allocate(StructSize * newLength);
            MemoryUtilities.CopyMemory(ArrayPointer, oldPointer, newLength > oldLength ? oldLength : newLength);
            MemoryUtilities.Free(oldPointer);
        }
        
        void IDisposable.Dispose()
        {
            MemoryUtilities.Free(ArrayPointer);
        }
    }
}