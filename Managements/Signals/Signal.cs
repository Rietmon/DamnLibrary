using System;

namespace DamnLibrary.Managements.Signals
{
    public abstract class Signal { }
    
    public abstract class Signal<T> : Signal where T : Signal
    {
        public static void Subscribe(Action<T> callback) => SignalSystem.Subscribe(callback);
        public static void Unsubscribe(Action<T> callback) => SignalSystem.Unsubscribe(callback);
    }
}
