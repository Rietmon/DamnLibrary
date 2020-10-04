using System;
using System.Collections;
using System.Collections.Generic;
using Rietmon.Common.Behaviours;
using Rietmon.Common.Serialization;
using UnityEngine;

public class SerializeTransform : UnityBehaviour, ISerializableComponent
{
    [SerializeField] private ToSerializeFlags toSerialize;
    
    public void Serialize(SerializationStream stream)
    {
        if (toSerialize.HasFlag(ToSerializeFlags.Nothing))
            return;
        
        if (toSerialize.HasFlag(ToSerializeFlags.Position))
            stream.Write(transform.position);
        if (toSerialize.HasFlag(ToSerializeFlags.Rotation))
            stream.Write(transform.rotation);
        if (toSerialize.HasFlag(ToSerializeFlags.Scale))
            stream.Write(transform.localScale);
    }

    public void Deserialize(SerializationStream stream)
    {
        if (toSerialize.HasFlag(ToSerializeFlags.Nothing))
            return;

        if (toSerialize.HasFlag(ToSerializeFlags.Position))
            transform.position = stream.Read<Vector3>();
        if (toSerialize.HasFlag(ToSerializeFlags.Rotation))
            transform.rotation = stream.Read<Quaternion>();
        if (toSerialize.HasFlag(ToSerializeFlags.Scale))
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
