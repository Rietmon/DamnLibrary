#if ENABLE_MEMORY_UTILITIES
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Rietmon.Extensions
{
    public unsafe class ManagedBytesArray<T> : IUnsafeArray<T, byte> where T : struct
    {
        public int Length
        {
            get => length;
            set
            {
                ((IUnsafeArray<T, byte>)this).Resize(value, length);
                length = value;
            }
        }

        ushort IUnsafeArray<T, byte>.StructSize => structSize;

        byte* IUnsafeArray<T, byte>.ArrayPointer
        {
            get => arrayPointer;
            set => arrayPointer = value;
        }

        private readonly ushort structSize;
        
        private int length;
        private byte* arrayPointer;

        public ManagedBytesArray(int length)
        {
            structSize = (ushort)MemoryUtilities.SizeOf<T>();
            this.length = length;
            arrayPointer = (byte*)MemoryUtilities.Allocate(structSize * length);
        }
        
        public ManagedBytesArray(T[] array)
        {
            structSize = (ushort)MemoryUtilities.SizeOf<T>(); 
            length = array.Length;
            arrayPointer = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(array, 0).ToPointer();
        }

        public T this[int index]
        {
            get
            {
                if (index >= length)
                    return default;
            
                return MemoryUtilities.FromPointer<T>(arrayPointer + index * structSize);
            }
            set
            {
                if (index >= length)
                    return;
            
                MemoryUtilities.CopyMemory(arrayPointer + index * structSize, value.ToPointer(), structSize);
            }
        }

        public T[] ToArray() => MemoryUtilities.FromPointerToStructureArray<T>(arrayPointer, Length);

        public static implicit operator ManagedBytesArray<T>(T[] array) => new(array);
    }
    
    public unsafe class ManagedArray<T> : IUnsafeArray<T, T> where T : unmanaged
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

        private T* arrayPointer;
        private int length;

        public ManagedArray(int length)
        {
            structSize = (ushort)MemoryUtilities.SizeOf<T>();
            this.length = length;
            arrayPointer = (T*)MemoryUtilities.Allocate(structSize * length);
        }

        public ManagedArray(T[] array)
        {
            structSize = (ushort)MemoryUtilities.SizeOf<T>(); 
            length = array.Length;
            arrayPointer = (T*)Marshal.UnsafeAddrOfPinnedArrayElement(array, 0).ToPointer();
        }

        public T* this[int index]
        {
            get => &arrayPointer[index];
            set => arrayPointer[index] = value[0];
        }

        public T[] ToArray() => MemoryUtilities.FromPointerToArray<T>(arrayPointer, Length);

        public static implicit operator ManagedArray<T>(T[] array) => new(array);
    }
}
#endif