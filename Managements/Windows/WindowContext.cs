#if UNITY_5_3_OR_NEWER
using DamnLibrary.Behaviours;

namespace DamnLibrary.Managements.Windows
{
    public abstract class WindowContext
    {
        public WindowBehaviour OwnerWindow { get; internal set; }
    }
}
#endif