#if UNITY_2020
using System.Threading.Tasks;
using Rietmon.Behaviours;
using Rietmon.Extensions;
using Rietmon.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rietmon.Management
{
    public static class ResourcesManager
    {
        private const string PathToResourcesWindows = "Prefabs/Windows/{0}";
        private const string PathToResourcesTextures = "Textures/{0}";
        private const string PathToResourcesPrefabs = "Prefabs/{0}";
        private const string PathToResourcesAudio = "Audio/{0}";
        
        public static async Task<Prefab<WindowBehaviour>> GetWindowPrefabAsync(string windowName) =>
            await GetAsset<WindowBehaviour>(PathToResourcesWindows.Format(windowName), windowName);
        
        public static async Task<Sprite> GetSprite(string spriteName) =>
            await GetAsset<Sprite>(PathToResourcesTextures.Format(spriteName), spriteName);
        
        public static async Task<Texture2D> GetTexture(string textureName) =>
            await GetAsset<Texture2D>(PathToResourcesTextures.Format(textureName), textureName);
        
        public static async Task<Prefab<T>> GetPrefab<T>(string prefabName) where T : Object =>
            await GetAsset<T>(PathToResourcesPrefabs.Format(prefabName), prefabName);
        
        public static async Task<AudioClip> GetAudio(string audioName) =>
            await GetAsset<AudioClip>(PathToResourcesAudio.Format(audioName), audioName);

        public static async Task<T> GetAsset<T>(string assetName) where T : Object =>
            Internal_VerifyAsset(await Internal_GetAssetAsync<T>(assetName), assetName);

        public static T[] GetAllAssets<T>(string path) where T : Object => 
            Internal_GetAssets<T>(path);
        
        public static async Task<T> GetAsset<T>(string pathToAsset, string assetName) where T : Object =>
            Internal_VerifyAsset(await Internal_GetAssetAsync<T>(pathToAsset), assetName);
        
        private static async Task<T> Internal_GetAssetAsync<T>(string assetName) where T : Object
        {
            var startLoadingFrame = Time.frameCount;
            var loadOperation = Resources.LoadAsync<T>(assetName);
            await TaskUtilities.WaitUntil(() => loadOperation.isDone);
            Debug.Log($"[{nameof(ResourcesManager)}] ({nameof(Internal_GetAssetAsync)}) Asset with the name {assetName} was loaded in {Time.frameCount - startLoadingFrame} frames.");
            return (T)loadOperation.asset;
        }
        
        private static T[] Internal_GetAssets<T>(string path) where T : Object
        {
            var loadOperation = Resources.LoadAll<T>(path);
            Debug.Log($"[{nameof(ResourcesManager)}] ({nameof(Internal_GetAssets)}) Assets by path {path} was loaded.");
            return loadOperation;
        }
        
        private static T Internal_VerifyAsset<T>(T asset, string assetName) where T : Object
        {
            if (asset) 
                return asset;
        
            Debug.LogError($"[{nameof(ResourcesManager)}] ({nameof(Internal_VerifyAsset)}) Error at loading asset with the name {assetName}. Result equal a null.");
            return default;
        }
    }
}
#endif