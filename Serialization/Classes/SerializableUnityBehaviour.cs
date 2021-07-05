#if ENABLE_SERIALIZATION && UNITY_2020
using Rietmon.Attributes;
using Rietmon.Behaviours;
using Rietmon.Extensions;
using Rietmon.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(SerializableObject))]
public abstract class SerializableUnityBehaviour : UnityBehaviour, ISerializable
{
    public short SerializableId => serializableId;
    
    public bool WasDeserialized { get; private set; }
    
    [SerializeField, ReadOnly] private short serializableId;

    public void Serialize(SerializationStream stream)
    {
        OnSerialize(stream);
    }

    protected virtual void OnSerialize(SerializationStream stream) { }

    public void Deserialize(SerializationStream stream)
    {
        OnDeserialize(stream);
        WasDeserialized = true;
    }
    
    protected virtual void OnDeserialize(SerializationStream stream) { }

#if UNITY_EDITOR
    [ContextMenu("Generate Serializable Id")]
    private void Editor_GenerateSerializableId()
    {
        serializableId = RandomUtilities.RandomShort;
        EditorUtility.SetDirty(this);
    }

    private void Reset()
    {
        Editor_GenerateSerializableId();
    }
#endif
}
#endif