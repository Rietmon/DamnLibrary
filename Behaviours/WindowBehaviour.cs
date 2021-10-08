#if UNITY_5_3_OR_NEWER 
using System.Threading.Tasks;
using Rietmon.Behaviours;
using Rietmon.Management;
using UnityEngine;
#pragma warning disable 1998

namespace Rietmon.Behaviours
{
    public abstract class WindowBehaviour : UnityBehaviour
    {
        public string WindowName { get; set; }
        public object[] Arguments { get; set; }

        public virtual async Task OnOpenAsync() { }

        public virtual async Task OnCloseAsync() { }

        public async void CloseAsync() => await WindowsManager.CloseAsync(this);

        protected T GetArgument<T>(int index, T defaultValue = default)
        {
            if (Arguments.Length <= index)
                return defaultValue;

            if (Arguments[index] is T result)
                return result;

            Debug.LogError(
                $"[{nameof(WindowBehaviour)}] ({nameof(GetArgument)}) Unable to cast argument with index {index} to {typeof(T).Name}");
            return defaultValue;
        }
    }
}
#endif