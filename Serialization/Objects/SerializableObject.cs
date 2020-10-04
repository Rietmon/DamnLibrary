using System.Collections.Generic;
using Rietmon.Common.Behaviours;
using UnityEngine;

namespace Rietmon.Common.Serialization
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
