#if UNITY_5_3_OR_NEWER 
using UnityEngine;

namespace Rietmon.Attributes
{
    /// <summary>
    /// Made field only viewable in the inspector
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }
}
#endif