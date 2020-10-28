using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class GameObjectsExtensions
    {
        private static readonly Dictionary<GameObject, Dictionary<Type, Component>> objectsPull = new Dictionary<GameObject, Dictionary<Type, Component>>();
    
        public static T GetComponentFromPull<T>(this GameObject gameObject) where T : Component
        {
            var type = typeof(T);

            T HandleComponent(Dictionary<Type, Component> componentsPull)
            {
                var result = gameObject.GetComponent<T>();
                componentsPull.Add(type, result);

                return result;
            }

            if (objectsPull.TryGetValue(gameObject, out var pull))
            {
                if (pull.TryGetValue(type, out var pullResult))
                    return (T)pullResult;

                return HandleComponent(pull);
            }
        
            pull = new Dictionary<Type, Component>();
            objectsPull.Add(gameObject, pull);

            return HandleComponent(pull);
        }
    }
}
