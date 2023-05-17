using DamnLibrary.Behaviours;

namespace DamnLibrary.Management
{
    public abstract class WindowContext
    {
        public WindowBehaviour OwnerWindow { get; internal set; }
    }
}