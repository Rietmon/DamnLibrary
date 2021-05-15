using System;
using System.Collections.Generic;
using Rietmon.Behaviours;
using Rietmon.Extensions;
using UnityEngine;

namespace Rietmon.Serialization
{
    [AddComponentMenu("Serialization/SerializableObject")]
    public class SerializableObject : SerializableUnityBehaviour
    {
        public ISerializable[] SerializableComponents { get; private set; }
        
        [SerializeField] private bool serializeAllComponents;

        [SerializeField] private Component[] serializableComponents;

        private void OnEnable()
        {
            SerializableComponents = serializeAllComponents 
                ? GetComponents<ISerializable>() 
                : serializableComponents.SmartCast((component) => (ISerializable)component);
        }

        private void Reset()
        {
            serializeAllComponents = true;
        }
    }
}
