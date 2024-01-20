using UnityEngine;

namespace DamnLibrary.Animations
{
    [CreateAssetMenu(fileName = "SpriteSequenceAnimation", menuName = "DamnLibrary/Animations/SpriteSequenceAnimation")]
    public class SpriteSequenceAnimation : ScriptableObject
    {
        [field: SerializeField] public Sprite[] Sprites { get; set; }

        public int Length => Sprites.Length;
        
        [field: SerializeField] public SpriteSequenceAnimationType AnimationType { get; set; } = 
            SpriteSequenceAnimationType.ForwardAndRepeat;

        [field: SerializeField] public float FrameDuration { get; set; } = 0.5f;
    }
}
