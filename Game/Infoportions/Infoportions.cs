using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Rietmon.DS;
using Rietmon.Extensions;
using Rietmon.Serialization;

namespace Rietmon.Game
{
    [StaticSerializable(1), DamnScriptable]
    public static class Infoportions
    {
        private static List<string> infoportions = new List<string>();

        public static void AddInfoportion(string name)
        {
            if (HasInfoportion(name))
                return;

            infoportions.Add(name);
        }

        public static bool HasInfoportion(string name) => infoportions.Contains(name);

        public static void RemoveInfoportion(string name) => infoportions.Remove(name);

        private static void Serialize(SerializationStream stream)
        {
            stream.Write(infoportions);
        }

        private static void Deserialize(SerializationStream stream)
        {
            infoportions = stream.Read<List<string>>();
        }

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
                await UniTask.WaitUntil(() => HasInfoportion(arguments[0]));

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            DamnScriptEngine.RegisterMethod("OnHasntInfoportion", async (code, arguments) =>
            {
                await UniTask.WaitUntil(() => !HasInfoportion(arguments[0]));

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
        }
    }
}
