using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Rietmon.Extensions;
using UnityEngine;

namespace Rietmon.DS
{
    public static class DamnScriptEngine
    {
        private static readonly Dictionary<string, DamnScriptNativeMethod> methods =
            new Dictionary<string, DamnScriptNativeMethod>();

        public static async UniTask InitializeAsync()
        {
            var damnScriptableTypes = AssemblyUtilities.GetAllAttributeInherits<DamnScriptableAttribute>();
            foreach (var damnScriptable in damnScriptableTypes)
                damnScriptable.SafeInvokeStaticMethod("RegisterDamnScriptMethods");

            InitializeGlobals();
        }

        public static bool IsMethod(string str) => methods.ContainsKey(str);

        public static void RegisterMethod(string name, Func<DamnScriptCode, string[], bool> method) =>
            methods.Add(name, method);

        public static void RegisterMethod(string name, Func<DamnScriptCode, string[], UniTask<bool>> method) =>
            methods.Add(name, method);

        public static void RegisterMethod(string name, DamnScriptNativeMethod method) => methods.Add(name, method);

        public static void UnregisterMethod(string name) => methods.Remove(name);

        public static async UniTask<bool> ExecuteStringAsync(string str)
        {
            if (str.IsNullOrEmpty())
                return true;

            var damnScriptCode = new DamnScriptCode(DamnScriptParser.ParseCode(str).First(), null);

            return await damnScriptCode.ExecuteAsync();
        }

        public static bool ExecuteString(string str)
        {
            if (str.IsNullOrEmpty())
                return true;

            var damnScriptCode = new DamnScriptCode(DamnScriptParser.ParseCode(str).First(), null);

            return damnScriptCode.Execute();
        }

        public static async UniTask<bool> ExecuteAsync(DamnScriptCode owner, string[] codes)
        {
            var methodName = codes[0];
            var arguments = codes.CopyWithout(0);

            if (!methods.TryGetValue(methodName, out var method))
            {
                Debug.LogError($"[DamnScriptEngine] (ExecuteAsync) Unable to find method with the name {methodName}");
                return true;
            }

            return await method.ExecuteAsync(owner, arguments);
        }

        public static bool Execute(DamnScriptCode owner, string[] codes)
        {
            var methodName = codes[0];
            var arguments = codes.CopyWithout(0);

            if (!methods.TryGetValue(methodName, out var method))
            {
                Debug.LogError($"[DamnScriptEngine] (ExecuteAsync) Unable to find method with the name {methodName}");
                return true;
            }

            return method.Execute(owner, arguments);
        }

        public static bool TryExecuteMore(int wasUsedArguments, DamnScriptCode code, string[] arguments,
            bool defaultReturnValue = true)
        {
            if (arguments.Length <= wasUsedArguments)
                return defaultReturnValue;

            var otherArguments = arguments.CopyFromTo(wasUsedArguments, arguments.Length - 1);
            return Execute(code, otherArguments);
        }

        public static async UniTask<bool> TryExecuteMoreAsync(int wasUsedArguments, DamnScriptCode code,
            string[] arguments, bool defaultReturnValue = true)
        {
            if (arguments.Length <= wasUsedArguments)
                return defaultReturnValue;

            var otherArguments = arguments.CopyFromTo(wasUsedArguments, arguments.Length - 1);
            return await ExecuteAsync(code, otherArguments);
        }

        private static void InitializeGlobals()
        {
            RegisterMethod("Log", async (code, arguments) =>
            {
                Debug.Log($"[DamnScriptEngine] (Log) Object \"{code.Owner.name}\" saying:\n{arguments[0]}");
                return await TryExecuteMoreAsync(1, code, arguments);
            });
            RegisterMethod("LogWarning", async (code, arguments) =>
            {
                Debug.LogWarning(
                    $"[DamnScriptEngine] (LogWarning) Object \"{code.Owner.name}\" saying:\n{arguments[0]}");
                return await TryExecuteMoreAsync(1, code, arguments);
            });
            RegisterMethod("LogError", async (code, arguments) =>
            {
                Debug.LogError($"[DamnScriptEngine] (LogError) Object \"{code.Owner.name}\" saying:\n{arguments[0]}");
                return await TryExecuteMoreAsync(1, code, arguments);
            });

            RegisterMethod("If", async (code, arguments) =>
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
                    if (await ExecuteAsync(code,
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

                    return await ExecuteAsync(code, codes);
                }
                else
                {
                    if (elseConditionIndex == -1)
                        return true;

                    var codes = arguments.CopyFromTo(elseConditionIndex + 1, arguments.Length - 1);

                    return await ExecuteAsync(code, codes);
                }
            });

            RegisterMethod("TRUE", async (code, arguments) => await TryExecuteMoreAsync(0, code, arguments));
            RegisterMethod("FALSE", async (code, arguments) => await TryExecuteMoreAsync(0, code, arguments, false));

            RegisterMethod("GoTo", async (code, arguments) =>
            {
                var damnScript = code.Owner.GetComponent<IDamnScriptExecutor>().Script;
                damnScript.BeginAsync(arguments[0]);
                return await TryExecuteMoreAsync(1, code, arguments);
            });
            RegisterMethod("GoToAndReturn", async (code, arguments) =>
            {
                var damnScript = code.Script;
                damnScript.BeginAsync(arguments[0]);
                code.Parent.Stop();

                return await TryExecuteMoreAsync(1, code, arguments);
            });
        }
    }
}
