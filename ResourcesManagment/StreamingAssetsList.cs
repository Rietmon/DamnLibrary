using UnityEngine;

namespace Rietmon.Common.ResourcesManagement
{
    [CreateAssetMenu(fileName = "StreamingAssetsList", menuName = "Game/Assets/StreamingAssetsList")]
    public class StreamingAssetsList : ScriptableObject
    {
        private static StreamingAssetsList instance;
        private static StreamingAssetsList Instance
        {
            get
            {
                if (!instance)
                    instance = UnityEngine.Resources.Load<StreamingAssetsList>("StreamingAssetsList");

                return instance;
            }
        }
    
        public static string[] ScriptsList => Instance.scriptsList;

        [SerializeField] private string[] scriptsList;
    }
}
