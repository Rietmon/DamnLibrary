using System.Collections.Generic;
using Rietmon.Behaviours;
using UnityEngine;

namespace Rietmon.Serialization
{
    [AddComponentMenu("Serialization/SerializationObject")]
    public class SerializableObject : UnityBehaviour
    {
        public bool SerializeAllComponents => serializeAllComponents;

        public List<ISerializableComponent> SerializedComponents
        {
            get
            {
                var result = new List<ISerializableComponent>();
            
                foreach (var component in serializedComponents)
                    result.Add((ISerializableComponent)component);

                return result;
            }
        }

        [SerializeField] private bool serializeAllComponents;

        [SerializeField] private List<Component> serializedComponents;

        private void Reset()
        {
            serializeAllComponents = true;
        }
    }
}
