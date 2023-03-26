#if ENABLE_MEMORY_UTILITIES
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DamnLibrary.Extensions
{
    public unsafe struct SharedArray<T> : IUnsafeArray<T, T> where T : unmanaged
    {
        public int Length
        {
            get => length;
            set
            {
                ((IUnsafeArray<T, T>)this).Resize(value, length);
                length = value;
            }
        }

        ushort IUnsafeArray<T, T>.StructSize => structSize;

        T* IUnsafeArray<T, T>.ArrayPointer
        {
            get => arrayPointer;
            set => arrayPointer = value;
        }

        private readonly ushort structSize;
        
        private int length;
        private T* arrayPointer;

        public SharedArray(int length)
        {
            structSize = (ushort)MemoryUtilities.SizeOf<T>();
            this.length = length;
            arrayPointer = (T*)MemoryUtilities.Allocate(structSize * length);
        }

        public SharedArray(T[] array)
        {
            structSize = (ushort)MemoryUtilities.SizeOf<T>(); 
            length = array.Length;
            arrayPointer = (T*)Marshal.UnsafeAddrOfPinnedArrayElement(array, 0).ToPointer();
        }

        public T this[int index]
        {
            get => arrayPointer[index];
            set => arrayPointer[index] = value;
        }

        /// <summary>
        /// Returns array os T[] from pointer
        /// </summary>
        /// <returns>Array</returns>
        public T[] ToArray() => MemoryUtilities.FromPointerToArray<T>(arrayPointer, Length);

        public static implicit operator SharedArray<T>(T[] array) => new(array);
    }
}
#endif