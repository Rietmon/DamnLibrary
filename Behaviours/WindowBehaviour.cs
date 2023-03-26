#if UNITY_5_3_OR_NEWER 
using System.Threading.Tasks;
using DamnLibrary.Management;
using DamnLibrary.Behaviours;
using DamnLibrary.Debugging;
using UnityEngine;
#pragma warning disable 1998

namespace DamnLibrary.Behaviours
{
    public abstract class WindowBehaviour : DamnBehaviour
    {
        /// <summary>
        /// Name of this window
        /// </summary>
        public string WindowName { get; set; }
        
        /// <summary>
        /// Non-casted context of this window. Use WindowContext property to get casted context
        /// </summary>
        public WindowContext BaseWindowContext { get; set; }
        
        /// <summary>
        /// Will be called after creating object
        /// </summary>
        public virtual async Task OnOpenAsync() { }

        /// <summary>
        /// Will be called before destroying object
        /// </summary>
        public virtual async Task OnCloseAsync() { }

        /// <summary>
        /// Show window. Can be overriden to add custom logic
        /// </summary>
        public virtual void Show() => gameObject.SetActive(true);
        
        /// <summary>
        /// Hide window. Can be overriden to add custom logic
        /// </summary>
        public virtual void Hide() => gameObject.SetActive(false);

        /// <summary>
        /// Close window
        /// </summary>
        public void Close() => WindowsManager.Close(this);

        /// <summary>
        /// Close async window
        /// </summary>
        public async void CloseAsync() => await WindowsManager.CloseAsync(this);
    }
    
    public abstract class WindowBehaviour<TContext> : WindowBehaviour where TContext : WindowContext
    {
        /// <summary>
        /// Casted context of this window
        /// </summary>
        public TContext WindowContext => windowContext ??= BaseWindowContext.To<TContext>();

        private TContext windowContext;
    }
}
#endif