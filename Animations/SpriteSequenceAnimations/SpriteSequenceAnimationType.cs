#if UNITY_5_3_OR_NEWER 
namespace DamnLibrary.Animations.SpriteSequenceAnimations
{
    public enum SpriteSequenceAnimationType : byte
    {
        /// <summary>
        /// Empty value
        /// </summary>
        None,
        /// <summary>
        /// Indexing example: 0-0-0-0...
        /// </summary>
        OneFrame,
        /// <summary>
        /// Indexing example 0-1-2-0-1-2...
        /// </summary>
        ForwardAndRepeat,
        /// <summary>
        /// Indexing example 0-1-2-1-0...
        /// </summary>
        ForwardAndBackward
    }
}
#endif