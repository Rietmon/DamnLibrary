#if ENABLE_MEMORY_UTILITIES
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DamnLibrary.Extensions
{
    public static unsafe class MemoryUtilities
    {
#if DEBUG_ENABLE_WATCHING_MEMORY_ALLOCATING
        /// <summary>
        /// Contains all allocated pointers data
        /// </summary>
        public static List<AllocatedPointerDebugInfo> AllocatedPointers { get; } = new();
#endif
        
#if ENABLE_WINDOWS_UTILITIES
        /// <summary>
        /// Copy memory from source to destination
        /// </summary>
        /// <param name="destination">Destination</param>
        /// <param name="source">Source</param>
        /// <param name="size">Size</param>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "CopyMemory")]
        public static extern void CopyMemory(void* destination, void* source, int size);

        /// <summary>
        /// Set each byte of memory to a specified value
        /// </summary>
        /// <param name="destination">Destination</param>
        /// <param name="value">Value for cell</param>
        /// <param name="size">Size</param>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "memset")]
        public static extern void MemorySet(void* destination, int value, int size);
        
        /// <summary>
        /// Is pointer bad for reading
        /// </summary>
        /// <param name="pointer">Pointer</param>
        /// <param name="size">Size</param>
        /// <returns>True if pointer is bad for reading</returns>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "IsBadReadPtr")]
        public static extern bool IsBadReadPointer(void* pointer, int size);
#endif

        /// <summary>
        /// Allocate memory
        /// </summary>
        /// <param name="size">Size</param>
        /// <returns>Pointer to allocated memory</returns>
        public static void* Allocate(int size) => InternalAllocate(size);

        private static void* InternalAllocate(int count)
        {
#if !DEBUG_ENABLE_WATCHING_MEMORY_ALLOCATING
            return Marshal.AllocHGlobal(count).ToPointer();
#else
            var intPointer = Marshal.AllocHGlobal(count);
            AllocatedPointers.Add(intPointer);
            return intPointer.ToPointer();
#endif
        }

        /// <summary>
        /// Free memory
        /// </summary>
        /// <param name="pointer">Pointer to allocated memory</param>
        public static void Free(void* pointer) => InternalFree(pointer);

        private static void InternalFree(void* pointer)
        {
#if !DEBUG_ENABLE_WATCHING_MEMORY_ALLOCATING
            Marshal.FreeHGlobal(new IntPtr(pointer));
#else
            var intPointer = new IntPtr(pointer);
            AllocatedPointers.Remove(intPointer);
            Marshal.FreeHGlobal(new IntPtr(pointer));
#endif
        }

#if !ENABLE_WINDOWS_UTILITIES
        /// <summary>
        /// Copy memory from source to destination
        /// </summary>
        /// <param name="destination">Destination</param>
        /// <param name="source">Source</param>
        /// <param name="size">Size</param>
        public static void CopyMemory(void* destination, void* source, int size)
        {
            var destinationBytesPointer = (byte*)destination;
            var sourceBytesPointer = (byte*)source;

            for (var i = 0; i < size; i++)
                destinationBytesPointer[i] = sourceBytesPointer[i];
        }
#endif
        
        /// <summary>
        /// Get pointer to value
        /// </summary>
        /// <param name="value">Value</param>
        /// <typeparam name="T">Value type</typeparam>
        /// <returns>Pointer to value</returns>
        public static void* ToPointer<T>(this T value)
        {
            var type = value.GetType();
            if (value is string str)
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                fixed (void* pointer = bytes)
                    return pointer;
            }

            if (type.IsValueType)
            {
                var pointer = Allocate(Marshal.SizeOf<T>());
                Marshal.StructureToPtr(value, new IntPtr(pointer), true);
                return pointer;
            }

            var handler = GCHandle.Alloc(value);
            return GCHandle.ToIntPtr(handler).ToPointer();
        }

        /// <summary>
        /// Cast pointer to value
        /// </summary>
        /// <param name="pointer">Pointer</param>
        /// <param name="freeAfterConverting">If true pointer will be free after converting</param>
        /// <param name="size">Size of pointer</param>
        /// <param name="encoding">Encoding if pointer is a string</param>
        /// <typeparam name="T">Value type</typeparam>
        /// <returns>Value</returns>
        public static T FromPointer<T>(void* pointer, bool freeAfterConverting = false, int size = -1, Encoding encoding = null)
        {
            var type = typeof(T);
            if (type == typeof(string))
            {
                var data = (byte*)pointer;
                encoding ??= Encoding.UTF8;
                var result = (T)(object)encoding.GetString(data, size);
                if (freeAfterConverting)
                    Free(pointer);
                return result;
            }

            if (type.IsValueType)
            {
                var result = Marshal.PtrToStructure<T>(new IntPtr(pointer));
                if (freeAfterConverting)
                    Free(pointer);
                return result;
            }

            var handler = GCHandle.FromIntPtr(new IntPtr(pointer));
            if (freeAfterConverting)
                Free(pointer);
            return (T)handler.Target;
        }

        /// <summary>
        /// Cast pointer to array
        /// </summary>
        /// <param name="pointer">Pointer</param>
        /// <param name="elementsCount">Elements in array</param>
        /// <param name="freeAfterConverting">If true pointer will be free after converting</param>
        /// <typeparam name="T">Array type</typeparam>
        /// <returns>Array</returns>
        public static T[] FromPointerToArray<T>(void* pointer, int elementsCount, bool freeAfterConverting = false) where T : unmanaged
        {
            if (elementsCount <= 0)
                return Array.Empty<T>();

            var result = new T[elementsCount];
            var managedPointer = (T*)pointer;
            for (var i = 0; i < elementsCount; i++)
                result[i] = managedPointer[i];

            if (freeAfterConverting)
                Free(pointer);

            return result;
        }

        /// <summary>
        /// Cast pointer to array that contains structures
        /// </summary>
        /// <param name="pointer">Pointer</param>
        /// <param name="elementsCount">Elements in array</param>
        /// <param name="freeAfterConverting">If true pointer will be free after converting</param>
        /// <typeparam name="T">Array type</typeparam>
        /// <returns>Array</returns>
        public static T[] FromPointerToStructureArray<T>(void* pointer, int elementsCount, bool freeAfterConverting = false) where T : struct
        {
            if (elementsCount <= 0)
                return Array.Empty<T>();

            var structureSize = Marshal.SizeOf<T>();

            var result = new T[elementsCount];
            for (var i = 0; i < elementsCount; i++)
                result[i] = Marshal.PtrToStructure<T>(IntPtr.Add(new IntPtr(pointer), i * structureSize));

            if (freeAfterConverting)
                Free(pointer);

            return result;
        }

        /// <summary>
        /// Cast pointer to string
        /// </summary>
        /// <param name="pointer">Pointer</param>
        /// <param name="length">String length</param>
        /// <param name="freeAfterConverting">If true pointer will be free after converting</param>
        /// <returns>String</returns>
        public static string FromBytesToString(void* pointer, int length, bool freeAfterConverting = false)
        {
            var bytes = FromPointerToStructureArray<byte>(pointer, length, freeAfterConverting);
            return Encoding.UTF8.GetString(bytes, 0, length);
        }

        /// <summary>
        /// Return size of type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Size in bytes</returns>
        public static int SizeOf<T>()
        {
            var type = typeof(T);
            if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(char) || type == typeof(bool)) return 1;
            if (type == typeof(short) || type == typeof(ushort)) return 2;
            if (type == typeof(int) || type == typeof(uint) || type == typeof(float)) return 4;
            if (type == typeof(long) || type == typeof(ulong) || type == typeof(double)) return 8;
            if (type == typeof(decimal)) return 16;

            return Marshal.SizeOf<T>();
        }

        /// <summary>
        /// Return length of char pointer
        /// </summary>
        /// <param name="pointer">Char pointer</param>
        /// <returns>Char pointer length</returns>
        public static int GetCharPointerLength(char* pointer) => new string(pointer).Length;
    }

#if DEBUG_ENABLE_WATCHING_MEMORY_ALLOCATING
    public readonly unsafe struct AllocatedPointerDebugInfo
    {
        public IntPtr AllocatedPointer { get; }

        public string StackTrace { get; }
    
        public AllocatedPointerDebugInfo(IntPtr pointer)
        {
            AllocatedPointer = pointer;
            StackTrace = Environment.StackTrace;
        }

        public override bool Equals(object obj) => obj is AllocatedPointerDebugInfo other && this == other;

        public override int GetHashCode() => (AllocatedPointer.GetHashCode() * 397) ^ (StackTrace != null ? StackTrace.GetHashCode() : 0);

        public static bool operator ==(AllocatedPointerDebugInfo left, AllocatedPointerDebugInfo right) =>
            left.AllocatedPointer == right.AllocatedPointer;

        public static bool operator !=(AllocatedPointerDebugInfo left, AllocatedPointerDebugInfo right) => !(left == right);

        public static implicit operator AllocatedPointerDebugInfo(IntPtr pointer) =>
            new(pointer);
        public static implicit operator AllocatedPointerDebugInfo(void* pointer) =>
            new(new IntPtr(pointer));
    }
#endif
}
#endif