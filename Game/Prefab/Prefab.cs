﻿#if UNITY_2020
using System;
using Rietmon.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rietmon.Game
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

        public T SimpleInstantiate() => Object.Instantiate(prefabObject);

        public T SimpleInstantiate(Transform transform) => Object.Instantiate(prefabObject, transform);

        public T FullInstantiate() =>
            FullInstantiate(PrefabTransform.position, PrefabTransform.rotation, PrefabTransform.localScale, DefaultParent);

        public T FullInstantiate(Vector3 position) =>
            FullInstantiate(position, PrefabTransform.rotation, PrefabTransform.localScale, DefaultParent);

        public T FullInstantiate(Quaternion rotation) =>
            FullInstantiate(PrefabTransform.position, rotation, PrefabTransform.localScale, DefaultParent);

        public T FullInstantiate(Transform parent) =>
            FullInstantiate(PrefabTransform.position, PrefabTransform.rotation, PrefabTransform.localScale, parent);

        public T FullInstantiate(Transform parent, Vector3 position) =>
            FullInstantiate(position, PrefabTransform.rotation, PrefabTransform.localScale, parent);

        public T FullInstantiate(Vector3 position, Quaternion rotation) =>
            FullInstantiate(position, rotation, PrefabTransform.localScale, DefaultParent);

        public T FullInstantiate(Vector3 position, Vector3 scale) =>
            FullInstantiate(position, PrefabTransform.rotation, scale, DefaultParent);

        public T FullInstantiate(Quaternion rotation, Vector3 scale) =>
            FullInstantiate(PrefabTransform.position, rotation, scale, DefaultParent);

        public T FullInstantiate(Vector3 position, Quaternion rotation, Vector3 scale) =>
            FullInstantiate(position, rotation, scale, DefaultParent);

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