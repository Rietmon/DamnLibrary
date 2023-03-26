#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace DamnLibrary.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Remove this component
        /// </summary>
        /// <param name="component">Component</param>
        public static void RemoveComponent(this Component component) => Object.Destroy(component);

        /// <summary>
        /// Destroy the game object which has this component
        /// </summary>
        /// <param name="component">Component</param>
        public static void DestroyObject(this Component component) => Object.Destroy(component.gameObject);

        /// <summary>
        /// Set active GameObject
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="state">State</param>
        public static void SetObjectActive(this Component component, bool state) => component.gameObject.SetActive(state);
    }
}
#endif