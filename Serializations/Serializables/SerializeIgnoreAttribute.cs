using System;

namespace DamnLibrary.Serializations.Serializables
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeIgnoreAttribute : Attribute { }
}