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

        protected T AddComponent<T>() where T : Component => gameObject.AddComponent<T>();
		
        protected bool HasComponent<T>() where T : Component => GetComponent<T>();
		
        protected int GetComponentsCount() => GetComponents<Component>()?.Length ?? 0;

        protected void RemoveComponent<T>() where T : Component => Destroy(GetComponent<T>());

        protected void RemoveThisComponent() => Destroy(this);

        protected void SetGameObjectActive(bool state) => gameObject.SetActive(state);

        protected void DestroyThisGameObject() => Destroy(gameObject);

        protected void DestroyGameObject(Component component) => Destroy(component.gameObject);
    }
}
#endif