#if UNITY_2020 && ENABLE_DAMN_SCRIPT
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
using UnityEngine;

namespace Rietmon.DS
{
    public class DamnScriptCode
    {
        public GameObject Owner => Script.Owner;
        public DamnScript Script => Parent.Parent;
        public DamnScriptRegion Parent { get; }
    
        private readonly string[] codes;

        public DamnScriptCode(string[] codes, DamnScriptRegion parent)
        {
            Parent = parent;
            this.codes = codes;
        }

#if ENABLE_UNI_TASK
        public async UniTask<bool> ExecuteAsync() => await DamnScriptEngine.ExecuteAsync(this, codes);
#else
        public async Task<bool> ExecuteAsync() => await DamnScriptEngine.ExecuteAsync(this, codes);
#endif
    
        public bool Execute() => DamnScriptEngine.Execute(this, codes);
    }
  
}
#endif