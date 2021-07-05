using System;
using System.Reflection;
#if ENABLE_DAMN_SCRIPT
using Rietmon.DS;
#endif
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class TypeExtensions
    {
        public static void SafeInvokeStaticMethod(this Type type, string methodName, params object[] arguments)
        {
            var methodInfo = type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
                Debug.LogWarning(
                    $"[{nameof(TypeExtensions)}] ({nameof(SafeInvokeStaticMethod)}) Unable to find static method \"{methodName}\" in type {type.FullName}!");
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
                    $"[{nameof(TypeExtensions)}] ({nameof(SafeInvokeMethod)}) Unable to find method \"{methodName}\" in type {type.FullName}!");
                return;
            }

            methodInfo.Invoke(owner, arguments);
        }
    }
}