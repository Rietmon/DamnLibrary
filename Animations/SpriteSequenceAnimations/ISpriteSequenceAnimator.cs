#if UNITY_5_3_OR_NEWER
using System;
using UnityEngine;

namespace DamnLibrary.Animations.SpriteSequenceAnimations
{
    public interface ISpriteSequenceAnimator : IDisposable
    {
        /// <summary>
        /// Is animator playing now
        /// </summary>
        bool IsPlaying { get; set; }
    
        /// <summary>
        /// Is animator paused now
        /// </summary>
        bool IsPaused { get; set; }
    
        /// <summary>
        /// Duration of one frame in seconds
        /// </summary>
        float FrameDuration { get; set; }
    
        /// <summary>
        /// Animation sequence type
        /// </summary>
        SpriteSequenceAnimationType AnimationType { get; set; }
    
        /// <summary>
        /// Sprite renderer that will be animated
        /// </summary>
        SpriteRenderer SpriteRenderer { get; set; }
    
        /// <summary>
        /// Sprites for animation
        /// </summary>
        Sprite[] SpritesSequence { get; set; }
    
        /// <summary>
        /// Current sprite animation index
        /// </summary>
        int CurrentFrameIndex { get; set; }

        /// <summary>
        /// Force skip next or current frame
        /// </summary>
        void ForceSkipFrame();
    }
}
#endif
