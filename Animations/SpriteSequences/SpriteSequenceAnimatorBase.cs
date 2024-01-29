using DamnLibrary.Behaviours;
using DamnLibrary.Utilities.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace DamnLibrary.Animations
{
    public abstract class BaseSpriteSequenceAnimator : DamnBehaviour
    {
        public bool IsPlaying { get; set; }
        public SpriteSequenceAnimation CurrentAnimation { get; private set; }
        public int CurrentFrame { get; private set; }
        public bool IsAnimationFinished => CurrentAnimation.AnimationType == SpriteSequenceAnimationType.OnlyForward 
                                           && CurrentFrame == CurrentAnimation.Length - 1;
        
        protected abstract Sprite Sprite { set; }
        
        [SerializeField] private SpriteSequenceAnimation[] animations;
        [SerializeField] private string defaultAnimationKey;
        [SerializeField] private bool playOnStart = true;

        private float selfTime;

        private void Start()
        {
            if (!defaultAnimationKey.IsNullOrEmpty())
                DoTransition(defaultAnimationKey);
            else
                DoTransition(0);
            IsPlaying = playOnStart;
        }
        
        public void DoTransition(string animationKey)
        {
            var animation = animations.FindOrDefault((data) => data.name == animationKey);
            if (animation is null)
            {
                Debug.LogError(
                    $"[{nameof(SpriteSequenceAnimator2D)}] ({nameof(DoTransition)} Animation with key {animationKey} not found!");
                return;
            }

            IsPlaying = true;
            CurrentAnimation = animation;
            selfTime = 0;
            CurrentFrame = 0;
        }
        
        public void DoTransition(int animationIndex)
        {
            if (animationIndex < 0 || animationIndex >= animations.Length)
            {
                Debug.LogError(
                    $"[{nameof(SpriteSequenceAnimator2D)}] ({nameof(DoTransition)} Animation with index {animationIndex} not found!");
                return;
            }
            
            IsPlaying = true;
            CurrentAnimation = animations[animationIndex];
            selfTime = 0;
            CurrentFrame = 0;
        }

        public void SetFrame(string animationKey, int frame)
        {
            IsPlaying = false;
            var animation = animations.FindOrDefault((data) => data.name == animationKey);
            if (animation is null)
            {
                Debug.LogError(
                    $"[{nameof(SpriteSequenceAnimator2D)}] ({nameof(DoTransition)} Animation with key {animationKey} not found!");
                return;
            }
            
            frame = math.clamp(0, animation.Length - 1, frame);
            Sprite = animation.Sprites[frame];
        }

        private void LateUpdate()
        {
            if (!IsPlaying || CurrentAnimation is null)
                return;
            
            selfTime += Time.deltaTime;
            TrySetNextFrame();
            Sprite = CurrentAnimation.Sprites[CurrentFrame];
        }

        private void TrySetNextFrame()
        {
            var frameIndex = (int)(selfTime / CurrentAnimation.FrameDuration);
            var length = CurrentAnimation.Length;
            switch (CurrentAnimation.AnimationType)
            {
                case SpriteSequenceAnimationType.OneFrame:
                    frameIndex = 0;
                    break;
                case SpriteSequenceAnimationType.ForwardAndRepeat:
                    frameIndex = (frameIndex + 1) % length;
                    break;
                case SpriteSequenceAnimationType.ForwardAndBackward:
                    length *= 2;
                    frameIndex = (frameIndex + 1) % length;
                    if (frameIndex >= CurrentAnimation.Length)
                        frameIndex = length - frameIndex - 1;
                    break;
                case SpriteSequenceAnimationType.OnlyForward:
                    frameIndex++;
                    if (frameIndex >= CurrentAnimation.Length)
                        frameIndex = CurrentAnimation.Length - 1;
                    break;
                case SpriteSequenceAnimationType.None:
                    break;
                default:
                    Debug.LogError(
                        $"[{nameof(SpriteSequenceAnimator2D)}] ({nameof(TrySetNextFrame)}) Unknown animation type {CurrentAnimation.AnimationType}!");
                    CurrentFrame = 0;
                    return;
            }

            CurrentFrame = frameIndex;
        }
    }
}