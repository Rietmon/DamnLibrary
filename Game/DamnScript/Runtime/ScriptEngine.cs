#if ENABLE_DAMN_SCRIPT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rietmon.DamnScript.Executing;
using Rietmon.DamnScript.Parsers;
using System.Threading.Tasks;
using Rietmon.DamnScript.Data;
using Rietmon.Debugging;
using Rietmon.Extensions;

namespace Rietmon.DamnScript
{
    [DamnScriptable]
    public static class ScriptEngine
    {
        public static void Initialize()
        {
            var damnScriptableTypes = AssemblyUtilities.GetAllAttributeInherits<DamnScriptableAttribute>();
            foreach (var damnScriptable in damnScriptableTypes)
                damnScriptable.SafeInvokeStaticMethod("RegisterDamnScriptMethods");
        }

        public static void AddMethod(string name, Func<ScriptCode, string[], Task<bool>> method) =>
            NativeMethodsManager.Add(name, method);
        
        public static async Task<bool> InvokeAsync(ScriptCode owner, string[] codes)
        {
            if (!PrepareToInvoke(codes, out var method, out var arguments))
                return true;

            return await method.InvokeAsync(owner, arguments);
        }

        public static bool Invoke(ScriptCode owner, string[] codes) => 
            InvokeAsync(owner, codes).Result;
        
        public static async Task<bool> TryExecuteMoreAsync(int wasUsedArguments, ScriptCode code,
            string[] arguments, bool defaultReturnValue = true)
        {
            if (arguments.Length <= wasUsedArguments)
                return defaultReturnValue;

            var otherArguments = arguments.CopyFromTo(wasUsedArguments, arguments.Length - 1);
            return await InvokeAsync(code, otherArguments);
        }

        public static bool TryExecuteMore(int wasUsedArguments, ScriptCode code, string[] arguments, bool defaultReturnValue = true) =>
            TryExecuteMoreAsync(wasUsedArguments, code, arguments, defaultReturnValue).Result;
        
        public static ScriptData CreateDataFromFile(string filePath)
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            var data = ScriptDatasManager.Get(name);
            return data ?? ScriptDatasManager.Create(name, File.ReadAllText(filePath));
        }
        
        public static ScriptData CreateDataFromCode(string name, string code)
        {
            var data = ScriptDatasManager.Get(name);
            return data ?? ScriptDatasManager.Create(name, code);
        }

        private static bool PrepareToInvoke(string[] codes, out NativeMethod method, out string[] arguments)
        {
            var methodName = codes[0];
            arguments = codes.CopyWithout(0);

            return NativeMethodsManager.TryGet(methodName, out method);
        }

        [DamnScriptable]
        private static void RegisterDamnScriptMethods()
        {
            AddMethod("Log", async (code, arguments) =>
            {
                UniversalDebugger.Log($"[DamnScript] (Log): {arguments.GetObject(0)}");
                return await TryExecuteMoreAsync(1, code, arguments);
            });
            AddMethod("LogWarning", async (code, arguments) =>
            {
                UniversalDebugger.LogWarning($"[DamnScript] (LogWarning): {arguments.GetObject(0)}");
                return await TryExecuteMoreAsync(1, code, arguments);
            });
            AddMethod("LogError", async (code, arguments) =>
            {
                UniversalDebugger.LogError($"[DamnScript] (LogError): {arguments.GetObject(0)}");
                return await TryExecuteMoreAsync(1, code, arguments);
            });

            AddMethod("If", async (code, arguments) =>
            {
                var thenConditionIndex = arguments.IndexOf("Then");
                var orConditionIndices = arguments.IndicesOf("Or");
                var elseConditionIndex = arguments.IndexOf("Else");

                var startConditionIndices = new List<int>();
                var endConditionIndices = new List<int>();

                startConditionIndices.Add(0);
                endConditionIndices.Add(orConditionIndices.Length > 0
                    ? orConditionIndices[0] - 1
                    : thenConditionIndex - 1);
                for (var i = 0; i < orConditionIndices.Length; i++)
                {
                    startConditionIndices.Add(orConditionIndices[i] + 1);
                    endConditionIndices.Add(orConditionIndices.Length > i + 1
                        ? orConditionIndices[i + 1] - 1
                        : thenConditionIndex - 1);
                }

                var isTrue = false;
                for (var i = 0; i < startConditionIndices.Count; i++)
                {
                    if (await InvokeAsync(code,
                        arguments.CopyFromTo(startConditionIndices[i], endConditionIndices[i])))
                    {
                        isTrue = true;
                        break;
                    }
                }

                if (isTrue)
                {
                    var codes = arguments.CopyFromTo(thenConditionIndex + 1,
                        elseConditionIndex != -1 ? elseConditionIndex - 1 : arguments.Length - 1);

                    return await InvokeAsync(code, codes);
                }
                else
                {
                    if (elseConditionIndex == -1)
                        return true;

                    var codes = arguments.CopyFromTo(elseConditionIndex + 1, arguments.Length - 1);

                    return await InvokeAsync(code, codes);
                }
            });

            AddMethod("TRUE", async (code, arguments) => await TryExecuteMoreAsync(0, code, arguments));
            AddMethod("FALSE", async (code, arguments) => await TryExecuteMoreAsync(0, code, arguments, false));

            AddMethod("GoTo", async (code, arguments) =>
            {
                code.Script.Begin(arguments[0]);
                return await TryExecuteMoreAsync(1, code, arguments);
            });
            AddMethod("GoToAndReturn", async (code, arguments) =>
            {
                var damnScript = code.Script;
                damnScript.Begin(arguments[0]);
                code.Parent.Stop();

                return await TryExecuteMoreAsync(1, code, arguments);
            });
        }
    }
}
#endif