﻿using System.Threading.Tasks;
using DamnLibrary.Management;
using DamnLibrary.Management.Signals;

namespace DamnLibrary.Extensions
{
    public static class SignalExtensions
    {
        /// <summary>
        /// SignalSystem.Notify implementation
        /// </summary>
        /// <param name="signal">Signal</param>
        public static void Notify(this Signal signal) => SignalSystem.Notify(signal);

        /// <summary>
        /// SignalSystem.NotifyAsync implementation
        /// </summary>
        /// <param name="signal">Signal</param>
        /// <param name="waitForComplete">If true await will waiting untill all actions will be finished</param>
        public static async Task NotifyAsync(this Signal signal, bool waitForComplete = true) => 
            await SignalSystem.NotifyAsync(signal, waitForComplete);
    }
}