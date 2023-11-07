#if ENABLE_DAMN_SCRIPT
using DamnLibrary.Games.DamnScript.Compiling;
using DamnLibrary.Games.DamnScript.Parsers;
using DamnLibrary.Games.DamnScript.Runtime.Data;
using DamnLibrary.Games.DamnScript.Runtime.Executing;
using DamnLibrary.Games.DamnScript.Runtime.Native;
#if ENABLE_SERIALIZATION
#endif

namespace DamnLibrary.Games.DamnScript.Runtime
{
    [DamnScriptable]
    public static class ScriptEngine
    {
        /// <summary>
        /// Initialize the script engine.
        /// Will register all damn script methods
        /// </summary>
        public static void Initialize()
        {
            var damnScriptableTypes = AssemblyUtilities.GetAllAttributeInherits<DamnScriptableAttribute>();
            foreach (var damnScriptable in damnScriptableTypes)
                damnScriptable.SafeInvokeStaticMethod("RegisterDamnScriptMethods");
        }

        /// <summary>
        /// Add method to the script engine
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="method">Method</param>
        public static void AddMethod(string name, Func<ScriptCode, string[], Task<bool>> method) =>
            NativeMethodsManager.Add(name, method);
        
        /// <summary>
        /// Invoke a code async with the given arguments
        /// </summary>
        /// <param name="owner">ScriptCode</param>
        /// <param name="codes">Arguments or other code</param>
        /// <returns>Executing task with result that mean is line ended?</returns>
        public static async Task<bool> InvokeAsync(ScriptCode owner, string[] codes)
        {
            if (!PrepareToInvoke(codes, out var method, out var arguments))
            {
                UniversalDebugger.LogError($"[{nameof(ScriptEngine)}] ({nameof(InvokeAsync)}) Unable to find method with the name {codes[0]}");
                return true;
            }

            return await method.InvokeAsync(owner, arguments);
        }

        /// <summary>
        /// Invoke a code with the given arguments and wait for the result
        /// </summary>
        /// <param name="owner">ScriptCode</param>
        /// <param name="codes">Arguments or other code</param>
        /// <returns>Is line ended?</returns>
        public static bool Invoke(ScriptCode owner, string[] codes) => 
            InvokeAsync(owner, codes).Result;
        
        /// <summary>
        /// If line isn't ended, try to execute more code async
        /// </summary>
        /// <param name="wasUsedArguments">How much arguments was used?</param>
        /// <param name="code">ScriptCode</param>
        /// <param name="arguments">Arguments or other code</param>
        /// <param name="forceReturnValue">Force return value to keep executing or go to next line</param>
        /// <returns>Executing task with result that mean is line ended?</returns>
        public static async Task<bool> TryExecuteMoreAsync(int wasUsedArguments, ScriptCode code,
            string[] arguments, bool? forceReturnValue = null)
        {
            if (arguments.Length <= wasUsedArguments)
                return forceReturnValue ?? true;

            var otherArguments = arguments.CopyFromTo(wasUsedArguments, arguments.Length - 1);
            var executingMoreResult = await InvokeAsync(code, otherArguments);

            return forceReturnValue ?? executingMoreResult;
        }

        /// <summary>
        /// If line isn't ended, try to execute more code
        /// </summary>
        /// <param name="wasUsedArguments">How much arguments was used?</param>
        /// <param name="code">ScriptCode</param>
        /// <param name="arguments">Arguments or other code</param>
        /// <param name="forceReturnValue">Force return value to keep executing or go to next line</param>
        /// <returns>Is line ended?</returns>
        public static bool TryExecuteMore(int wasUsedArguments, ScriptCode code, string[] arguments, bool forceReturnValue = true) =>
            TryExecuteMoreAsync(wasUsedArguments, code, arguments, forceReturnValue).Result;
        
        /// <summary>
        /// Create a script data from .ds file
        /// </summary>
        /// <param name="filePath">Path to .ds file</param>
        /// <returns>ScriptData</returns>
        public static ScriptData CreateDataFromFile(string filePath)
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            var data = ScriptDatasManager.Get(name);
            return data ?? ScriptDatasManager.Create(name, File.ReadAllText(filePath));
        }
        
        /// <summary>
        /// Create a script data from code with the given name
        /// </summary>
        /// <param name="name">Script name</param>
        /// <param name="code">Script code</param>
        /// <returns>ScriptData</returns>
        public static ScriptData CreateDataFromCode(string name, string code)
        {
            var data = ScriptDatasManager.Get(name);
            return data ?? ScriptDatasManager.Create(name, code);
        }
        
#if ENABLE_SERIALIZATION
        /// <summary>
        /// Create data from compiled .dsc code
        /// </summary>
        /// <param name="name">Script name</param>
        /// <param name="code">Script code</param>
        /// <returns>ScriptData</returns>
        public static ScriptData CreateDataFromCompiledCode(string name, byte[] code)
        {
            var data = ScriptDatasManager.Get(name);
            return data ?? ScriptCompiler.DecompileFromBytes(code);
        }

        /// <summary>
        /// Create data from compiled .dsc file
        /// </summary>
        /// <param name="filePath">Path to .dsc file</param>
        /// <returns>ScriptData</returns>
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