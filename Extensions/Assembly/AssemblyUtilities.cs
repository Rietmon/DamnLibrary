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

        public static Assembly[] MainAssemblies => mainAssemblies ??= GetAssemblies(mainAssemblyNames).ToArray();

        private static Assembly[] mainAssemblies;

        public static IEnumerable<Assembly> GetAssemblies(params string[] names)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (names.Contains(assembly.GetName().FullName))
                    yield return assembly;
            }
        }

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

        public static IEnumerable<Type> GetAllInheritsFromAllAssemblies<T>()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(T)))
                    yield return type;
            }
        }

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