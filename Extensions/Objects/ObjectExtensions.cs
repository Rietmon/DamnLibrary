#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace DamnLibrary.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Total destroy all object. If it's a component - will destroy GameObject parent
        /// </summary>
        /// <param name="obj">Object</param>
        public static void TotalDestroy(this Object obj)
        {
            if (obj is Component component)
                component.DestroyObject();
            else
                Object.Destroy(obj);
        }
        
        /// <summary>
        /// Return transform universally for GameObject and Component
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Transform</returns>
        public static Transform GetTransform(this Object obj)
        {
            switch (obj)
            {
                case Component component:
                    return component.transform;
                case GameObject gameObject:
                    return gameObject.transform;
                default:
                    return null;
            }
        }
    }
}
#endif