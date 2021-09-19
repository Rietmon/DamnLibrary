#if ENABLE_DAMN_SCRIPT
using System;
using System.Collections.Generic;
using System.IO;
using Rietmon.DamnScript.Executing;
using System.Threading.Tasks;
#if ENABLE_SERIALIZATION
using Rietmon.DamnScript.Compiling;
#endif
using Rietmon.DamnScript.Data;
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
            string[] arguments, bool? forceReturnValue = null)
        {
            if (arguments.Length <= wasUsedArguments)
                return forceReturnValue ?? true;

            var otherArguments = arguments.CopyFromTo(wasUsedArguments, arguments.Length - 1);
            var executingMoreResult = await InvokeAsync(code, otherArguments);

            return forceReturnValue ?? executingMoreResult;
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
        
#if ENABLE_SERIALIZATION
        public static ScriptData CreateDataFromCompiledCode(string name, byte[] code)
        {
            var data = ScriptDatasManager.Get(name);
            return data ?? ScriptCompiler.DecompileFromBytes(code);
        }

        public static ScriptData CreateFromCompiledFile(string filePath)
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            var data = ScriptDatasManager.Get(name);
            return data ?? ScriptCompiler.DecompileFromFile(filePath);
        }
#endif

        private static bool PrepareToInvoke(string[] codes, out NativeMethod method, out string[] arguments)
        {
            var methodName = codes[0];
            arguments = codes.CopyWithout(0);

            return NativeMethodsManager.TryGet(methodName, out method);
        }

        [DamnScriptable]
        private static void RegisterDamnScriptMethods()
        {
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
        }
    }
}
#endif