#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT && UNITY_5_3_OR_NEWER
using DamnLibrary.Behaviours;
using DamnLibrary.DamnScript.UnityAssets;
using UnityEngine;

namespace DamnLibrary.DamnScript.Runtime
{
    public class DefaultScriptExecutor : DamnBehaviour
    {
        /// <summary>
        /// Executor of the script
        /// </summary>
        public ScriptExecutor Executor { get; private set; }

        [SerializeField] private DamnScriptAsset script;

        private void Start()
        {
            Executor = new ScriptExecutor();
            Executor.CreateScriptFromCode(script.name, script.Content);
            Executor.StartRegion();
        }
    }
#endif
}