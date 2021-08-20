using System.Collections.Generic;
#if UNITY_2020 && ENABLE_DAMN_SCRIPT
using Rietmon.DS;
#endif
using Rietmon.Extensions;
#if ENABLE_SERIALIZATION
using Rietmon.Serialization;
#endif

namespace Rietmon.Game
{
#if UNITY_2020 && ENABLE_DAMN_SCRIPT
    [DamnScriptable]
#endif
    public static class Infoportions
    {
        private static readonly List<string> infoportions = new List<string>();

        public static void AddInfoportion(string name)
        {
            if (HasInfoportion(name))
                return;

            infoportions.Add(name);
        }

        public static bool HasInfoportion(string name) => infoportions.Contains(name);

        public static void RemoveInfoportion(string name) => infoportions.Remove(name);

#if UNITY_2020 && ENABLE_DAMN_SCRIPT
        private static void RegisterDamnScriptMethods()
        {
            DamnScriptEngine.RegisterMethod("HasInfoportion", async (code, arguments) =>
            {
                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments, 
                    HasInfoportion(arguments.GetArgument(0)));
            });
            
            DamnScriptEngine.RegisterMethod("AddInfoportion", async (code, arguments) =>
            {
                AddInfoportion(arguments[0]);

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            DamnScriptEngine.RegisterMethod("RemoveInfoportion", async (code, arguments) =>
            {
                RemoveInfoportion(arguments[0]);

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            DamnScriptEngine.RegisterMethod("OnHasInfoportion", async (code, arguments) =>
            {
#if ENABLE_UNI_TASK
                await UniTask.WaitUntil(() => HasInfoportion(arguments[0]));
#else
                await TaskUtilities.WaitUntil(() => HasInfoportion(arguments[0]));
#endif

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            DamnScriptEngine.RegisterMethod("OnHasntInfoportion", async (code, arguments) =>
            {
#if ENABLE_UNI_TASK
                await UniTask.WaitUntil(() => !HasInfoportion(arguments[0]));
#else
                await TaskUtilities.WaitUntil(() => !HasInfoportion(arguments[0]));
#endif

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
        }
#endif
    }
}
