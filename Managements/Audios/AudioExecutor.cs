#if UNITY_5_3_OR_NEWER 
using System;

namespace DamnLibrary.Management.Audios
{
    public class AudioExecutor
    {
        public bool IsPlaying => isPlayingMethod.Invoke();

        public float Volume
        {
            get => getVolumeMethod.Invoke();
            set => setVolumeMethod?.Invoke(value);
        }

        private readonly Func<bool> isPlayingMethod;

        private readonly Action stopMethod;

        private readonly Action<float> setVolumeMethod;

        private readonly Func<float> getVolumeMethod;

        public AudioExecutor(Func<bool> isPlaying, Action stop, Action<float> setVolume, Func<float> getVolume)
        {
            isPlayingMethod = isPlaying;
            stopMethod = stop;
            setVolumeMethod = setVolume;
            getVolumeMethod = getVolume;
        }

        public void Stop() => stopMethod?.Invoke();
    }
}
#endif
