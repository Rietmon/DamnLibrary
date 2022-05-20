using System.Diagnostics;
using DamnLibrary.Extensions;

namespace DamnLibrary.Debugging
{
    public class DebugStopwatch
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        
        public void Start()
        {
            stopwatch.Start();
        }

        /// <summary>
        /// Will be stopped stopwatch and will write in console string.
        /// Message example: "[DebugStopwatch]: Task has invoked in 40ms."
        /// </summary>
        /// <param name="messageFormat">Message format example: "Task has invoked in {0}ms."</param>
        public void Stop(string messageFormat)
        {
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            UniversalDebugger.Log($"[{nameof(DebugStopwatch)}]: {messageFormat.Format(elapsedMilliseconds)}");
        }

        /// <summary>
        /// Will be stopped stopwatch and return elapsed time in ms.
        /// </summary>
        public long Stop()
        {
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}