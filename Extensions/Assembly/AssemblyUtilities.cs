using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DamnLibrary.Extensions
{
    public static class AssemblyUtilities
    {
        private static readonly string[] mainAssemblyNames =
        {
#if UNITY_5_3_OR_NEWER 
            "Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
            "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
#endif
        };

        /// <summary>
        /// Loaded assemblies
        /// </summary>
        public static Assembly[] MainAssemblies => mainAssemblies ??= GetAssemblies(mainAssemblyNames).ToArray();

        private static Assembly[] mainAssemblies;

        /// <summary>
        /// Return assemblies with the names
        /// </summary>
        /// <param name="names">Names of assemblies</param>
        /// <returns>Array of assemblies</returns>
        public static IEnumerable<Assembly> GetAssemblies(params string[] names)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (names.Contains(assembly.GetName().FullName))
                    yield return assembly;
            }
        }

        /// <summary>
        /// Return all types with the attribute from all assemblies
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <returns>Array of types</returns>
        public static IEnumerable<Type> GetAllAttributeInheritsFromAllAssemblies<T>()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.GetCustomAttributes(typeof(T), true).Length > 0)
                        yield return type;
                }
            }
        }

        /// <summary>
        /// Return all types with the attribute from assemblies
        /// </summary>
        /// <param name="assemblyNames">Assemblies names</param>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <returns>Array of types</returns>
        public static IEnumerable<Type> GetAllAttributeInherits<T>(params string[] assemblyNames)
        {
            var assemblies = assemblyNames == null || assemblyNames.Length == 0
                ? AppDomain.CurrentDomain.GetAssemblies()
                : MainAssemblies;
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.GetCustomAttributes(typeof(T), true).Length > 0)
                        yield return type;
                }
            }
        }

        /// <summary>
        /// Return all types that inherits from T from all assemblies
        /// </summary>
        /// <typeparam name="T">Parent class</typeparam>
        /// <returns>Array of types</returns>
        public static IEnumerable<Type> GetAllInheritsFromAllAssemblies<T>()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(T)))
                    yield return type;
            }
        }

        /// <summary>
        /// Return all types that inherits from T from assemblies
        /// </summary>
        /// <param name="assemblyNames">Assemblies names</param>
        /// <typeparam name="T">Parent class</typeparam>
        /// <returns>Array of types</returns>
        public static IEnumerable<Type> GetAllInheritsFrom<T>(params string[] assemblyNames)
        {
            var assemblies = assemblyNames != null && assemblyNames.Length > 0
                ? AppDomain.CurrentDomain.GetAssemblies()
                : MainAssemblies;
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(T)))
                        yield return type;
                }
            }
        }
    }
}