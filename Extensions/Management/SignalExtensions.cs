using Rietmon.Management;
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Rietmon.Extensions
{
    public static class SignalExtensions
    {
        public static void Notify(this Signal signal) => SignalSystem.Notify(signal);

#if ENABLE_UNI_TASK
        public static async UniTask NotifyAsync(this Signal signal, bool waitForComplete = true) =>
#else
        public static async Task NotifyAsync(this Signal signal, bool waitForComplete = true) => 
#endif
            await SignalSystem.NotifyAsync(signal, waitForComplete);
    }
}