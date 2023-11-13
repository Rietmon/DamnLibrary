#if ENABLE_SERIALIZATION
using System;

namespace DamnLibrary.Serializations.Serializables
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeIncludeAttribute : Attribute { }
}
#endif