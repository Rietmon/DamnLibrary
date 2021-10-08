﻿using Rietmon.Management;
using System.Threading.Tasks;

namespace Rietmon.Extensions
{
    public static class SignalExtensions
    {
        public static void Notify(this Signal signal) => SignalSystem.Notify(signal);

        public static async Task NotifyAsync(this Signal signal, bool waitForComplete = true) => 
            await SignalSystem.NotifyAsync(signal, waitForComplete);
    }
}