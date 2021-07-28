using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Rietmon.Other;

namespace Rietmon.Management
{
    public static class SignalSystem
    {
        private static readonly Dictionary<Type, List<Pair<SignalSystemId, Action<Signal>>>> signalCallbacks =
            new Dictionary<Type, List<Pair<SignalSystemId, Action<Signal>>>>();

        public static void Subscribe<T>(Action<T> callback) where T : Signal
        {
            void SignalWrapper(Signal signal) => 
                callback?.Invoke((T)signal);
            
            var signalType = typeof(T);
            if (!signalCallbacks.TryGetValue(signalType, out var callbacks))
            {
                callbacks = new List<Pair<SignalSystemId, Action<Signal>>>();
                signalCallbacks.Add(signalType, callbacks);
            }

            callbacks.Add(new Pair<SignalSystemId, Action<Signal>>(callback.Method.MethodHandle, SignalWrapper));
        }

        public static void Unsubscribe<T>(Action<T> callback) where T : Signal
        {
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

        public static void Notify<T>(T signal) where T : Signal
        {
            var signalType = signal.GetType();
            if (!signalCallbacks.TryGetValue(signalType, out var callbacks))
                return;
            
            foreach (var callback in callbacks)
                callback?.Second.Invoke(signal);
        }
        
        public static async UniTask NotifyAsync<T>(T signal) where T : Signal
        {
            var signalType = signal.GetType();
            if (!signalCallbacks.TryGetValue(signalType, out var callbacks))
                return;
            
            foreach (var callback in callbacks)
            {
                var asyncResult = callback?.Second.BeginInvoke(signal, null, null);
                await UniTask.WaitUntil(() => asyncResult.IsCompleted);
            }
        }
    } 
}