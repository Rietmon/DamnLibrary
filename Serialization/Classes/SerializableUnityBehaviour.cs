using System.Collections;
using System.Collections.Generic;
using Rietmon.Attributes;
using Rietmon.Behaviours;
using Rietmon.Serialization;
using UnityEngine;

[RequireComponent(typeof(SerializableObject))]
public abstract class SerializableUnityBehaviour : UnityBehaviour, ISerializable
{
    public Id SerializableId => serializableId;
    
    public bool WasDeserialized { get; private set; }
    
    [SerializeField, ReadOnly, HideInInspector] private Id serializableId;

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
}
