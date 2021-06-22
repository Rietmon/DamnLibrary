using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteSequenceAsyncAnimator : ISpriteSequenceAnimator
{
    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            if (isPlaying == value)
                return;

            isPlaying = value;
            if (value)
                Handler(-1);
            else
                needToStopAnimation = true;
        }
    }

    public bool IsPaused { get; set; }

    public float FrameDuration { get; set; }
    
    public SpriteSequenceAnimationType AnimationType { get; set; }
    
    public SpriteRenderer SpriteRenderer { get; set; }

    public Sprite[] SpritesSequence { get; set; }
    
    public int CurrentFrameIndex { get; set; }

    private int currentAnimationDirection = 1;

    private bool needToContinueAnimation;

    private bool needToStopAnimation;

    private bool isPlaying;

    public SpriteSequenceAsyncAnimator(bool startPlaying = true)
    {
        IsPlaying = startPlaying;
    }

    public void ForceSkipFrame() => needToContinueAnimation = true;

    private async void Handler(float animationDuration)
    {
        if (animationDuration != -1)
        {
            UniTask.Run(async () =>
            {
                await UniTask.Delay((int)(animationDuration * 1000));
                IsPlaying = false;
            });
        }

        while (!needToStopAnimation)
        {
            needToContinueAnimation = false;
            
            if (SpriteRenderer == null || SpritesSequence == null || SpritesSequence.Length == 0 || IsPaused)
            {
                await UniTask.WhenAny(UniTask.Yield(PlayerLoopTiming.Update).ToUniTask(), UniTask.WaitUntil(() => needToStopAnimation));
                continue;
            }
            
            CurrentFrameIndex = TryGetNextIndex();
            SpriteRenderer.sprite = SpritesSequence[CurrentFrameIndex];
            if (FrameDuration > 0)
                await UniTask.WhenAny(UniTask.Delay((int)(FrameDuration * 1000)), UniTask.WaitUntil(() => needToStopAnimation || needToContinueAnimation));
            else
                await UniTask.WhenAny(UniTask.Yield(PlayerLoopTiming.Update).ToUniTask(), UniTask.WaitUntil(() => needToStopAnimation || needToContinueAnimation));
        }

        needToStopAnimation = false;
    }

    private int TryGetNextIndex()
    {
        switch (AnimationType)
        {
            case SpriteSequenceAnimationType.OneFrame:
                return CurrentFrameIndex;
            case SpriteSequenceAnimationType.ForwardAndRepeat:
            {
                CurrentFrameIndex++;
                if (CurrentFrameIndex >= SpritesSequence.Length)
                    CurrentFrameIndex = 0;
        
                return CurrentFrameIndex;
            }
            case SpriteSequenceAnimationType.ForwardAndBackward:
            {
                if (currentAnimationDirection == 1)
                {
                    CurrentFrameIndex++;
                    if (CurrentFrameIndex >= SpritesSequence.Length)
                    {
                        CurrentFrameIndex -= 2;
                        currentAnimationDirection = -1;
                    }
        
                    return CurrentFrameIndex;
                }
                else
                {
                    CurrentFrameIndex--;
                    if (CurrentFrameIndex <= -1)
                    {
                        CurrentFrameIndex += 2;
                        currentAnimationDirection = 1;
                    }
        
                    return CurrentFrameIndex;
                }
            }
            default:
                return 0;
        }
    }

    public void Dispose()
    {
        SpriteRenderer = null;
        needToStopAnimation = true;
        SpritesSequence = null;
    }
}
