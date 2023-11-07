namespace DamnLibrary.Managements.Windows
{
    public enum WindowsDataProviderType : byte
    {
        Resources,
#if ENABLE_ADDRESSABLE
        Addressable
#endif
    }
}