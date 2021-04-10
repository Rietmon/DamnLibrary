using System;
using Rietmon.Behaviours;
using Rietmon.Serialization;
using UnityEngine;

public class SerializeTransform : UnityBehaviour, ISerializable
{
    [SerializeField] private ToSerializeFlags toSerialize;
    
    public void Serialize(SerializationStream stream)
    {
        if ((toSerialize & ToSerializeFlags.Nothing) != 0)
            return;
        
        if ((toSerialize & ToSerializeFlags.Position) != 0)
            stream.Write(transform.position);
        if ((toSerialize & ToSerializeFlags.Rotation) != 0)
            stream.Write(transform.rotation);
        if ((toSerialize & ToSerializeFlags.Scale) != 0)
            stream.Write(transform.localScale);
    }

    public void Deserialize(SerializationStream stream)
    {
        if ((toSerialize & ToSerializeFlags.Nothing) != 0)
            return;

        if ((toSerialize & ToSerializeFlags.Position) != 0)
            transform.position = stream.Read<Vector3>();
        if ((toSerialize & ToSerializeFlags.Rotation) != 0)
            transform.rotation = stream.Read<Quaternion>();
        if ((toSerialize & ToSerializeFlags.Scale) != 0)
            transform.localScale = stream.Read<Vector3>();
    }
    
    [Flags]
    private enum ToSerializeFlags
    {
        Nothing = 0,
        Position = 1, 
        Rotation = 2,
        Scale = 4
    }
}
