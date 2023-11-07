using System.Collections.Generic;
using DamnLibrary.Utilities;
using DamnLibrary.Utilities.Extensions;
#if ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Parsers;
using DamnLibrary.DamnScript.Runtime;
#endif

namespace DamnLibrary.Games
{
#if UNITY_5_3_OR_NEWER  && ENABLE_DAMN_SCRIPT
    [DamnScriptable]
#endif
    public static class Infoportion
    {
        private static readonly List<string> infoportions = new();

        /// <summary>
        /// Add infoportion to the list
        /// </summary>
        /// <param name="name">Infoportion name</param>
        public static void AddInfoportion(string name)
        {
            if (HasInfoportion(name))
                return;

            infoportions.Add(name);
        }

        /// <summary>
        /// Check if infoportion was added
        /// </summary>
        /// <param name="name">Infoportion name</param>
        /// <returns>True if was added</returns>
        public static bool HasInfoportion(string name) => infoportions.Contains(name);

        /// <summary>
        /// Remove infoportion from the list
        /// </summary>
        /// <param name="name">Infoportion name</param>
        public static void RemoveInfoportion(string name) => infoportions.Remove(name);

#if UNITY_5_3_OR_NEWER  && ENABLE_DAMN_SCRIPT
        private static void RegisterDamnScriptMethods()
        {
            ScriptEngine.AddMethod("HasInfoportion", async (code, arguments) =>
            {
                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments, 
                    HasInfoportion(arguments.GetObject(0)));
            });
            
            ScriptEngine.AddMethod("AddInfoportion", async (code, arguments) =>
            {
                AddInfoportion(arguments[0]);

                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            ScriptEngine.AddMethod("RemoveInfoportion", async (code, arguments) =>
            {
                RemoveInfoportion(arguments[0]);

                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            ScriptEngine.AddMethod("OnHasInfoportion", async (code, arguments) =>
            {
                await TaskUtilities.WaitUntil(() => HasInfoportion(arguments[0]));

                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            ScriptEngine.AddMethod("OnHasntInfoportion", async (code, arguments) =>
            {
                await TaskUtilities.WaitUntil(() => !HasInfoportion(arguments[0]));

                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
        }
#endif
    }
}
