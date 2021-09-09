#if UNITY_5_3_OR_NEWER 
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rietmon.Behaviours
{
    public abstract class UIUnityBehaviour : UIBehaviour
    {
        public static List<UIUnityBehaviour> Behaviours => new List<UIUnityBehaviour>(FindObjectsOfType<UIUnityBehaviour>());
        
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
        
        public bool WasAllDeserialized { get; set; }

        private readonly Dictionary<Type, Component> pullComponents = new Dictionary<Type, Component>();

        private Transform _transform;
        private RectTransform _rectTransform;

        public virtual void OnAfterAllSerialize() { }
        
        public virtual void OnAfterAllDeserialize() { }

        public T AddComponent<T>() where T : Component => gameObject.AddComponent<T>();

        public T GetComponentFromPull<T>() where T : Component
        {
            var type = typeof(T);
            if (pullComponents.TryGetValue(type, out var pullResult))
                return (T)pullResult;

            var result = GetComponent<T>();
            pullComponents.Add(type, result);

            return result;
        }

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