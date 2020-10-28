using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Rietmon.Debugging
{
    public static class Debugger
    {
        public static string PathToLogFile { get; set; }

        public static bool RewriteFileAfterEachMessage { get; set; }

        public static Action OnNotify { get; set; }

        public static Action<string, string> OnMessage { get; set; }

        public static Action<string, string> OnWarning { get; set; }

        public static Action<string, string> OnError { get; set; }

        public static List<IDebugNotifier> Notifiers { get; } = new List<IDebugNotifier>();

        public static List<string> Messages { get; } = new List<string>();

        public static string Log
        {
            get
            {
                var log = "";
                foreach (var message in Messages)
                    log += $"{message}\n";
                return log;
            }
        }

        public static void Start()
        {
            Application.logMessageReceivedThreaded += OnLogMessage;
        }

        public static void Notify(string message)
        {
            OnNotify?.Invoke();

            foreach (var notifier in Notifiers)
                notifier.Notify();
        }

        public static void LoadLogFile(string pathToFile = "")
        {
            if (string.IsNullOrEmpty(pathToFile))
                pathToFile = PathToLogFile;

            var reader = new StringReader(File.ReadAllText(pathToFile));

            string currentLine;
            while (!string.IsNullOrEmpty(currentLine = reader.ReadLine()))
                Messages.Add(currentLine);
        }

        public static void SaveLogFile()
        {
            if (string.IsNullOrEmpty(PathToLogFile))
                return;

            File.WriteAllText(PathToLogFile, Log);
        }

        private static void OnLogMessage(string condition, string stacktrace, LogType type)
        {
            Messages.Add($"{DateTime.Now.ToString("G")} [{type.ToString()}]: {condition}");

            if (RewriteFileAfterEachMessage)
                SaveLogFile();

            switch (type)
            {
                case LogType.Log:
                    OnMessage?.Invoke(condition, stacktrace);
                    break;
                case LogType.Warning:
                    OnWarning?.Invoke(condition, stacktrace);
                    break;
                default:
                    OnError?.Invoke(condition, stacktrace);
                    break;
            }
        }
    }
}
