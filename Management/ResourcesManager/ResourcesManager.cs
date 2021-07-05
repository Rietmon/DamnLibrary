#if UNITY_2020
using System.Threading.Tasks;
using Rietmon.Extensions;
using Rietmon.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rietmon.Management
{
    public static class ResourcesManager
    {
        private const string PathToDataWindows = "Prefabs/Windows/{0}";
        
        public static async Task<Prefab<WindowBehaviour>> GetWindowPrefabAsync(string windowName) =>
                Internal_VerifyAsset(await Internal_GetAssetAsync<WindowBehaviour>(PathToDataWindows.Format(windowName)), windowName);
        
        public static async Task<T> Internal_GetAssetAsync<T>(string assetName) where T : Object
        {
            var startLoadingFrame = Time.frameCount;
            var loadOperation = Resources.LoadAsync<T>(assetName);
            await TaskUtilities.WaitUntil(() => loadOperation.isDone);
            Debug.Log($"[{nameof(ResourcesManager)}] ({nameof(Internal_GetAssetAsync)}) Asset with the name {assetName} was loaded in {Time.frameCount - startLoadingFrame} frames.");
            return (T)loadOperation.asset;
        }
        
        public static T Internal_VerifyAsset<T>(T asset, string assetName) where T : Object
        {
            if (asset) 
                return asset;
        
            Debug.LogError($"[{nameof(ResourcesManager)}] ({nameof(Internal_VerifyAsset)}) Error at loading asset with the name {assetName}. Result equal a null.");
            return default;
        }
    }
}
#endif