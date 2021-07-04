#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
using Rietmon.Behaviours;
using UnityEngine;

public abstract class WindowBehaviour : UnityBehaviour
{
    public string WindowName { get; set; }
    public object[] Arguments { get; set; }
    
#if ENABLE_UNI_TASK
    public virtual async UniTask OnOpenAsync() { }
#else
    public virtual async Task OnOpenAsync() { }
#endif
    
#if ENABLE_UNI_TASK
    public virtual async UniTask OnCloseAsync() { }
#else
    public virtual async Task OnCloseAsync() { }
#endif

    public async void CloseAsync() => await WindowsManager.CloseAsync(this);

    protected T GetArgument<T>(int index, T defaultValue = default)
    {
        if (Arguments.Length <= index)
            return defaultValue;

        if (Arguments[index] is T result)
            return result;
        
        Debug.LogError($"[{nameof(WindowBehaviour)}] ({nameof(GetArgument)}) Unable to cast argument with index {index} to {typeof(T).Name}");
        return defaultValue;
    }
}
