﻿using System;
using System.Threading.Tasks;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

namespace DamnLibrary.Utilities
{
    public static class TaskUtilities
    {
        /// <summary>
        /// Task.Yield implementation as Task result
        /// </summary>
        public static async Task Yield(int framesCount = 1)
        {
            for (var i = 0; i < framesCount; i++)
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
            async Task Handler()
            {
                while (condition()) await Task.Delay(frequency);
            }

            var waitTask = Handler();

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
#if !UNITY_WEBGL
            async Task Handler()
            {
                while (!condition()) await Task.Delay(frequency);
            }

            var waitTask = Handler();

            return waitTask == await Task.WhenAny(waitTask, Task.Delay(timeout));
#else
            async Task Handler()
            {
                var framesDelay = (frequency / Time.deltaTime).RoundToIntByMath();
                while (!condition())
                {
                    for (var i = 0; i < framesDelay; i++)
                        await Yield();
                }
            }

            var waitTask = Handler();

            await waitTask;

            return true;
#endif
        }

        /// <summary>
        /// Task.Delay implementation with WebGL support
        /// </summary>
        /// <param name="milliseconds">Milliseconds</param>
        public static async Task Delay(int milliseconds)
        {
#if !UNITY_WEBGL
            await Task.Delay(milliseconds);
#else
            while (milliseconds > 0)
            {
                await Task.Yield();
                milliseconds -= (int)(Time.deltaTime * 1000);
            }
#endif
        }

        /// <summary>
        /// Forget task for sync execution
        /// </summary>
        /// <param name="task">Task</param>
        public static void Forget(this Task task) => task.ConfigureAwait(false);
    }
}