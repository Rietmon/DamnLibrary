using System;
using System.Collections;
using System.Collections.Generic;
using Rietmon.Common.Behaviours;
using Rietmon.Common.Extensions;
using Rietmon.Common.Serialization;
using UnityEngine;

[RequireComponent(typeof(SerializableObject))]
public class SerializeGameObject : UnityBehaviour, ISerializableComponent
{
    [SerializeField] private ToSerializeFlags toSerialize;

    [SerializeField] private bool serializeChildes;

    [SerializeField] private bool serializeAllChildes;
    
    public void Serialize(SerializationStream stream)
    {
        if (toSerialize.HasFlag(ToSerializeFlags.Nothing))
            return;

        if (toSerialize.HasFlag(ToSerializeFlags.ActiveSelf))
        {
            stream.Write(gameObject.activeSelf);
            if (serializeChildes)
            {
                var childes = serializeAllChildes ? transform.GetAllChildes() : transform.GetChildes();
                foreach (var child in childes)
                    stream.Write(child.gameObject.activeSelf);
            }
        }
    }

    public void Deserialize(SerializationStream stream)
    {
        if (toSerialize.HasFlag(ToSerializeFlags.Nothing))
            return;

        if (toSerialize.HasFlag(ToSerializeFlags.ActiveSelf))
        {
            gameObject.SetActive(stream.Read<bool>());
            if (serializeChildes)
            {
                var childes = serializeAllChildes ? transform.GetAllChildes() : transform.GetChildes();
                foreach (var child in childes)
                    child.gameObject.SetActive(stream.Read<bool>());
            }
        }
    }

    [Flags]
    private enum ToSerializeFlags
    {
        Nothing = 0,
        ActiveSelf = 1
    }
}