﻿using System;
using Rietmon.Behaviours;
using Rietmon.Extensions;
using Rietmon.Serialization;
using UnityEngine;

[RequireComponent(typeof(SerializableObject))]
public class SerializeGameObject : UnityBehaviour, ISerializable
{
    [SerializeField] private ToSerializeFlags toSerialize;

    [SerializeField] private bool serializeChildes;

    [SerializeField] private bool serializeAllChildes;
    
    public void Serialize(SerializationStream stream)
    {
        if ((toSerialize & ToSerializeFlags.Nothing) != 0)
            return;

        if ((toSerialize & ToSerializeFlags.ActiveSelf) != 0)
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
        if ((toSerialize & ToSerializeFlags.Nothing) != 0)
            return;

        if ((toSerialize & ToSerializeFlags.ActiveSelf) != 0)
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