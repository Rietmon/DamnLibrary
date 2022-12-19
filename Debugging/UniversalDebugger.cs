#if ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript;
#endif
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace DamnLibrary.Debugging
{
#if ENABLE_DAMN_SCRIPT
    [DamnScriptable]
#endif
    public static class UniversalDebugger
    {
        public static Action<string> OnLog { get; set; }
        public static Action<string> OnWarning { get; set; }
        public static Action<string> OnError { get; set; }

        static UniversalDebugger()
        {
#if UNITY_5_3_OR_NEWER
            OnLog = Debug.Log;
            OnWarning = Debug.LogWarning;
            OnError = Debug.LogError;
#else
            OnLog = (message) => WriteToConsole(message, ConsoleColor.White);
            OnWarning = (message) => WriteToConsole(message, ConsoleColor.Yellow);
            OnError = (message) => WriteToConsole(message, ConsoleColor.Red);
#endif
        }

        internal static void Log(string message) => OnLog?.Invoke(message);
        internal static void LogWarning(string message) => OnWarning?.Invoke(message);
        internal static void LogError(string message) => OnError?.Invoke(message);

        private static void WriteToConsole(string message, ConsoleColor color)
        {
            var cashedColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = cashedColor;
        }

#if ENABLE_DAMN_SCRIPT
        private static void RegisterDamnScriptMethods()
        {
            ScriptEngine.AddMethod("Log", async (code, arguments) =>
            {
                Log($"[DamnScript] (Log): {arguments.GetObject(0)}");
                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            ScriptEngine.AddMethod("LogWarning", async (code, arguments) =>
            {
                LogWarning($"[DamnScript] (LogWarning): {arguments.GetObject(0)}");
                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            ScriptEngine.AddMethod("LogError", async (code, arguments) =>
            {
                LogError($"[DamnScript] (LogError): {arguments.GetObject(0)}");
                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
        }
#endif
    }
}