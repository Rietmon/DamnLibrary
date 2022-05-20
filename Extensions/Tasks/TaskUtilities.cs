using System;
using System.Threading.Tasks;

namespace DamnLibrary.Extensions
{
    public static class TaskUtilities
    {
        /// <summary>
        /// Task.Yield implementation as Task result
        /// </summary>
        public static async Task Yield()
        {
            await Task.Yield();
        }
        
        /// <summary>
        /// Waiting while condition is true
        /// </summary>
        /// <param name="condition">Condition method</param>
        /// <param name="frequency">Delay before new check</param>
        /// <param name="timeout">Time out for checking, -1 = infinity</param>
        /// <returns>true - condition has been completed, false - timeout</returns>
        public static async Task<bool> WaitWhile(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) await Task.Delay(frequency);
            });

            return waitTask == await Task.WhenAny(waitTask, Task.Delay(timeout));
        }

        /// <summary>
        /// Waiting until condition is false
        /// </summary>
        /// <param name="condition">Condition method</param>
        /// <param name="frequency">Delay before new check</param>
        /// <param name="timeout">Time out for checking, -1 = infinity</param>
        /// <returns>true - condition has been completed, false - timeout</returns>
        public static async Task<bool> WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequency);
            });

            return waitTask == await Task.WhenAny(waitTask, Task.Delay(timeout));
        }
    }
}