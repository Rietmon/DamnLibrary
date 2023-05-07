#if UNITY_5_3_OR_NEWER 
using System.Threading.Tasks;
using DamnLibrary.Management;
using DamnLibrary.Management.Animations;
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
        protected internal WindowContext BaseContext { get; set; }
        
        [field: SerializeField] public WindowAnimator Animator { get; set; }

        /// <summary>
        /// Will be called after creating object
        /// </summary>
        public virtual async Task OnOpen() { }
        
        public virtual async Task OnOpenAnimationOver() { }
        
        public virtual async Task OnShow() { }
        
        public virtual async Task OnShowAnimationOver() { }
        
        public virtual async Task OnHide() { }
        
        public virtual async Task OnHideAnimationOver() { }

        /// <summary>
        /// Will be called before destroying object
        /// </summary>
        public virtual async Task OnClose() { }
        
        public virtual async Task OnCloseAnimationOver() { }

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
        public async void CloseAsync() => await WindowsManager.CloseAsync(this);
    }
    
    public abstract class WindowBehaviour<TContext> : WindowBehaviour where TContext : WindowContext
    {
        /// <summary>
        /// Casted context of this window
        /// </summary>
        public TContext Context => context ??= BaseContext.To<TContext>();

        private TContext context;
    }
}
#endif