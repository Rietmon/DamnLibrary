using System;
using UnityEngine;

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
