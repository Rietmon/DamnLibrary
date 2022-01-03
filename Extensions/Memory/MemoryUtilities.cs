#if ENABLE_MEMORY_UTILITIES
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Rietmon.Extensions
{
    public static unsafe class MemoryUtilities
    {
#if DEBUG_ENABLE_WATCHING_MEMORY_ALLOCATING
        public static readonly List<AllocatedPointerDebugInfo> allocatedPointers = new List<AllocatedPointerDebugInfo>();
#endif
        
#if ENABLE_WINDOWS_UTILITIES
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "CopyMemory")]
        public static extern void CopyMemory(void* destination, void* source, int size);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "memset")]
        public static extern void MemorySet(void* destination, int value, int count);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "IsBadReadPtr")]
        public static extern bool IsBadReadPointer(void* pointer, int length);
#endif

        public static void* Allocate(int count) => InternalAllocate(count);

        private static void* InternalAllocate(int count)
        {
#if !DEBUG_ENABLE_WATCHING_MEMORY_ALLOCATING
            return Marshal.AllocHGlobal(count).ToPointer();
#else
            var intPointer = Marshal.AllocHGlobal(count);
            allocatedPointers.Add(intPointer);
            return intPointer.ToPointer();
#endif
        }

        public static void Free(void* pointer) => InternalFree(pointer);

        private static void InternalFree(void* pointer)
        {
#if !DEBUG_ENABLE_WATCHING_MEMORY_ALLOCATING
            Marshal.FreeHGlobal(new IntPtr(pointer));
#else
            var intPointer = new IntPtr(pointer);
            allocatedPointers.Remove(intPointer);
            Marshal.FreeHGlobal(new IntPtr(pointer));
#endif
        }

#if !ENABLE_WINDOWS_UTILITIES
        public static void CopyMemory(void* destination, void* source, int size)
        {
            var destinationBytesPointer = (byte*)destination;
            var sourceBytesPointer = (byte*)source;

            for (var i = 0; i < size; i++)
                destinationBytesPointer[i] = sourceBytesPointer[i];
        }
#endif
        
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

        public static T FromPointer<T>(void* pointer, bool freeAfterConverting = false, int length = -1, Encoding encoding = null)
        {
            var type = typeof(T);
            if (type == typeof(string))
            {
                var data = (byte*)pointer;
                encoding ??= Encoding.UTF8;
                var result = (T)(object)encoding.GetString(data, length);
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

        public static string FromBytesToString(void* pointer, int length, bool freeAfterConverting = false)
        {
            var bytes = FromPointerToStructureArray<byte>(pointer, length, freeAfterConverting);
            return Encoding.UTF8.GetString(bytes, 0, length);
        }

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

        public static int GetCharPointerLength(char* pointer) => new string(pointer).Length;
    }

#if DEBUG_ENABLE_WATCHING_MEMORY_ALLOCATING
    public readonly unsafe struct AllocatedPointerDebugInfo
    {
        public readonly IntPtr allocatedPointer;

        public readonly string stackTrace;
    
        public AllocatedPointerDebugInfo(IntPtr pointer)
        {
            allocatedPointer = pointer;
            stackTrace = Environment.StackTrace;
        }

        public bool Equals(AllocatedPointerDebugInfo other) => this == other;

        public override bool Equals(object obj) => obj is AllocatedPointerDebugInfo other && this == other;

        public override int GetHashCode() => (allocatedPointer.GetHashCode() * 397) ^ (stackTrace != null ? stackTrace.GetHashCode() : 0);

        public static bool operator ==(AllocatedPointerDebugInfo left, AllocatedPointerDebugInfo right) =>
            left.allocatedPointer == right.allocatedPointer;

        public static bool operator !=(AllocatedPointerDebugInfo left, AllocatedPointerDebugInfo right) => !(left == right);

        public static implicit operator AllocatedPointerDebugInfo(IntPtr pointer) =>
            new AllocatedPointerDebugInfo(pointer);
        public static implicit operator AllocatedPointerDebugInfo(void* pointer) =>
            new AllocatedPointerDebugInfo(new IntPtr(pointer));
    }
#endif
}
#endif