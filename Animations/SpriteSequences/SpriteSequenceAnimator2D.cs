#if UNITY_5_3_OR_NEWER
using UnityEngine;
using UnityEngine.UI;

namespace DamnLibrary.Animations
{
    public sealed class SpriteSequenceAnimator2D : BaseSpriteSequenceAnimator
    {
        protected override Sprite Sprite
        {
            set => spriteRenderer.sprite = value;
        }
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        
#if UNITY_EDITOR
        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
#endif
    }
}
#endif