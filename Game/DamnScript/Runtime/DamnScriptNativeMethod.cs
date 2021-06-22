using System;
using Cysharp.Threading.Tasks;

namespace Rietmon.DS
{
    public class DamnScriptNativeMethod
    {
        public bool IsAsync => asyncFunction != null;
    
        private readonly Func<DamnScriptCode, string[], UniTask<bool>> asyncFunction;
    
        private readonly Func<DamnScriptCode, string[], bool> function;

        public DamnScriptNativeMethod(Func<DamnScriptCode, string[], UniTask<bool>> function) => asyncFunction = function;
    
        public DamnScriptNativeMethod(Func<DamnScriptCode, string[], bool> function) => this.function = function;
    
        public bool Execute(DamnScriptCode gameObject, string[] codes) => function.Invoke(gameObject, codes);

        public async UniTask<bool> ExecuteAsync(DamnScriptCode gameObject, string[] codes) =>
            await asyncFunction.Invoke(gameObject, codes);

        public static implicit operator DamnScriptNativeMethod(Func<DamnScriptCode, string[], UniTask<bool>> function) =>
            new DamnScriptNativeMethod(function);
        public static implicit operator DamnScriptNativeMethod(Func<DamnScriptCode, string[], bool> function) =>
            new DamnScriptNativeMethod(function);
    } 
}