using System;

namespace DamnLibrary.Managements.Signals
{
    public abstract class Internal_Signal { }
    
    public abstract class Signal<T> : Internal_Signal where T : Internal_Signal
    {
        public static void Subscribe(Action<T> callback) => SignalSystem.Subscribe(callback);
        public static void Unsubscribe(Action<T> callback) => SignalSystem.Unsubscribe(callback);
    }
}
