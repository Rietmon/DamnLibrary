using System.Threading.Tasks;
using DamnLibrary.Managements.Signals;

namespace DamnLibrary.Utilities.Extensions
{
    public static class SignalExtensions
    {
        /// <summary>
        /// SignalSystem.Notify implementation
        /// </summary>
        /// <param name="internalSignal">Signal</param>
        public static void Notify(this Internal_Signal internalSignal) => SignalSystem.Notify(internalSignal);

        // Rietmon: Removed be cause it calls in separate thread and rise multi-thread exceptions in Unity
        // public static async Task NotifyAsync(this Internal_Signal internalSignal, bool waitForComplete = true) => 
        //     await SignalSystem.NotifyAsync(internalSignal, waitForComplete);
    }
}