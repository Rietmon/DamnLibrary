#if UNITY_5_3_OR_NEWER 
using System.Threading.Tasks;
using DamnLibrary.Management;
using DamnLibrary.Management.Animations;
using UnityEngine;

namespace DamnLibrary.Behaviours
{
    public abstract class WindowBehaviour : DamnBehaviour
    {
        /// <summary>
        /// Name of this window
        /// </summary>
        public string WindowName { get; set; }
        
        [field: SerializeField] public WindowAnimator Animator { get; set; }
        
        protected internal WindowContext BaseContext { get; set; }

        /// <summary>
        /// Will be called after creating object
        /// </summary>
        protected internal virtual Task OnOpen() => Task.CompletedTask;
        
        protected internal virtual Task OnOpenAnimationOver() => Task.CompletedTask;
        
        protected internal virtual Task OnShow() => Task.CompletedTask;
        
        protected internal virtual Task OnShowAnimationOver() => Task.CompletedTask;
        
        protected internal virtual Task OnHide() => Task.CompletedTask;
        
        protected internal virtual Task OnHideAnimationOver() => Task.CompletedTask;

        /// <summary>
        /// Will be called before destroying object
        /// </summary>
        protected internal virtual Task OnClose() => Task.CompletedTask;
        
        protected internal virtual Task OnCloseAnimationOver() => Task.CompletedTask;

        /// <summary>
        /// Show window. Can be overriden to add custom logic
        /// </summary>
        public virtual async Task ShowAsync() => await WindowsManager.ShowAsync(this);
        
        /// <summary>
        /// Hide window. Can be overriden to add custom logic
        /// </summary>
        public virtual async Task HideAsync() => await WindowsManager.HideAsync(this);

        /// <summary>
        /// Close async window
        /// </summary>
        public async Task CloseAsync() => await WindowsManager.CloseAsync(this);
    }
    
    public abstract class WindowBehaviour<TContext> : WindowBehaviour where TContext : WindowContext
    {
        /// <summary>
        /// Casted context of this window
        /// </summary>
        protected TContext Context => context ??= BaseContext as TContext;

        private TContext context;
    }
}
#endif