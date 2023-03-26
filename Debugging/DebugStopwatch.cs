using System.Diagnostics;
using DamnLibrary.Extensions;

namespace DamnLibrary.Debugging
{
    public class DebugStopwatch
    {
        private readonly Stopwatch stopwatch = new();
        
        /// <summary>
        /// Start stopwatch
        /// </summary>
        public void Start()
        {
            stopwatch.Start();
        }

        /// <summary>
        /// Stop stopwatch and return elapsed time in ms.
        /// </summary>
        public long Stop()
        {
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}