namespace DamnLibrary.Management
{
    public abstract class WindowContext
    {
        public T To<T>() where T : WindowContext => this as T;
    }
}