#if UNITY_5_3_OR_NEWER 
using System.Threading.Tasks;
using DamnLibrary.Management;
using DamnLibrary.Behaviours;
using DamnLibrary.Debugging;
using UnityEngine;
#pragma warning disable 1998

namespace DamnLibrary.Behaviours
{
    public abstract class WindowBehaviour : UnityBehaviour
    {
        public string WindowName { get; set; }
        
        public WindowContext BaseWindowContext { get; set; }
        
        /// <summary>
        /// Will be called after creating object
        /// </summary>
        public virtual async Task OnOpenAsync() { }

        /// <summary>
        /// Will be called before destroying object
        /// </summary>
        public virtual async Task OnCloseAsync() { }

        public virtual void Show() => gameObject.SetActive(true);
        
        public virtual void Hide() => gameObject.SetActive(false);

        public void Close() => WindowsManager.Close(this);

        public async void CloseAsync() => await WindowsManager.CloseAsync(this);
    }
    
    public abstract class WindowBehaviour<TContext> : WindowBehaviour where TContext : WindowContext
    {
        public TContext WindowContext => windowContext ??= BaseWindowContext.To<TContext>();

        private TContext windowContext;
    }
}
#endif