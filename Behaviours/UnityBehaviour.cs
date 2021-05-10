using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rietmon.Behaviours
{
    public abstract class UnityBehaviour : MonoBehaviour
    {
        public static List<UnityBehaviour> Behaviours => new List<UnityBehaviour>(Object.FindObjectsOfType<UnityBehaviour>());
        
        private static readonly List<UnityBehaviour> behaviours = new List<UnityBehaviour>();
        
        public new Transform transform
        {
            get
            {
                if (!_transform)
                    _transform = gameObject.transform;

                return _transform;
            }
        }

        private readonly Dictionary<Type, Component> pullComponents = new Dictionary<Type, Component>();

        private Transform _transform;

        public UnityBehaviour()
        {
            //behaviours.Add(this);
        }

        public virtual void OnAfterAllSerialize() { }
        
        public virtual void OnAfterAllDeserialize() { }

        public void AddComponent<T>() where T : Component => gameObject.AddComponent<T>();

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

        protected virtual void OnDestroy()
        {
            //behaviours.Remove(this);
        }
    }
}
