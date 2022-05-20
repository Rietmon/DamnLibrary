#if ENABLE_SERIALIZATION && UNITY_5_3_OR_NEWER 
using DamnLibrary.Attributes;
using DamnLibrary.Behaviours;
using DamnLibrary.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DamnLibrary.Serialization
{
    public abstract class SerializableUnityBehaviour : UnityBehaviour
    {
        /// <summary>
        /// Unique Id for serialization
        /// </summary>
        public short SerializableId => serializableId;

        /// <summary>
        /// Is object has been deserialized
        /// </summary>
        public bool WasDeserialized { get; private set; }

        [SerializeField, ReadOnly] private short serializableId;

        internal void Serialize(SerializationStream stream)
        {
            OnSerialize(stream);
        }

        internal void Deserialize(SerializationStream stream)
        {
            OnDeserialize(stream);
            WasDeserialized = true;
        }

        protected virtual void OnSerialize(SerializationStream stream) { }

        protected virtual void OnDeserialize(SerializationStream stream) { }

#if UNITY_EDITOR
        [ContextMenu("Generate Serializable Id")]
        private void Editor_GenerateSerializableId()
        {
            serializableId = RandomUtilities.RandomShort;
            EditorUtility.SetDirty(this);
        }

        protected virtual void Reset()
        {
            Editor_GenerateSerializableId();
        }
#endif
    }
}
#endif