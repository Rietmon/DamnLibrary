#if UNITY_5_3_OR_NEWER 
using System.Threading.Tasks;
using DamnLibrary.Management;
using DamnLibrary.Behaviours;
using UnityEngine;
#pragma warning disable 1998

namespace DamnLibrary.Behaviours
{
    public abstract class WindowBehaviour : UnityBehaviour
    {
        public string WindowName { get; set; }
        
        public object[] Arguments { get; set; }

        /// <summary>
        /// Will be called after creating object
        /// </summary>
        public virtual async Task OnOpenAsync() { }

        /// <summary>
        /// Will be called before destroying object
        /// </summary>
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