using System;
using System.Diagnostics;
using DamnLibrary.Utilities.Extensions;
using Debug = UnityEngine.Debug;
#if ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Parsers;
using DamnLibrary.DamnScript.Runtime;
#endif
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace DamnLibrary.Debugs
{
#if ENABLE_DAMN_SCRIPT
    [DamnScriptable]
#endif
    public static class UniversalDebugger
    {
        /// <summary>
        /// Callback for log
        /// </summary>
        public static Action<object> OnLog { get; set; }
        
        /// <summary>
        /// Callback for warning
        /// </summary>
        public static Action<object> OnWarning { get; set; }
        
        /// <summary>
        /// Callback for error
        /// </summary>
        public static Action<object> OnError { get; set; }

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

        [HideInCallstack]
        internal static void Log(object message) 
#if !DISABLE_LOGS
            => OnLog?.Invoke(message);
#else 
            { }
#endif
        [HideInCallstack]
        internal static void LogWarning(object message) 
#if !DISABLE_LOGS
            => OnWarning?.Invoke(message);
#else 
            { }
#endif
        
        [HideInCallstack]
        internal static void LogError(object message) 
#if !DISABLE_LOGS
            => OnError?.Invoke(message);
#else 
            { }
#endif

#if !UNITY_5_3_OR_NEWER
        private static void WriteToConsole(object message, ConsoleColor color)
        {
            var cashedColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);z
            Console.ForegroundColor = cashedColor;
        }
#endif

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