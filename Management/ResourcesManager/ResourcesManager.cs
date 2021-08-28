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
            await GetAssetAsync<WindowBehaviour>(PathToResourcesWindows.Format(windowName));
        
        public static Prefab<WindowBehaviour> GetWindowPrefab(string windowName) => 
            GetAsset<WindowBehaviour>(PathToResourcesWindows.Format(windowName));
        
        public static async Task<Sprite> GetSpriteAsync(string spriteName) =>
            await GetAssetAsync<Sprite>(PathToResourcesTextures.Format(spriteName));
        
        public static Prefab<Sprite> GetSprite(string spriteName) =>
            GetAsset<Sprite>(PathToResourcesTextures.Format(spriteName));
        
        public static async Task<Texture2D> GetTextureAsync(string textureName) =>
            await GetAssetAsync<Texture2D>(PathToResourcesTextures.Format(textureName));
        
        public static Texture2D GetTexture(string textureName) =>
            GetAsset<Texture2D>(PathToResourcesTextures.Format(textureName));
        
        public static async Task<Prefab<T>> GetPrefabAsync<T>(string prefabName) where T : Object =>
            await GetAssetAsync<T>(PathToResourcesPrefabs.Format(prefabName));
        
        public static Prefab<T> GetPrefab<T>(string prefabName) where T : Object =>
            GetAsset<T>(PathToResourcesPrefabs.Format(prefabName));
        
        public static async Task<AudioClip> GetAudioAsync(string audioName) =>
            await GetAssetAsync<AudioClip>(PathToResourcesAudio.Format(audioName));
        
        public static AudioClip GetAudio(string audioName) =>
            GetAsset<AudioClip>(PathToResourcesAudio.Format(audioName));

        public static async Task<T> GetAssetAsync<T>(string assetPath) where T : Object
        {
            var startLoadingFrame = Time.frameCount;
            var loadOperation = Resources.LoadAsync<T>(assetPath);
            await TaskUtilities.WaitUntil(() => loadOperation.isDone);
            Debug.Log($"[{nameof(ResourcesManager)}] ({nameof(GetAssetAsync)}) Asset on the path {assetPath} was loaded in {Time.frameCount - startLoadingFrame} frames.");
            return VerifyAsset((T)loadOperation.asset, assetPath);
        }
        
        public static T GetAsset<T>(string assetPath) where T : Object
        {
            var asset = Resources.Load<T>(assetPath);
            Debug.Log($"[{nameof(ResourcesManager)}] ({nameof(GetAssetAsync)}) Asset on the path {assetPath} was loaded.");
            return VerifyAsset(asset, assetPath);
        }

        public static T[] GetAllAssets<T>(string path) where T : Object
        {
            var assets = Resources.LoadAll<T>(path);
            Debug.Log($"[{nameof(ResourcesManager)}] ({nameof(GetAllAssets)}) Assets on the path {path} was loaded.");
            return assets;
        }
        
        private static T VerifyAsset<T>(T asset, string assetPath) where T : Object
        {
            if (asset) 
                return asset;
        
            Debug.LogError($"[{nameof(ResourcesManager)}] ({nameof(VerifyAsset)}) Error at loading asset on the path {assetPath}. Result equal a null.");
            return default;
        }
    }
}
#endif