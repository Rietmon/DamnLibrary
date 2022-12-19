#if UNITY_5_3_OR_NEWER  && ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript;
#endif
#if ENABLE_SERIALIZATION
#endif

namespace DamnLibrary.Game
{
#if UNITY_5_3_OR_NEWER  && ENABLE_DAMN_SCRIPT
    [DamnScriptable]
#endif
    public static class Infoportions
    {
        private static readonly List<string> infoportions = new();

        public static void AddInfoportion(string name)
        {
            if (HasInfoportion(name))
                return;

            infoportions.Add(name);
        }

        public static bool HasInfoportion(string name) => infoportions.Contains(name);

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
