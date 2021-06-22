using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Rietmon.Behaviours;
using Rietmon.DS;
using Rietmon.Extensions;
using UnityEngine;

[DamnScriptable]
public class AudioManager : SingletonBehaviour<AudioManager>
{
    private const byte AudioSourcesPullCount = 16;

    private static PullManager<AudioSource> pullManager;

    public static async UniTask InitializeAsync()
    {
        var exampleObject = new GameObject("AudioSource").AddComponent<AudioSource>();
        pullManager = new PullManager<AudioSource>(
            AudioSourcesPullCount, 
            exampleObject, 
            (source) => Instantiate(source, Instance.transform));
        pullManager.EnableGameObjectOptimization();
    }

    public static AudioExecutor Play(AudioClip clip)
    {
        var pullCallback = (Action)EmptyCallback;
        var audioSource = pullManager.GetObject(ref pullCallback);

        ClearAudioSource(audioSource);

        audioSource.clip = clip;
        audioSource.Play();

        var needToStop = false;

        async void StopAudioSourceCallback()
        {
            await Task.WhenAny(UniTask.WaitUntil(() => !audioSource.isPlaying).AsTask(), UniTask.WaitUntil(() => needToStop).AsTask());
            audioSource.Stop();
            pullCallback?.Invoke();
        }
        
        StopAudioSourceCallback();
        return CreateExecutor(audioSource, () => needToStop = true);
    }

    private static void ClearAudioSource(AudioSource audioSource)
    {
        audioSource.transform.position = Vector3.zero;
        audioSource.volume = 1;
    }

    private static AudioExecutor CreateExecutor(AudioSource audioSource, Action stopCallback) =>
        new AudioExecutor(
            () => audioSource.isPlaying, 
            stopCallback, 
            (volume) => audioSource.volume = volume,
            () => audioSource.volume);

    protected override void OnDestroy()
    {
        pullManager?.Dispose();
    }

    private static void EmptyCallback() { }

    private static void RegisterDamnScriptMethods()
    {
        DamnScriptEngine.RegisterMethod("PlayAudio", async (code, arguments) =>
        {
            var audioName = arguments.GetArgument(0);
            
            return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
        });
    }
}
