#if UNITY_5_3_OR_NEWER && ENABLE_DAMN_SCRIPT
using UnityEngine;

namespace DamnLibrary.DamnScript
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