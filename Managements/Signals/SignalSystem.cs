using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DamnLibrary.Debugs;
using DamnLibrary.Types.Pairs;
using DamnLibrary.Utilities;

namespace DamnLibrary.Managements.Signals
{
    public static class SignalSystem
    {
        private static readonly Dictionary<Type, List<Pair<SignalSystemId, Action<Internal_Signal>>>> signalCallbacks = new();

        internal static void Subscribe<T>(Action<T> callback) where T : Internal_Signal
        {
            void SignalWrapper(Internal_Signal signal) => 
                callback((T)signal);

            if (callback == null)
                return;
            
            var signalType = typeof(T);
            if (!signalCallbacks.TryGetValue(signalType, out var callbacks))
            {
                callbacks = new List<Pair<SignalSystemId, Action<Internal_Signal>>>();
                signalCallbacks.Add(signalType, callbacks);
            }

            callbacks.Add(new Pair<SignalSystemId, Action<Internal_Signal>>(callback.Method.MethodHandle, SignalWrapper));
        }

        internal static void Unsubscribe<T>(Action<T> callback) where T : Internal_Signal
        {
            if (callback == null)
                return;
            
            var signalType = typeof(T);
            if (!signalCallbacks.TryGetValue(signalType, out var callbacks))
                return;

            var signalId = (SignalSystemId)callback.Method.MethodHandle;
            
            for (var i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].First == signalId)
                {
                    callbacks.RemoveAt(i);
                    return;
                }
            }
        }

        internal static void Notify<T>(T signal) where T : Internal_Signal
        {
            var signalType = signal.GetType();
            if (!signalCallbacks.TryGetValue(signalType, out var callbacks))
                return;
            
            foreach (var callback in callbacks)
                callback.Second?.Invoke(signal);
        }
        
        internal static async Task NotifyAsync<T>(T signal, bool waitForComplete = true) where T : Internal_Signal
        {
            var signalType = signal.GetType();
            if (!signalCallbacks.TryGetValue(signalType, out var callbacks))
                return;
            
            foreach (var callback in callbacks)
            {
                var asyncResult = callback.Second?.BeginInvoke(signal, null, null);
                if (asyncResult == null)
                {
                    UniversalDebugger.LogError($"[{nameof(SignalSystem)}] ({nameof(NotifyAsync)}) Unable to invoke method {callback.Second?.Method.Name}. May be it is null.");
                    continue;
                }
                if (waitForComplete)
                {
                    await TaskUtilities.WaitUntil(() => asyncResult.IsCompleted);
                }
            }
        }
    } 
}