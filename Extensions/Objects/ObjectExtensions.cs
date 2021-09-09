#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class ObjectExtensions
    {
        public static void TotalDestroy(this Object obj)
        {
            if (obj is Component component)
                component.DestroyObject();
            else
                Object.Destroy(obj);
        }
        
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