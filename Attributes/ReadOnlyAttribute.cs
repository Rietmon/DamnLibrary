#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace DamnLibrary.Attributes
{
    /// <summary>
    /// Made field only readable in the inspector
    /// </summary>
    public sealed class ReadOnlyAttribute : PropertyAttribute { }
}
#endif