#if UNITY_2020
using System;
using UnityEngine;

namespace Rietmon.Animations
{
    public interface ISpriteSequenceAnimator : IDisposable
    {
        bool IsPlaying { get; set; }
    
        bool IsPaused { get; set; }
    
        float FrameDuration { get; set; }
    
        SpriteSequenceAnimationType AnimationType { get; set; }
    
        SpriteRenderer SpriteRenderer { get; set; }
    
        Sprite[] SpritesSequence { get; set; }
    
        int CurrentFrameIndex { get; set; }

        void ForceSkipFrame();
    }
}
#endif
