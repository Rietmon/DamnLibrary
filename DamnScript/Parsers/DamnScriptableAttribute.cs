#if ENABLE_DAMN_SCRIPT
using System;

namespace DamnLibrary.DamnScript.Parsers
{
    /// <summary>
    /// Mark a class as a DamnScriptable.
    /// Create a private static method with the name "RegisterDamnScriptMethods" to create functions
    /// This method will be called automatically when DamnSccript will be initializing
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DamnScriptableAttribute : Attribute { }
}
#endif