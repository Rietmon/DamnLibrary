#if ENABLE_DAMN_SCRIPT
using System;

namespace DamnLibrary.DamnScript
{
    /// <summary>
    /// Mark a class as a DamnScriptable.
    /// Create a private static method with the name "RegisterDamnScriptMethods" to create functions
    /// This method will be called automatically when DamnSccript will be initializing
    /// </summary>
    public class DamnScriptableAttribute : Attribute { }
}
#endif