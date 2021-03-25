using System.Collections.Generic;
using System.Linq;
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

        public static List<Transform> GetAllChildes(this Transform transform)
        {
            var result = new List<Transform> {transform};
        
            for (var i = 0; i < result.Count; i++)
                result.AddRange(result[i].GetChildes());

            return result;
        }
    
        public static List<T> GetAllChildes<T>(this Transform transform)
        {
            var transforms = GetChildes(transform);

            return transforms.Select(t => t.GetComponent<T>()).ToList();
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
