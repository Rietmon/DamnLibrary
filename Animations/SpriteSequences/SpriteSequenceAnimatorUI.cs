using UnityEngine;
using UnityEngine.UI;

namespace DamnLibrary.Animations
{
    public sealed class SpriteSequenceAnimatorUI : BaseSpriteSequenceAnimator
    {
        protected override Sprite Sprite
        {
            set => image.sprite = value;
        }
        
        [SerializeField] private Image image;
        
#if UNITY_EDITOR
        private void Reset()
        {
            image = GetComponent<Image>();
        }
#endif
    }
}