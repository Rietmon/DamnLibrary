#if ENABLE_ADDRESSABLE && UNITY_5_3_OR_NEWER 
using System.Threading.Tasks;
using DamnLibrary.Behaviours;
using DamnLibrary.Game;
using UnityEngine;
using UnityEngine.AddressableAssets;
using DamnLibrary.Debugging;
using DamnLibrary.Extensions;
using UnityEngine.U2D;

namespace DamnLibrary.Management
{
    public static partial class AddressableManager
    {
        private const string PathToDataWindows = "Assets/Data/Runtime/Prefabs/Windows/{0}.prefab";
        private const string PathToSpritesAtlases = "Assets/Data/Runtime/Atlases/{0}.spriteatlas";
        private const string PathToTextures = "Assets/Data/Runtime/Textures/{0}";
        private const string PathToPrefabs = "Assets/Data/Runtime/Prefabs/{0}.prefab";
        private const string PathToAudio = "Assets/Data/Runtime/Audio/{0}";
        
        public static async Task<Prefab<WindowBehaviour>> GetWindowPrefabAsync(string windowName) =>
            await GetGameObjectOrComponentAsync<WindowBehaviour>(PathToDataWindows.Format(windowName));
        public static Prefab<WindowBehaviour> GetWindowPrefab(string windowName) =>
            GetGameObjectOrComponent<WindowBehaviour>(PathToDataWindows.Format(windowName));

        public static async Task<SpriteAtlas> GetSpriteAtlasAsync(string atlasPath) =>
            await GetAssetAsync<SpriteAtlas>(PathToSpritesAtlases.Format(atlasPath));
        public static SpriteAtlas GetSpriteAtlas(string atlasPath) =>
            GetAsset<SpriteAtlas>(PathToSpritesAtlases.Format(atlasPath));
        
        public static async Task<Sprite> GetSpriteAsync(string spritePath) =>
            await GetAssetAsync<Sprite>(PathToTextures.Format(spritePath));
        public static Sprite GetSprite(string spritePath) =>
            GetAsset<Sprite>(PathToTextures.Format(spritePath));
        
        public static async Task<Texture2D> GetTextureAsync(string texturePath) =>
            await GetAssetAsync<Texture2D>(PathToTextures.Format(texturePath));
        public static Texture2D GetTexture(string texturePath) =>
            GetAsset<Texture2D>(PathToTextures.Format(texturePath));
        
        public static async Task<Prefab<T>> GetPrefabAsync<T>(string prefabPath) where T : Object =>
            await GetGameObjectOrComponentAsync<T>(PathToPrefabs.Format(prefabPath));
        public static Prefab<T> GetPrefab<T>(string prefabPath) where T : Object =>
            GetGameObjectOrComponent<T>(PathToPrefabs.Format(prefabPath));

        public static async Task<AudioClip> GetAudioAsync(string audioName) =>
            await GetAssetAsync<AudioClip>(PathToAudio.Format(audioName));
        public static AudioClip GetAudio(string audioName) => 
            GetAsset<AudioClip>(PathToAudio.Format(audioName));
        
        public static async Task<T> GetAssetAsync<T>(string assetName) where T : Object
        {
            var startLoadingFrame = Time.frameCount;
            var locations = await Addressables.LoadResourceLocationsAsync(assetName).Task;

            if (locations == null || locations.Count == 0)
            {
                UniversalDebugger.LogError(
                    $"[{nameof(AddressableManager)}] ({nameof(GetAssetAsync)}) Unable to find the location with the name {assetName}");
                return null;
            }

            var result = await Addressables.LoadAssetAsync<T>(locations[0]).Task;
            UniversalDebugger.Log(
                $"[{nameof(AddressableManager)}] ({nameof(GetAssetAsync)}) Asset with the name {assetName} was loaded in {Time.frameCount - startLoadingFrame} frames.");
            return VerifyAsset(result, assetName);
        }
        public static T GetAsset<T>(string assetName) where T : Object
        {
            var locations = Addressables.LoadResourceLocationsAsync(assetName).WaitForCompletion();

            if (locations == null || locations.Count == 0)
            {
                UniversalDebugger.LogError(
                    $"[{nameof(AddressableManager)}] ({nameof(GetAssetAsync)}) Unable to find the location with the name {assetName}");
                return null;
            }

            var result = Addressables.LoadAssetAsync<T>(locations[0]).WaitForCompletion();
            UniversalDebugger.Log($"[{nameof(AddressableManager)}] ({nameof(GetAssetAsync)}) Asset with the name {assetName} was loaded.");
            return VerifyAsset(result, assetName);
        }

        private static async Task<T> GetGameObjectOrComponentAsync<T>(string assetName) where T : Object
        {
            var asset = await GetAssetAsync<GameObject>(assetName);
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                return asset.GetComponent<T>();
            return (T)(Object)asset;
        }
        private static T GetGameObjectOrComponent<T>(string assetName) where T : Object
        {
            var asset = GetAsset<GameObject>(assetName);
            if (typeof(Component).IsAssignableFrom(typeof(T)))
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