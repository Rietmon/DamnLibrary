using System.Collections.Generic;
using Rietmon.Behaviours;
using UnityEngine;

namespace Rietmon.Serialization
{
    [AddComponentMenu("Serialization/SerializableObject")]
    public class SerializableObject : UnityBehaviour
    {
        public bool SerializeAllComponents => serializeAllComponents;

        public List<ISerializable> SerializedComponents
        {
            get
            {
                var result = new List<ISerializable>();
            
                foreach (var component in serializedComponents)
                    result.Add((ISerializable)component);

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
