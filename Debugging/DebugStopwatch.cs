using System.Diagnostics;
using Rietmon.Extensions;

namespace Rietmon.Debugging
{
    public class DebugStopwatch
    {
        private Stopwatch stopwatch = new Stopwatch();
        
        public void Start()
        {
            stopwatch.Start();
        }

        public void Stop(string messageFormat)
        {
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            UniversalDebugger.Log($"[{nameof(DebugStopwatch)}]: {messageFormat.Format(elapsedMilliseconds)}");
        }

        public long Stop()
        {
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}