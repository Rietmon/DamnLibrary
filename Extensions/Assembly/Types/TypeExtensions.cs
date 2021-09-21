using System;
using System.Reflection;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

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
#if UNITY_5_3_OR_NEWER 
                Debug.LogWarning(
                    $"[{nameof(TypeExtensions)}] ({nameof(SafeInvokeStaticMethod)}) Unable to find static method \"{methodName}\" in type {type.FullName}!");
#endif
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
#if UNITY_5_3_OR_NEWER 
                Debug.LogWarning(
                    $"[{nameof(TypeExtensions)}] ({nameof(SafeInvokeMethod)}) Unable to find method \"{methodName}\" in type {type.FullName}!");
#endif
                return;
            }

            methodInfo.Invoke(owner, arguments);
        }

        public static FieldInfo GetFieldByName(this Type type, string name)
        {
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return fields.Find((fieldInfo) => fieldInfo.Name == name);
        }

        public static PropertyInfo GetPropertyByName(this Type type, string name)
        {
            var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return properties.Find((propertyInfo) => propertyInfo.Name == name);
        }

        public static MethodInfo GetMethodByName(this Type type, string name)
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return methods.Find((methodInfo) => methodInfo.Name == name);
        }
    }
}