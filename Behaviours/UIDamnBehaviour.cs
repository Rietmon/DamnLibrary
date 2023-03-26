#if UNITY_5_3_OR_NEWER 
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DamnLibrary.Behaviours
{
    public abstract class UIDamnBehaviour : UIBehaviour
    {
        /// <summary>
        /// The Transform attached to this GameObject
        /// </summary>
        public new Transform transform
        {
            get
            {
                if (!_transform)
                    _transform = gameObject.transform;

                return _transform;
            }
        }
        
        /// <summary>
        /// The RectTransform attached to this GameObject
        /// </summary>
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

        /// <summary>
        /// Add component to this GameObject
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T : Component => gameObject.AddComponent<T>();

        /// <summary>
        /// Remove component from this GameObject. In this case will be removed first component of type T
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        public void RemoveComponent<T>() where T : Component => Destroy(GetComponent<T>());

        /// <summary>
        /// Remove component from this GameObject
        /// </summary>
        /// <param name="component">Reference to component</param>
        /// <typeparam name="T">Type of the component</typeparam>
        public void RemoveComponent<T>(T component) where T : Component => Destroy(component);

        /// <summary>
        /// Remove this component from this GameObject
        /// </summary>
        public void RemoveComponent() => RemoveComponent(this);

        /// <summary>
        /// Destroy this GameObject
        /// </summary>
        public void DestroyObject() => Destroy(gameObject);

        /// <summary>
        /// Destroy GameObject which have this component
        /// </summary>
        /// <param name="monoBehaviour">Reference to component</param>
        public void DestroyObject(Component component) => Destroy(component.gameObject);

        /// <summary>
        /// Set active GameObject
        /// </summary>
        /// <param name="state">State</param>
        public void SetObjectActive(bool state) => gameObject.SetActive(state);
    }
}
#endif