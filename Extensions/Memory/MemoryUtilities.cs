#if ENABLE_MEMORY_UTILITIES
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static unsafe class MemoryUtilities
{
#if ENABLE_WINDOWS_UTILITIES
    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "CopyMemory")]
    public static extern void CopyMemory(void* destination, void* source, int size);
#endif

    public static void* Allocate(int count) => Marshal.AllocHGlobal(count).ToPointer();

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
            var pointer = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
            Marshal.StructureToPtr(value, pointer, true);
            return pointer.ToPointer();
        }

        var handler = GCHandle.Alloc(value);
        return GCHandle.ToIntPtr(handler).ToPointer();
    }

    public static T FromPointer<T>(void* pointer, int length = -1, Encoding encoding = null)
    {
        var type = typeof(T);
        if (type == typeof(string))
        {
            var data = (byte*)pointer;
            encoding ??= Encoding.UTF8;
            var result = (T)(object)encoding.GetString(data, length);
            return result;
        }

        if (type.IsValueType)
        {
            var result = Marshal.PtrToStructure<T>(new IntPtr(pointer));
            return result;
        }

        var handler = GCHandle.FromIntPtr(new IntPtr(pointer));
        return (T)handler.Target;
    }

    public static int GetCharPointerLength(char* pointer) => new string(pointer).Length;
}
#endif