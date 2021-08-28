#if ENABLE_ADDRESSABLE && UNITY_2020
using System.Threading.Tasks;
using Rietmon.Behaviours;
using Rietmon.Game;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Rietmon.Extensions;
using UnityEngine.U2D;

namespace Rietmon.Management
{
    public static class AddressableManager
    {
        private const string PathToDataWindows = "Assets/Data/Runtime/Prefabs/Windows/{0}.prefab";
        private const string PathToSpritesAtlases = "Assets/Data/Runtime/Atlases/{0}.spriteatlas";
        private const string PathToAudioWav = "Assets/Data/Runtime/Audio/{0}.wav";
        private const string PathToAudioMp3 = "Assets/Data/Runtime/Audio/{0}.mp3";
        private const string PathToAudioOgg = "Assets/Data/Runtime/Audio/{0}.ogg";

        public static async Task<Prefab<WindowBehaviour>> GetWindowPrefabAsync(string windowName) =>
            VerifyAsset(
                await GetGameObjectComponent<WindowBehaviour>(PathToDataWindows.Format(windowName)),
                windowName);

        public static async Task<SpriteAtlas> GetSpriteAtlasAsync(string atlasPath) =>
            VerifyAsset(await GetAssetAsync<SpriteAtlas>(PathToSpritesAtlases.Format(atlasPath)),
                atlasPath);

        public static async Task<AudioClip> GetAudioAsync(string audioName)
        {
            AudioClip clip;
            if ((clip = await GetAssetAsync<AudioClip>(PathToAudioWav.Format(audioName))) != null)
                return VerifyAsset(clip, audioName);
            if ((clip = await GetAssetAsync<AudioClip>(PathToAudioMp3.Format(audioName))) != null)
                return VerifyAsset(clip, audioName);
            if ((clip = await GetAssetAsync<AudioClip>(PathToAudioOgg.Format(audioName))) != null)
                return VerifyAsset(clip, audioName);

            Debug.LogError(
                $"[{nameof(AddressableManager)}] ({nameof(GetAudioAsync)}) Unable to find audio with name {audioName}. Check extension!");
            return null;
        }

        public static async Task<T> GetAssetAsync<T>(string assetName) where T : Object
        {
            var startLoadingFrame = Time.frameCount;
            var locations = await Addressables.LoadResourceLocationsAsync(assetName).Task;

            if (locations == null || locations.Count == 0)
            {
                Debug.LogError(
                    $"[{nameof(AddressableManager)}] ({nameof(GetAssetAsync)}) Unable to find the location with the name {assetName}");
                return null;
            }

            var result = await Addressables.LoadAssetAsync<T>(locations[0]).Task;
            Debug.Log(
                $"[{nameof(AddressableManager)}] ({nameof(GetAssetAsync)}) Asset with the name {assetName} was loaded in {Time.frameCount - startLoadingFrame} frames.");
            return result;
        }

        private static async Task<T> GetGameObjectComponent<T>(string assetName) where T : Object =>
            (await GetAssetAsync<GameObject>(assetName)).GetComponent<T>();

        private static T VerifyAsset<T>(T asset, string assetName) where T : Object
        {
            if (asset)
                return asset;

            Debug.LogError(
                $"[{nameof(AddressableManager)}] ({nameof(VerifyAsset)}) Error at loading asset with the name {assetName}. Result equal a null.");
            return default;
        }
    }
}
#endif