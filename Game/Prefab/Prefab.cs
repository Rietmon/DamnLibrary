#if UNITY_5_3_OR_NEWER 
using System;
using DamnLibrary.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DamnLibrary.Game
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
        public T SimpleInstantiate() => Object.Instantiate(prefabObject, DefaultParent);

        /// <summary>
        /// Simple instantiate for non GameObject or Component types
        /// </summary>
        public T SimpleInstantiate(Transform transform) => Object.Instantiate(prefabObject, transform);

        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate() =>
            FullInstantiate(PrefabTransform.position, PrefabTransform.rotation, PrefabTransform.localScale, DefaultParent);
        
        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate(Vector3 position) =>
            FullInstantiate(position, PrefabTransform.rotation, PrefabTransform.localScale, DefaultParent);

        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate(Quaternion rotation) =>
            FullInstantiate(PrefabTransform.position, rotation, PrefabTransform.localScale, DefaultParent);

        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate(Transform parent) =>
            FullInstantiate(PrefabTransform.position, PrefabTransform.rotation, PrefabTransform.localScale, parent);

        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate(Transform parent, Vector3 position) =>
            FullInstantiate(position, PrefabTransform.rotation, PrefabTransform.localScale, parent);

        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate(Vector3 position, Quaternion rotation) =>
            FullInstantiate(position, rotation, PrefabTransform.localScale, DefaultParent);

        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate(Vector3 position, Vector3 scale) =>
            FullInstantiate(position, PrefabTransform.rotation, scale, DefaultParent);

        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate(Quaternion rotation, Vector3 scale) =>
            FullInstantiate(PrefabTransform.position, rotation, scale, DefaultParent);

        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate(Vector3 position, Quaternion rotation, Vector3 scale) =>
            FullInstantiate(position, rotation, scale, DefaultParent);

        /// <summary>
        /// Full instantiate for GameObject or Component types
        /// </summary>
        public T FullInstantiate(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent)
        {
            var result = Object.Instantiate(prefabObject);
            var transform = result.GetTransform();

            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;
            transform.parent = parent;

            return result;
        }

        public static implicit operator Prefab<T>(T component) => new Prefab<T>(component);

        public static implicit operator bool(Prefab<T> prefab) => prefab != null;

        public static implicit operator T(Prefab<T> prefab) => prefab.PrefabObject;
    }
}
#endif