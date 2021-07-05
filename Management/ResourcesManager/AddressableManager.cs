#if ENABLE_ADDRESSABLES && UNITY_2020
using System.Threading.Tasks;
using Rietmon.Game;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Rietmon.Extensions;
using UnityEngine.U2D;

public static class AddressableManager
{
    private const string PathToDataWindows = "Assets/Data/Runtime/Prefabs/Windows/{0}.prefab";
    private const string PathToSpritesAtlases = "Assets/Data/Runtime/Atlases/{0}.spriteatlas";
    private const string PathToAudioWav = "Assets/Data/Runtime/Audio/{0}.wav";
    private const string PathToAudioMp3 = "Assets/Data/Runtime/Audio/{0}.mp3";
    private const string PathToAudioOgg = "Assets/Data/Runtime/Audio/{0}.ogg";

    public static async Task<Prefab<WindowBehaviour>> GetWindowPrefabAsync(string windowName) =>
        Internal_VerifyAsset(await Internal_GetGameObjectComponent<WindowBehaviour>(PathToDataWindows.Format(windowName)), windowName);
    
    public static async Task<SpriteAtlas> GetSpriteAtlas(string atlasPath) =>
        Internal_VerifyAsset(await Internal_GetAssetAsync<SpriteAtlas>(PathToSpritesAtlases.Format(atlasPath)), atlasPath);

    public static async Task<AudioClip> GetAudio(string audioName)
    {
        AudioClip clip;
        if ((clip = await Internal_GetAssetAsync<AudioClip>(PathToAudioWav.Format(audioName))) != null)
            return Internal_VerifyAsset(clip, audioName);
        if ((clip = await Internal_GetAssetAsync<AudioClip>(PathToAudioMp3.Format(audioName))) != null)
            return Internal_VerifyAsset(clip, audioName);
        if ((clip = await Internal_GetAssetAsync<AudioClip>(PathToAudioOgg.Format(audioName))) != null)
            return Internal_VerifyAsset(clip, audioName);
        
        Debug.LogError($"[{nameof(AddressableManager)}] ({nameof(GetAudio)}) Unable to find audio with name {audioName}. Check extension!");
        return null;
    }

    public static async Task<T> Internal_GetAssetAsync<T>(string assetName) where T : Object
    {
        var startLoadingFrame = Time.frameCount;
        var locations = await Addressables.LoadResourceLocationsAsync(assetName).Task;

        if (locations == null || locations.Count == 0)
        {
            Debug.LogError($"[{nameof(AddressableManager)}] ({nameof(Internal_GetAssetAsync)}) Unable to find the location with the name {assetName}");
            return null;
        }

        var result = await Addressables.LoadAssetAsync<T>(locations[0]).Task;
        Debug.Log($"[{nameof(AddressableManager)}] ({nameof(Internal_GetAssetAsync)}) Asset with the name {assetName} was loaded in {Time.frameCount - startLoadingFrame} frames.");
        return result;
    }

    public static async Task<T> Internal_GetGameObjectComponent<T>(string assetName) where T : Object =>
        (await Internal_GetAssetAsync<GameObject>(assetName)).GetComponent<T>();

    public static T Internal_VerifyAsset<T>(T asset, string assetName) where T : Object
    {
        if (asset) 
            return asset;
        
        Debug.LogError($"[{nameof(AddressableManager)}] ({nameof(Internal_VerifyAsset)}) Error at loading asset with the name {assetName}. Result equal a null.");
        return default;
    }
}
#endif