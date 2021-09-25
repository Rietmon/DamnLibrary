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

    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "memset")]
    public static extern void MemorySet(void* destination, int value, int count);
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

    public static T[] FromPointerToArray<T>(void* pointer, int elementsCount) where T : unmanaged
    {
        if (elementsCount <= 0)
            return Array.Empty<T>();

        var result = new T[elementsCount];
        var managedPointer = (T*)pointer;
        for (var i = 0; i < elementsCount; i++)
            result[i] = managedPointer[i];

        return result;
    }

    public static T[] FromPointerToStructureArray<T>(void* pointer, int elementsCount) where T : struct
    {
        if (elementsCount <= 0)
            return Array.Empty<T>();

        var structureSize = Marshal.SizeOf<T>();
        
        var result = new T[elementsCount];
        for (var i = 0; i < elementsCount; i++)
            result[i] = Marshal.PtrToStructure<T>(IntPtr.Add(new IntPtr(pointer), i * structureSize));

        return result;
    }

    public static string FromBytesToString(void* pointer, int length)
    {
        var bytes = FromPointerToStructureArray<byte>(pointer, length);
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
#endif