using Cysharp.Threading.Tasks;
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

        public async UniTask<bool> ExecuteAsync() => await DamnScriptEngine.ExecuteAsync(this, codes);
    
        public bool Execute() => DamnScriptEngine.Execute(this, codes);
    }
  
}