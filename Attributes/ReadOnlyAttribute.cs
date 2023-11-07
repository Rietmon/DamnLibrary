#if UNITY_5_3_OR_NEWER
namespace DamnLibrary.Attributes
{
    /// <summary>
    /// Made field only readable in the inspector
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }
}
#endif