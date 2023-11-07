#if UNITY_5_3_OR_NEWER
using System;
using DamnLibrary.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DamnLibrary.Games.Prefab
{
    [Serializable]
    public class Prefab<T> where T : Object
    {
        public T PrefabObject => prefabObject;

        public Transform DefaultParent
        {
            get => defaultParent;
            set => defaultParent = value;
        }

        private Transform PrefabTransform => prefabObject.GetTransform();

        [SerializeField] private T prefabObject;

        [SerializeField] private Transform defaultParent;

        public Prefab(T component)
        {
            prefabObject = component;
        }

        /// <summary>
        /// Simple instantiate for non GameObject or Component types
        /// </summary>
        public T Instantiate() => Object.Instantiate(prefabObject, DefaultParent);
        
        /// <summary>
        /// Simple instantiate for non GameObject or Component types
        /// </summary>
        public T Instantiate(Vector3 position)
        {
            var result = Instantiate();
            result.GetTransform().position = position;
            return result;
        }

        /// <summary>
        /// Simple instantiate for non GameObject or Component types
        /// </summary>
        public T Instantiate(Transform transform) => Object.Instantiate(prefabObject, transform);

        public static implicit operator Prefab<T>(T component) => new(component);

        public static implicit operator bool(Prefab<T> prefab) => prefab != null;

        public static implicit operator T(Prefab<T> prefab) => prefab.PrefabObject;
    }
}
#endif