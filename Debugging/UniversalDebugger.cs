using System;

namespace Rietmon.Debugging
{
    public static class UniversalDebugger
    {
        public static Action<string> OnLog { get; set; }
        public static Action<string> OnWarning { get; set; }
        public static Action<string> OnError { get; set; }

        static UniversalDebugger()
        {
#if UNITY_5_3_OR_NEWER
            OnLog = (message) => Debug.Log(message);
            OnWarning = (message) => Debug.LogWarning(message);
            OnError = (message) => Debug.LogError(message);
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
    }
}