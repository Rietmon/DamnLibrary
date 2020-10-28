using UnityEngine;

namespace Rietmon.Game
{
    public class TransformWrapper
    {
        public Vector3 position
        {
            get => isUsingLocal ? transform.localPosition : transform.position;
            set
            {
                if (isUsingLocal)
                    transform.localPosition = value;
                else
                    transform.position = value;
            }
        }

        public Vector3 rotation
        {
            get => isUsingLocal ? transform.localRotation.eulerAngles : transform.rotation.eulerAngles;
            set
            {
                if (isUsingLocal)
                    transform.localRotation = Quaternion.Euler(value);
                else
                    transform.rotation = Quaternion.Euler(value);
            }
        }

        public Vector3 scale
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }

        public bool isUsingLocal;

        private Transform transform;

        public TransformWrapper(Transform transform)
        {
            this.transform = transform;
        }
    
        public static implicit operator TransformWrapper(Transform transform) => new TransformWrapper(transform);

        public static implicit operator Transform(TransformWrapper wrapper) => wrapper.transform;
    }
}
