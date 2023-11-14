#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace DamnLibrary.Utilities.Extensions
{
    public static class ComponentExtensions
    {
        public static T AddComponent<T>(this Component component) where T : Component => 
            component.gameObject.AddComponent<T>();
		
        public static bool HasComponent<T>(this Component component) where T : Component => 
            component.GetComponent<T>();
		
        public static int GetComponentsCount(this Component component) => 
            component.GetComponents<Component>()?.Length ?? 0;

        public static void RemoveComponent<T>(this Component component) where T : Component => 
            Object.Destroy(component.GetComponent<T>());

        public static void RemoveThisComponent(this Component component) => 
            Object.Destroy(component);

        public static void SetGameObjectActive(this Component component, bool state) => 
            component.gameObject.SetActive(state);

        public static void DestroyThisGameObject(this Component component) => 
            Object.Destroy(component.gameObject);

        public static void DestroyGameObject(this Component component, Component other) => 
            Object.Destroy(other.gameObject);
    }
}
#endif