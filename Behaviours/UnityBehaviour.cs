#if UNITY_5_3_OR_NEWER 
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DamnLibrary.Behaviours
{
    public abstract class UnityBehaviour : MonoBehaviour
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
        
        private Transform _transform;

        public T AddComponent<T>() where T : Component => gameObject.AddComponent<T>();

        public void RemoveComponent<T>() where T : Component => Destroy(GetComponent<T>());

        public void RemoveComponent<T>(T component) where T : Component => Destroy(component);

        public void RemoveComponent() => RemoveComponent(this);

        public void DestroyObject() => Destroy(gameObject);

        public void DestroyObject(MonoBehaviour monoBehaviour) => Destroy(monoBehaviour.gameObject);

        public void SetObjectActive(bool state) => gameObject.SetActive(state);

        protected virtual void OnDestroy() { }
    }
}
#endif