using UnityEngine;
using UnityEngine.UI;

namespace Rietmon.Game
{
    public class ColorWrapper
    {
        public Color color
        {
            get
            {
                switch (wrapperMode)
                {
                    case 1: return image.color;
                    case 2: return text.color;
                }
            
                Debug.LogError($"Error in the wrapper! Mode was {wrapperMode}");
                return Color.black;
            }
            set
            {
                switch (wrapperMode)
                {
                    case 1: image.color = value; break;
                    case 2: text.color = value; break;
                }   
            }
        }

        private readonly int wrapperMode;

        private readonly Image image;

        private readonly Text text;
        
        public ColorWrapper(Image image)
        {
            this.image = image;
            wrapperMode = 1;
        }

        public ColorWrapper(Text text)
        {
            this.text = text;
            wrapperMode = 2;
        }

        public static implicit operator ColorWrapper(Image image) => new ColorWrapper(image);
        public static implicit operator ColorWrapper(Text text) => new ColorWrapper(text);

        public static implicit operator Image(ColorWrapper wrapper) => wrapper.image;
        public static implicit operator Text(ColorWrapper wrapper) => wrapper.text;
    }
}
