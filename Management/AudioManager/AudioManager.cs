#if UNITY_2020
using System;
using System.Threading.Tasks;
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
#endif
using Rietmon.Behaviours;
#if ENABLE_DAMN_SCRIPT
using Rietmon.DS;
#endif
using Rietmon.Extensions;
using UnityEngine;

namespace Rietmon.Management
{
#if ENABLE_DAMN_SCRIPT
    [DamnScriptable]
#endif
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        private static PullManager<AudioSource> pullManager;
        
        [SerializeField] private int audioSourcesPullCount = 16;

        public static void Initialize()
        {
            if (Instance == null)
            {
                Debug.LogWarning($"[{nameof(AudioManager)}] ({nameof(Initialize)}) Creating instance automatically!");
                var audioManager = new GameObject("AudioManager").AddComponent<AudioManager>();
                DontDestroyOnLoad(audioManager);
            }
            var exampleObject = new GameObject("AudioSource").AddComponent<AudioSource>();
            pullManager = new PullManager<AudioSource>(
                Instance.audioSourcesPullCount,
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
                await Task.WhenAny(TaskUtilities.WaitUntil(() => !audioSource.isPlaying),
                    TaskUtilities.WaitUntil(() => needToStop));
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

        private static void EmptyCallback()
        {
        }

#if ENABLE_DAMN_SCRIPT
        private static void RegisterDamnScriptMethods()
        {
            DamnScriptEngine.RegisterMethod("PlayAudio", async (code, arguments) =>
            {
                var audioName = arguments.GetArgument(0);

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
        }
#endif
        
#if UNITY_EDITOR
        private void Reset()
        {
            audioSourcesPullCount = 16;
        }
#endif
    }
}
#endif
