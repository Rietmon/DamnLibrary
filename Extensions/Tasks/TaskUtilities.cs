using System;
using System.Threading.Tasks;

namespace Rietmon.Extensions
{
    public static class TaskUtilities
    {
        public static async Task Yield()
        {
            await Task.Yield();
        }
        
        public static async Task WaitWhile(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) await Task.Delay(frequency);
            });

            if(waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }

        public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask, 
                Task.Delay(timeout))) 
                throw new TimeoutException();
        }
    }
}