using System;
using Rietmon.Serialization;
using UnityEngine;

[RequireComponent(typeof(SerializableObject))]
public class SerializeTransform : SerializableUnityBehaviour
{
    [SerializeField] private ToSerializeFlags toSerialize;

    protected override void OnSerialize(SerializationStream stream)
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

    protected override void OnDeserialize(SerializationStream stream)
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
