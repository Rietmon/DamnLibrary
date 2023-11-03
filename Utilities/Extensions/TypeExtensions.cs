using System.Reflection;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

namespace DamnLibrary.DamnLibrary.Utilities.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Safe invoke static method. If the method doesn't exists, it will log a warning
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="methodName">Method name</param>
        /// <param name="arguments">Arguments</param>
        public static void SafeInvokeStaticMethod(this Type type, string methodName, params object[] arguments)
        {
            var methodInfo = type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
#if UNITY_5_3_OR_NEWER 
                UniversalDebugger.LogWarning(
                    $"[{nameof(TypeExtensions)}] ({nameof(SafeInvokeStaticMethod)}) Unable to find static method \"{methodName}\" in type {type.FullName}!");
#endif
                return;
            }

            methodInfo.Invoke(null, arguments);
        }

        /// <summary>
        /// Safe invoke method. If the method doesn't exists, it will log a warning
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="owner">Owner</param>
        /// <param name="methodName">Method name</param>
        /// <param name="arguments">Arguments</param>
        public static void SafeInvokeMethod(this Type type, object owner, string methodName, params object[] arguments)
        {
            var methodInfo = type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
#if UNITY_5_3_OR_NEWER 
                UniversalDebugger.LogWarning(
                    $"[{nameof(TypeExtensions)}] ({nameof(SafeInvokeMethod)}) Unable to find method \"{methodName}\" in type {type.FullName}!");
#endif
                return;
            }

            methodInfo.Invoke(owner, arguments);
        }

        /// <summary>
        /// Return a field by name. Field can be private or static. If the field doesn't exists return default
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="name">Field name</param>
        /// <returns>FieldInfo</returns>
        public static FieldInfo GetFieldByName(this Type type, string name)
        {
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return fields.FindOrDefault((fieldInfo) => fieldInfo.Name == name);
        }

        /// <summary>
        /// Return a property by name. Property can be private or static. If the property doesn't exists return default
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="name">Property name</param>
        /// <returns>PropertyInfo</returns>
        public static PropertyInfo GetPropertyByName(this Type type, string name)
        {
            var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return properties.FindOrDefault((propertyInfo) => propertyInfo.Name == name);
        }

        /// <summary>
        /// Return a method by name. Method can be private or static. If the method doesn't exists return default
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="name">Method name</param>
        /// <returns>MethodInfo</returns>
        public static MethodInfo GetMethodByName(this Type type, string name)
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return methods.FindOrDefault((methodInfo) => methodInfo.Name == name);
        }

        public static object GetValue(this MemberInfo member, object owner = null) =>
            member switch
            {
                FieldInfo field => field.GetValue(owner),
                PropertyInfo property => property.GetValue(owner),
                _ => throw new ArgumentOutOfRangeException(nameof(member), member, "Unsupported type!")
            };

        public static void SetValue(this MemberInfo member, object value, object owner = null)
        {
            switch (member)
            {
                case FieldInfo field: field.SetValue(owner, value);
                    break;
                case PropertyInfo property: property.SetValue(owner, value);
                    break;
            }
        }
    }
}