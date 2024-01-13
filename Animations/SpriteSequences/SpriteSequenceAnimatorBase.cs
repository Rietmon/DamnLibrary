using DamnLibrary.Animations.SpriteSequences;
using DamnLibrary.Behaviours;
using DamnLibrary.Utilities.Extensions;
using UnityEngine;

namespace DamnLibrary.Animations
{
    public abstract class BaseSpriteSequenceAnimator : DamnBehaviour
    {
        public bool IsPlaying { get; set; }
        public SpriteSequenceAnimation CurrentAnimation { get; private set; }
        public int CurrentFrame { get; private set; }
        
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
            if (animation == null)
            {
                Debug.LogError(
                    $"[{nameof(SpriteSequenceAnimator2D)}] ({nameof(DoTransition)} Animation with key {animationKey} not found!");
                return;
            }
            CurrentAnimation = animation;
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
            
            CurrentAnimation = animations[animationIndex];
            CurrentFrame = 0;
        }

        private void LateUpdate()
        {
            if (!IsPlaying)
                return;
            
            selfTime += Time.deltaTime;
            TrySetNextFrame();
            Sprite = CurrentAnimation.Sprites[CurrentFrame];
        }

        private void TrySetNextFrame()
        {
            var frameIndex = (int)(selfTime / CurrentAnimation.FrameDuration);
            switch (CurrentAnimation.AnimationType)
            {
                case SpriteSequenceAnimationType.OneFrame:
                    frameIndex %= CurrentAnimation.Sprites.Length;
                    break;
                case SpriteSequenceAnimationType.ForwardAndRepeat:
                    frameIndex %= CurrentAnimation.Sprites.Length;
                    break;
                case SpriteSequenceAnimationType.ForwardAndBackward:
                    frameIndex %= CurrentAnimation.Sprites.Length * 2;
                    if (frameIndex >= CurrentAnimation.Sprites.Length)
                        frameIndex = CurrentAnimation.Sprites.Length - (frameIndex - CurrentAnimation.Sprites.Length) - 1;
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