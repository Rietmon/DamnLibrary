using System;
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Rietmon.DS
{
    public class DamnScriptNativeMethod
    {
        public bool IsAsync => asyncFunction != null;
    
#if ENABLE_UNI_TASK
        private readonly Func<DamnScriptCode, string[], UniTask<bool>> asyncFunction;
#else
        private readonly Func<DamnScriptCode, string[], Task<bool>> asyncFunction;
#endif
    
        private readonly Func<DamnScriptCode, string[], bool> function;

#if ENABLE_UNI_TASK
        public DamnScriptNativeMethod(Func<DamnScriptCode, string[], UniTask<bool>> function) => asyncFunction = function;
#else
        public DamnScriptNativeMethod(Func<DamnScriptCode, string[], Task<bool>> function) => asyncFunction = function;
#endif
    
        public DamnScriptNativeMethod(Func<DamnScriptCode, string[], bool> function) => this.function = function;
    
        public bool Execute(DamnScriptCode gameObject, string[] codes) => function.Invoke(gameObject, codes);

#if ENABLE_UNI_TASK
        public async UniTask<bool> ExecuteAsync(DamnScriptCode gameObject, string[] codes) =>
            await asyncFunction.Invoke(gameObject, codes);
#else
            public async Task<bool> ExecuteAsync(DamnScriptCode gameObject, string[] codes) =>
                    await asyncFunction.Invoke(gameObject, codes);
#endif

#if ENABLE_UNI_TASK
        public static implicit operator DamnScriptNativeMethod(Func<DamnScriptCode, string[], UniTask<bool>> function) =>
            new DamnScriptNativeMethod(function);
#else
            public static implicit operator DamnScriptNativeMethod(Func<DamnScriptCode, string[], Task<bool>> function) =>
                    new DamnScriptNativeMethod(function);
#endif
        public static implicit operator DamnScriptNativeMethod(Func<DamnScriptCode, string[], bool> function) =>
            new DamnScriptNativeMethod(function);
    } 
}