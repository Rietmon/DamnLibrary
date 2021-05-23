using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Rietmon.DS;
using UnityEngine;

public static class TypeExtensions
{
    public static void SafeInvokeStaticMethod(this Type type, string methodName, params object[] arguments)
    {
        var methodInfo = type.GetMethod(methodName,
            BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
        if (methodInfo == null)
        {
            Debug.LogWarning(
                $"[{nameof(DamnScriptEngine)}] ({nameof(SafeInvokeStaticMethod)}) Unable to find static method \"{methodName}\" in type {type.FullName}!");
            return;
        }

        methodInfo.Invoke(null, arguments);
    }
    
    public static void SafeInvokeMethod(this Type type, object owner, string methodName, params object[] arguments)
    {
        var methodInfo = type.GetMethod(methodName,
            BindingFlags.Public | BindingFlags.NonPublic);
        if (methodInfo == null)
        {
            Debug.LogWarning(
                $"[{nameof(DamnScriptEngine)}] ({nameof(SafeInvokeMethod)}) Unable to find method \"{methodName}\" in type {type.FullName}!");
            return;
        }

        methodInfo.Invoke(owner, arguments);
    }
}
