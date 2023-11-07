#if UNITY_5_3_OR_NEWER && ENABLE_DAMN_SCRIPT
using UnityEngine;

namespace DamnLibrary.DamnScript.UnityAssets
{
    public class DamnScriptAsset : ScriptableObject
    {
        /// <summary>
        /// Script content
        /// </summary>
        public string Content
        {
            get => content;
            internal set => content = value;
        }

        [SerializeField] private string content;
    }
}
#endif