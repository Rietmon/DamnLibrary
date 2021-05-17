using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rietmon.Serialization
{
    public class StaticSerializableAttribute : Attribute
    {
        public short SerializableId { get; }

        public StaticSerializableAttribute(short serializableId)
        {
            SerializableId = serializableId;
        }
    }
}