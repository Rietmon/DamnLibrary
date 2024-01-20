#if UNITY_5_3_OR_NEWER
using System;
using System.Threading.Tasks;
using DamnLibrary.Behaviours;
using DamnLibrary.Managements.Windows.Animations;
using UnityEngine;

namespace DamnLibrary.Managements.Windows
{
    public abstract class Internal_Window : DamnBehaviour
    {
        public string WindowName { get; set; }
        public bool IsClosing { get; internal set; }
        
        [field: SerializeField] public WindowAnimator Animator { get; set; }
        
        protected internal WindowContext BaseContext { get; set; }
        
        protected internal virtual Task OnOpen() => Task.CompletedTask;
        
        protected internal virtual Task OnOpenAnimationOver() => Task.CompletedTask;
        
        protected internal virtual Task OnShow() => Task.CompletedTask;
        
        protected internal virtual Task OnShowAnimationOver() => Task.CompletedTask;
        
        protected internal virtual Task OnHide() => Task.CompletedTask;
        
        protected internal virtual Task OnHideAnimationOver() => Task.CompletedTask;
        
        protected internal virtual Task OnClose() => Task.CompletedTask;
        
        protected internal virtual Task OnCloseAnimationOver() => Task.CompletedTask;
        
        public virtual Task ShowAsync() => WindowsManager.ShowAsync(this);
        
        public virtual Task HideAsync() => WindowsManager.HideAsync(this);
        
        public Task CloseAsync() => WindowsManager.CloseAsync(this);

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            Animator = GetComponent<WindowAnimator>();
        }
#endif
    }
    
    public abstract class Window<TContext, TWindow> : Internal_Window 
        where TContext : WindowContext 
        where TWindow : Internal_Window
    {
        protected TContext Context => context ??= BaseContext as TContext;

        private TContext context;
        
        public static Task<TWindow> OpenAsync(TContext context) => WindowsManager.OpenAsync<TWindow>(context);
    }
}
#endif