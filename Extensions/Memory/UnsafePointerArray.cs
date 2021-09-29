using System;

namespace Rietmon.Extensions
{
    public unsafe class UnsafeArray<T> : IDisposable where T : struct
    {
        public int Length
        {
            get => length;
            set
            {
                Resize(value, length);
                length = value;
            }
        }

        private readonly short structSize;
        
        private int length;

        private byte* arrayPointer;

        public UnsafeArray(int length)
        {
            structSize = (short)MemoryUtilities.SizeOf<T>();
            this.length = length;
            arrayPointer = (byte*)MemoryUtilities.Allocate(structSize * length);
        }

        public T this[int index]
        {
            get => GetValue(index);
            set => SetValue(index, value);
        }

        public T GetValue(int index)
        {
            if (index >= length)
                return default;
            
            return MemoryUtilities.FromPointer<T>(arrayPointer + index * structSize);
        }

        public void SetValue(int index, T value)
        {
            if (index >= length)
                return;
            
            MemoryUtilities.CopyMemory(arrayPointer + index * structSize, value.ToPointer(), structSize);
        }

        private void Resize(int newLength, int oldLength)
        {
            var oldPointer = arrayPointer;
            arrayPointer = (byte*)MemoryUtilities.Allocate(structSize * newLength);
            MemoryUtilities.CopyMemory(arrayPointer, oldPointer, oldLength * structSize);
            MemoryUtilities.Free(oldPointer);
        }

        void IDisposable.Dispose()
        {
            MemoryUtilities.Free(arrayPointer);
        }
    }
    
    public unsafe class UnsafePointerArray<T> where T : unmanaged
    {
        public int Length
        {
            get => length;
            set
            {
                Resize(value, length);
                length = value;
            }
        }

        private readonly short structSize;

        private T* arrayPointer;
        
        private int length;

        public UnsafePointerArray(int length)
        {
            structSize = (short)MemoryUtilities.SizeOf<T>();
            this.length = length;
            arrayPointer = (T*)MemoryUtilities.Allocate(structSize * length);
        }

        public T* this[int index]
        {
            get => &arrayPointer[index];
            set => arrayPointer[index] = value[0];
        }

        private void Resize(int newLength, int oldLength)
        {
            var oldPointer = arrayPointer;
            arrayPointer = (T*)MemoryUtilities.Allocate(structSize * newLength);
            MemoryUtilities.CopyMemory(arrayPointer, oldPointer, newLength > oldLength ? oldLength : newLength);
            MemoryUtilities.Free(oldPointer);
        }
    }
}