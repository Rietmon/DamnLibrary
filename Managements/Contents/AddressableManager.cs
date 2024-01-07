#if ENABLE_ADDRESSABLE && UNITY_5_3_OR_NEWER
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using DamnLibrary.Behaviours;
using DamnLibrary.Debugs;
using DamnLibrary.Games;
using DamnLibrary.Managements.Windows;
using DamnLibrary.Utilities.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace DamnLibrary.Managements.Contents
{
    public static class AddressableManager
    {
        private const string PathToDataWindows = "Assets/Data/Runtime/Prefabs/Windows/{0}.prefab";
        private const string PathToSpritesAtlases = "Assets/Data/Runtime/Atlases/{0}.spriteatlas";
        private const string PathToTextures = "Assets/Data/Runtime/Art/Textures/{0}";
        private const string PathToPrefabs = "Assets/Data/Runtime/Prefabs/{0}.prefab";
        private const string PathToAudio = "Assets/Data/Runtime/Audio/{0}";
        
        private static readonly Type componentType = typeof(Component);
        
        public static async Task<Prefab<T>> GetWindowPrefabAsync<T>(string windowName) where T : Internal_Window =>
            await GetGameObjectOrComponentAsync<T>(PathToDataWindows.Format(windowName));
        public static Prefab<T> GetWindowPrefab<T>(string windowName) where T : Internal_Window =>
            GetGameObjectOrComponent<T>(PathToDataWindows.Format(windowName));

        public static Task<SpriteAtlas> GetSpriteAtlasAsync(string atlasPath) =>
            GetAssetAsync<SpriteAtlas>(PathToSpritesAtlases.Format(atlasPath));
        public static SpriteAtlas GetSpriteAtlas(string atlasPath) =>
            GetAsset<SpriteAtlas>(PathToSpritesAtlases.Format(atlasPath));
        
        public static Task<Sprite> GetSpriteDirectlyAsync(string spritePath) =>
            GetAssetAsync<Sprite>(PathToTextures.Format(spritePath));
        public static Sprite GetSpriteDirectly(string spritePath) => 
            GetAsset<Sprite>(PathToTextures.Format(spritePath));

        public static Task<Sprite> GetSpriteAsync(string spritePath)
        {
            var spriteName = Path.GetFileNameWithoutExtension(spritePath);
            return GetAssetAsync<Sprite>(PathToTextures.Format($"{spritePath}[{spriteName}]"));
        }
        public static Sprite GetSprite(string spritePath)
        {
            var spriteName = Path.GetFileNameWithoutExtension(spritePath);
            return GetAsset<Sprite>(PathToTextures.Format($"{spritePath}[{spriteName}]"));
        }
        
        public static Task<Texture2D> GetTextureAsync(string texturePath) =>
            GetAssetAsync<Texture2D>(PathToTextures.Format(texturePath));
        public static Texture2D GetTexture(string texturePath) =>
            GetAsset<Texture2D>(PathToTextures.Format(texturePath));
        
        public static async Task<Prefab<T>> GetPrefabAsync<T>(string prefabPath) where T : Object =>
            await GetGameObjectOrComponentAsync<T>(PathToPrefabs.Format(prefabPath));
        public static Prefab<T> GetPrefab<T>(string prefabPath) where T : Object =>
            GetGameObjectOrComponent<T>(PathToPrefabs.Format(prefabPath));

        public static Task<AudioClip> GetAudioAsync(string audioName) =>
            GetAssetAsync<AudioClip>(PathToAudio.Format(audioName));
        public static AudioClip GetAudio(string audioName) => 
            GetAsset<AudioClip>(PathToAudio.Format(audioName));
        
        public static async Task<T> GetAssetAsync<T>(string assetName) where T : Object
        {
            var startLoadingTick = Stopwatch.GetTimestamp();
            var result = await Addressables.LoadAssetAsync<T>(assetName).Task;
            var ticksCount = Stopwatch.GetTimestamp() - startLoadingTick;
            UniversalDebugger.Log(
                $"[{nameof(AddressableManager)}] ({nameof(GetAssetAsync)}) " + 
                $"Asset with the name {assetName} was loaded in {ticksCount.ToString()} ticks " +
                $"(about {(ticksCount / (double)Stopwatch.Frequency).ToString(CultureInfo.InvariantCulture)} ms).");
            return VerifyAsset(result, assetName);
        }
        
        public static T GetAsset<T>(string assetName) where T : Object
        {
            var result = Addressables.LoadAssetAsync<T>(assetName).WaitForCompletion();
            UniversalDebugger.Log($"[{nameof(AddressableManager)}] ({nameof(GetAssetAsync)}) Asset with the name {assetName} was loaded.");
            return VerifyAsset(result, assetName);
        }

        private static async Task<T> GetGameObjectOrComponentAsync<T>(string assetName) where T : Object
        {
            var asset = await GetAssetAsync<GameObject>(assetName);
            if (componentType.IsAssignableFrom(typeof(T)))
                return asset.GetComponent<T>();
            return (T)(Object)asset;
        }
        
        private static T GetGameObjectOrComponent<T>(string assetName) where T : Object
        {
            var asset = GetAsset<GameObject>(assetName);
            if (componentType.IsAssignableFrom(typeof(T)))
                return asset.GetComponent<T>();
            return (T)(Object)asset;
        }

        private static T VerifyAsset<T>(T asset, string assetName) where T : Object
        {
            if (asset)
                return asset;

            UniversalDebugger.LogError(
                $"[{nameof(AddressableManager)}] ({nameof(VerifyAsset)}) Error at loading asset with the name {assetName}. Result equal a null.");
            return default;
        }
    }
}
#endif