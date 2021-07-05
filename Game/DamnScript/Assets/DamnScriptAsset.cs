#if UNITY_2020 && ENABLE_DAMN_SCRIPT
using UnityEngine;

namespace Rietmon.DS
{
    public class DamnScriptAsset : ScriptableObject
    {
        public string Content
        {
            get => content;
            internal set => content = value;
        }

        [SerializeField] private string content;
    }
}
#endif