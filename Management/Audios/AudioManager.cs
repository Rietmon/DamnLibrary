#if UNITY_5_3_OR_NEWER 
using System;
using System.Threading.Tasks;
using DamnLibrary.Behaviours;
using DamnLibrary.Debugging;
using DamnLibrary.Extensions;
using DamnLibrary.Management.Pools;
using UnityEngine;

namespace DamnLibrary.Management.Audios
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        private static PoolManager<AudioSource> poolManager;
        
        [SerializeField] private int audioSourcesPullCount = 16;

        public static void Initialize()
        {
            if (Instance == null)
            {
                UniversalDebugger.LogWarning($"[{nameof(AudioManager)}] ({nameof(Initialize)}) Creating instance automatically!");
                var audioManager = new GameObject("AudioManager").AddComponent<AudioManager>();
                DontDestroyOnLoad(audioManager);
            }
            var exampleObject = new GameObject("AudioSource").AddComponent<AudioSource>();
            poolManager = new PoolManager<AudioSource>(
                Instance.audioSourcesPullCount,
                exampleObject,
                (source) => Instantiate(source, Instance.transform));
            poolManager.EnableGameObjectOptimization();
        }

        public static AudioExecutor Play(AudioClip clip)
        {
            var poolCallback = (Action)EmptyCallback;
            var audioSource = poolManager.GetObject(ref poolCallback);

            ClearAudioSource(audioSource);

            audioSource.clip = clip;
            audioSource.Play();

            var needToStop = false;

            async void StopAudioSourceCallback()
            {
                await Task.WhenAny(TaskUtilities.WaitUntil(() => !audioSource.isPlaying),
                    TaskUtilities.WaitUntil(() => needToStop));
                audioSource.Stop();
                poolCallback?.Invoke();
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
            new(
                () => audioSource.isPlaying,
                stopCallback,
                (volume) => audioSource.volume = volume,
                () => audioSource.volume);

        private void OnDestroy()
        {
            poolManager?.Dispose();
        }

        private static void EmptyCallback()
        {
        }

#if UNITY_EDITOR
        private void Reset()
        {
            audioSourcesPullCount = 16;
        }
#endif
    }
}
#endif
