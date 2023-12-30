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

        /// <summary>
        /// SignalSystem.NotifyAsync implementation
        /// </summary>
        /// <param name="internalSignal">Signal</param>
        /// <param name="waitForComplete">If true await will waiting untill all actions will be finished</param>
        public static async Task NotifyAsync(this Internal_Signal internalSignal, bool waitForComplete = true) => 
            await SignalSystem.NotifyAsync(internalSignal, waitForComplete);
    }
}