#if UNITY_5_3_OR_NEWER 
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DamnLibrary.Behaviours
{
    public abstract class UIUnityBehaviour : UIBehaviour
    {
        public new Transform transform
        {
            get
            {
                if (!_transform)
                    _transform = gameObject.transform;

                return _transform;
            }
        }
        
        public RectTransform rectTransform
        {
            get
            {
                if (!_rectTransform)
                    _rectTransform = (RectTransform)gameObject.transform;

                return _rectTransform;
            }
        }
        
        private Transform _transform;
        private RectTransform _rectTransform;

        public T AddComponent<T>() where T : Component => gameObject.AddComponent<T>();

        public void RemoveComponent<T>() where T : Component => Destroy(GetComponent<T>());

        public void RemoveComponent<T>(T component) where T : Component => Destroy(component);

        public void RemoveComponent() => RemoveComponent(this);

        /// <summary>
        /// Will destroy object-parent of this component
        /// </summary>
        public void DestroyObject() => Destroy(gameObject);

        public void DestroyObject(Component component) => Destroy(component.gameObject);

        /// <summary>
        /// Set GameObject active state
        /// </summary>
        public void SetObjectActive(bool state) => gameObject.SetActive(state);
    }
}
#endif