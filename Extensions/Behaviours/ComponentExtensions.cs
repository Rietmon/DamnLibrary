#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class ComponentExtensions
    {
        public static void RemoveComponent(this Component component) => Object.Destroy(component);

        public static void DestroyObject(this Component component) => Object.Destroy(component.gameObject);

        public static void SetObjectActive(this Component component, bool state) => component.gameObject.SetActive(state);
    }
}
#endif