namespace DamnLibrary.Management
{
    public enum WindowsDataProviderType : byte
    {
        Resources,
#if ENABLE_ADDRESSABLE
        Addressable
#endif
    }
}