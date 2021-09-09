#if UNITY_5_3_OR_NEWER 
using System.Collections.Generic;
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class TransformExtensions
    {
        public static List<Transform> GetChildes(this Transform transform)
        {
            var result = new List<Transform>();
        
            for (var i = 0; i < transform.childCount; i++)
                result.Add(transform.GetChild(i));

            return result;
        }

        public static List<T> GetChildes<T>(this Transform transform)
        {
            var result = new List<T>();
        
            for (var i = 0; i < transform.childCount; i++)
                result.Add(transform.GetChild(i).GetComponent<T>());

            return result;
        }

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

        public static Vector3 GetLocalPositionRelative(this Transform transform, Transform relative) =>
            transform.position - relative.position;
    
        public static Quaternion GetLocalRotationRelative(this Transform transform, Transform relative) =>
            transform.rotation * relative.rotation;

        public static void SetPositionWithoutChildes(this Transform transform, Vector3 position)
        {
            var childes = GetChildes(transform);

            foreach (var child in childes)
                child.parent = null;

            transform.position = position;
        
            foreach (var child in childes)
                child.parent = transform;
        }

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