#if UNITY_5_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;

namespace DamnLibrary.Utilities.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Get component in parents
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Component</returns>
        public static T GetComponentInParents<T>(this Transform transform)
        {
            var currentParent = transform.parent;
            while (currentParent != null)
            {
                if (currentParent.TryGetComponent<T>(out var component))
                    return component;

                currentParent = currentParent.parent;
            }

            return default;
        }
        
        /// <summary>
        /// Get transforms of childes
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns>Transforms of childes</returns>
        public static List<Transform> GetChildes(this Transform transform)
        {
            var result = new List<Transform>();
        
            for (var i = 0; i < transform.childCount; i++)
                result.Add(transform.GetChild(i));

            return result;
        }

        /// <summary>
        /// Get components of childes
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Components of childes</returns>
        public static List<T> GetChildes<T>(this Transform transform)
        {
            var result = new List<T>();
        
            for (var i = 0; i < transform.childCount; i++)
                result.Add(transform.GetChild(i).GetComponent<T>());

            return result;
        }

        /// <summary>
        /// Get all childes of transform (include childes of childes etc...)
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="includeRoot">If true - include current transform</param>
        /// <returns>Transforms of childes</returns>
        public static List<Transform> GetAllChildes(this Transform transform, bool includeRoot = false)
        {
            var result = new List<Transform>();
            
            void GetAllSubChildes(Transform start)
            {
                for (var i = 0; i < start.childCount; i++)
                {
                    result.Add(start.GetChild(i));
                    GetAllSubChildes(start.GetChild(i));
                }
            }

            if (includeRoot)
                result.Add(transform);
            
            GetAllSubChildes(transform);

            return result;
        }
    
        /// <summary>
        /// Get all components of childes of transform (include childes of childes etc...)
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="includeRoot">If true - include current transform</param>
        /// <returns>Components of childes</returns>
        public static List<T> GetAllChildes<T>(this Transform transform, bool includeRoot = false)
        {
            var transforms = GetAllChildes(transform, includeRoot);
            var result = new List<T>();

            foreach (var child in transforms)
            {
                if (child.TryGetComponent<T>(out var component))
                    result.Add(component);
            }
            
            return result;
        }
    
        /// <summary>
        /// Find child by name
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="name">Child name</param>
        /// <returns>Child transform</returns>
        public static Transform FindChildByName(this Transform transform, string name)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>
        /// Get local position relative to another transform
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="relative">Relative to</param>
        /// <returns>Relative position</returns>
        public static Vector3 GetLocalPositionRelative(this Transform transform, Transform relative) =>
            transform.position - relative.position;
    
        /// <summary>
        /// Get local rotation relative to another transform
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="relative">Relative to</param>
        /// <returns>Relative rotation</returns>
        public static Quaternion GetLocalRotationRelative(this Transform transform, Transform relative) =>
            transform.rotation * relative.rotation;

        /// <summary>
        /// Set position without changing childes positions
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="position">Position</param>
        public static void SetPositionWithoutChildes(this Transform transform, Vector3 position)
        {
            var childes = GetChildes(transform);

            foreach (var child in childes)
                child.parent = null;

            transform.position = position;
        
            foreach (var child in childes)
                child.parent = transform;
        }

        /// <summary>
        /// Set rotation without changing childes rotations
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="rotation">Rotation</param>
        public static void SetRotationWithoutChildes(this Transform transform, Quaternion rotation)
        {
            var childes = GetChildes(transform);

            foreach (var child in childes)
                child.parent = null;

            transform.rotation = rotation;
        
            foreach (var child in childes)
                child.parent = transform;
        }
    }
}
#endif